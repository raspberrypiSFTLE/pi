using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using FaceDetection.Implementation.Models;
using System.Threading;

namespace FaceDetection.Implementation
{
    public class ProcessManager
    {
        private ILEDs _leds;
        private PiCamera _piCamera;
        private IdentifyPerson _identifyPerson;

        private static object _lockObject = new object();

        private AutoResetEvent autoresetEvent = new AutoResetEvent(true);

        public ProcessManager(int snapshotsInterval)
        {
            _leds = new LEDs();
            _piCamera = new PiCamera(snapshotsInterval);
            //_piCamera.imageCapturedEvent += _piCamera_imageCapturedEvent;
            _identifyPerson = new IdentifyPerson();
        }

        public void Start()
        {
            Console.Write("Initialize Pi ...");
            Pi.Init<BootstrapWiringPi>();
            Console.WriteLine("Done");

            try
            {
                StartFlow();

                var motionSensor = new PirHC501();
                motionSensor.motionStartedEvent += MotionSensor_motionStartedEvent;
                motionSensor.motionStoppedEvent += MotionSensor_motionStoppedEvent;
                motionSensor.Start();

                
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed. " + e.Message);
            }
        }

        private void MotionSensor_motionStoppedEvent()
        {
            //if (!processInProgress)
            //{
            //    _leds.Update(ProcessState.Sleep);
            //}

            //_piCamera.StopCapturingImages();
        }

        private void MotionSensor_motionStartedEvent()
        {
            //_piCamera.StartCapturingImages();
            Console.WriteLine("Release semaphore");
            autoresetEvent.Set();
        }

        private async void StartFlow()
        {
            while (true)
            {
                Console.WriteLine("Wait one");

                autoresetEvent.WaitOne();
                Console.WriteLine("Continue process");

                byte[] imageContent;
                lock (_lockObject)
                {
                    _leds.Update(ProcessState.WaitingPersonDetection);

                    imageContent = _piCamera.StartCapturingImages();

                    if (!Faces.IsDetectedFace(imageContent))
                    {
                        _leds.Update(ProcessState.Sleep);
                        Console.WriteLine($"No faces detected in image");
                    }
                }

                var persons = await _identifyPerson.IdentifyPersonAsync(imageContent).ConfigureAwait(false);
                Console.WriteLine($"Persons in capture:\n Person: {String.Join(";\n Person: ", persons.Select(p => p.ToString()).ToArray())}");

                if (persons.Any(p => p.Unrecognized) || !persons.Any())
                {
                    _leds.Update(ProcessState.NoPersonRecognized);
                }
                else
                {
                    _leds.Update(ProcessState.AllPersonsRecognized);
                }
            }
        }
    }
}

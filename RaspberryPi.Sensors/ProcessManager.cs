using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors
{
    public class ProcessManager
    {
        private ILEDs _leds;
        private IPiCamera _piCamera;

        public ProcessManager(int snapshotsInterval)
        {
            _leds = new LEDs();
            _piCamera = new PiCamera(snapshotsInterval);
        }

        public void Start()
        {
            Console.Write("Initialize Pi ...");
            Pi.Init<BootstrapWiringPi>();
            Console.WriteLine("Done");

            try
            {
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
            _leds.Update(ProcessState.Sleep);
            _piCamera.StopCapturingImages();
        }

        private void MotionSensor_motionStartedEvent()
        {
            _leds.Update(ProcessState.MotionDetected);
            _piCamera.StartCapturingImages();
        }
    }
}

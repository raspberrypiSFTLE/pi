using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using FaceDetection.Implementation.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace FaceDetection.Implementation
{
    public class ProcessManager
    {
        private ILEDs _leds;
        private PiCamera _piCamera;
        private IdentifyPerson _identifyPerson;
        private IMemoryCache _cache;
        private ReplyBag _replyBag;
        private TTSBuilder _ttsBuilder;
        private SoundPlayer _soundPlayer;
        private ReplyBuilder _replyBuilder;

        private static object _lockObject = new object();
        private int _snapshotsInterval = 3;

        private static AutoResetEvent autoresetEvent = new AutoResetEvent(false);

        public ProcessManager(int snapshotsInterval, IMemoryCache cache, ReplyBag replyBag, TTSBuilder ttsBuilder, SoundPlayer soundPlayer, ReplyBuilder replyBuilder)
        {
            _leds = new LEDs();
            _leds.Update(ProcessState.Sleep);
            _piCamera = new PiCamera();
            _identifyPerson = new IdentifyPerson();
            _cache = cache;
            _replyBag = replyBag;
            _ttsBuilder = ttsBuilder;
            _soundPlayer = soundPlayer;
            _replyBuilder = replyBuilder;

            if (snapshotsInterval == 0) snapshotsInterval = 5;
            _snapshotsInterval = snapshotsInterval;
        }

        public void Start()
        {
            Console.Write("Initialize Pi ...");
            Pi.Init<BootstrapWiringPi>();
            Console.WriteLine("Done");

            try
            {
                new Thread(() =>
                {
                    StartFlow();
                }).Start();

                var motionSensor = new PirHC501(_snapshotsInterval);
                motionSensor.motionStartedEvent += MotionSensor_motionStartedEvent;
                motionSensor.Start();
            }
            catch (Exception e)
            {
                WriteToConsole("Failed. " + e.Message);
            }
        }

        private void MotionSensor_motionStartedEvent()
        {
            WriteToConsole("Release semaphore");
            autoresetEvent.Set();
        }

        private void StartFlow()
        {
            string wavFileName = "sample";

            while (true)
            {
                try
                {
                    WriteToConsole("Wait one");

                    autoresetEvent.WaitOne();
                    WriteToConsole("---- Continue process----");

                    _leds.Update(ProcessState.Sleep);
                    byte[] imageContent;

                    imageContent = _piCamera.TakeSnapshot();

                    if (!Faces.IsDetectedFace(imageContent))
                    {

                        WriteToConsole($"No faces detected in image");
                        WriteToConsole($" ---- End process -----");
                        continue;
                    }
                    _leds.Update(ProcessState.WaitingPersonDetection);

                    var persons = _identifyPerson.IdentifyPersonAsync(imageContent).GetAwaiter().GetResult();
                    WriteToConsole($"Persons in capture:\n Person: {String.Join(";\n Person: ", persons.Select(p => p.ToString()).ToArray())}");

                    if (persons.All(p => p.Unrecognized) || !persons.Any())
                    {
                        _leds.Update(ProcessState.NoPersonRecognized);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (persons.Any(p => p.Unrecognized))
                        {
                            _leds.Update(ProcessState.PartialPersonsRecognized);
                        }
                        else
                        {
                            _leds.Update(ProcessState.AllPersonsRecognized);
                        }

                        List<Person> personsToProcess = new List<Person>();
                        foreach (var person in persons)
                        {
                            bool seen;
                            bool alreadySeen = _cache.TryGetValue(person.match.personId, out seen);
                            if (!alreadySeen)
                            {
                                _cache.Set(person.match.personId, true, new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromMinutes(1)));
                                personsToProcess.Add(person);
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                WriteToConsole($"I have seen {person.match.name} in the past minutes");
                            }
                        }


                        if (personsToProcess.Count > 0)
                        {
                            var replies = _replyBuilder.BuildReplies(persons);

                            _ttsBuilder.BuildWavAsync(replies, wavFileName).GetAwaiter().GetResult();

                            _soundPlayer.PlayOnPi(wavFileName);
                        }
                    }
                }
                catch(Exception ex)
                {
                    WriteToConsole($"{ ex.Message},\n Stack trace: {ex.StackTrace}");
                }


                _leds.Update(ProcessState.Sleep);
                WriteToConsole($" ---- End process -----");
            }
        }


        private void WriteToConsole(string message)
        {
            Console.WriteLine($"{message}: {DateTime.Now:dd:MM:yyyy HH:mm:ss.fff}");
        }
    }
}

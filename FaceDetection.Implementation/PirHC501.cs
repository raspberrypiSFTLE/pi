using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;
using System.Timers;

namespace FaceDetection.Implementation
{
    public class PirHC501
    {
        private bool _isMotion = false;
        private GpioPin _pin;

        private static Timer timer;
        private static int firedEventsCount = 0;

        public delegate void MotionStarted();

        public event MotionStarted motionStartedEvent;

        public PirHC501(int snapshotsIntervalSeconds)
        {
            _pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            _pin.PinMode = GpioPinDriveMode.Input;

            timer = new Timer(snapshotsIntervalSeconds * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = false;
        }

        public void Start()
        {
            // register callback whenever an interrupt takes place
            _pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        }

        private void ISRCallback()
        {
            if (!_isMotion && !timer.Enabled )
            {
                //start the timer
                timer.Enabled = true;
                _isMotion = true;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            firedEventsCount++;
            Console.WriteLine("Timer callback");

            motionStartedEvent?.Invoke();

            if (firedEventsCount == 3)
            {
                this.ResetTimer();
            }
        }

        private void ResetTimer()
        {
            Console.WriteLine($"Reset timer {_isMotion}");
            timer.Enabled = false;
            _isMotion = false;
            firedEventsCount = 0;
        }
    }
}

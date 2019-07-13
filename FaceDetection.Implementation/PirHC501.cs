using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace FaceDetection.Implementation
{
    public class PirHC501
    {
        private bool _isMotion = false;
        private GpioPin _pin;

        private static Timer timer;

        public delegate void MotionStarted();
        public delegate void MotionStopped();

        public event MotionStarted motionStartedEvent;
        public event MotionStopped motionStoppedEvent;

        public PirHC501()
        {
            _pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            _pin.PinMode = GpioPinDriveMode.Input;
        }

        public void Start()
        {
            // register callback whenever an interrupt takes place
            _pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        }

        private void ISRCallback()
        {
            if (_isMotion )
            {
                if (timer == null)
                {
                    timer = new Timer(
                    callback: new TimerCallback(TimerTask),
                    state: this,
                    dueTime: 5000,
                    period: Timeout.Infinite);
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                // if there is no motion change state
                ChangeState();
            }
        }

        private static void TimerTask(object timerState)
        {
            ((PirHC501)timerState).ChangeState();
            ((PirHC501)timerState).Reset();
        }

        public void ChangeState()
        {
            _isMotion = !_isMotion;

            if (_isMotion)
            {
                Console.WriteLine($"Motion detected: {DateTime.Now:dd:MM:yyyy HH:mm:ss.fff}");
                motionStartedEvent?.Invoke();
            }
            else
            {
                Console.WriteLine($"No motion detected: {DateTime.Now:dd:MM:yyyy HH:mm:ss.fff}");
                motionStoppedEvent?.Invoke();
            }
        }

        private void Reset()
        {
            timer.Dispose();
            timer = null;
        }
    }
}

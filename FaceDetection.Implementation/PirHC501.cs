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
            if (!_isMotion && timer == null)
            {
                timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: new TimerState {  Count = 0 },
                dueTime: 30000, // 30s
                period: 1000);

                ChangeState();
            }
        }

        private void TimerTask(object timerState)
        {
            var state = timerState as TimerState;
            state.Count++;
            Console.WriteLine("Timer callback");

            motionStartedEvent?.Invoke();

            if (state.Count == 10)
            {
                this.Reset();
            }
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
            Console.WriteLine($"Reset timer {_isMotion}");

            if (_isMotion)
            {
                ChangeState();
            }

            Console.WriteLine($"Dispose timer");


            timer.Dispose();
            timer = null;
        }
    }

    public class TimerState
    {
        public int Count { get; set; }
    }
}

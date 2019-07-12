using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors
{
    public class PirHC501
    {

        private bool _keepActivated = true;
        private bool _isActivated = false;
        private GpioPin _pin;

        private GpioPin _ledPin;

        private IPiCamera _piCamera;
        private ILEDs _leds;

        private static Timer timer;

        public PirHC501(IPiCamera piCamera, ILEDs leds)
        {
            //_ledPin = (GpioPin)Pi.Gpio[BcmPin.Gpio23];
            //_ledPin.PinMode = GpioPinDriveMode.Output;
            

            _pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            _pin.PinMode = GpioPinDriveMode.Input;

            _piCamera = piCamera;
            _leds = leds;
        }

        private static void TimerTask(object timerState)
        {
            ((PirHC501)timerState).ChangeState();
            ((PirHC501)timerState).Reset();
            //Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
            //var state = timerState as TimerState;
            //Interlocked.Increment(ref state.Counter);
        }

        public void Start()
        {
            _pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        }

        private void ISRCallback()
        {
            if (_isActivated )
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
                ChangeState();
            }
        }

        public void ChangeState()
        {
            _isActivated = !_isActivated;
            //_ledPin.Write(_isActivated);
            if (_isActivated)
            {
                Console.WriteLine($"Motion detected: {DateTime.Now.ToLongDateString()}");
                _leds.Update(ProcessState.MotionDetected);
                _piCamera.StartCapturingImages();
            }
            else
            {
                Console.WriteLine($"No motion detected: {DateTime.Now.ToLongDateString()}");
                _leds.Update(ProcessState.Sleep);
                _piCamera.StopCapturingImages();
            }

            //Console.WriteLine($"Pin Activated...{_isActivated}");
        }

        private void Reset()
        {
            timer.Dispose();
            timer = null;
        }
    }
}

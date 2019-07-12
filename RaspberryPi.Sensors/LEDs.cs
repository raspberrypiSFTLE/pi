using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors
{
    public class LEDs : ILEDs
    {
        private GpioPin _redPin;
        private GpioPin _greenPin;
        private GpioPin _bluePin;

        private static Timer _blinkingTimer;

        public LEDs()
        {
            _redPin = (GpioPin)Pi.Gpio[BcmPin.Gpio16];
            _redPin.PinMode = GpioPinDriveMode.Output;

            _greenPin = (GpioPin)Pi.Gpio[BcmPin.Gpio20];
            _greenPin.PinMode = GpioPinDriveMode.Output;

            _bluePin = (GpioPin)Pi.Gpio[BcmPin.Gpio21];
            _bluePin.PinMode = GpioPinDriveMode.Output;
        }


        public void Update(ProcessState state)
        {
            if (state != ProcessState.WaitingPersonDetection && _blinkingTimer != null)
            {
                _blinkingTimer.Dispose();
                _blinkingTimer = null;
            }

            switch (state)
            {
                case ProcessState.WaitingPersonDetection:
                    var blinkState = new BlinkState();
                    _blinkingTimer = new Timer(
                       callback: new TimerCallback(StartBlinking),
                       state: blinkState,
                       dueTime: 0,
                       period: 500);
                    break;

                case ProcessState.Sleep:
                    _redPin.Write(false);
                    _greenPin.Write(false);
                    _bluePin.Write(false);
                    break;

                case ProcessState.MotionDetected:
                    _greenPin.Write(false);
                    _redPin.Write(false);
                    _bluePin.Write(true);
                    break;

                case ProcessState.PersonRecognized:
                    _greenPin.Write(true);
                    _redPin.Write(false);
                    _bluePin.Write(false);
                    break;

                case ProcessState.PersonUnrecognized:
                    _greenPin.Write(false);
                    _redPin.Write(true);
                    _bluePin.Write(false);
                    break;
            }
        }

        private void StartBlinking(object state)
        {
            var blinkState = (state as BlinkState);
            blinkState.BlinkValue = !blinkState.BlinkValue;
            _greenPin.Write(blinkState.BlinkValue);
            _redPin.Write(blinkState.BlinkValue);
            _bluePin.Write(false);
        }
    }

    class BlinkState
    {
        public bool BlinkValue = true;
    }
}

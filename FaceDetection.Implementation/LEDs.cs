using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace FaceDetection.Implementation
{
    public class LEDs : ILEDs
    {
        private GpioPin _led1_RedPin;
        private GpioPin _led1_GreenPin;
        private GpioPin _led1_BluePin;

        private GpioPin _led2_RedPin;
        private GpioPin _led2_GreenPin;
        private GpioPin _led2_BluePin;

        private static Timer _blinkingTimer;

        public LEDs()
        {
            _led1_RedPin = (GpioPin)Pi.Gpio[BcmPin.Gpio16];
            _led1_RedPin.PinMode = GpioPinDriveMode.Output;

            _led1_GreenPin = (GpioPin)Pi.Gpio[BcmPin.Gpio21];
            _led1_GreenPin.PinMode = GpioPinDriveMode.Output;

            _led1_BluePin = (GpioPin)Pi.Gpio[BcmPin.Gpio20];
            _led1_BluePin.PinMode = GpioPinDriveMode.Output;


            _led2_RedPin = (GpioPin)Pi.Gpio[BcmPin.Gpio13];
            _led2_RedPin.PinMode = GpioPinDriveMode.Output;

            _led2_GreenPin = (GpioPin)Pi.Gpio[BcmPin.Gpio19];
            _led2_GreenPin.PinMode = GpioPinDriveMode.Output;

            _led2_BluePin = (GpioPin)Pi.Gpio[BcmPin.Gpio26];
            _led2_BluePin.PinMode = GpioPinDriveMode.Output;
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
                    _led1_RedPin.Write(false);
                    _led1_GreenPin.Write(false);
                    _led1_BluePin.Write(false);

                    _led2_RedPin.Write(false);
                    _led2_GreenPin.Write(false);
                    _led2_BluePin.Write(false);
                    break;

                case ProcessState.MotionDetected:
                    _led1_GreenPin.Write(false);
                    _led1_RedPin.Write(false);
                    _led1_BluePin.Write(true);

                    _led2_GreenPin.Write(false);
                    _led2_RedPin.Write(false);
                    _led2_BluePin.Write(true);
                    break;

                case ProcessState.AllPersonsRecognized:
                    _led1_GreenPin.Write(true);
                    _led1_RedPin.Write(false);
                    _led1_BluePin.Write(false);

                    _led2_GreenPin.Write(true);
                    _led2_RedPin.Write(false);
                    _led2_BluePin.Write(false);
                    break;

                case ProcessState.NoPersonRecognized:
                    _led1_GreenPin.Write(false);
                    _led1_RedPin.Write(true);
                    _led1_BluePin.Write(false);

                    _led2_GreenPin.Write(false);
                    _led2_RedPin.Write(true);
                    _led2_BluePin.Write(false);
                    break;

                case ProcessState.PartialPersonsRecognized:
                    _led1_GreenPin.Write(false);
                    _led1_RedPin.Write(true);
                    _led1_BluePin.Write(false);

                    _led2_GreenPin.Write(true);
                    _led2_RedPin.Write(false);
                    _led2_BluePin.Write(false);
                    break;
            }
        }

        private void StartBlinking(object state)
        {
            var blinkState = (state as BlinkState);
            blinkState.BlinkValue = !blinkState.BlinkValue;
            _led1_GreenPin.Write(false);
            _led1_RedPin.Write(false);
            _led1_BluePin.Write(blinkState.BlinkValue);

            _led2_GreenPin.Write(false);
            _led2_RedPin.Write(false);
            _led2_BluePin.Write(blinkState.BlinkValue);
        }
    }

    class BlinkState
    {
        public bool BlinkValue = true;
    }
}

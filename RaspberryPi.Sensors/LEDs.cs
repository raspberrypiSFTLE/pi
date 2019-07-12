using System;
using System.Collections.Generic;
using System.Text;
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
            switch (state)
            {
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

                case ProcessState.WaitingPersonDetection:
                    
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
    }
}

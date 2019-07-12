using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors
{
    public class PirHC501
    {
        private bool _isActivated = false;
        private GpioPin _pin;

        private GpioPin _ledPin;
        public PirHC501()
        {
            _ledPin = (GpioPin)Pi.Gpio[BcmPin.Gpio23];
            _ledPin.PinMode = GpioPinDriveMode.Output;

            Console.WriteLine("Gpio Interrupts");
            _pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            _pin.PinMode = GpioPinDriveMode.Input;
        }

        public void Start()
        {
            _pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        }

        private void ISRCallback()
        {
            _isActivated = !_isActivated;
            _ledPin.Write(_isActivated);

            Console.WriteLine($"Pin Activated...{_isActivated}");
        }
    }
}

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

        private IPiCamera _piCamera;

        public PirHC501(IPiCamera piCamera)
        {
            //_ledPin = (GpioPin)Pi.Gpio[BcmPin.Gpio23];
            //_ledPin.PinMode = GpioPinDriveMode.Output;

            Console.WriteLine("Gpio Interrupts");
            _pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            _pin.PinMode = GpioPinDriveMode.Input;

            _piCamera = piCamera;
        }

        public void Start()
        {
            _pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        }

        private void ISRCallback()
        {
            _isActivated = !_isActivated;
            //_ledPin.Write(_isActivated);
            if (_isActivated)
            {
                Console.WriteLine($"Motion detected: {DateTime.Now.ToLongDateString()}");
                _piCamera.CaptureImage();
            }

            Console.WriteLine($"Pin Activated...{_isActivated}");
        }
    }
}

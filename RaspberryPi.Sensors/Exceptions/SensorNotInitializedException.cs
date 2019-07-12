using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors.Exceptions
{
    public class SensorNotInitializedException: Exception
    {
        public SensorNotInitializedException(string message) : base(message)
        {
        }
        public SensorNotInitializedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

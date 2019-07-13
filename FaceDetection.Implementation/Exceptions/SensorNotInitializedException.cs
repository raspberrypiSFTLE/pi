using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation.Exceptions
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

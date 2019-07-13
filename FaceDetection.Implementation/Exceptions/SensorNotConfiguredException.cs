using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FaceDetection.Implementation.Exceptions
{
    class SensorNotConfiguredException : Exception
    {
        public SensorNotConfiguredException()
        {
        }

        public SensorNotConfiguredException(string message) : base(message)
        {
        }

        public SensorNotConfiguredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SensorNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

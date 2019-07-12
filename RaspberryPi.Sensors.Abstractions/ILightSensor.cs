using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors.Abstractions
{
    public interface ILightSensor
    {
        /// <summary>
        /// Read iluminance value in lux
        /// </summary>
        /// <returns>Return iluminance value in lux (lumen per sq. meter)</returns>
        double GetIluminance();
    }
}

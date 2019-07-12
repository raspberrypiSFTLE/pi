using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors.Abstractions
{
    public interface IBarometricPressureSensor
    {
        /// <summary>
        /// Read pressure from sensor
        /// </summary>
        /// <returns>Returns pressure in Pa</returns>
        double GetPressure();
    }
}

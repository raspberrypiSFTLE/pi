using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors.Abstractions
{
    public interface ITemperatureSensor
    {
        /// <summary>
        /// Read value from a temperature sensor in Celsius degrees
        /// </summary>
        /// <returns>Temperature in Celsius degrees</returns>
        double GetTemperature();       
    }
}

using RaspberryPi.Sensors.Abstractions;
using RaspberryPi.Sensors.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO.Abstractions;

namespace RaspberryPi.Sensors
{
    public enum Bh1750Mode
    {
        Unconfigured = 0,
        ContinuousStandardResolution = 0x10,
        ContinuousHighResolution = 0x11,
        ContinuousLowResolution = 0x13,
        OneTimeStandardResolution = 0x20,
        OneTimeHighResolution = 0x21,
        OneTimeLowResolution = 0x23
    };

    public class Bh1750FviSensor : ILightSensor
    {
        private byte measurementTime;
        private readonly II2CBus i2cBus = null;
        private II2CDevice i2cDevice = null;
        private readonly byte sensorAddress = 0;
        private const byte DefaultMeasurementTime = 69;

        public const byte DefaultI2CAddress = 0x23;

        private int GetMaxTimeForConfiguration()
        {
            int retValue = 120;
            switch (Mode)
            {
                case Bh1750Mode.ContinuousLowResolution:
                case Bh1750Mode.OneTimeLowResolution:
                    retValue = 24 * MeasurementTime / DefaultMeasurementTime;
                    break;

                case Bh1750Mode.ContinuousHighResolution:
                case Bh1750Mode.ContinuousStandardResolution:
                case Bh1750Mode.OneTimeHighResolution:
                case Bh1750Mode.OneTimeStandardResolution:
                    retValue = 180 * MeasurementTime / DefaultMeasurementTime;
                    break;
            }

            return retValue;
        }

        private int GetMaxTimeForDataRead()
        {
            int retVal = 10;
            switch (Mode)
            {
                case Bh1750Mode.OneTimeLowResolution:
                    retVal = 24 * MeasurementTime / DefaultMeasurementTime;
                    break;

                case Bh1750Mode.OneTimeHighResolution:
                case Bh1750Mode.OneTimeStandardResolution:
                    retVal = 180 * MeasurementTime / DefaultMeasurementTime;
                    break;
            }

            return retVal;
        }


        public Bh1750Mode Mode
        {
            get;
            set;
        }

        public bool IsContinuosMode
        {
            get
            {
                return (((byte)Mode >> 4) == 0x01);
            }
        }

        public bool IsHighResolution
        {
            get
            {
                return (Mode == Bh1750Mode.ContinuousHighResolution || Mode == Bh1750Mode.OneTimeHighResolution);
            }
        }

        public byte MeasurementTime
        {
            get
            {
                return measurementTime;
            }
            set
            {
                if (value < 32 && value > 254)
                {
                    throw new ArgumentException("Measurement time must be a value between 32 and 254");
                }

                if (i2cDevice == null)
                {
                    throw new SensorNotInitializedException("Seems that the device is not connected. In order to set the measurement time the device must be connected and initialized");
                }

                //Set MT register. In order to set it we need to send two bytes. First byte is in the form 01000(v7)(v6)(v5) and the second byte
                //is 011(v4)(v3)(v2)(v1)(v0)
                //where (v0), (v1) ... (v7) are the bits 0, 1, 2, ... 7 of the new value (measurement time)
                byte firstByte = (byte)(0b0100_0000 | (value >> 5));
                byte secondByte = (byte)(0b01100000 | (value & 0b00011111));

                i2cDevice.WriteAddressByte(sensorAddress, firstByte);
                i2cDevice.WriteAddressByte(sensorAddress, secondByte);
                i2cDevice.WriteAddressByte(sensorAddress, (byte)Mode);
                Thread.Sleep(10);
                measurementTime = value;
                Thread.Sleep(GetMaxTimeForConfiguration());

            }
        }

        private ushort ReadRawIluminance()
        {
            ushort retVal = 0;

            //in order to read the value from the sensor, first we must write the mode to the sensor
            if (Mode == Bh1750Mode.Unconfigured)
            {
                throw new SensorNotConfiguredException("Sensor mode is not configured.");
            }
            if (i2cDevice == null)
            {
                throw new SensorNotInitializedException("The sensor is not initialized.");
            }
            int waitTime = GetMaxTimeForDataRead();
            if (!IsContinuosMode)
            {
                i2cDevice.WriteAddressByte(sensorAddress, (byte)Mode);
                Thread.Sleep(waitTime);
            }
            var bigEndianValue = i2cDevice.ReadAddressWord(sensorAddress);
            retVal = (ushort)((bigEndianValue & 0x00FF) << 8 | ((bigEndianValue & 0xFF00) >> 8));

            Console.WriteLine($"Raw value: {retVal}");

            return retVal;
        }


        public Bh1750FviSensor(II2CBus i2cBus, byte i2cAddress = DefaultI2CAddress)
        {
            this.i2cBus = i2cBus;
            this.sensorAddress = i2cAddress;
            this.measurementTime = DefaultMeasurementTime;
        }

        public void Connect(Bh1750Mode mode)
        {
            i2cDevice = i2cBus.AddDevice(sensorAddress);
            Mode = mode;
            //i2cDevice.WriteAddressByte(sensorAddress, (byte)1);
            // i2cDevice.WriteAddressByte(sensorAddress, (byte)Mode);
            Thread.Sleep(GetMaxTimeForConfiguration());
        }

        public double GetIluminance()
        {
            double retVal = 0.0;
            retVal = ReadRawIluminance();
            
            if (MeasurementTime != DefaultMeasurementTime)
            {
                retVal = retVal * DefaultMeasurementTime / (double)MeasurementTime;
                if (IsHighResolution)
                {
                    retVal /= 2.0;
                }
            }
            retVal /= 1.2;

            return retVal;
        }
    }
}

using RaspberryPi.Sensors.Abstractions;
using RaspberryPi.Sensors.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors
{
    public enum Bmp180Mode
    {
        UltraLowPower = 0,
        Standard,
        HighResolution,
        UltraHighResolution
    };

    public sealed class Bmp180Sensor : ITemperatureSensor, IBarometricPressureSensor
    {
        private readonly int i2cAddress;
        private readonly Bmp180Mode mode;
        private II2CDevice i2cDevice = null;
        private II2CBus i2cBus = null;
        private Bmp180CalibrationData calibrationData = new Bmp180CalibrationData();

        private enum Register
        {
            AC1 = 0xAA,
            AC2 = 0xAC,
            AC3 = 0xAE,
            AC4 = 0xB0,
            AC5 = 0xB2,
            AC6 = 0xB4,
            B1 = 0xB6,
            B2 = 0xB8,
            MB = 0xBA,
            MC = 0xBC,
            MD = 0xBE,
            ChipId = 0xD0,
            Version = 0xD1,
            SoftReset = 0xE0,
            Control = 0xF4,    
            Data =  0xF6,
            DataExt = 0xF8
        }

        private enum Command
        {
            ReadTemperature = 0x2E,
            ReadPressureUltraLowPower = 0x34,
            ReadPressureStandard = 0x74,
            ReadPressureHighResolution = 0xB4,
            ReadPressureUltraHighResolution = 0xF4
        }

        public const byte DefaultI2CAddress = 0x77;


        private Int16 ReadInt16FromRegister(Register registerToRead)
        {
            if (i2cDevice == null)
            {
                throw new SensorNotInitializedException("Cannot read from device register. Device is not yet connected, please call the connect method first.");
            }

            //sensor stores data with msb first (big-endian) and we need to convert to little endian
            short bigEndianValue = (short)i2cDevice.ReadAddressWord((int)registerToRead);
            short littleEndianValue = (short)((bigEndianValue & 0x00FF) << 8 | ((bigEndianValue & 0xFF00) >> 8));
            return littleEndianValue;
        }

        private UInt16 ReadUInt16FromRegister(Register registerToRead)
        {
            var retVal = ReadInt16FromRegister(registerToRead);
            return (ushort)retVal;
        }

        private byte ReadByteFromRegister(Register registerToRead)
        {
            if (i2cDevice == null)
            {
                throw new SensorNotInitializedException("Cannot read from device register. Device is not yet connected, please call the connect method first.");
            }
            return i2cDevice.ReadAddressByte((int)registerToRead);
        }

        private void WriteCommand(Command command)
        {
            if (i2cDevice == null)
            {
                throw new SensorNotInitializedException("Cannot write command. Device is not yet connected, please call the connect method first.");
            }
            i2cDevice.WriteAddressByte((int)Register.Control, (byte)command);
        }

        private void LoadCalibrationData()
        {
            calibrationData.AC1 = ReadInt16FromRegister(Register.AC1);
            calibrationData.AC2 = ReadInt16FromRegister(Register.AC2);
            calibrationData.AC3 = ReadInt16FromRegister(Register.AC3);
            calibrationData.AC4 = ReadUInt16FromRegister(Register.AC4);
            calibrationData.AC5 = ReadUInt16FromRegister(Register.AC5);
            calibrationData.AC6 = ReadUInt16FromRegister(Register.AC6);
            calibrationData.B1 = ReadInt16FromRegister(Register.B1);
            calibrationData.B2 = ReadInt16FromRegister(Register.B2);
            calibrationData.MB = ReadInt16FromRegister(Register.MB);
            calibrationData.MC = ReadInt16FromRegister(Register.MC);
            calibrationData.MD = ReadInt16FromRegister(Register.MD);
        }

        private int ComputeB5(int rawValue)
        {
            int retValue = 0;

            int X1 = (rawValue - (int)calibrationData.AC6)* ((int)calibrationData.AC5) >> 15;
            int X2 = (((int)calibrationData.MC) << 11) / (X1 + (int)calibrationData.MD);
            retValue = X1 + X2;

            return retValue;
        }

        private short ReadRawTemperature()
        {
            WriteCommand(Command.ReadTemperature);
            Thread.Sleep(5);
            return ReadInt16FromRegister(Register.Data);
        }

        private Command GetPressureCommand()
        {
            Command retVal = Command.ReadPressureUltraLowPower;
            switch (mode)
            {
                case Bmp180Mode.UltraLowPower:
                    retVal = Command.ReadPressureUltraLowPower;
                    break;

                case Bmp180Mode.Standard:
                    retVal = Command.ReadPressureStandard;
                    break;

                case Bmp180Mode.HighResolution:
                    retVal = Command.ReadPressureHighResolution;
                    break;

                case Bmp180Mode.UltraHighResolution:
                    retVal = Command.ReadPressureUltraHighResolution;
                    break;
            }
            return retVal;
        }

        private int GetPressureComputeTime()
        {
            int retVal = 5;
            switch (mode)
            {
                case Bmp180Mode.UltraLowPower:
                    retVal = 5;
                    break;

                case Bmp180Mode.Standard:
                    retVal = 8;
                    break;

                case Bmp180Mode.HighResolution:
                    retVal = 14;
                    break;

                case Bmp180Mode.UltraHighResolution:
                    retVal = 26;
                    break;
            }

            return retVal;
        }

        private int ReadRawPressure()
        {
            WriteCommand(GetPressureCommand());
            Thread.Sleep(GetPressureComputeTime());
            int retVal = ReadInt16FromRegister(Register.Data);
            retVal <<= 8;
            var extendedData = ReadByteFromRegister(Register.DataExt);
            retVal += extendedData;
            retVal >>= (8 - (byte)mode);

            return retVal;
        }

        public byte ChipId
        {
            get;
            private set;
        }

        public Bmp180CalibrationData CalibrationData
        {
            get
            {
                return calibrationData;
            }
        }

        public Bmp180Sensor(II2CBus i2cBus, int i2cAddress = Bmp180Sensor.DefaultI2CAddress, Bmp180Mode mode = Bmp180Mode.UltraLowPower)
        {
            this.i2cAddress = i2cAddress;
            this.mode = mode;
            this.i2cBus = i2cBus;
        }

        public void Connect()
        {           
            i2cDevice = i2cBus.AddDevice(i2cAddress);
            ChipId = ReadByteFromRegister(Register.ChipId);
            LoadCalibrationData();
        }
    

        public double GetPressure()
        {
            int rawTemp = ReadRawTemperature();
            int rawPress = ReadRawPressure();

            //temp compensation
            int compensatedTemp = ComputeB5(rawTemp);

            //pressure compensation according to datasheet
            int b6 = compensatedTemp - 4000;
            int x1 = (calibrationData.B2 * ((b6 << 1) >> 12)) >> 11;
            int x2 = (calibrationData.AC2 * b6) >> 11;
            int x3 = x1 + x2;
            int b3 = (((int)calibrationData.AC1 * 4 + x3) << (byte)mode + 2) >> 2;
            x1 = (calibrationData.AC3 * b6) >> 13;
            x2 = (calibrationData.B1 * ((b6 << 1) >> 12)) >> 16;
            x3 = (x1 + x2 + 2) >> 2;
            uint b4 = (calibrationData.AC4 * (uint)(x3 + 32768)) >> 15;
            uint b7 = (uint)((uint)(rawPress - b3) * (50000 >> (byte)mode));
            int p = (b7 < 0x80000000) ? ((int)((b7 << 1) / 4)) : ((int)((b7 / b4) << 1) ) ;
            x1 = (p >> 8) * (p >> 8);
            x1 = (x1 * 3038) >> 16;
            x2 = (-7357 * p) >> 16;
            p = p + (x1 + x2 + 3971) >> 4;

            return p;
        }

        public double GetTemperature()
        {
            double retVal = 0.0;

            var rawTempData = ReadRawTemperature();
            var B5 = ComputeB5(rawTempData);
            retVal = ((B5 + 8) >> 4);
            retVal /= 10.0;

            return retVal;
        }

        
    }
}

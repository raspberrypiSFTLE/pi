using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors
{
    public class Bmp180CalibrationData
    {
        public short AC1 { get; set; }
        public short AC2 { get; set; }
        public short AC3 { get; set; }
        public ushort AC4 { get; set; }
        public ushort AC5 { get; set; }
        public ushort AC6 { get; set; }
        public short B1 { get; set; }
        public short B2 { get; set; }
        public short MB { get; set; }
        public short MC { get; set; }
        public short MD { get; set; }

        public Bmp180CalibrationData()
        {
            //initialize with default
            AC1 = 408;
            AC2 = -72;
            AC3 = -14383;
            AC4 = 32741;
            AC5 = 32757;
            AC6 = 23153;
            B1 = 6190;
            B2 = 4;
            MB = -32768;
            MC = -8711;
            MD = 2868;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors
{
    public enum ProcessState
    {
        Sleep = 0,

        MotionDetected = 1,

        WaitingPersonDetection = 2,

        PersonRecognized = 3,

        PersonUnrecognized = 4
    }

    public interface ILEDs
    {
        void Update(ProcessState state);
    }
}

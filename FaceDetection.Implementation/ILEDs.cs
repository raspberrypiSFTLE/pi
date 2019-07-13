using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation
{
    public enum ProcessState
    {
        Sleep = 0,

        MotionDetected = 1,

        WaitingPersonDetection = 2,

        AllPersonsRecognized = 3,

        NoPersonRecognized = 4,

        PartialPersonsRecognized = 5
    }

    public interface ILEDs
    {
        void Update(ProcessState state);
    }
}

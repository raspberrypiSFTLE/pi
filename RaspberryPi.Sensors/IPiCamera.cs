using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors
{
    public interface IPiCamera
    {
        void StartCapturingImages();

        void StopCapturingImages();

        void CaptureStream();

        void StopStream();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Sensors
{
    public interface IPiCamera
    {
        void CaptureImage();

        void CaptureStream();

        void StopStream();
    }
}

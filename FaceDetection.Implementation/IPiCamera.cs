using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation
{
    public interface IPiCamera
    {
        void StartCapturingImages();

        void StopCapturingImages();

        void CaptureStream();

        void StopStream();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;

namespace RaspberryPi.Sensors
{
    public class PiCamera : IPiCamera
    {
        private int _imageCounter = 1;
        private int _snapshotInterval = 3; //in seconds

        private bool _captureImage = false;
        private static Timer _timer;

        public PiCamera(int snapshotInterval)
        {
            _snapshotInterval = snapshotInterval;
        }

        public void StopCapturingImages()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            _captureImage = false;
        }

        public void StartCapturingImages()
        {
            _captureImage = true;

            if (_timer != null)
            {
                return;
            }

            _timer = new Timer(
                   callback: new TimerCallback(TakeSnapshot),
                   state: null,
                   dueTime: 1000,
                   period: _snapshotInterval * 2000);
            
        }

        private void TakeSnapshot(object state)
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var captureName = $"image_{ _imageCounter}";
            var targetPath = $"/home/pi/camera-captures/{captureName}.jpg";

            _imageCounter++;
            //int capturesCount = Directory.GetFiles($"/home/pi/camera-captures/").Length;
            if (_imageCounter >= 99)
            {
                //reset counter
                _imageCounter = 1;
            }

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}. File name: {captureName}");
        }




        public void CaptureStream()
        {
        }

        public void StopStream()
        {
        }
    }
}

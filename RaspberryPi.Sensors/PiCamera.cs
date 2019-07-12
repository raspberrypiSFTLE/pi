using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unosquare.RaspberryIO;

namespace RaspberryPi.Sensors
{
    public class PiCamera : IPiCamera
    {
        private int _imageCounter = 1;

        public void CaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var captureName = $"image_{ _imageCounter}";
            var targetPath = $"/home/pi/camera-captures/{captureName}.jpg";

            _imageCounter++;
            //int capturesCount = Directory.GetFiles($"/home/pi/camera-captures/").Length;
            if (_imageCounter >= 100)
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

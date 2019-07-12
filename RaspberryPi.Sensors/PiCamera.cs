using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Camera;

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
            // Setup our working variables
            var videoByteCount = 0;
            var videoEventCount = 0;
            var startTime = DateTime.UtcNow;

            // Configure video settings
            var videoSettings = new CameraVideoSettings()
            {
                CaptureTimeoutMilliseconds = 0,
                CaptureDisplayPreview = false,
                ImageFlipVertically = true,
                CaptureExposure = CameraExposureMode.Night,
                CaptureWidth = 1920,
                CaptureHeight = 1080
            };

            try
            {
                // Start the video recording
                Pi.Camera.OpenVideoStream(videoSettings,
                    onDataCallback: (data) =>
                    {
                        videoByteCount += data.Length;
                        videoEventCount++;
                        Console.WriteLine($"Callback {data.Length}");
                    },
                    onExitCallback: null);

                // Wait for user interaction
                startTime = DateTime.UtcNow;
                Console.WriteLine("Press any key to stop reading the video stream . . .");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }
            finally
            {
                // Always close the video stream to ensure raspivid quits
                Pi.Camera.CloseVideoStream();

                // Output the stats
                var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
                var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
                Console.WriteLine($"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds");
            }
        }

        public void StopStream()
        {
        }
    }
}

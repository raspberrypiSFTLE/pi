using RaspberryPi.Sensors.DHTSensor;
using System;
using System.IO;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Camera;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Initialize WiringPi ...");
            Pi.Init<BootstrapWiringPi>();
            Console.WriteLine("Done");

            try
            {
                var piCamera = new PiCamera();
                var pir = new PirHC501(piCamera);
                pir.Start();
                Console.ReadKey();


                //Console.WriteLine("Select sensor:");
                //Console.WriteLine("[1]: BMP180");
                //Console.WriteLine("[2]: GY-30");
                //Console.WriteLine("[3]: DHT");
                //Console.WriteLine("[4]: Led");
                //Console.WriteLine("[5]: Button and Led");
                //Console.WriteLine("[6]: PWM");
                //Console.WriteLine("[7]: PWM");
                //Console.WriteLine("[8]: Pir");
                //Console.WriteLine("[9]: Camera");

                //var inputOption = Console.ReadLine();
                //var option = int.Parse(inputOption);

                //switch (option)
                //{
                //    case 1:
                //        RunBMP180();
                //        break;
                //    case 2:
                //        RunGy30();
                //        break;
                //    case 3:
                //        RunDHT11();
                //        break;
                //    case 4:
                //        LedStart();
                //        break;
                //    case 5:
                //        ButtonWithLed();
                //        break;
                //    case 6:
                //        HardwarePwm();
                //        break;
                //    case 7:
                //        SoftwarePwm();
                //        break;
                //    case 8:
                //        StartPir();
                //        break;
                //    case 9:
                //        StartCamera();
                //        break;
                //    case 10:
                //        LedRedRGB();
                //        break;
                //    case 11:
                //        TestCaptureVideo();
                //        break;
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed. " + e.Message);
            }
            // Console.Write
        }

        private static void RunBMP180()
        {
            Bmp180Sensor bmpSensor = new Bmp180Sensor(Pi.I2C, Bmp180Sensor.DefaultI2CAddress, Bmp180Mode.UltraLowPower);
            Console.Write("Connecting to the BMP 180 sensor ...");
            bmpSensor.Connect();
            Console.WriteLine("Done\n");
            Console.WriteLine("Calibration data: ");
            Console.WriteLine($"AC1     {bmpSensor.CalibrationData.AC1}");
            Console.WriteLine($"AC2     {bmpSensor.CalibrationData.AC2}");
            Console.WriteLine($"AC3     {bmpSensor.CalibrationData.AC3}");
            Console.WriteLine($"AC4     {bmpSensor.CalibrationData.AC4}");
            Console.WriteLine($"AC5     {bmpSensor.CalibrationData.AC5}");
            Console.WriteLine($"AC6     {bmpSensor.CalibrationData.AC6}");
            Console.WriteLine($"B1      {bmpSensor.CalibrationData.B1}");
            Console.WriteLine($"B2      {bmpSensor.CalibrationData.B2}");
            Console.WriteLine($"MB      {bmpSensor.CalibrationData.MB}");
            Console.WriteLine($"MC      {bmpSensor.CalibrationData.MC}");
            Console.WriteLine($"MD      {bmpSensor.CalibrationData.MD}");

            while (true)
            {
                Console.Write("Read Temperature ... ");
                var tempValue = bmpSensor.GetTemperature();
                Console.WriteLine($"Done. {tempValue} deg. Celsius");
                Console.Write("Read Pressure ... ");
                var pressure = bmpSensor.GetPressure();
                Console.WriteLine($"Done. {pressure} Pa");

                Thread.Sleep(1000);
            }
        }

        private static void RunGy30()
        {

            Bh1750FviSensor lightSensor = new Bh1750FviSensor(Pi.I2C, Bh1750FviSensor.DefaultI2CAddress);
            Console.Write("Connecting to light sensor...");
            lightSensor.Connect(Bh1750Mode.ContinuousStandardResolution);
            Console.WriteLine(" Done. ");
            while (true)
            {
                Console.WriteLine(" Reading value... ");
                var detectedLight = lightSensor.GetIluminance();
                Console.WriteLine($"Done. {detectedLight}");

                Thread.Sleep(2000);
            }
        }

        private static void RunDHT11()
        {
            try
            {
                var dht = new DHT((GpioPin)Pi.Gpio[BcmPin.Gpio18], DHTSensorTypes.DHT11);
                while (true)
                {
                    try
                    {
                        var d = dht.ReadData();
                        Console.WriteLine(DateTime.UtcNow);
                        Console.WriteLine(" temp: " + d.TempCelcius);
                        Console.WriteLine(" hum: " + d.Humidity);
                    }
                    catch (DHTException)
                    {
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " - " + e.StackTrace);
            }
        }

        private static void LedStart()
        {
            bool value = true;

            var blinkingPin = Pi.Gpio[18];
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            for (var i = 0; i < 20; i++)
            {
                value = !value;
                blinkingPin.Write(value);
                Thread.Sleep(500);
            }
        }

        private static void ButtonWithLed()
        {
            Console.WriteLine("Gpio Interrupts");
            var pin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            pin.PinMode = GpioPinDriveMode.Input;
            pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ButtpnPressedHandler);
            Console.ReadKey();
        }

        private static void ButtpnPressedHandler()
        {
            bool value = true;
            var blinkingPin = (GpioPin)Pi.Gpio[BcmPin.Gpio24];
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            for (var i = 0; i < 20; i++)
            {
                value = !value;
                blinkingPin.Write(value);
                System.Threading.Thread.Sleep(500);
            }
        }

        private static void HardwarePwm()
        {
            var greenPin = (GpioPin)Pi.Gpio[BcmPin.Gpio13];
            greenPin.PinMode = GpioPinDriveMode.PwmOutput;
            var redPin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            redPin.PinMode = GpioPinDriveMode.PwmOutput;

            while (true)
            {
                for (var x = 0; x <= 100; x++)
                {
                    redPin.PwmRegister = (int)redPin.PwmRange / 100 * x;
                    greenPin.PwmRegister = (int)greenPin.PwmRange - ((int)greenPin.PwmRange / 100 * x);
                    Thread.Sleep(10);
                }

                for (var x = 0; x <= 100; x++)
                {
                    redPin.PwmRegister = (int)redPin.PwmRange - ((int)redPin.PwmRange / 100 * x);
                    greenPin.PwmRegister = (int)greenPin.PwmRange / 100 * x;
                    Thread.Sleep(10);
                }
            }
        }

        private static void SoftwarePwm()
        {
            var range = 100;
            var redPin = (GpioPin)Pi.Gpio[BcmPin.Gpio18];
            redPin.PinMode = GpioPinDriveMode.Output;
            redPin.StartSoftPwm(0, range);

            var greenPin = (GpioPin)Pi.Gpio[BcmPin.Gpio13];
            greenPin.PinMode = GpioPinDriveMode.Output;
            greenPin.StartSoftPwm(0, range);

            var bluePin = (GpioPin)Pi.Gpio[BcmPin.Gpio24];
            bluePin.PinMode = GpioPinDriveMode.Output;
            bluePin.StartSoftPwm(0, range);

            while (true)
            {
                for (var x = 0; x <= 100; x++)
                {
                    redPin.SoftPwmValue = range / 100 * x;
                    greenPin.SoftPwmValue = range / 100 * x;
                    bluePin.SoftPwmValue = range / 100 * x;
                    Thread.Sleep(10);
                }

                for (var x = 0; x <= 100; x++)
                {
                    redPin.SoftPwmValue = range - (range / 100 * x);
                    greenPin.SoftPwmValue = range - (range / 100 * x);
                    bluePin.SoftPwmValue = range - (range / 100 * x);
                    Thread.Sleep(10);
                }
            }
        }

        private static void StartPir()
        {
            var piCamera = new PiCamera();
            var pir = new PirHC501(piCamera);
            pir.Start();
            Console.ReadKey();
        }

        private static void StartCamera()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var targetPath = $"/home/pi/camera-captures/image.jpg";
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");
        }

        private static void LedRedRGB()
        {
            bool value = true;

            var blinkingPin = Pi.Gpio[15];
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            for (var i = 0; i < 20; i++)
            {
                value = !value;
                blinkingPin.Write(value);
                Thread.Sleep(500);
            }
        }

        static void TestCaptureVideo()
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
    }
}

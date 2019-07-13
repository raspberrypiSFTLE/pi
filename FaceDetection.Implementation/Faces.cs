using System;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace FaceDetection.Implementation
{
    public class Faces
    {
        static Mat faceMask = Cv2.ImRead("Data/venice_mask.png");

        static Mat testImage = Cv2.ImRead("Data/testimg.jpg");

        private static Rect[] DetectFaceRectangles(Mat img, double scalingFactor)
        {
            var cascade = new CascadeClassifier("Models/haarcascade_frontalface_default.xml");
            img.Resize(Size.Zero, scalingFactor, scalingFactor, InterpolationFlags.Area);
            Mat grey = new Mat();
            Cv2.CvtColor(img, grey, ColorConversionCodes.BGR2GRAY);

            Rect[] faces = cascade.DetectMultiScale(grey, 1.3, 2, HaarDetectionType.ScaleImage, new Size(100, 100));
            return faces;
        }

        public static bool IsDetectedFace(string imagePath, double scalingFactor = 0.5)
        {
            var imageMat = Cv2.ImRead(imagePath);
            return DetectFacesRectanglesInternal(imageMat, scalingFactor);
        }

        public static bool IsDetectedFace(byte[] imageData, double scalingFactor = 0.5)
        {
            var imageDataClone = imageData.ToArray();
            using (var ms = new MemoryStream(imageDataClone))
            {
                var imageMat = BitmapConverter.ToMat(new System.Drawing.Bitmap(ms));
                return DetectFacesRectanglesInternal(imageMat, scalingFactor);
            }
        }

        private static bool DetectFacesRectanglesInternal(Mat imageMat, double scalingFactor)
        {
            var faces = DetectFaceRectangles(imageMat, scalingFactor);
            return faces.Length > 0;
        }

        public static void StartVideoCapture()
        {
            VideoCapture capture = new VideoCapture(0);

            using (Window window = new Window("Camera"))
            using (Mat image = new Mat()) // Frame image buffer
            {
                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    capture.Read(image);
                    if (image.Empty()) break;
                    UpdateImageWithDetectedFaces(image);
                    window.ShowImage(image);
                    Cv2.WaitKey(20);
                }
            }
        }

        public static void UpdateImageWithDetectedFaces(Mat img)
        {
            var faces = DetectFaceRectangles(img, 0.5);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, face, Scalar.Green, 3);
            }
        }

        public static void UpdateImageWithDetectedFacesFunny(Mat img)
        {
            var faces = DetectFaceRectangles(img, 0.5);
            foreach (var face in faces)
            {
                FunnyFaces(img);
            }
        }

        public static void FunnyFaces(Mat img)
        {
            var cascade = new CascadeClassifier("Models/haarcascade_frontalface_default.xml");
            Mat grey = new Mat();
            Cv2.CvtColor(img, grey, ColorConversionCodes.BGR2GRAY);

            Rect[] faces = cascade.DetectMultiScale(grey, 1.5, 5, HaarDetectionType.ScaleImage, new Size(100, 100));
            foreach (var face in faces)
            {
                try
                {
                    if (face.X > 0 && face.Y > 0)
                    {
                        int h = Convert.ToInt32(1.0 * face.Height);
                        int w = Convert.ToInt32(1.0 * face.Width);
                        int shiftVertical = Convert.ToInt32(0.1 * h);
                        int y = face.Y - shiftVertical;
                        var x = face.X;
                        var frameRoi = img.SubMat(y, y + h, x, x + w);
                        var faceMaskSmall = new Mat();
                        var s = new Size(w, h);
                        Cv2.Resize(faceMask, faceMaskSmall, s, 0, 0, InterpolationFlags.Area);
                        Mat greyMask = new Mat();
                        Cv2.CvtColor(faceMaskSmall, greyMask, ColorConversionCodes.BGR2GRAY);
                        Mat mask = new Mat();
                        Cv2.Threshold(greyMask, mask, 180, 255, ThresholdTypes.BinaryInv);
                        // from now on we are playing with the hanibal mask greyed out
                        Mat maskInv = new Mat();
                        Cv2.BitwiseNot(mask, maskInv);
                        Mat maskedFace = new Mat();
                        // 
                        Cv2.BitwiseAnd(faceMaskSmall, faceMaskSmall, maskedFace, mask);
                        Mat maskedFrame = new Mat();
                        Cv2.BitwiseAnd(frameRoi, frameRoi, maskedFrame);
                        var maskRectangle = new Mat();
                        Cv2.Add(maskedFace, frameRoi, maskRectangle);
                        img[y, y + maskRectangle.Height, face.X, face.X + maskRectangle.Width] = maskRectangle;

                    }
                }
                catch
                {
                    // TODO: update code to set position properly
                    Console.WriteLine("Out of range!");
                }
            }

        }
    }
}

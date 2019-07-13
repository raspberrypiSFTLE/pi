using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation.Models
{
    public class FaceAttributes
    {
        public double smile { get; set; }
        public string gender { get; set; }
        public double age { get; set; }
        public FacialHair facialHair { get; set; }
        public string glasses { get; set; }
        public Emotion emotion { get; set; }
        public Blur blur { get; set; }
        public Exposure exposure { get; set; }
        public Noise noise { get; set; }
        public Makeup makeup { get; set; }
        public List<Accessory> accessories { get; set; }
    }
}

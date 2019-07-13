using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FaceDetection.Implementation
{
    public class EmotionReplies
    {
        public List<string> Anger { get; set; }
        public List<string> Contempt { get; set; }
        public List<string> Disgust { get; set; }
        public List<string> Fear { get; set; }
        public List<string> Hapiness { get; set; }
        public List<string> Neutral { get; set; }
        public List<string> Sadness { get; set; }
        public List<string> Surprise { get; set; }
        public EmotionReplies()
        {
            Anger = File.ReadAllLines("replies/emotion/anger.txt").ToList();
            Contempt = File.ReadAllLines("replies/emotion/contempt.txt").ToList();
            Disgust = File.ReadAllLines("replies/emotion/disgust.txt").ToList();
            Fear = File.ReadAllLines("replies/emotion/fear.txt").ToList();
            Hapiness = File.ReadAllLines("replies/emotion/hapiness.txt").ToList();
            Neutral = File.ReadAllLines("replies/emotion/neutral.txt").ToList();
            Sadness = File.ReadAllLines("replies/emotion/sadness.txt").ToList();
            Surprise = File.ReadAllLines("replies/emotion/surprise.txt").ToList();
        }
    }
}

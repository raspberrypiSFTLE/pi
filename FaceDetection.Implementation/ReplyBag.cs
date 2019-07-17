using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FaceDetection.Implementation
{
    public class ReplyBag
    {
        public List<string> AgeOlderReplies { get; set; }
        public List<string> AgeYoungerReplies { get; set; }
        public List<string> Chitchat { get; set; }
        public List<string> Explain { get; set; }
        public List<string> GenderFemaleReplies { get; set; }
        public List<string> GenderMaleReplies { get; set; }
        public List<string> Glasses { get; set; }
        public List<string> Goodbye { get; set; }
        public List<string> Greetings { get; set; }
        public List<string> Intro { get; set; }
        public List<string> Puns { get; set; }
        public List<string> Identify { get; set; }
        public EmotionReplies EmotionReplies { get; set; }
        public List<string> AllMaleReplies { get; set; }
        public List<string> AllFemaleReplies { get; set; }
        public List<string> UnrecognizedOneReplies { get; set; }
        public List<string> UnrecognizedMultipleReplies { get; set; }
        public Dictionary<string, string> PersonalReplies { get; set; }

        public ReplyBag()
        {
            AgeOlderReplies = File.ReadAllLines("replies/ageolder.txt").ToList();
            AgeYoungerReplies = File.ReadAllLines("replies/ageyounger.txt").ToList();
            Chitchat = File.ReadAllLines("replies/chitchat.txt").ToList();
            Explain = File.ReadAllLines("replies/explain.txt").ToList();
            GenderFemaleReplies = File.ReadAllLines("replies/genderfemale.txt").ToList();
            GenderMaleReplies = File.ReadAllLines("replies/gendermale.txt").ToList();
            Glasses = File.ReadAllLines("replies/glasses.txt").ToList();
            Goodbye = File.ReadAllLines("replies/goodbye.txt").ToList();
            Greetings = File.ReadAllLines("replies/greetings.txt").ToList();
            Intro = File.ReadAllLines("replies/intro.txt").ToList();
            Puns = File.ReadAllLines("replies/puns.txt").ToList();
            Identify = File.ReadAllLines("replies/identify.txt").ToList();
            AllMaleReplies = File.ReadAllLines("replies/allmale.txt").ToList();
            AllFemaleReplies = File.ReadAllLines("replies/allfemale.txt").ToList();
            UnrecognizedOneReplies = File.ReadAllLines("replies/unrecognizedperson.txt").ToList();
            UnrecognizedMultipleReplies = File.ReadAllLines("replies/unrecognizedpersonmultiple.txt").ToList();

            EmotionReplies = new EmotionReplies();

            PersonalReplies = new Dictionary<string, string>();
            var lines = File.ReadAllLines("replies/personalreplies.txt").ToList();
            foreach (var line in lines)
            {
                var items = line.Split('#');
                PersonalReplies.Add(items[0], items[1]);
            }
            Console.WriteLine("replies loaded");
            Console.WriteLine("AgeOlderReplies " + AgeOlderReplies.Count);
            Console.WriteLine("EmotionRepliesContempt " + EmotionReplies.Contempt.Count);
            Console.WriteLine("PersonalReplies " + PersonalReplies.Count);
        }
    }
}

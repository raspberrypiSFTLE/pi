using FaceDetection.Implementation.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation
{
    public class ReplyBuilder
    {
        private readonly ReplyBag replyBag;
        private readonly Random random;
        private readonly int neutralEmotionPercentage = 70;
        private readonly int ageOlderPercentage = 50;
        private readonly int ageYoungerPercentage = 80;
        private readonly int genderFemalePercentage = 60;
        private readonly int genderMalePercentage = 40;

        public ReplyBuilder()
        {
            replyBag = new ReplyBag();
            random = new Random();
        }
        public List<Reply> BuildReplies(Person faceInformation)
        {
            var replies = new List<Reply>();
            string textEN;
            int randomValue;

            //custom reply for the special ones
            if (replyBag.PersonalReplies.ContainsKey(faceInformation.match.personId))
            {
                replies.Add(new Reply
                {
                    Text = replyBag.PersonalReplies.GetValueOrDefault(faceInformation.match.personId),
                    Language = "ro-RO"
                });

                return replies;
            }
            else
            {
                textEN = replyBag.Greetings[random.Next(0, replyBag.Greetings.Count - 1)];

                textEN += "... " + replyBag.Identify[random.Next(0, replyBag.Identify.Count - 1)] + " " + faceInformation.match.name;

                if (faceInformation.faceAttributes.age > faceInformation.match.age)
                {
                    randomValue = random.Next(100);
                    if (randomValue < ageOlderPercentage)
                    {
                        textEN += "... " + replyBag.AgeOlderReplies[random.Next(0, replyBag.AgeOlderReplies.Count - 1)];
                    }
                }
                else
                {
                    randomValue = random.Next(100);
                    if (randomValue < ageYoungerPercentage)
                    {
                        textEN += ". " + replyBag.AgeYoungerReplies[random.Next(0, replyBag.AgeYoungerReplies.Count - 1)];
                    }
                }

                if (faceInformation.faceAttributes.glasses != "NoGlasses")
                {
                    textEN += "... " + replyBag.Glasses[random.Next(0, replyBag.Glasses.Count - 1)];
                }

                if (faceInformation.faceAttributes.gender == "male")
                {
                    randomValue = random.Next(100);
                    if (randomValue < genderMalePercentage)
                    {
                        textEN += "... " + replyBag.GenderMaleReplies[random.Next(0, replyBag.GenderMaleReplies.Count - 1)];
                    }
                }
                else
                {
                    if (faceInformation.faceAttributes.gender == "female")
                    {
                        randomValue = random.Next(100);
                        if (randomValue < genderFemalePercentage)
                        {
                            textEN += "... " + replyBag.GenderFemaleReplies[random.Next(0, replyBag.GenderFemaleReplies.Count - 1)];
                        }
                    }
                }

                //Emotions
                if (faceInformation.faceAttributes.emotion.anger > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Anger[random.Next(0, replyBag.EmotionReplies.Anger.Count - 1)];
                }

                if (faceInformation.faceAttributes.emotion.contempt > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Contempt[random.Next(0, replyBag.EmotionReplies.Contempt.Count - 1)];
                }

                if (faceInformation.faceAttributes.emotion.disgust > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Disgust[random.Next(0, replyBag.EmotionReplies.Disgust.Count - 1)];
                }

                randomValue = random.Next(100);
                if (randomValue < neutralEmotionPercentage)
                {
                    if (faceInformation.faceAttributes.emotion.neutral > 0.5)
                    {
                        textEN += "... " + replyBag.EmotionReplies.Neutral[random.Next(0, replyBag.EmotionReplies.Neutral.Count - 1)];
                    }
                }

                if (faceInformation.faceAttributes.emotion.fear > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Fear[random.Next(0, replyBag.EmotionReplies.Fear.Count - 1)];
                }

                if (faceInformation.faceAttributes.emotion.surprise > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Surprise[random.Next(0, replyBag.EmotionReplies.Surprise.Count - 1)];
                }

                if (faceInformation.faceAttributes.emotion.sadness > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Sadness[random.Next(0, replyBag.EmotionReplies.Sadness.Count - 1)];
                }

                if (faceInformation.faceAttributes.emotion.happiness > 0.5)
                {
                    textEN += "... " + replyBag.EmotionReplies.Hapiness[random.Next(0, replyBag.EmotionReplies.Hapiness.Count - 1)];
                }

                replies.Add(new Reply
                {
                    Text = textEN,
                    Language = "en-US"
                });

            }

            return replies;
        }
    }
}

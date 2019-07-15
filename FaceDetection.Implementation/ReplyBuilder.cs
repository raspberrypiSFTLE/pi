using FaceDetection.Implementation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceDetection.Implementation
{
    public class ReplyBuilder
    {
        private readonly ReplyBag _replyBag;
        private readonly Random random;
        private readonly int neutralEmotionPercentage = 70;
        private readonly int ageOlderPercentage = 50;
        private readonly int ageYoungerPercentage = 80;
        private readonly int genderFemalePercentage = 60;
        private readonly int genderMalePercentage = 40;

        public ReplyBuilder(ReplyBag replyBag)
        {
            _replyBag = replyBag;
            random = new Random();
        }
        public List<Reply> BuildReplies(List<Person> persons)
        {
            var replies = new List<Reply>();
            string textEN = "";
            int randomValue;

            var recognizedPersonCount = persons.Count(p => !p.Unrecognized);
            var unrecognizedPersonCount = persons.Count(p => p.Unrecognized);

            if (persons.All(p => p.Unrecognized) || !persons.Any())
            {
                textEN = _replyBag.Greetings[random.Next(0, _replyBag.Greetings.Count - 1)];

                replies.Add(new Reply
                {
                    Text = textEN + ". I don't know who you are",
                    Language = "en-US"
                });

                return replies;
            }

            if (recognizedPersonCount > 1)
            {
                textEN = _replyBag.Greetings[random.Next(0, _replyBag.Greetings.Count - 1)];

                if (recognizedPersonCount > 1)
                {

                    textEN += "... " + _replyBag.Identify[random.Next(0, _replyBag.Identify.Count - 1)];
                    for (int i = 0; i < persons.Count; i++)
                    {
                        if (i == persons.Count - 1)
                        {
                            textEN += "and " + persons[i].match.name;
                        }
                        else
                        {
                            textEN += ", " + persons[i].match.name;
                        }
                    }

                    if (unrecognizedPersonCount > 1)
                    {
                        replies.Add(new Reply
                        {
                            Text = textEN + ". The rest of you guys I don't know",
                            Language = "en-US"
                        });
                    }
                    else if(unrecognizedPersonCount == 1)
                    {
                        replies.Add(new Reply
                        {
                            Text = textEN + ". I don't know the other person. Are you sure you're in the right place?",
                            Language = "en-US"
                        });
                    }

                    replies.Add(new Reply
                    {
                        Text = textEN,
                        Language = "en-US"
                    });

                    return replies;
                }

            }
            else if (recognizedPersonCount == 1)
            {
                var faceInformation = persons[0];

                //custom reply for the special ones
                if (_replyBag.PersonalReplies.ContainsKey(faceInformation.match.personId))
                {
                    replies.Add(new Reply
                    {
                        Text = _replyBag.PersonalReplies.GetValueOrDefault(faceInformation.match.personId),
                        Language = "ro-RO"
                    });

                }
                else
                {
                    textEN = _replyBag.Greetings[random.Next(0, _replyBag.Greetings.Count - 1)];

                    textEN += "... " + _replyBag.Identify[random.Next(0, _replyBag.Identify.Count - 1)] + " " + faceInformation.match.name;

                    if (faceInformation.faceAttributes.age > faceInformation.match.age)
                    {
                        randomValue = random.Next(100);
                        if (randomValue < ageOlderPercentage)
                        {
                            textEN += "... " + _replyBag.AgeOlderReplies[random.Next(0, _replyBag.AgeOlderReplies.Count - 1)];
                        }
                    }
                    else
                    {
                        randomValue = random.Next(100);
                        if (randomValue < ageYoungerPercentage)
                        {
                            textEN += ". " + _replyBag.AgeYoungerReplies[random.Next(0, _replyBag.AgeYoungerReplies.Count - 1)];
                        }
                    }

                    if (faceInformation.faceAttributes.glasses != "NoGlasses")
                    {
                        textEN += "... " + _replyBag.Glasses[random.Next(0, _replyBag.Glasses.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.gender == "male")
                    {
                        randomValue = random.Next(100);
                        if (randomValue < genderMalePercentage)
                        {
                            textEN += "... " + _replyBag.GenderMaleReplies[random.Next(0, _replyBag.GenderMaleReplies.Count - 1)];
                        }
                    }
                    else
                    {
                        if (faceInformation.faceAttributes.gender == "female")
                        {
                            randomValue = random.Next(100);
                            if (randomValue < genderFemalePercentage)
                            {
                                textEN += "... " + _replyBag.GenderFemaleReplies[random.Next(0, _replyBag.GenderFemaleReplies.Count - 1)];
                            }
                        }
                    }

                    //Emotions
                    if (faceInformation.faceAttributes.emotion.anger > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Anger[random.Next(0, _replyBag.EmotionReplies.Anger.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.emotion.contempt > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Contempt[random.Next(0, _replyBag.EmotionReplies.Contempt.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.emotion.disgust > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Disgust[random.Next(0, _replyBag.EmotionReplies.Disgust.Count - 1)];
                    }

                    randomValue = random.Next(100);
                    if (randomValue < neutralEmotionPercentage)
                    {
                        if (faceInformation.faceAttributes.emotion.neutral > 0.5)
                        {
                            textEN += "... " + _replyBag.EmotionReplies.Neutral[random.Next(0, _replyBag.EmotionReplies.Neutral.Count - 1)];
                        }
                    }

                    if (faceInformation.faceAttributes.emotion.fear > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Fear[random.Next(0, _replyBag.EmotionReplies.Fear.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.emotion.surprise > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Surprise[random.Next(0, _replyBag.EmotionReplies.Surprise.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.emotion.sadness > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Sadness[random.Next(0, _replyBag.EmotionReplies.Sadness.Count - 1)];
                    }

                    if (faceInformation.faceAttributes.emotion.happiness > 0.5)
                    {
                        textEN += "... " + _replyBag.EmotionReplies.Hapiness[random.Next(0, _replyBag.EmotionReplies.Hapiness.Count - 1)];
                    }

                    textEN += "... " + _replyBag.Goodbye[random.Next(0, _replyBag.Goodbye.Count - 1)];

                    replies.Add(new Reply
                    {
                        Text = textEN,
                        Language = "en-US"
                    });
                }

                if (unrecognizedPersonCount > 1)
                {
                    replies.Add(new Reply
                    {
                        Text = textEN + ". The rest of you guys I don't know",
                        Language = "en-US"
                    });
                }
                else if (unrecognizedPersonCount == 1)
                {
                    replies.Add(new Reply
                    {
                        Text = textEN + ". I don't know the other person. Are you sure you're in the right place?",
                        Language = "en-US"
                    });
                }

                return replies;
            }

            return replies;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation.Models
{
    public class Person
    {
        public string faceId { get; set; }
        public FaceAttributes faceAttributes { get; set; }
        public PersonMatch match { get; set; }

        public override string ToString()
        {
            return match != null ? $"Name: '{ match.name }', Age: '{ match.age }'"
                : "Unrecognized person";
        }

        public bool Unrecognized
        {
            get
            {
                return match == null || string.IsNullOrWhiteSpace(match.name);
            }
        }
    }
}

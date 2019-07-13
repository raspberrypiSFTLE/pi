using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation
{
    public class Person
    {
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

    public class PersonMatch
    {
        public string name { get; set; }
        public int age { get; set; }

        public string nickname { get; set; }
    }
}

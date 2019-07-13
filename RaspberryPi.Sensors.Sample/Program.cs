using System;
using System.IO;
using System.Linq;
using Unosquare.RaspberryIO;
using Unosquare.Swan;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] imageContent = File.ReadAllBytes("C:\\Users\\olv\\Downloads\\vedete.jpg");
            //var personNames = new IdentifyPerson().IdentifyPersonAsync(imageContent).GetAwaiter().GetResult();
            //Console.WriteLine($"Persons in capture:\n Person: {String.Join(";\n Person: ", personNames.Select(p => p.ToString()).ToArray())}");

            //Console.ReadKey();
            //return;

            Pi.Init<BootstrapWiringPi>();
            int snapshotIntervalSeconds = 9;
            foreach (var argument in args)
            {
                var keyValueArgument = argument.Split(':');
                if (keyValueArgument.Length == 2)
                {
                    if (keyValueArgument[0] == "snapshotInterval")
                    {
                        int parsedValue = 0;
                        int.TryParse(keyValueArgument[1], out parsedValue);
                        snapshotIntervalSeconds = Math.Max(snapshotIntervalSeconds, parsedValue);
                        Console.WriteLine(snapshotIntervalSeconds);
                    }
                }
            }

            var processManager = new ProcessManager(snapshotIntervalSeconds);
            processManager.Start();

            Console.ReadKey();
        }
    }
}

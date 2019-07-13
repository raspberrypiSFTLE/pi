using System;
using System.IO;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] imageContent = File.ReadAllBytes("C:\\Users\\olv\\Downloads\\image_1.jpg");
            //var personName = new IdentifyPerson().IdentifyPersonAsync(imageContent).GetAwaiter().GetResult();
            //Console.WriteLine($"person: {personName}");

            //Console.ReadKey();
            //return;

            Pi.Init<BootstrapWiringPi>();
            int snapshotIntervalSeconds = 5;
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

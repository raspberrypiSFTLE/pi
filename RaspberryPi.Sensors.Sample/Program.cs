using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Camera;
using Unosquare.Swan.Formatters;
using Unosquare.WiringPi;

namespace RaspberryPi.Sensors.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            int snapshotIntervalSeconds = 2;
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

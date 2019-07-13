using FaceDetection.Implementation;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Linq;
using Unosquare.RaspberryIO;
using Unosquare.Swan;
using Unosquare.WiringPi;

namespace FaceDetection
{
    class Program
    {
        private static IMemoryCache _cache;
        private static ReplyBag replyBag;

        static void Main(string[] args)
        {
            //byte[] imageContent = File.ReadAllBytes("C:\\Users\\olv\\Downloads\\vedete.jpg");
            //var personNames = new IdentifyPerson().IdentifyPersonAsync(imageContent).GetAwaiter().GetResult();
            //Console.WriteLine($"Persons in capture:\n Person: {String.Join(";\n Person: ", personNames.Select(p => p.ToString()).ToArray())}");

            //Console.ReadKey();
            //return;

            Pi.Init<BootstrapWiringPi>();
            int snapshotIntervalSeconds = 0;
            foreach (var argument in args)
            {
                var keyValueArgument = argument.Split(':');
                if (keyValueArgument.Length == 2)
                {
                    if (keyValueArgument[0] == "snapshotInterval")
                    {
                        int parsedValue = 0;
                        int.TryParse(keyValueArgument[1], out parsedValue);
                        snapshotIntervalSeconds = parsedValue;
                        Console.WriteLine(snapshotIntervalSeconds);
                    }
                }
            }

            _cache = new MemoryCache(new MemoryCacheOptions());
            replyBag = new ReplyBag();
            TTSBuilder ttsBuilder = new TTSBuilder(_cache);
            SoundPlayer soundPlayer = new SoundPlayer();
            ReplyBuilder replyBuilder = new ReplyBuilder(replyBag);
            var processManager = new ProcessManager(snapshotIntervalSeconds, _cache, replyBag, ttsBuilder, soundPlayer, replyBuilder);
            processManager.Start();

            Console.ReadKey();
        }
    }
}

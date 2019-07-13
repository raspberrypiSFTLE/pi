using System;
using System.Collections.Generic;
using System.Text;

namespace FaceDetection.Implementation
{
    public class SoundPlayer
    {
        public void PlayOnWindows(string wavFileName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C powershell -c (New-Object Media.SoundPlayer \"{wavFileName}.wav\").PlaySync()"
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public void PlayOnPi(string wavFileName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "aplay",
                Arguments = $"{wavFileName}.wav"
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}

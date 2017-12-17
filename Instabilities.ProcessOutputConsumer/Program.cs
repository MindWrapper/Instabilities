using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Instabilities.ProcessOutputConsumer
{

    public class Program
    {
        static void Main()
        {
            ProduceAndConsumeMessagesUsingQueue(1000);
            ProduceAndConsumeMessagesUsingQueue(2000);
            ProduceAndConsumeMessagesUsingQueue(3000);
        }

        static void ProduceAndConsumeMessagesUsingQueue(int expectedLinesCount)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "Instabilities.ProcessOutputProducer.exe",
                Arguments = $"{expectedLinesCount}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var p = new Process
            {
                StartInfo = startInfo
            };

            var processOutout = new Queue<string>();
            p.OutputDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrEmpty(eventArgs.Data))
                {
                    processOutout.Enqueue(eventArgs.Data);
                }
            };

            p.Start();
            p.BeginOutputReadLine();

            var done = false;
            var actualMessageCount = 0;
            while (!done)
            {
                string msg;
                while (processOutout.Count > 0  && (msg = processOutout.Dequeue()) != null)
                {
                    if (msg == "Done")
                    {
                        done = true;
                        break;
                    }
                    ++actualMessageCount;
                }
                Thread.Sleep(10);
            }

            p.WaitForExit();
            p.Close();
            Console.WriteLine($"Lines read: {actualMessageCount} Expected: {expectedLinesCount}");
        }

    }
}

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
            ProduceAndConsumeMessages(100);
            ProduceAndConsumeMessages(1000);
            ProduceAndConsumeMessages(10000);
        }

        static void ProduceAndConsumeMessages(int expectedLinesCount)
        {
            var processOutput = StartProducerProcess(expectedLinesCount);
            var actualMessageCount = CountProducedLines(processOutput);
            Console.WriteLine($"Lines read: {actualMessageCount} Expected: {expectedLinesCount}");
        }

        static Queue<string> StartProducerProcess(int expectedLinesCount)
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
            return processOutout;
        }

        static int CountProducedLines(Queue<string> processOutput)
        {
            var done = false;
            var actualMessageCount = 0;
            while (!done)
            {
                string msg;
                while (processOutput.Count > 0 && (msg = processOutput.Dequeue()) != null)
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
            return actualMessageCount;
        }
    }
}

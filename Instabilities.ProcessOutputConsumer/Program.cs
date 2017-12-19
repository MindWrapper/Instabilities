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
            for (var i = 0; i < 100; i++)
            {
                ProduceAndConsumeMessages(1000);
            }
        }

        static void ProduceAndConsumeMessages(int expectedLinesCount)
        {
            var processOutputQueue = new Queue<string>();

            // starts producer process which redirects output to processOutputQueue
            var process = StartProducerProcess(expectedLinesCount, processOutputQueue);

            // returns number of messages read from processOutputQueue
            var actualMessageCount = CountProducedLines(ref processOutputQueue);
            Console.WriteLine($"Lines read: {actualMessageCount} Expected: {expectedLinesCount}");
            if (actualMessageCount != expectedLinesCount)
            {
                Console.WriteLine("Instability found!");
            }
            process.WaitForExit(-1);
        }

        static Process StartProducerProcess(int expectedLinesCount, Queue<string> processOutputQueue)
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

            p.OutputDataReceived += (sender, eventArgs) =>
            {
                processOutputQueue.Enqueue(eventArgs.Data);
            };

            p.Start();
            p.BeginOutputReadLine();
            return p;
        }

        static int CountProducedLines(ref Queue<string> processOutputQueue)
        {
            var done = false;
            var linesCount = 0;
            while (!done)
            {
                string msg;
                while (processOutputQueue.Count > 0 && (msg = processOutputQueue.Dequeue()) != null)
                {
                    if (msg == "Done")
                    {
                        done = true;
                        break;
                    }
                    ++linesCount;
                }
                Thread.Sleep(10);
            }
            return linesCount;
        }
    }
}

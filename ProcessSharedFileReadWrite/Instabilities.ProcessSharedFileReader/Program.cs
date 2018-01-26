using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Instabilities.ProcessSharedFileReader
{
    class Program
    {
        static void Main()
        {
            const int linesCount = 1000;
            const int lineLength = 100;
            var fileName = Path.GetTempFileName();
            try
            {
                StartWriterProcessAndVerifyOutput(linesCount, lineLength, fileName);
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        private static void StartWriterProcessAndVerifyOutput(int linesCount, int lineLenght, string fileName)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "Instabilities.ProcessSharedFileWriter.exe",
                Arguments = $"{linesCount} {lineLenght} \"{fileName}\"",
                UseShellExecute = true,
            };
            var p = Process.Start(startInfo);
            // give a start process some time to start write to file
            Thread.Sleep(1000);
            var s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var readLinesCount = 0;
            var incompleteLines = 0;
            using (var streamReader = new StreamReader(s))
            {
                while (true)
                {
                    var line = streamReader.ReadLine();
                    if (line == null)
                    {
                        var aditionalDataCanBeExpected = !p.HasExited;
                        if (aditionalDataCanBeExpected)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        break;
                    }
                    ++readLinesCount;
                    if (line.Length != lineLenght)
                    {
                        ++incompleteLines;
                        Console.WriteLine("Incomplete line found");
                    }
                }
            }
            Console.WriteLine($"Read lines count: {readLinesCount} Expected {linesCount}. Incomplete lines count {incompleteLines}");
            p.WaitForExit();   
        }
    }
}

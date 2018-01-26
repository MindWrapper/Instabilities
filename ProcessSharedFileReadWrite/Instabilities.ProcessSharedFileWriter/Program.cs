using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Instabilities.ProcessSharedFileWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            var linesCount = int.Parse(args[0]);
            var lineLength = int.Parse(args[1]);
            var fileName = args[2];
            ProduceOutput(linesCount, lineLength, fileName);
        }

        private static void ProduceOutput(int linesCount, int lineLenght, string outpath)
        {
            var random = new Random();
            using (var fs = new FileStream(outpath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var line = ToUtf8( new string('A', lineLenght));
                var sw = new StreamWriter(fs, Encoding.UTF8);
                for (var i = 0; i < linesCount; ++i) 
                {
                    Thread.Sleep(random.Next(0, 5));
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }

        static string ToUtf8(string str)
        {
           var bytes = Encoding.Default.GetBytes(str);
           return Encoding.UTF8.GetString(bytes);
        }
    }
}

using System;

namespace Instabilities.ProcessOutputProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var count = int.Parse(args[0]);
            
            for (var i = 0; i < count; ++i)
            {
                Console.WriteLine($"Message #{i}");
            }

            Console.WriteLine("Done");
        }
    }
}

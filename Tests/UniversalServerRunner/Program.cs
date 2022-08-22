using System;
using System.IO;
using System.Linq;
using System.Threading;
using Applitools;

namespace UniversalServerRunner
{
    class Program
    {
        private static int _port;

        static void Main(string[] args)
        {
            _port = UniversalSdkNativeLoader.Start();

            int timeoutMilliseconds;
            if (int.TryParse(args.FirstOrDefault(), out timeoutMilliseconds))
            {
                File.WriteAllText(@"C:\Users\EP\Documents\1.txt", timeoutMilliseconds.ToString());
                Thread.Sleep(timeoutMilliseconds);
                return;
            }

            File.WriteAllText(@"C:\Users\EP\Documents\1.txt", "ReadKey");
            Console.ReadKey();
        }
    }
}

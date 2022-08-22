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
                Thread.Sleep(timeoutMilliseconds);
                return;
            }

            Console.ReadKey();
        }
    }
}

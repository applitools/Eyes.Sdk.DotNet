using System;
using System.Threading;
using Applitools;

namespace UniversalServerRunner
{
    class Program
    {
        private static int _port;

        static void Main(string[] args)
        {
            Console.WriteLine("Started!");

            _port = UniversalSdkNativeLoader.Start();

            Thread.Sleep(30000);

            Environment.Exit(0);
        }
    }
}

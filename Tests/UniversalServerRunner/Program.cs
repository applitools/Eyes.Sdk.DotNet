using System;
using Applitools;

namespace UniversalServerRunner
{
    class Program
    {
        private static int _port;

        static void Main(string[] args)
        {
            _port = UniversalSdkNativeLoader.Start();

            Console.WriteLine("Runner Started");

            Console.ReadLine();
        }
    }
}

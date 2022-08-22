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
            
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}

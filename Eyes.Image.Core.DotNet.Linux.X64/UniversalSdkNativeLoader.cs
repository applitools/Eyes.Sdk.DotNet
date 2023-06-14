using System;
using System.Diagnostics;
using System.IO;

namespace Applitools
{
    public static class UniversalSdkNativeLoader
    {
        private static int port_;
        private static Process nativeProc_;
        private static readonly object lockObject_ = new object();

        private static readonly string universalDebug_ =
            "true" == Environment.GetEnvironmentVariable("APPLITOOLS_UNIVERSAL_DEBUG")?.ToLower() ? "--debug" : "";

        public static int Start()
        {
            return TryStartProcess_();
        }

        private static int TryStartProcess_()
        {
            lock (lockObject_)
            {
                if (nativeProc_ == null || nativeProc_.HasExited)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    const string os = "linux-x64";
                    const string suffix = "linux";

                    var path = GetServerFilePath(basePath, os, suffix);
                    CommandExecutor.ExecCommand($"chmod 755 {path}");

                    if (File.Exists(path) == false)
                    {
                        throw new Exception($"Universal Server file '{path}' does not exist.");
                    }

                    nativeProc_ = StartProcess(path);

                    port_ = GetPort(nativeProc_);

                    Debug.WriteLine($"Universal Server process ID: {nativeProc_.Id}.");
                }
            }

            return port_;
        }

        private static string GetServerFilePath(string basePath, string os, string suffix)
        {
            return Path.Combine(Path.GetDirectoryName(basePath), "runtimes", os, "native", "core-" + suffix);
        }

        private static Process StartProcess(string path)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(path,
                    $"universal --no-singleton --shutdown-mode stdin {universalDebug_}")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Start();

            return process;
        }

        private static int GetPort(Process process)
        {
            string portStr;
            try
            {
                portStr = process.StandardOutput.ReadLine();
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Unable to read Universal Server StandardOutput. Process HasExited: {process.HasExited}", e);
            }

            if (int.TryParse(portStr, out int port))
            {
                return port;
            }

            throw new Exception($"'{portStr}' is not valid port number");
        }
    }
}
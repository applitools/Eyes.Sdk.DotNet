using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Applitools
{
    using System.Runtime.InteropServices;

    public class UniversalSdkNativeLoader
    {
        private static Process nativeProc_;

        public static void Start()
        {
            TryStartProcess_();
        }

        public void Stop()
        {
            TryStopProcess_();
        }

        public bool IsRunning => nativeProc_ != null && !nativeProc_.HasExited;

        private static void TryStartProcess_()
        {
            if (nativeProc_ == null || nativeProc_.HasExited)
            {
                string basePath = Assembly.GetExecutingAssembly().Location;
                string path;
                string os, suffix;
                if (OperatingSystemChecker.IsWindows())
                {
                    os = "win-x64";
                    suffix = "win.exe";
                    path = GetServerFilePath(basePath, os, suffix);
                }
                else if (OperatingSystemChecker.IsMacOs())
                {
                    os = "mac-x64";
                    suffix = "macos";
                    path = GetServerFilePath(basePath, os, suffix);
                }
                else if (OperatingSystemChecker.IsLinux())
                {
                    os = "linux-x64";
                    suffix = "linux";
                    path = GetServerFilePath(basePath, os, suffix);
                    CommandExecutor.ExecCommand($"chmod 755 {path}");
                }
                else
                {
                    throw new NotSupportedException($"The operation system is not supported: {RuntimeInformation.OSDescription}");
                }

                nativeProc_ = Process.Start(path);
            }
        }

        private static string GetServerFilePath(string basePath, string os, string suffix)
        {
            return Path.Combine(Path.GetDirectoryName(basePath), "runtimes", os, "native", "eyes-universal-" + suffix);
        }

        private void TryStopProcess_()
        {
            if (nativeProc_ != null && !nativeProc_.HasExited)
            {
                nativeProc_.Kill();
            }
        }
    }
}

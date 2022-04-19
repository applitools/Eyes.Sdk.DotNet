using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Applitools
{
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
                string osVersion = Environment.OSVersion.VersionString.ToLower();
                string os, suffix;
                if (osVersion.Contains("windows"))
                {
                    os = "win-x64";
                    suffix = "win.exe";
                    path = GetServerFilePath(basePath, os, suffix);
                }
                else if (osVersion.Contains("mac"))
                {
                    os = "mac-x64";
                    suffix = "macos";
                    path = GetServerFilePath(basePath, os, suffix);
                }
                else
                {
                    os = "linux-x64";
                    suffix = "linux";
                    path = GetServerFilePath(basePath, os, suffix);
                    CommandExecutor.ExecCommand($"chmod 755 {path}");
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

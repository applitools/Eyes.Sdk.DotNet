using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Applitools.Universal
{
    public class SocketMessageManager
    {
        private static Process nativeProc_;
        
        private void TryStartProcess_()
        {
            if (nativeProc_ == null || nativeProc_.HasExited)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string osVersion = Environment.OSVersion.VersionString.ToLower();
                string os, suffix;
                if (osVersion.Contains("windows")) { os = "win-x64"; suffix = "win.exe"; }
                else if (osVersion.Contains("mac")) { os = "mac-x64"; suffix = "macos"; }
                else { os = "linux-x64"; suffix = "linux"; }

                path = Path.Combine(Path.GetDirectoryName(path), "runtimes", os, "native", "eyes-universal-" + suffix);
                nativeProc_ = Process.Start(path);
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Applitools
{
    using System.Runtime.InteropServices;

    public class UniversalSdkNativeLoader
    {
        private static int port_ = 21077;
        private static Process nativeProc_;

        public static int Start()
        {
            return TryStartProcess_();
        }

        public void Stop()
        {
            TryStopProcess_();
        }

        public bool IsRunning => nativeProc_ != null && !nativeProc_.HasExited;

        private static int TryStartProcess_()
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

                    if (File.Exists(path) == false)
                    {
                        CopyPackageFolder(basePath);
                    }
                }
                else if (OperatingSystemChecker.IsMacOs())
                {
                    os = "osx-x64";
                    suffix = "macos";
                    path = GetServerFilePath(basePath, os, suffix);
                    CommandExecutor.ExecCommand($"chmod 755 {path}");
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

                if (File.Exists(path) == false)
                {
                    throw new Exception($"Universal Server file '{path}' does not exist.");
                }

                nativeProc_ = StartProcess(path);
                
                port_ = GetPort(nativeProc_);
            }

            return port_;
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

        private static Process StartProcess(string path)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            return process;
        }

        private static int GetPort(Process process)
        {
            try
            {
                var portStr = process.StandardOutput.ReadLine();
                return string.IsNullOrEmpty(portStr) ? port_ : int.Parse(portStr);
            }
            catch
            {
                return port_;
            }
        }

        private static void CopyPackageFolder(string basePath)
        {
            var packageName = "eyes.sdk.universal.core";
            var packageFolderPathEnv = @"%userprofile%\.nuget\packages";
            var nugetFolder = Environment.ExpandEnvironmentVariables(packageFolderPathEnv);
            var packageFolderPath = Path.Combine(nugetFolder, packageName);
            var directoryInfo = new DirectoryInfo(packageFolderPath);
            var latestPackageName = directoryInfo.GetDirectories().Select(d => d.Name).OrderByDescending(i => i).FirstOrDefault();
            var latestPackagePath = Path.Combine(packageFolderPath, latestPackageName);

            var targetRootFolder = Path.GetDirectoryName(basePath);

            CopyFilesRecursively(latestPackagePath, targetRootFolder);
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}

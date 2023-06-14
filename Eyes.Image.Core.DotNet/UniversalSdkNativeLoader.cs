using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Applitools
{
    using System.Runtime.InteropServices;

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
                    string path;
                    string os, suffix;
                    if (OperatingSystemChecker.IsWindows())
                    {
                        os = "win-x64";
                        suffix = "win.exe";
                        path = GetServerFilePath(basePath, os, suffix);

                        if (File.Exists(path) == false)
                        {
                            CopyCorePackageFolder(basePath);
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
                        if (File.Exists("/etc/alpine-release"))
                        {
                            os = "linux-alpine";
                            suffix = "alpine";
                        }
                        else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                        {
                            os = "linux-arm64";
                            suffix = "linux-arm64";
                        }
                        else
                        {
                            os = "linux-x64";
                            suffix = "linux";
                        }

                        path = GetServerFilePath(basePath, os, suffix);
                        CommandExecutor.ExecCommand($"chmod 755 {path}");
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"The operation system is not supported: {RuntimeInformation.OSDescription}");
                    }

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

        private static void CopyCorePackageFolder(string basePath)
        {
            var packageName = "eyes.image.core";
            var packageFolderPathEnv = @"%userprofile%\.nuget\packages";
            var nugetFolder = Environment.ExpandEnvironmentVariables(packageFolderPathEnv);
            var packageFolderPath = Path.Combine(nugetFolder, packageName);
            var directoryInfo = new DirectoryInfo(packageFolderPath);
            var latestPackageName = directoryInfo.GetDirectories().Select(d => d.Name).OrderByDescending(i => i)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(latestPackageName) == false)
            {
                var latestPackagePath = Path.Combine(packageFolderPath, latestPackageName);

                var targetRootFolder = Path.GetDirectoryName(basePath);

                CopyFilesRecursively(latestPackagePath, targetRootFolder);
            }
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Applitools;
using NUnit.Framework;

namespace Eyes.Selenium.UnitTests
{
    public static class OperatingSystemChecker
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsMacOs() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    
    public class UniversalServerProcessTests
    {
        private Process toolProcess_;

        //[Test]
        public void UniversalServerProcess_NormalToolProcessExit_UniversalServerProcessExited()
        {
            var currentProcess = Process.GetCurrentProcess();

            var processName = "UniversalServerRunner";
            var processFileName = OperatingSystemChecker.IsWindows() ? $"{processName}.exe" : processName;
            var processFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, processFileName);
            toolProcess_ = StartToolProcess(processFilePath);
            Thread.Sleep(2000);
            var universalServerRunnerProcess = currentProcess.GetChildProcesses().FirstOrDefault(p => p.ProcessName == processName);
            Assert.NotNull(universalServerRunnerProcess, $"{processName} Process should Exist");

            var universalServerProcessName = GetUniversalServerProcessName();
            var universalServerProcess = GetUniversalServerProcess(universalServerRunnerProcess, universalServerProcessName);
            Assert.NotNull(universalServerProcess, $"{universalServerProcessName} Process should Exist");

            Console.WriteLine("1");
            Thread.Sleep(5000);

            universalServerProcess = GetUniversalServerProcess(universalServerRunnerProcess, universalServerProcessName);
            Assert.Null(universalServerProcess, $"{universalServerProcessName} Process should not Exists");
        }

        //[Test]
        public void UniversalServerProcess_KillToolProcess_UniversalServerProcessExited()
        {
            var currentProcess = Process.GetCurrentProcess();

            var processName = "UniversalServerRunner";
            var processFileName = OperatingSystemChecker.IsWindows() ? $"{processName}.exe" : processName;
            var processFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, processFileName);
            toolProcess_ = StartToolProcess(processFilePath);
            Thread.Sleep(2000);
            var universalServerRunnerProcess = currentProcess.GetChildProcesses().FirstOrDefault(p => p.ProcessName == processName);
            Assert.NotNull(universalServerRunnerProcess, $"{processName} Process should Exist");

            var universalServerProcessName = GetUniversalServerProcessName();
            var universalServerProcess = GetUniversalServerProcess(universalServerRunnerProcess, universalServerProcessName);
            Assert.NotNull(universalServerProcess, $"{universalServerProcessName} Process should Exist");

            toolProcess_.Kill();
            toolProcess_.WaitForExit();
            Thread.Sleep(2000);

            universalServerProcess = GetUniversalServerProcess(universalServerRunnerProcess, universalServerProcessName);
            Assert.Null(universalServerProcess, $"{universalServerProcessName} Process should not Exists");
        }

        [TearDown]
        public void TearDown()
        {
            toolProcess_?.Kill();
            toolProcess_?.WaitForExit();
        }

        private static Process GetUniversalServerProcess(Process universalServerRunnerProcess, string universalServerProcessName)
        {
            return universalServerRunnerProcess.GetChildProcesses().FirstOrDefault(p => p.ProcessName == universalServerProcessName);
        }

        private static Process StartToolProcess(string processFileName)
        {
            if (File.Exists(processFileName) == false)
            {
                throw new Exception($"The file {processFileName} does not exist.");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(processFileName)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            return process;
        }

        private string GetUniversalServerProcessName()
        {
            string suffix;
            if (OperatingSystemChecker.IsWindows())
            {
                suffix = "win";
            }
            else if (OperatingSystemChecker.IsMacOs())
            {
                suffix = "macos";
            }
            else if (OperatingSystemChecker.IsLinux())
            {
                suffix = "linux";
            }
            else
            {
                throw new NotSupportedException($"The operation system is not supported: {RuntimeInformation.OSDescription}");
            }

            return "eyes-universal-" + suffix;
        }
    }

    public static class ProcessExtensions
    {
        public static IList<Process> GetChildProcesses(this Process process)
            => new ManagementObjectSearcher(
                    $"Select * From Win32_Process Where ParentProcessID={process.Id}")
                .Get()
                .Cast<ManagementObject>()
                .Select(mo =>
                    Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])))
                .ToList();
    }
}
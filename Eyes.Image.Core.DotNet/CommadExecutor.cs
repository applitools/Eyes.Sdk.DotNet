using System.Diagnostics;

namespace Applitools
{
    internal static class CommandExecutor
    {
        public static void ExecCommand(string cmd)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{cmd}\""
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
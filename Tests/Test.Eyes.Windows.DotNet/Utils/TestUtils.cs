using Applitools.Windows;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Applitools.Tests.Utils
{
    public class TestUtils
    {
        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Tests");

        public static string InitLogPath([CallerMemberName]string testName = null)
        {
            string dateString = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
            string extendedTestName = $"{testName}_{dateString}";
            string logsPath = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
            string path = Path.Combine(logsPath, "DotNet", extendedTestName);
            return path;
        }

        public static ILogHandler InitLogHandler([CallerMemberName]string testName = null, string logPath = null)
        {
            if (!RUNS_ON_CI)
            {
                string path = logPath ?? InitLogPath(testName);
                return new FileLogHandler(Path.Combine(path, "log.log"), true, true);
            }
            return new NunitLogHandler(false);
        }

        public static void SetupLogging(Eyes eyes, [CallerMemberName] string testName = null)
        {
            ILogHandler logHandler = null;
            if (!RUNS_ON_CI)
            {
                string path = InitLogPath(testName);
                eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                {
                    Path = path,
                    Prefix = testName + "_"
                };
                logHandler = new FileLogHandler(Path.Combine(path, testName + ".log"), true, true);
            }
            else
            {
                logHandler = new NunitLogHandler(false);
            }

            if (logHandler != null)
            {
                eyes.SetLogHandler(logHandler);
            }
        }
    }
}

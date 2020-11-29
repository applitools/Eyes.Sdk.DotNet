using Applitools.Metadata;
using Applitools.Selenium;
using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;

namespace Applitools.Utils
{
    public class TestUtils
    {

        public static void SetupLogging(Eyes eyes, [CallerMemberName]string testName = null)
        {
            ILogHandler logHandler;
            if (Environment.GetEnvironmentVariable("CI") == null)
            {
                string path = InitLogPath(testName);
                eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                {
                    Path = path,
                    Prefix = testName + "_"
                };
                logHandler = new FileLogHandler(Path.Combine(path, $"{testName}.log"), true, false);
            }
            else
            {
                logHandler = new StdoutLogHandler(true);
                //logHandler_ = new TraceLogHandler(true);
            }

            if (logHandler != null)
            {
                eyes.SetLogHandler(logHandler);
            }
        }

        public static List<object[]> GeneratePermutationsList(List<List<object>> lists)
        {
            List<object[]> result = new List<object[]>();
            GeneratePermutations_(lists, result, 0, null);
            return result;
        }

        public static object[][] GeneratePermutations(List<List<object>> lists)
        {
            List<object[]> result = GeneratePermutationsList(lists);
            return result.ToArray();
        }

        public static object[][] GeneratePermutations(params List<object>[] lists)
        {
            return GeneratePermutations(new List<List<object>>(lists));
        }

        public static object[][] GeneratePermutations(params object[][] arrays)
        {
            List<object> lists = new List<object>();
            foreach (object[] array in arrays)
            {
                lists.Add(new List<object>(array));
            }
            return GeneratePermutations(lists);
        }


        private static void GeneratePermutations_(List<List<object>> lists, List<object[]> result, int depth, List<object> permutation)
        {
            if (depth == lists.Count)
            {
                if (permutation != null)
                {
                    result.Add(permutation.ToArray());
                }
                return;
            }

            List<object> listInCurrentDepth = lists[depth];
            foreach (object newItem in listInCurrentDepth)
            {
                if (permutation == null || depth == 0)
                {
                    permutation = new List<object>();
                }

                permutation.Add(newItem);
                GeneratePermutations_(lists, result, depth + 1, permutation);
                permutation.Remove(permutation.Count - 1);
            }
        }

        public static string InitLogPath([CallerMemberName]string testName = null)
        {
            string dateString = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
            string extendedTestName = $"{testName}_{dateString}";
            string logsPath = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
            string path = Path.Combine(logsPath, "DotNet", extendedTestName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static ILogHandler InitLogHandler([CallerMemberName]string testName = null, string logPath = null, bool verbose = true)
        {
            if (Environment.GetEnvironmentVariable("CI") == null)
            {
                string path = logPath ?? InitLogPath(testName);
                return new FileLogHandler(Path.Combine(path, "log.log"), true, verbose);
            }
            return new StdoutLogHandler(verbose);
        }

        public static SessionResults GetSessionResults(Eyes eyes, TestResults testResults)
        {
            string apiSessionUrl = testResults?.ApiUrls?.Session;
            if (string.IsNullOrWhiteSpace(apiSessionUrl)) return null;
            UriBuilder uriBuilder = new UriBuilder(apiSessionUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["format"] = "json";
            query["AccessToken"] = testResults.SecretToken;
            query["apiKey"] = eyes.ApiKey;
            uriBuilder.Query = query.ToString();

            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            HttpWebResponse metaResults = client.Get(uriBuilder.ToString());
            SessionResults sessionResults = metaResults.DeserializeBody<SessionResults>(false);
            return sessionResults;
        }
    }
}

using Applitools.Metadata;
using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;

namespace Applitools.Tests.Utils
{
    public class TestUtils
    {

        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public static readonly bool RUN_HEADLESS = "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_RUN_HEADLESS"), StringComparison.OrdinalIgnoreCase) || RUNS_ON_CI;

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
      
        public static SessionResults GetSessionResults(string apiKey, TestResults testResults)
        {
            string apiSessionUrl = testResults?.ApiUrls?.Session;
            if (string.IsNullOrWhiteSpace(apiSessionUrl)) return null;
            UriBuilder uriBuilder = new UriBuilder(apiSessionUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["format"] = "json";
            query["AccessToken"] = testResults.SecretToken;
            query["apiKey"] = apiKey;
            uriBuilder.Query = query.ToString();

            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            HttpWebResponse metaResults = client.Get(uriBuilder.ToString());
            SessionResults sessionResults = metaResults.DeserializeBody<SessionResults>(false);
            return sessionResults;
        }
    }
}

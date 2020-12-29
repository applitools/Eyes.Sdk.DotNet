using Applitools.Metadata;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;

namespace Applitools.Tests.Utils
{
    internal partial class TestUtils
    {
        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public const string COVERED_BY_GENERATED_TESTS_MESSAGE = "covered by generated tests";

        public static string InitLogPath([CallerMemberName] string testName = null)
        {
            string dateString = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
            string extendedTestName = $"{testName}_{dateString}";
            string logsPath = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
            string path = Path.Combine(logsPath, "DotNet", extendedTestName);
            return path;
        }

        public static ILogHandler InitLogHandler([CallerMemberName] string testName = null, string logPath = null)
        {
            if (!RUNS_ON_CI)
            {
                string path = logPath ?? InitLogPath(testName);
                return new FileLogHandler(Path.Combine(path, "log.log"), true, true);
            }
            return new NunitLogHandler(false);
        }

        public static void SetupLogging(EyesBase eyes, [CallerMemberName] string testName = null)
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

#if !NET461
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
            SessionResults sessionResults = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            TimeSpan timeout = TimeSpan.FromSeconds(40);
            while (sessionResults == null && stopwatch.Elapsed < timeout)
            {
                using (HttpWebResponse metaResults = client.Get(uriBuilder.ToString()))
                {
                    sessionResults = metaResults.DeserializeBody<SessionResults>(false);
                }
                if (sessionResults != null && sessionResults.ActualAppOutput.Length > 0) break;
                System.Threading.Thread.Sleep(500);
            }
            return sessionResults;
        }

        public static string GetDom(string apiKey, TestResults testResults, string domId)
        {
            string apiSessionUrl = testResults?.AppUrls?.Session;
            if (string.IsNullOrWhiteSpace(apiSessionUrl)) return null;
            UriBuilder uriBuilder = new UriBuilder(apiSessionUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["apiKey"] = apiKey;
            uriBuilder.Query = query.ToString();
            uriBuilder.Path = $"/api/images/dom/{domId}/";

            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            using (HttpWebResponse response = client.Get(uriBuilder.ToString()))
            using (Stream s = response.GetResponseStream())
            {
                var json = new StreamReader(s).ReadToEnd();
                return json;
            }
        }

        public static BatchInfo GetBatchInfo(EyesBaseConfig eyes)
        {
            UriBuilder uriBuilder = new UriBuilder(eyes.ServerUrl);
            uriBuilder.Path = $"api/sessions/batches/{eyes.Batch.Id}/bypointerid";
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["apiKey"] = eyes.ApiKey;
            uriBuilder.Query = query.ToString();
            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            using (HttpWebResponse batchInfoResponse = client.Get(uriBuilder.ToString()))
            {
                BatchInfo batchInfo = batchInfoResponse.DeserializeBody<BatchInfo>(false);
                return batchInfo;
            }
        }
#endif

        public static void CompareSimpleRegionsList_(Region[] actualRegions, HashSet<Region> expectedRegions, string type)
        {
            HashSet<Region> expectedRegionsClone = new HashSet<Region>(expectedRegions);
            if (expectedRegions.Count > 0)
            {
                foreach (Region region in actualRegions)
                {
                    if (!expectedRegionsClone.Remove(region))
                    {
                        Assert.Fail("actual {0} region {1} not found in expected regions list", type, region);
                    }
                }
                Assert.IsEmpty(expectedRegionsClone, "not all expected {0} regions found in actual regions list.", type);
            }
        }

    }
}
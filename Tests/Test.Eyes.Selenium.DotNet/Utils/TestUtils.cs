using Applitools.Metadata;
using Applitools.Selenium;
using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace Applitools.Tests.Utils
{
    internal partial class TestUtils
    {

        public static readonly bool RUNS_ON_TRAVIS = "true".Equals(Environment.GetEnvironmentVariable("TRAVIS"), StringComparison.OrdinalIgnoreCase);
        public static readonly bool RUN_HEADLESS = !Debugger.IsAttached || RUNS_ON_CI;// "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_RUN_HEADLESS"), StringComparison.OrdinalIgnoreCase) || RUNS_ON_CI;
        public static readonly bool IS_RELEASE_CANDIDATE = Environment.GetEnvironmentVariable("TRAVIS_TAG")?.Contains("RELEASE_CANDIDATE") ?? false;
        public static readonly string LOGS_PATH = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";

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

        public static void SetupLogging(Eyes eyes, [CallerMemberName] string testName = null)
        {
            string path = InitLogPath(testName);
            if (!RUNS_ON_CI)
            {
                eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                {
                    Path = path,
                    Prefix = testName + "_"
                };
            }
            Eyes.moveWindow_ = !Debugger.IsAttached;
            SetupLogging(eyes.runner_, testName, path);
        }

        public static void SetupLogging(EyesRunner runner, [CallerMemberName] string testName = null, string path = null)
        {
            ILogHandler logHandler;
            if (!RUNS_ON_CI)
            {
                path = path ?? InitLogPath(testName);
                logHandler = new FileLogHandler(Path.Combine(path, testName + ".log"), true, true);
                //if (eyes.runner_ is VisualGridRunner visualGridRunner)
                //{
                //    visualGridRunner.DebugResourceWriter = new FileDebugResourceWriter(path);
                //    ((VisualGridEyes)eyes.activeEyes_).debugResourceWriter_ = visualGridRunner.DebugResourceWriter;
                //}
            }
            else
            {
                logHandler = new NunitLogHandler(false);
            }

            if (logHandler != null)
            {
                runner.SetLogHandler(logHandler);
                logHandler.Open();
            }

        }

        public static string GetStepDom(EyesBase eyes, ActualAppOutput actualAppOutput)
        {
            ArgumentGuard.NotNull(eyes, nameof(eyes));
            ArgumentGuard.NotNull(actualAppOutput, nameof(actualAppOutput));

            UriBuilder uriBuilder = new UriBuilder(eyes.ServerUrl);
            uriBuilder.Path = $"/api/images/dom/{actualAppOutput.Image.DomId}";

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["apiKey"] = eyes.ApiKey;
            uriBuilder.Query = query.ToString();
            HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
            client.ConfigureRequest += Client_ConfigureRequest;
            HttpWebResponse response = client.GetJson(uriBuilder.ToString());
            Stream stream = response.GetResponseStream();
            string result;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            client.ConfigureRequest -= Client_ConfigureRequest;
            return result;
        }

        private static void Client_ConfigureRequest(object sender, HttpWebRequestEventArgs e)
        {
            e.HttpWebRequest.AutomaticDecompression = DecompressionMethods.All;
        }

        public static string SanitizeForFilename(string testName)
        {
            string lastTestName = testName.Replace('.', '_').Replace('(', '_').Replace(')', '_').Replace(' ', '_').Replace(',', '_');
            while (testName != lastTestName)
            {
                testName = lastTestName;
                lastTestName = lastTestName.Replace("__", "_");
            }

            return lastTestName;
        }

    }
}

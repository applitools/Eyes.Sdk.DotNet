using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Applitools.Tests.Utils
{
    public partial class TestResultReportSummary
    {
        [JsonProperty("sdk")]
        public string SdkName => "dotnet";

        [JsonProperty("id")]
        public string Id { get; set; } = Environment.GetEnvironmentVariable("APPLITOOLS_REPORT_ID") ?? "0000-0000";

        [JsonProperty("sandbox")]
        public bool Sandbox => SendToSandbox();
        
        internal static bool SendToSandbox()
        {
            // specifically request to send to sandbox...
            bool b1 = "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_REPORT_TO_SANDBOX"), StringComparison.OrdinalIgnoreCase);
            // not a release build and 
            bool b2 = !(Environment.GetEnvironmentVariable("TRAVIS_TAG")?.Contains("RELEASE_CANDIDATE") ?? false);
            // not full coverage
            bool b3 = !ReportingTestSuite.IS_FULL_COVERAGE;

            bool endResult = b1 || (b2 && b3);
            return endResult;
        }

        [JsonProperty("results")]
        public HashSet<TestResult> Results { get; } = new HashSet<TestResult>();

        public void AddResult(TestResult result)
        {
            if (!Results.TryGetValue(result, out TestResult testResult))
            {
                Results.Add(result);
            }
            else
            {
                testResult.Passed = result.Passed;
            }
        }
    }
}

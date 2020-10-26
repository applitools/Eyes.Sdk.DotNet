using Newtonsoft.Json;
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
        public bool Sandbox { get; set; } =
            "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_REPORT_TO_SANDBOX"), StringComparison.OrdinalIgnoreCase) ||
            Environment.GetEnvironmentVariable("TRAVIS_TAG") == null ||
            !Environment.GetEnvironmentVariable("TRAVIS_TAG").Contains("RELEASE_CANDIDATE");

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

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
        public List<TestResult> Results { get; } = new List<TestResult>();

        public bool AddResult(TestResult result)
        {
            bool newResult = !Results.Contains(result);
            Results.Add(result);
            return newResult;
        }
    }
}

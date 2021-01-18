using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Applitools.Tests.Utils
{
    public class TestResultGenerated : TestResult
    {
        public TestResultGenerated(string testName, bool passed, Dictionary<string, object> parameters, string browser, string mode) : base(testName, passed, parameters)
        {
            this.Parameters = new Dictionary<string, object> { ["browser"] = browser, ["mode"] = mode };
        }

        [JsonProperty("isGeneric")]
        public bool IsGeneric => true;
    }
}
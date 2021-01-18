using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Applitools.Tests.Utils
{
    public class TestResultGeneratedAppium : TestResult
    {
        public TestResultGeneratedAppium(string testName, bool passed, Dictionary<string, object> parameters) : base(testName, passed, parameters)
        { }

        [JsonProperty("isGeneric")]
        public bool IsGeneric => true;
    }
}
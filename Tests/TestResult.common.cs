using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Applitools.Tests.Utils
{
    public class TestResult : IEquatable<TestResult>
    {
        public TestResult(string testName, bool passed, Dictionary<string, object> parameters)
        {
            TestName = testName;
            Parameters = parameters;
            Passed = passed;
        }

        [JsonProperty("test_name")]
        public string TestName { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty("passed")]
        public bool Passed { get; set; }

        public bool Equals([AllowNull] TestResult other)
        {
            if (other == null) return false;
            return TestName.Equals(other.TestName) &&
                AreDictionariesEqual(Parameters, other.Parameters);
        }

        private bool AreDictionariesEqual(IDictionary<string, object> d1, IDictionary<string, object> d2)
        {
            if (d1.Count != d2.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, object> pair in d1)
            {
                if (d2.TryGetValue(pair.Key, out object value))
                {
                    if ((value == null && pair.Value != null) || (value != null && !value.Equals(pair.Value)))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestResult);
        }

        public override int GetHashCode()
        {
            return TestName.GetHashCode();
        }
    }
}
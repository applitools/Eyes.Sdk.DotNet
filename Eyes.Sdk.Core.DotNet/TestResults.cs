using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    /// <summary>
    /// Eyes test results.
    /// </summary>
    public class TestResults
    {
        /// <summary>
        /// Creates a new <see cref="TestResults"/> instance.
        /// </summary>
        public TestResults(bool isNew = false)
        {
            Url = string.Empty;
        }

        internal IServerConnector ServerConnector { private get; set; }

        public string SecretToken { get; set; }

        public string Id { get; set; }

        public string BatchId { get; set; }

        public string BatchName { get; set; }

        public string BranchName { get; set; }

        public string AppName { get; set; }

        public string Name { get; set; }

        public bool IsDifferent { get; set; }

        public bool IsAborted { get; set; }

        /// <summary>
        /// The URL where test results can be viewed.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Whether or not this is a new test.
        /// </summary>
        public bool IsNew { get; internal set; }

        /// <summary>
        /// The test status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TestResultsStatus Status { get; set; }

        /// <summary>
        /// Whether or not this test passed.
        /// </summary>
        public bool IsPassed
        {
            get { return Status == TestResultsStatus.Passed; }
        }

        /// <summary>
        /// Returns the total number of test steps.
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// Returns the total number of test steps that matched the baseline.
        /// </summary>
        public int Matches { get; set; }

        /// <summary>
        /// Returns the total number of test steps that did not match the baseline.
        /// </summary>
        public int Mismatches { get; set; }

        /// <summary>
        /// Returns the total number of baseline test steps that were missing in
        /// the test.
        /// </summary>
        public int Missing { get; set; }

        /// <summary>
        /// Returns the total number of test steps that exactly matched the baseline.
        /// </summary>
        public int ExactMatches { get; set; }

        /// <summary>
        /// Returns the total number of test steps that strictly matched the
        /// baseline.
        /// </summary>
        public int StrictMatches { get; set; }

        /// <summary>
        /// Returns the total number of test steps that matched the baseline by
        /// content.
        /// </summary>
        public int ContentMatches { get; set; }

        /// <summary>
        /// Returns the total number of test steps that matched the baseline by
        /// layout.
        /// </summary>
        public int LayoutMatches { get; set; }

        /// <summary>
        /// Returns the total number of test steps that matched the baseline without
        /// performing any comparison.
        /// </summary>
        public int NoneMatches { get; set; }

        /// <summary>
        /// Returns the total number of new test steps.
        /// </summary>
        public int New { get; set; }

        public SessionUrls AppUrls { get; set; }

        public SessionUrls ApiUrls { get; set; }

        public string HostApp { get; set; }

        public string HostOS { get; set; }

        public StepInfo[] StepsInfo { get; set; }

        public DateTime StartedAt { get; set; }

        public long Duration { get; set; }

        public RectangleSize HostDisplaySize { get; set; }

        public string BaselineId { get; set; }

        public ImageMatchSettings DefaultMatchSettings { get; set; }

        public SessionAccessibilityStatus AccessibilityStatus { get; set; }

        public void Delete()
        {
            this.ServerConnector.DeleteSession(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonUtils.Serialize(this);
        }
    }
}

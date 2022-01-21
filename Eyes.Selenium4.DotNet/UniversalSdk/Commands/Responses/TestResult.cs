using Applitools;
using Applitools.Utils.Geometry;

namespace Applitools.Selenium.Commands.Responses
{
    public class TestResult
    {
        public string TestId { get; set; }
        public string name { get; set; }
        public string secretToken { get; set; }
        public string status { get; set; } //enum
        public string appName { get; set; }
        public string batchId { get; set; }
        public string batchName { get; set; }
        public string branchName { get; set; }
        public string hostOS { get; set; }
        public string hostApp { get; set; }
        public RectangleSize hostDisplaySize { get; set; }
        public SessionAccessibilityStatus accessibilityStatus { get; set; }
        public string startedAt { get; set; }
        public int? duration { get; set; }
        public bool? isNew { get; set; }
        public bool? isDifferent { get; set; }
        public bool? isAborted { get; set; }
        public SessionUrls appUrls { get; set; }
        public SessionUrls apiUrls { get; set; }
        public StepInfo[] stepsInfo { get; set; }
        public int? steps { get; set; }
        public int? matches { get; set; }
        public int? mismatches { get; set; }
        public int? missing { get; set; }
        public int? exactMatches { get; set; }
        public int? strictMatches { get; set; }
        public int? contentMatches { get; set; }
        public int? layoutMatches { get; set; }
        public int? noneMatches { get; set; }
        public string url { get; set; }
    }
}
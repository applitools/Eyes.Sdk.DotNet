using System.Collections.Generic;
using Applitools.Utils.Geometry;

namespace Applitools.Selenium.Commands.Responses
{
    public class TestResult
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string SecretToken { get; set; }
        public string Status { get; set; } //enum
        public string AppName { get; set; }
        public string BatchId { get; set; }
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

    public class EyesCloseResponsePayload
    {
        public IList<TestResults> Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesCloseResponse
    {
        public string Name { get; set; }

        public EyesCloseResponsePayload Payload { get; set; }
    }
}
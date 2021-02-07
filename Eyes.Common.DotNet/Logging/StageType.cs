using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StageType
    {
        Called,
        Retry,
        JobInfo,
        UploadStart,
        UploadComplete,
        MatchStart,
        MatchComplete,
        DomScript,
        CaptureScreenshot,
        RenderStatus,
        DownloadResource,
        CheckResource,
        UploadResource,
        RequestSent,
        RequestCompleted,
        RequestFailed,
        CloseBatch,
        TestResults,
        Timeout,
        Complete,
        Failed,
        Start,
        Skipped,
        Disabled
    }
}
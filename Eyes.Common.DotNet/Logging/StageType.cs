namespace Applitools
{
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
        Timeout
    }
}
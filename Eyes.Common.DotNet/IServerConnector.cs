using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;

namespace Applitools
{
    public interface IServerConnector : IDeleteSession
    {
        WebProxy Proxy { get; set; }
        string ApiKey { get; set; }
        Uri ServerUrl { get; set; }

        TimeSpan Timeout { get; set; }
        string SdkName { get; set; }
        string AgentId { get; set; }

        void CloseBatch(string batchId);
        RenderingInfo GetRenderingInfo();
        MatchResult MatchWindow(MatchWindowData data);
        void MatchWindow(TaskListener<MatchResult> listener, MatchWindowData matchWindowData);
        TestResults EndSession(RunningSession runningSession, bool isAborted, bool save);
        string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId, IList<Rectangle> regions, string language);
        string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes);
        string PostDomCapture(string domJson);
        RunningSession StartSession(SessionStartInfo sessionStartInfo_);
        void StartSession(TaskListener<RunningSession> taskListener, SessionStartInfo sessionStartInfo);
        Task<List<JobInfo>> GetJobInfo(IRenderRequest[] renderRequests);
        void SendLogs(LogSessionsClientEvents clientEvents);
        void UploadImage(TaskListener<string> uploadListener, byte[] screenshotBytes);
        void CheckResourceStatus(TaskListener<bool?[]> taskListener, string renderId, HashObject[] hashes);
        Task<WebResponse> RenderPutResourceAsTask(string renderId, IVGResource resource);
    }
}

using Applitools.Ufg;
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
        MatchResult MatchWindow(RunningSession runningSession, MatchWindowData data);
        TestResults EndSession(RunningSession runningSession, bool isAborted, bool save);
        string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId, IList<Rectangle> regions, string language);
        string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes);
        string PostDomCapture(string domJson);
        RunningSession StartSession(SessionStartInfo sessionStartInfo);
        Task<List<JobInfo>> GetJobInfo(RenderRequest[] renderRequests);
        void SendLogs(LogSessionsClientEvents clientEvents);
    }
}

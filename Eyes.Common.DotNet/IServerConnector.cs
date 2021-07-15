using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;

namespace Applitools
{
    public interface IServerConnector : IDeleteSession
    {
        ProxySettings Proxy { get; set; }
        string ApiKey { get; set; }
        Uri ServerUrl { get; set; }

        TimeSpan Timeout { get; set; }
        string SdkName { get; set; }
        string AgentId { get; set; }

        void CloseBatch(string batchId);
        RenderingInfo GetRenderingInfo();
        void MatchWindow(TaskListener<MatchResult> listener, MatchWindowData matchWindowData, params string[] testIds);
        string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId,
            IList<Rectangle> regions, string language);

        string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes);
        void PostDomCapture(TaskListener<string> listener, string domJson, params string[] testIds);
        void StartSession(TaskListener<RunningSession> taskListener, SessionStartInfo sessionStartInfo);
        void EndSession(TaskListener<TestResults> taskListener, SessionStopInfo sessionStopInfo);
        void GetJobInfo(TaskListener<IList<JobInfo>> listener, IList<IRenderRequest> renderRequests);
        void SendLogs(LogSessionsClientEvents clientEvents);
        void UploadImage(TaskListener<string> uploadListener, byte[] screenshotBytes, params string[] testIds);
        void CheckResourceStatus(TaskListener<bool?[]> taskListener, HashSet<string> testIds, string renderId, HashObject[] hashes);
        void RenderPutResource(TaskListener<HttpResponseMessage> listener, string renderId, IVGResource resource);
        void Render(TaskListener<List<RunningRender>> renderListener, IList<IRenderRequest> requests);
        void RenderStatusById(TaskListener<List<RenderStatusResults>> pollingListener,
            IList<string> testIds, IList<string> renderIds);
        Dictionary<DeviceName, DeviceSize> GetEmulatedDevicesSizes();
        Dictionary<IosDeviceName, DeviceSize> GetIosDevicesSizes();
    }
}

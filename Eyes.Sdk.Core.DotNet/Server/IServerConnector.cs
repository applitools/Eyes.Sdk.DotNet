using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace Applitools
{
    public interface IServerConnector
    {
        WebProxy Proxy { get; set; }
        string ApiKey { get; set; }
        Uri ServerUrl { get; set; }
        TimeSpan Timeout { get; set; }
        string SdkName { get; set; }
        bool DontCloseBatches { get; }
        string AgentId { get; set; }

        void CloseBatch(string batchId);
        RenderingInfo GetRenderingInfo();
        ResourceFuture CreateResourceFuture(RGridResource resource);
        MatchResult MatchWindow(RunningSession runningSession, MatchWindowData data);
        void DeleteSession(TestResults testResults);
        TestResults EndSession(RunningSession runningSession, bool isAborted, bool save);
        string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId, IList<Rectangle> regions, string language);
        string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes);
        string PostDomCapture(string domJson);
        RunningSession StartSession(SessionStartInfo sessionStartInfo);
        Dictionary<IosDeviceName, DeviceSize> GetIosDevicesSizes();
        Dictionary<DeviceName, DeviceSize> GetEmulatedDevicesSizes();
        Dictionary<BrowserType, string> GetUserAgents();
    }
}

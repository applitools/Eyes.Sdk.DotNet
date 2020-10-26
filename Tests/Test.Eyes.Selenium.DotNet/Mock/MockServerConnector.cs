using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnector : IServerConnector
    {
        private readonly Logger logger_;

        public MockServerConnector(Logger logger, Uri serverUrl)
        {
            logger_ = logger;
            ServerUrl = serverUrl;
        }

        public WebProxy Proxy { get; set; }
        public string ApiKey { get; set; }
        public Uri ServerUrl { get; set; }
        public TimeSpan Timeout { get; set; }
        public string SdkName { get; set; }
        public string AgentId { get; set; }

        public bool DontCloseBatches { get; }
        internal List<MatchWindowData> MatchWindowCalls { get; } = new List<MatchWindowData>();
        internal List<MatchWindowData> ReplaceMatchedStepCalls { get; } = new List<MatchWindowData>();
        internal Dictionary<string, RunningSession> Sessions { get; } = new Dictionary<string, RunningSession>();
        internal Dictionary<string, SessionStartInfo> SessionsStartInfo { get; } = new Dictionary<string, SessionStartInfo>();
        internal List<string> SessionIds { get; } = new List<string>();

        public bool AsExcepted { get; set; } = true;

        public string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes)
        {
            throw new NotImplementedException();
        }

        public void CloseBatch(string batchId)
        {
        }

        public void DeleteSession(TestResults testResults)
        {
            logger_.Log("deleting session: {0}", testResults.Id);
        }

        public delegate void AfterEndSessionDelegate(RunningSession runningSession, bool isAborted, bool save);
        public event AfterEndSessionDelegate AfterEndSession;

        public TestResults EndSession(RunningSession runningSession, bool isAborted, bool save)
        {
            logger_.Log("ending session: {0}", runningSession.SessionId);
            TestResults testResults = new TestResults();
            AfterEndSession?.Invoke(runningSession, isAborted, save);
            return testResults;
        }

        public RenderingInfo GetRenderingInfo()
        {
            return new RenderingInfo();
        }

        public string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId, IList<Rectangle> regions, string language)
        {
            throw new NotImplementedException();
        }

        public MatchResult MatchWindow(RunningSession runningSession, MatchWindowData data)
        {
            if (data.Options.ReplaceLast)
            {
                ReplaceMatchedStepCalls.Add(data);
            }
            else
            {
                MatchWindowCalls.Add(data);
            }
            return new MatchResult() { AsExpected = this.AsExcepted };
        }

        public string PostDomCapture(string domJson)
        {
            throw new NotImplementedException();
        }

        public delegate (bool, RunningSession) OnStartSessionDelegate(SessionStartInfo sessionStartInfo);
        public event OnStartSessionDelegate OnStartSession;

        public RunningSession StartSession(SessionStartInfo sessionStartInfo)
        {
            logger_.Log("starting session: {0}", sessionStartInfo);

            RunningSession newSession = null;
            bool continueInvocation = true;
            if (OnStartSession != null)
            {
                (continueInvocation, newSession) = OnStartSession(sessionStartInfo);
            }
            if (continueInvocation)
            {
                newSession = new RunningSession() { isNewSession_ = false, SessionId = Guid.NewGuid().ToString() };
                SessionIds.Add(newSession.SessionId);
                Sessions.Add(newSession.SessionId, newSession);
                SessionsStartInfo.Add(newSession.SessionId, sessionStartInfo);
            }
            return newSession;
        }

        public Dictionary<IosDeviceName, DeviceSize> GetIosDevicesSizes()
        {
            return new Dictionary<IosDeviceName, DeviceSize>();
        }

        public Dictionary<DeviceName, DeviceSize> GetEmulatedDevicesSizes()
        {
            return new Dictionary<DeviceName, DeviceSize>();
        }

        public Dictionary<BrowserType, string> GetUserAgents()
        {
            return new Dictionary<BrowserType, string>();
        }
    }
}

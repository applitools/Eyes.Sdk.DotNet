using Applitools.Ufg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnector : ServerConnector
    {
        public MockServerConnector(Logger logger, Uri serverUrl)
            : base(logger, serverUrl)
        {
            logger.Verbose("created");
        }

        internal List<MatchWindowData> MatchWindowCalls { get; } = new List<MatchWindowData>();
        internal List<MatchWindowData> ReplaceMatchedStepCalls { get; } = new List<MatchWindowData>();
        internal Dictionary<string, RunningSession> Sessions { get; } = new Dictionary<string, RunningSession>();
        internal Dictionary<string, SessionStartInfo> SessionsStartInfo { get; } = new Dictionary<string, SessionStartInfo>();
        internal List<string> SessionIds { get; } = new List<string>();

        public bool AsExcepted { get; set; } = true;

        public override void CloseBatch(string batchId)
        {
            Logger.Log("closing batch: {0}", batchId);
        }

        public override void DeleteSession(TestResults testResults)
        {
            Logger.Log("deleting session: {0}", testResults.Id);
        }

        public delegate void AfterEndSessionDelegate(RunningSession runningSession, bool isAborted, bool save);
        public event AfterEndSessionDelegate AfterEndSession;

        protected override TestResults EndSessionInternal(RunningSession runningSession, bool isAborted, bool save)
        {
            Logger.Log("ending session: {0}", runningSession.SessionId);
            TestResults testResults = new TestResults();
            AfterEndSession?.Invoke(runningSession, isAborted, save);
            return testResults;
        }

        public override RenderingInfo GetRenderingInfo()
        {
            return new RenderingInfo();
        }

        public override MatchResult MatchWindow(RunningSession runningSession, MatchWindowData data)
        {
            if (data.Options.ReplaceLast)
            {
                Logger.Verbose("replace last step");
                ReplaceMatchedStepCalls.Add(data);
            }
            else
            {
                Logger.Verbose("add new step");
                MatchWindowCalls.Add(data);
            }
            return new MatchResult() { AsExpected = this.AsExcepted };
        }

        public delegate (bool, RunningSession) OnStartSessionDelegate(SessionStartInfo sessionStartInfo);
        public event OnStartSessionDelegate OnStartSession;

        protected override RunningSession StartSessionInternal(SessionStartInfo sessionStartInfo)
        {
            Logger.Log("starting session: {0}", sessionStartInfo);

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

        public async override Task<List<JobInfo>> GetJobInfo(RenderRequest[] renderRequests)
        {
            Logger.Verbose("getting job info");
            await Task.Delay(10);
            return new List<JobInfo>(new JobInfo[] { new JobInfo() });
        }
    }
}

using Applitools.Utils;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnector : ServerConnector
    {
        public MockServerConnector(Logger logger, Uri serverUrl)
            : base(logger, new Uri("http://some.url.com"))
        {
            logger.Verbose("created");
            HttpRestClientFactory = new MockHttpRestClientFactory();
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

        protected override void EndSessionInternal(TaskListener<TestResults> taskListener, SessionStopInfo sessionStopInfo)
        {
            Logger.Log("ending session: {0}", sessionStopInfo.RunningSession.SessionId);
            TestResults testResults = new TestResults();
            AfterEndSession?.Invoke(sessionStopInfo.RunningSession, sessionStopInfo.Aborted, sessionStopInfo.UpdateBaseline);
            taskListener.OnComplete(testResults);
        }

        public override RenderingInfo GetRenderingInfo()
        {
            RenderingInfo renderingInfo = new RenderingInfo();
            renderingInfo.ResultsUrl = new Uri("https://some.url.com");
            return renderingInfo;
        }

        public override MatchResult MatchWindow(MatchWindowData data)
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

        public delegate (bool, bool, RunningSession) OnStartSessionDelegate(SessionStartInfo sessionStartInfo);
        public event OnStartSessionDelegate OnStartSession;

        protected override void StartSessionInternal(TaskListener<RunningSession> taskListener, SessionStartInfo sessionStartInfo)
        {
            Logger.Log("starting session: {0}", sessionStartInfo);

            RunningSession newSession = null;
            bool continueInvocation = true;
            bool callBase = false;
            if (OnStartSession != null)
            {
                (callBase, continueInvocation, newSession) = OnStartSession(sessionStartInfo);
            }
            if (continueInvocation)
            {
                newSession = new RunningSession() { isNewSession_ = false, SessionId = Guid.NewGuid().ToString() };
                SessionIds.Add(newSession.SessionId);
                Sessions.Add(newSession.SessionId, newSession);
                SessionsStartInfo.Add(newSession.SessionId, sessionStartInfo);
            }
            if (callBase)
            {
                base.StartSessionInternal(taskListener, sessionStartInfo);
            }
            else
            {
                EnsureHttpClient_();
                taskListener.OnComplete(newSession);
            }
        }

        public override void PostDomCapture(TaskListener<string> listener, string domJson)
        {
            listener.OnComplete("http://some.targeturl.com/dom");
        }
    }

    class MockHttpRestClientFactory : IHttpRestClientFactory
    {
        public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
        {
            HttpRestClient mockedHttpRestClient = new HttpRestClient(serverUrl, agentId, jsonSerializer);
            mockedHttpRestClient.WebRequestCreator = new MockWebRequestCreator();
            return mockedHttpRestClient;
        }
    }

    class MockWebRequestCreator : IWebRequestCreate
    {
        private static readonly string BASE_LOCATION = "api/tasks/123412341234/";

        public WebRequest Create(Uri uri)
        {
            HttpWebRequest webRequest = Substitute.For<HttpWebRequest>();
            webRequest.RequestUri.Returns(uri);
            webRequest.Headers = new WebHeaderCollection();
            webRequest.GetRequestStream().Returns(new MemoryStream(new byte[10000]));
            webRequest.BeginGetResponse(default, default)
                       .ReturnsForAnyArgs(ci =>
                       {
                           AsyncCallback cb = ci.Arg<AsyncCallback>();
                           HttpWebRequest req = ci.Arg<HttpWebRequest>();
                           cb.Invoke(new MockAsyncResult(req));
                           return null;
                       });

            webRequest.EndGetResponse(default)
                .ReturnsForAnyArgs(ci =>
                {
                    MockAsyncResult mockAsyncResult = ci.Arg<MockAsyncResult>();
                    HttpWebRequest webRequest = ((HttpWebRequest)mockAsyncResult.AsyncState);
                    Uri uri = webRequest.RequestUri;
                    HttpWebResponse webResponse = Substitute.For<HttpWebResponse>();
                    WebHeaderCollection headers = new WebHeaderCollection();
                    webResponse.Headers.Returns(headers);
                    if (webRequest.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/api/sessions/running", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.Accepted);
                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "status");
                    }
                    else if (webRequest.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "status", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.Created);
                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "created");
                    }
                    else if (webRequest.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "created", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.OK);
                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "ok");
                    }
                    else
                    {
                        webResponse.ResponseUri.Returns(uri);
                        webResponse.StatusCode.Returns(HttpStatusCode.OK);
                    }
                    return webResponse;
                });

            return webRequest;
        }
    }
}

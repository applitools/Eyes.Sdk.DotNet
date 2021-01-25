using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnector : ServerConnector
    {
        public MockServerConnector(Logger logger, Uri serverUrl)
            : base(logger, new Uri("http://some.url.com"))
        {
            logger.Verbose("created");
            HttpRestClientFactory = new MockHttpRestClientFactory(Logger);
        }

        internal List<MatchWindowData> MatchWindowCalls { get; } = new List<MatchWindowData>();
        internal List<MatchWindowData> ReplaceMatchedStepCalls { get; } = new List<MatchWindowData>();
        internal Dictionary<string, RunningSession> Sessions { get; } = new Dictionary<string, RunningSession>();
        internal Dictionary<string, SessionStartInfo> SessionsStartInfo { get; } = new Dictionary<string, SessionStartInfo>();
        internal List<string> SessionIds { get; } = new List<string>();

        public bool AsExcepted { get; set; } = true;

        public List<string> RenderRequests { get; } = new List<string>();

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
            renderingInfo.ServiceUrl = new Uri("https://services.url.com");
            renderingInfo.StitchingServiceUrl = new Uri("https://stitching.url.com");
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

        protected override void SendUFGAsyncRequest_<T>(TaskListener<T> taskListener, HttpWebRequest request) where T : class
        {
            if (request.Method == "POST" && request.RequestUri.PathAndQuery == "/render")
            {
                Stream stream = request.GetRequestStream();
                stream.Position = 0;
                byte[] bytes = stream.ReadToEnd();
                string requestJsonStr = Encoding.UTF8.GetString(bytes);
                RenderRequests.Add(requestJsonStr);
                List<RunningRender> response = new List<RunningRender>();
                JArray list = (JArray)JsonConvert.DeserializeObject(requestJsonStr);
                for (int i = 0; i < list.Count; ++i)
                {
                    response.Add(new RunningRender(i.ToString(), "abc", RenderStatus.Rendered, null, false));
                }
                taskListener.OnComplete(response as T);
            }
            else
            {
                base.SendUFGAsyncRequest_(taskListener, request);
            }
        }

        public override void RenderStatusById(TaskListener<List<RenderStatusResults>> taskListener, IList<string> renderIds)
        {
            List<RenderStatusResults> results = new List<RenderStatusResults>();
            foreach (string renderId in renderIds)
            {
                RenderStatusResults result = new RenderStatusResults()
                {
                    RenderId = renderId,
                    Status = RenderStatus.Rendered,
                    ImageLocation = "http://image.some.url.com/" + renderId
                };
                results.Add(result);
            }
            taskListener.OnComplete(results);
        }

        public override void GetJobInfo(TaskListener<IList<JobInfo>> listener, IList<IRenderRequest> browserInfos)
        {
            IList<JobInfo> jobs = new List<JobInfo>();
            for (int i = 0; i < browserInfos?.Count; ++i)
            {
                jobs.Add(new JobInfo() { EyesEnvironment = "MockEnvironment", Renderer = "MockRenderer" });
            }
            listener.OnComplete(jobs);
        }

        public override void CheckResourceStatus(TaskListener<bool?[]> taskListener, string renderId, HashObject[] hashes)
        {
            bool?[] result = new bool?[hashes.Length];
            for (int i = 0; i < hashes.Length; ++i)
            {
                result[i] = true;
            }
            taskListener.OnComplete(result);
        }

        public override void PostDomCapture(TaskListener<string> listener, string domJson)
        {
            listener.OnComplete("http://some.targeturl.com/dom");
        }
    }

    class MockHttpRestClientFactory : IHttpRestClientFactory
    {
        public MockHttpRestClientFactory(Logger logger)
        {
            Logger = logger;
        }

        public Logger Logger { get; set; }

        public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
        {
            HttpRestClient mockedHttpRestClient = new HttpRestClient(serverUrl, agentId, jsonSerializer, Logger);
            mockedHttpRestClient.WebRequestCreator = new MockWebRequestCreator(Logger);
            return mockedHttpRestClient;
        }
    }

    class MockWebRequestCreator : IWebRequestCreate
    {
        private static readonly string BASE_LOCATION = "api/tasks/123412341234/";

        public MockWebRequestCreator(Logger logger)
        {
            Logger = logger;
        }

        public Logger Logger { get; }

        public WebRequest Create(Uri uri)
        {
            Logger?.Verbose("creating mock request for URI: {0}", uri);
            HttpWebRequest webRequest = Substitute.For<HttpWebRequest>();
            webRequest.RequestUri.Returns(uri);
            webRequest.Headers = new WebHeaderCollection();
            webRequest.GetRequestStream().Returns(new MemoryStream(new byte[1000000]));
            webRequest.BeginGetResponse(default, default)
                       .ReturnsForAnyArgs(ci =>
                       {
                           AsyncCallback cb = ci.Arg<AsyncCallback>();
                           HttpWebRequest req = ci.Arg<HttpWebRequest>();
                           Logger?.Verbose("BeginGerResponse called with method {0} for URI: {1}", req.Method, req.RequestUri);
                           cb.Invoke(new MockAsyncResult(req));
                           return null;
                       });

            webRequest.EndGetResponse(default)
                .ReturnsForAnyArgs(ci =>
                {
                    MockAsyncResult mockAsyncResult = ci.Arg<MockAsyncResult>();
                    HttpWebRequest webRequest = ((HttpWebRequest)mockAsyncResult.AsyncState);
                    string method = webRequest.Method;
                    Uri uri = webRequest.RequestUri;
                    Logger?.Verbose("EndGerResponse called for request with method {0} for URI: {1}", method, uri);
                    HttpWebResponse webResponse = Substitute.For<HttpWebResponse>();
                    WebHeaderCollection headers = new WebHeaderCollection();
                    webResponse.Headers.Returns(headers);
                    if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/api/sessions/running", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.Accepted);
                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "status");
                    }
                    else if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "status", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.Created);
                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "created");
                    }
                    else if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase) &&
                        uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "created", StringComparison.OrdinalIgnoreCase))
                    {
                        webResponse.StatusCode.Returns(HttpStatusCode.OK);
                        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"AsExpected\":true}"));
                        webResponse.GetResponseStream().Returns(stream);
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

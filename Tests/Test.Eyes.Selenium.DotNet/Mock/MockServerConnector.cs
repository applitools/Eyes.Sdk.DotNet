using Applitools.Ufg;
using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnector : ServerConnector
    {
        public MockServerConnector(Logger logger, Uri serverUrl, WebDriverProvider driverProvider)
            : base(logger, new Uri("http://some.url.com"))
        {
            logger.Verbose("created");
            HttpRestClientFactory = new MockHttpRestClientFactory(Logger, this);
            driverProvider_ = driverProvider;
        }

        internal List<byte[]> ImagesAsByteArrays { get; } = new List<byte[]>();
        internal List<MatchWindowData> MatchWindowCalls { get; } = new List<MatchWindowData>();
        internal List<MatchWindowData> ReplaceMatchedStepCalls { get; } = new List<MatchWindowData>();
        internal Dictionary<string, RunningSession> Sessions { get; } = new Dictionary<string, RunningSession>();
        internal Dictionary<string, SessionStartInfo> SessionsStartInfo { get; } = new Dictionary<string, SessionStartInfo>();
        internal List<string> SessionIds { get; } = new List<string>();
        private Dictionary<string, JToken> renderRequestsById_ = new Dictionary<string, JToken>();
        public bool AsExpected { get; set; } = true;

        private WebDriverProvider driverProvider_;

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

        protected override void MatchWindowImpl_(TaskListener<MatchResult> listener, MatchWindowData data, string[] testIds)
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

            base.MatchWindowImpl_(listener, data, testIds);
        }

        protected override void StartSessionInternal(TaskListener<RunningSession> taskListener, SessionStartInfo sessionStartInfo)
        {
            Logger.Log(TraceLevel.Info, Stage.Open, StageType.Called, new { sessionStartInfo });

            RunningSession newSession = null;
            bool continueInvocation = true;
            bool callBase = false;
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

        protected override void SendUFGAsyncRequest_<T>(TaskListener<T> taskListener, HttpRequestMessage request) //where T : class
        {
            if (request.Method == HttpMethod.Post &&
                request.RequestUri.PathAndQuery.StartsWith("/render", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes = request.Content.ReadAsByteArrayAsync().Result;
                string requestJsonStr = Encoding.UTF8.GetString(bytes);
                RenderRequests.Add(requestJsonStr);
                List<RunningRender> response = new List<RunningRender>();
                JArray list = (JArray)JsonConvert.DeserializeObject(requestJsonStr);
                foreach (JToken item in list)
                {
                    string id = Guid.NewGuid().ToString();
                    renderRequestsById_.Add(id, item);
                    response.Add(new RunningRender(id, "abc", RenderStatus.Rendered, null, false));
                }
                taskListener.OnComplete(response as T);
            }
            else
            {
                base.SendUFGAsyncRequest_(taskListener, request);
            }
        }

        public override void RenderStatusById(TaskListener<List<RenderStatusResults>> taskListener,
            IList<string> testIds, IList<string> renderIds)
        {
            List<RenderStatusResults> results = new List<RenderStatusResults>();
            foreach (string renderId in renderIds)
            {
                List<VGRegion> selectorRegions = new List<VGRegion>();
                JToken request = renderRequestsById_[renderId];
                JToken selectors = request["selectorsToFindRegionsFor"];
                if (selectors != null && selectors is JArray selectorsArr)
                {
                    IWebDriver driver = driverProvider_.ProvideDriver();
                    foreach (JObject selectorToken in selectorsArr)
                    {
                        string selector = selectorToken["selector"].Value<string>();
                        IWebElement elem = driver.FindElement(By.XPath(selector));
                        Rectangle r = EyesSeleniumUtils.GetElementBounds(elem);
                        selectorRegions.Add(new VGRegion() { X = r.X, Y = r.Y, Width = r.Width, Height = r.Height });
                    }
                }
                RenderStatusResults result = new RenderStatusResults()
                {
                    RenderId = renderId,
                    Status = RenderStatus.Rendered,
                    ImageLocation = "http://image.some.url.com/" + renderId,
                    SelectorRegions = selectorRegions.ToArray()
                };
                results.Add(result);
            }
            taskListener.OnComplete(results);
        }

        public override void GetJobInfo(TaskListener<IList<JobInfo>> listener, IList<IRenderRequest> renderRequests)
        {
            IList<JobInfo> jobs = new List<JobInfo>();
            for (int i = 0; i < renderRequests?.Count; ++i)
            {
                RenderRequest request = (RenderRequest)renderRequests[i];
                string renderer = request.Browser.Name.GetAttribute<System.Runtime.Serialization.EnumMemberAttribute>().Value;
                jobs.Add(new JobInfo() { EyesEnvironment = "MockEnvironment", Renderer = renderer });
            }
            listener.OnComplete(jobs);
        }

        public override void CheckResourceStatus(TaskListener<bool?[]> taskListener, HashSet<string> testId,
            string renderId, HashObject[] hashes)
        {
            bool?[] result = new bool?[hashes.Length];
            for (int i = 0; i < hashes.Length; ++i)
            {
                result[i] = true;
            }
            taskListener.OnComplete(result);
        }

        public override void PostDomCapture(TaskListener<string> listener, string domJson, params string[] testIds)
        {
            listener.OnComplete("http://some.targeturl.com/dom");
        }
    }

    class MockHttpRestClientFactory : IHttpRestClientFactory
    {
        public MockHttpRestClientFactory(Logger logger, MockServerConnector connector)
        {
            Logger = logger;
            connector_ = connector;
        }

        public Logger Logger { get; set; }

        private readonly MockServerConnector connector_;

        public MockHttpClientProvider Provider { get; set; }

        public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
        {
            Provider = new MockHttpClientProvider(Logger, connector_);
            HttpRestClient mockedHttpRestClient = new HttpRestClient(serverUrl, agentId, jsonSerializer, Logger, Provider);
            //mockedHttpRestClient.WebRequestCreator = new MockWebRequestCreator(Logger, connector_);
            return mockedHttpRestClient;
        }
    }

    class MockHttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient httpClient_;
        public MockHttpClientProvider(Logger logger, MockServerConnector connector)
        {
            HttpMessageHandler handler = new MockMessageProcessingHandler(logger, connector);
            httpClient_ = new HttpClient(handler);
        }

        public HttpClient GetClient(IWebProxy proxy)
        {
            return httpClient_;
        }
    }

    class MockMessageProcessingHandler : HttpMessageHandler
    {
        private static readonly string BASE_LOCATION = "api/tasks/123412341234/";
        private readonly MockServerConnector connector_;
        public Logger Logger { get; }

        public MockMessageProcessingHandler(Logger logger, MockServerConnector connector)
        {
            Logger = logger;
            connector_ = connector;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri uri = request.RequestUri;
            Logger?.Verbose("EndGetResponse called for request with method {0} for URI: {1}", request.Method, uri);

            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            HttpResponseMessage response = new HttpResponseMessage();
            response.RequestMessage = request;
            tcs.SetResult(response);

            if (request.Method == HttpMethod.Post &&
                uri.PathAndQuery.StartsWith("/api/sessions/running", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = HttpStatusCode.Accepted;
                response.Headers.Location = new Uri(CommonData.DefaultServerUrl + BASE_LOCATION + "status");
            }
            else if (request.Method == HttpMethod.Get &&
                uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "status", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = HttpStatusCode.Created;
                response.Headers.Location = new Uri(CommonData.DefaultServerUrl + BASE_LOCATION + "created");
            }
            else if (request.Method == HttpMethod.Delete &&
                uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "created", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = HttpStatusCode.OK;
                byte[] bytes = Encoding.UTF8.GetBytes($"{{\"AsExpected\":{connector_.AsExpected.ToString().ToLower()}}}");
                response.Content = new ByteArrayContent(bytes);
                response.Headers.Location = new Uri(CommonData.DefaultServerUrl + BASE_LOCATION + "ok");
            }
            else if (request.Method == HttpMethod.Put)
            {
                response.StatusCode = HttpStatusCode.OK;
                byte[] bytes = request.Content.ReadAsByteArrayAsync().Result;
                connector_.ImagesAsByteArrays.Add(bytes);
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }
            return tcs.Task;
        }
    }
}

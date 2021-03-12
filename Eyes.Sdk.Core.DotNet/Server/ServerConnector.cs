using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{

    /// <summary>
    /// Provides an API for communication with the Applitools Eyes server.
    /// </summary>
    public class ServerConnector : IServerConnector
    {
        #region Fields

        internal HttpRestClient httpClient_;

        private bool apiKeyChanged_ = true;
        private string apiKey_;

        private bool proxyChanged_ = false;
        private WebProxy proxy_;
        private readonly string proxyStr_ = CommonUtils.GetEnvVar("APPLITOOLS_PROXY");

        private RenderingInfo renderingInfo_;
        internal readonly JsonSerializer serializer_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ServerConnector"/> instance.
        /// </summary>
        public ServerConnector(Logger logger, Uri serverUrl = null)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));

            serializer_ = JsonUtils.CreateSerializer(false, false);
            Logger = logger;

            if (serverUrl != null)
            {
                ServerUrl = serverUrl;
            }

            Timeout = TimeSpan.FromMinutes(5);
            logger.Verbose("created");
        }

        internal HttpRestClient CreateHttpRestClient(Uri uri)
        {
            return HttpRestClientFactory.Create(uri, AgentId, serializer_);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the HTTP request timeout of this connector.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The API key identifying the user account.
        /// </summary>
        public string ApiKey
        {
            get
            {
                if (apiKey_ == null)
                {
                    apiKey_ = CommonUtils.GetEnvVar("APPLITOOLS_API_KEY");
                    apiKeyChanged_ = true;
                }
                return apiKey_;
            }
            set
            {
                apiKey_ = value;
                apiKeyChanged_ = true;
            }
        }

        /// <summary>
        /// Gets the Eyes server URL.
        /// </summary>
        public Uri ServerUrl { get; set; }

        /// <summary>
        /// The SDK name.
        /// </summary>
        public string SdkName { get; set; }

        /// <summary>
        /// The Agent ID of the SDK.
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// Gets or sets the proxy used to access the Eyes server or <c>null</c> to use the system 
        /// proxy.
        /// </summary>
        public WebProxy Proxy
        {
            get
            {
                if (proxy_ == null)
                {
                    if (!string.IsNullOrWhiteSpace(proxyStr_))
                    {
                        Logger.Log(TraceLevel.Notice, Stage.General, new { proxyStr_ });
                        proxy_ = new WebProxy(proxyStr_);
                        proxyChanged_ = true;
                    }
                    else
                    {
                        // Apply system web proxy configuration.
                        var proxy = WebRequest.GetSystemWebProxy();
                        if (proxy != null)
                        {
                            Uri proxyUri = proxy.GetProxy(new Uri("http://example.com"));
                            if (proxyUri != null)
                            {
                                proxy_ = new WebProxy(proxyUri);
                                proxy_.Credentials = CredentialCache.DefaultCredentials;
                                proxyChanged_ = true;
                            }
                        }
                    }
                }
                return proxy_;
            }
            set
            {
                proxy_ = value;
                Logger.Log(TraceLevel.Notice, Stage.General, new { proxy_ });
                proxyChanged_ = true;
            }
        }

        /// <summary>
        /// Message logger.
        /// </summary>
        protected internal Logger Logger { get; private set; }

        #endregion

        #region Methods

        // for testing purposes only
        internal RunningSession StartSession(SessionStartInfo sessionStartInfo)
        {
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(logger: Logger, testIds: "TEST");
            StartSessionInternal(listener, sessionStartInfo);
            return listener.Get();
        }

        /// <summary>
        /// Starts a new session.
        /// </summary>
        public void StartSession(TaskListener<RunningSession> taskListener, SessionStartInfo sessionStartInfo)
        {
            StartSessionInternal(taskListener, sessionStartInfo);
        }

        protected virtual void StartSessionInternal(TaskListener<RunningSession> taskListener, SessionStartInfo startInfo)
        {
            ArgumentGuard.NotNull(startInfo, nameof(startInfo));

            var body = new
            {
                StartInfo = startInfo
            };

            try
            {
                EnsureHttpClient_();
                httpClient_.PostJson(
                    new TaskListener<HttpResponseMessage>(
                        response =>
                        {
                            if (response == null)
                            {
                                throw new NullReferenceException("response is null");
                            }
                            // response.DeserializeBody disposes the response object's stream, 
                            // rendering all of its properties unusable, including StatusCode.
                            HttpStatusCode responseStatusCode = response.StatusCode;
                            RunningSession runningSession;
                            if (responseStatusCode == HttpStatusCode.ServiceUnavailable)
                            {
                                runningSession = new RunningSession();
                                runningSession.ConcurrencyFull = true;
                            }
                            else
                            {
                                runningSession = response.DeserializeBody<RunningSession>(
                                    true, serializer_, HttpStatusCode.OK, HttpStatusCode.Created);
                                if (runningSession.isNewSession_ == null)
                                {
                                    runningSession.isNewSession_ = responseStatusCode == HttpStatusCode.Created;
                                }
                                runningSession.ConcurrencyFull = false;
                            }
                            taskListener.OnComplete(runningSession);
                        }, ex => taskListener.OnFail(ex))
                    , "api/sessions/running", body);
            }
            catch (Exception ex)
            {
                throw new EyesException($"StartSession failed: {ex.Message}", ex);
            }
        }

        public virtual void DeleteSession(TestResults testResults)
        {
            ArgumentGuard.NotNull(testResults, nameof(testResults));

            HttpResponseMessage response = null;
            try
            {
                response = httpClient_.Delete($"api/sessions/batches/{testResults.BatchId}/{testResults.Id}?AccessToken={testResults.SecretToken}");
            }
            catch (Exception ex)
            {
                throw new EyesException($"Delete session failed: {ex.Message}", ex);
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <summary>
        /// Ends the input running session.
        /// </summary>
        public void EndSession(TaskListener<TestResults> taskListener, SessionStopInfo sessionStopInfo)
        {
            EndSessionInternal(taskListener, sessionStopInfo);
        }

        protected virtual void EndSessionInternal(TaskListener<TestResults> taskListener, SessionStopInfo sessionStopInfo)
        {
            ArgumentGuard.NotNull(sessionStopInfo, nameof(sessionStopInfo));
            ArgumentGuard.NotNull(sessionStopInfo.RunningSession, nameof(sessionStopInfo.RunningSession));

            httpClient_.DeleteJson(new TaskListener<HttpResponseMessage>(
                response =>
                {
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    taskListener.OnComplete(response.DeserializeBody<TestResults>(true));
                },
                ex => taskListener.OnFail(ex)
                ), $"api/sessions/running/{sessionStopInfo.RunningSession.Id}", sessionStopInfo);
        }

        internal IHttpRestClientFactory HttpRestClientFactory { get; set; } = new DefaultHttpRestClientFactory();

        public virtual void CloseBatch(string batchId)
        {
            CloseBatch(batchId, ServerUrl);
        }

        public virtual void CloseBatch(string batchId, Uri url)
        {
            ArgumentGuard.NotNull(batchId, nameof(batchId));
            if (httpClient_ == null)
            {
                EnsureHttpClient_(url);
            }
            HttpRestClient httpClient = CloneHttpClient_(url);
            HttpResponseMessage response = null;
            try
            {
                response = CloseBatchImpl_(batchId, httpClient);
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Close, StageType.CloseBatch, ex);
            }
            finally
            {
                response?.Dispose();
            }
        }

        protected virtual HttpResponseMessage CloseBatchImpl_(string batchId, HttpRestClient httpClient)
        {
            return httpClient.Delete($"api/sessions/batches/{batchId}/close/bypointerid");
        }

        private HttpRestClient CloneHttpClient_(Uri url)
        {
            HttpRestClient httpClient = httpClient_;
            if (httpClient.ServerUrl != url)
            {
                httpClient = httpClient_.Clone();
                httpClient.ServerUrl = url;
            }

            return httpClient;
        }

        public void MatchWindow(TaskListener<MatchResult> listener, MatchWindowData data, params string[] testIds)
        {
            ArgumentGuard.NotNull(data, nameof(data));

            if (data.AppOutput.ScreenshotBytes != null)
            {
                UploadImage(new TaskListener<string>(
                    returnedUrl =>
                    {
                        Logger.Log(TraceLevel.Notice, testIds, Stage.General, StageType.UploadComplete, new { returnedUrl });
                        if (returnedUrl == null)
                        {
                            listener.OnFail(new EyesException($"{nameof(MatchWindow)} failed: could not upload image to storage service."));
                            return;
                        }
                        try
                        {
                            data.AppOutput.ScreenshotUrl = returnedUrl;
                            MatchWindowImpl_(listener, data, testIds);
                        }
                        catch (Exception ex)
                        {
                            throw new EyesException($"{nameof(MatchWindow)} failed: {ex.Message}", ex);
                        }
                    },
                    ex => listener.OnFail(ex)
                ), data.AppOutput.ScreenshotBytes, testIds);
            }
            else if (data.AppOutput.ScreenshotUrl != null)
            {
                MatchWindowImpl_(listener, data, testIds);
            }
            else
            {
                throw new EyesException("Failed to upload image.");
            }
        }

        protected virtual void MatchWindowImpl_(TaskListener<MatchResult> listener, MatchWindowData data, string[] testIds)
        {
            string url = string.Format("api/sessions/running/{0}", data.RunningSession.Id);
            Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.MatchStart);
            httpClient_.PostJson(new TaskListener<HttpResponseMessage>(
                response =>
                {
                    Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.MatchComplete,
                        new { response?.StatusCode });
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    MatchResult matchResult = response.DeserializeBody<MatchResult>(true);
                    Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.MatchComplete, new { matchResult });
                    listener.OnComplete(matchResult);
                },
                e => listener.OnFail(e)
                ), url, data);
        }

        private void UploadData_(TaskListener<string> listener, byte[] bytes, string contentType, string mediaType,
            string[] testIds)
        {
            RenderingInfo renderingInfo = GetRenderingInfo();
            string targetUrl = renderingInfo?.ResultsUrl?.AbsoluteUri;
            if (targetUrl == null)
            {
                listener.OnComplete(null);
                return;
            }

            Guid guid = Guid.NewGuid();
            targetUrl = targetUrl.Replace("__random__", guid.ToString());
            Logger.Log(TraceLevel.Info, testIds, Stage.General, StageType.UploadStart, new { mediaType, targetUrl });

            HttpRequestMessage request = CreateHttpRequestMessageForUpload_(targetUrl, bytes, contentType, mediaType);
            httpClient_.SendAsyncRequest(new TaskListener<HttpResponseMessage>(
                response =>
                {
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    listener.OnComplete(targetUrl);
                },
                ex => listener.OnFail(ex)),
                request, Logger, new BackoffProvider(2), TimeSpan.FromMinutes(2));
        }

        private HttpRequestMessage CreateHttpRequestMessageForUpload_(string targetUrl, byte[] bytes,
            string contentType, string mediaType)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, new Uri(targetUrl));
            request.Content = new ByteArrayContent(bytes);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Headers.Add("X-Auth-Token", GetRenderingInfo().AccessToken);
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            return request;
        }

        // Used only by IN-REGION

        /// <summary>
        /// Adds the input image to the running session and returns its id.
        /// </summary>
        public string AddRunningSessionImage(RunningSession session, byte[] image)
        {
            ArgumentGuard.NotNull(session, nameof(session));
            ArgumentGuard.NotNull(image, nameof(image));

            try
            {
                using (var response = httpClient_.Post(
                    "api/sessions/running/" + session.Id + "/images",
                    new MemoryStream(image),
                    "application/octet-stream",
                    "application/json"))
                {
                    Uri locationUri = response.Headers.Location;
                    return locationUri.Segments.Last();
                }
            }
            catch (Exception ex)
            {
                throw new EyesException($"AddRunningSessionImage failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the text of the specified language appearing in the input image region
        /// </summary>
        public string[] GetTextInRunningSessionImage(
            RunningSession session,
            string imageId,
            IList<Rectangle> regions,
            string language = null)
        {
            ArgumentGuard.NotNull(session, nameof(session));
            ArgumentGuard.NotNull(imageId, nameof(imageId));
            ArgumentGuard.NotNull(regions, nameof(regions));

            var getText = new GetText_()
            {
                Regions = new List<Region>(regions.Select(r => new Region(r))),
                Language = language,
            };

            using (var response = httpClient_.PostJson(
                $"api/sessions/running/images/{imageId}/text",
                getText))
            {
                if (response == null)
                {
                    throw new NullReferenceException("response is null");
                }
                return response.DeserializeBody<string[]>(true);
            }
        }

        public virtual void GetJobInfo(TaskListener<IList<JobInfo>> listener, IList<IRenderRequest> renderRequests)
        {
            ArgumentGuard.NotNull(renderRequests, nameof(renderRequests));
            string[] testIds = renderRequests.Select(bi => bi.TestId).ToArray();
            Logger.Log(TraceLevel.Notice, testIds, Stage.Open, StageType.JobInfo, new { renderRequests });
            try
            {
                HttpRequestMessage request = CreateUfgHttpWebRequest_("job-info");
                Logger.Log(TraceLevel.Info, testIds, Stage.Open, StageType.RequestSent, new { request.RequestUri });

                httpClient_.SendAsyncRequest(new TaskListener<HttpResponseMessage>(
                    response =>
                    {
                        JObject[] jobInfosUnparsed = response.DeserializeBody<JObject[]>(true);
                        List<JobInfo> jobInfos = new List<JobInfo>();
                        foreach (JObject jobInfoUnparsed in jobInfosUnparsed)
                        {
                            JobInfo jobInfo = new JobInfo
                            {
                                Renderer = jobInfoUnparsed.Value<string>("renderer"),
                                EyesEnvironment = jobInfoUnparsed.Value<object>("eyesEnvironment")
                            };
                            jobInfos.Add(jobInfo);
                        }
                        Logger.Log(TraceLevel.Info, testIds, Stage.Open, StageType.RequestCompleted, new { request.RequestUri });
                        listener.OnComplete(jobInfos);
                    },
                    ex => listener.OnFail(ex)
                    ), request, Logger, new BackoffProvider());
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Open, StageType.JobInfo, e, testIds);
                listener.OnFail(e);
            }
        }

        public void SendLogs(LogSessionsClientEvents clientEvents)
        {
            EnsureHttpClient_();
            using (httpClient_.PostJson("api/sessions/log", clientEvents)) { }
        }

        public HttpRequestMessage CreateUfgHttpWebRequest_(string url, string fullAgentId = null,
            string method = "POST", string contentType = "application/json", string mediaType = "application/json",
            byte[] content = null)
        {
            RenderingInfo renderingInfo = GetRenderingInfo();
            return CreateUfgHttpWebRequest_(url, renderingInfo, fullAgentId ?? AgentId, method,
                contentType, mediaType, content);
        }

        public HttpRequestMessage CreateUfgHttpWebRequest_(string url, RenderingInfo renderingInfo,
            string fullAgentId, string method = "POST", string contentType = "application/json",
            string mediaType = "application/json", object content = null)
        {
            Uri uri = new Uri(renderingInfo.ServiceUrl, url);
            HttpRequestMessage request = httpClient_.CreateHttpRequestMessage(uri, method, content, contentType, mediaType);
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            if (fullAgentId != null)
            {
                request.Headers.Add("x-applitools-eyes-client", fullAgentId);
            }
            request.Headers.Add("x-applitools-eyes-client-request-id", Guid.NewGuid().ToString());
            return request;
        }

        public virtual RenderingInfo GetRenderingInfo()
        {
            if (renderingInfo_ == null)
            {
                renderingInfo_ = GetFromPath(renderingInfo_, "api/sessions/renderinfo", "Render Info");
            }
            return renderingInfo_;
        }

        private T GetFromPath<T>(T member, string path, string name)
        {
            try
            {
                EnsureHttpClient_();
                using (HttpResponseMessage response = httpClient_.GetJson(path))
                {
                    if (response == null)
                    {
                        throw new NullReferenceException($"Getting {name} failed: response is null");
                    }
                    member = response.DeserializeBody<T>(true, serializer_, HttpStatusCode.OK, HttpStatusCode.Created);
                }
            }
            catch (Exception ex)
            {
                throw new EyesException($"Getting {name} failed: {ex.Message}", ex);
            }
            return member;
        }

        public virtual void PostDomCapture(TaskListener<string> listener, string domJson, params string[] testIds)
        {
            Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.UploadStart);
            try
            {
                byte[] binData = Encoding.UTF8.GetBytes(domJson);
                Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.UploadResource,
                    new { UncompressedDataSize = binData.Length });
                using (MemoryStream compressedStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        zipStream.Write(binData, 0, binData.Length);
                    }
                    binData = compressedStream.ToArray();
                }

                Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.UploadResource,
                    new { CompressedDataSize = binData.Length });

                UploadData_(listener, binData, "application/octet-stream", "application/json", testIds);
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, StageType.UploadResource, ex, testIds);
                listener.OnFail(new EyesException($"PostDomSnapshot failed: {ex.Message}", ex));
            }
        }

        protected void EnsureHttpClient_(Uri url = null)
        {
            if (httpClient_ != null && httpClient_.ServerUrl.Equals(ServerUrl) && !apiKeyChanged_ && !proxyChanged_)
            {
                return;
            }
            if (ApiKey == null)
            {
                throw new EyesException("ApiKey is null.");
            }

            ServerUrl = ServerUrl ?? url;
            HttpRestClient httpClient = CreateHttpRestClient(ServerUrl);
            httpClient.FormatRequestUri = uri => uri.AddUriQueryArg("apiKey", ApiKey);
            httpClient.Proxy = Proxy;

            httpClient.RequestCompleted += (s, args) =>
            {
                TraceLevel level = (int)args.Response.StatusCode >= 300 ? TraceLevel.Notice : TraceLevel.Info;
                Logger.Log(level, Stage.General, StageType.RequestCompleted,
                    new { args.Request.Method, args.Request.RequestUri, args.Response.StatusCode, args.Elapsed.TotalMilliseconds });
            };

            httpClient.RequestFailed += (s, args) =>
            {
                Logger.Log(TraceLevel.Error, Stage.General, StageType.RequestFailed,
                 new { args.Request.Method, args.Request.RequestUri, args.Exception, args.Elapsed.TotalMilliseconds });
            };

            httpClient_ = httpClient;
            proxyChanged_ = false;
            apiKeyChanged_ = false;
        }

        public void UploadImage(TaskListener<string> listener, byte[] screenshotBytes, string[] testIds)
        {
            UploadData_(listener, screenshotBytes, "image/png", "image/png", testIds);
        }

        public virtual void CheckResourceStatus(TaskListener<bool?[]> taskListener, HashSet<string> testIds, string renderId, HashObject[] hashes)
        {
            renderId = renderId ?? "NONE";
            HttpRequestMessage request = CreateUfgHttpWebRequest_($"/query/resources-exist?rg_render-id={renderId}");
            Logger.Log(TraceLevel.Info, testIds, Stage.ResourceCollection, StageType.CheckResource, new { hashes, renderId });
            SendUFGAsyncRequest_(taskListener, request);
        }

        protected virtual void SendUFGAsyncRequest_<T>(TaskListener<T> taskListener, HttpRequestMessage request) where T : class
        {
            httpClient_.SendAsyncRequest(new TaskListener<HttpResponseMessage>(
              response =>
              {
                  if (response == null)
                  {
                      throw new NullReferenceException("response is null");
                  }
                  taskListener.OnComplete(response.DeserializeBody<T>(true));
              },
              ex => taskListener.OnFail(ex)), request, Logger, new BackoffProvider());
        }

        public void RenderPutResource(TaskListener<HttpResponseMessage> listener, string renderId, IVGResource resource)
        {
            ArgumentGuard.NotNull(resource, nameof(resource));
            byte[] content = resource.Content;
            ArgumentGuard.NotNull(content, nameof(resource.Content));

            string hash = resource.Sha256;
            string contentType = resource.ContentType;

            Logger.Log(TraceLevel.Info, resource.TestIds, Stage.Render,
                new { resourceHash = hash, resourceUrl = resource.Url, renderId });

            HttpRequestMessage request = CreateUfgHttpWebRequest_($"/resources/sha256/{hash}?render-id={renderId}",
                method: "PUT", contentType: contentType, mediaType: contentType ?? "application/octet-stream",
                content: content);

            httpClient_.SendAsyncRequest(listener, request, Logger, new BackoffProvider());
            Logger.Verbose("future created.");
        }

        public virtual void Render(TaskListener<List<RunningRender>> renderListener, IList<IRenderRequest> requests)
        {
            ArgumentGuard.NotNull(requests, nameof(requests));
            string fullAgentId = AgentId;
            foreach (IRenderRequest renderRequest in requests)
            {
                renderRequest.AgentId = fullAgentId;
                Logger.Log(TraceLevel.Info, renderRequest.TestId, Stage.Render, new { renderRequest });
            }

            string json = serializer_.Serialize(requests);
            byte[] content = Encoding.UTF8.GetBytes(json);
            HttpRequestMessage request = CreateUfgHttpWebRequest_("render", content: content);

            SendUFGAsyncRequest_(renderListener, request);
        }

        public virtual void RenderStatusById(TaskListener<List<RenderStatusResults>> taskListener,
            IList<string> testIds, IList<string> renderIds)
        {
            ArgumentGuard.NotNull(renderIds, nameof(renderIds));
            ArgumentGuard.NotNull(testIds, nameof(testIds));

            for (int i = 0; i < testIds.Count; i++)
            {
                Logger.Log(TraceLevel.Info, testIds[i], Stage.Render, StageType.RenderStatus,
                    new { renderId = renderIds[i] });
            }

            HttpRequestMessage request = CreateUfgHttpWebRequest_("render-status");
            // request.Timeout = 1000;

            SendUFGAsyncRequest_(taskListener, request);
        }

        #endregion

        #region Classes

        private class GetText_
        {
            public IList<Region> Regions { get; set; }

            public string Language { get; set; }
        }

        private class DefaultHttpRestClientFactory : IHttpRestClientFactory
        {
            public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
            {
                return new HttpRestClient(serverUrl, agentId, jsonSerializer);
            }
        }

        #endregion
    }
}

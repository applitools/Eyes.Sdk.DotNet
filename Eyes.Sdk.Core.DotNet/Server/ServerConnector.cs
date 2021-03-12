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
using System.Text;
using System.Threading.Tasks;
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
        private WebProxy proxy;
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
            get => proxy;
            set
            {
                proxy = value;
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
                    new TaskListener<HttpWebResponse>(
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

            HttpWebResponse response = null;
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
                response?.Close();
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

            httpClient_.DeleteJson(new TaskListener<HttpWebResponse>(
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
            HttpWebResponse response = null;
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
                response?.Close();
            }
        }

        protected virtual HttpWebResponse CloseBatchImpl_(string batchId, HttpRestClient httpClient)
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
            httpClient_.PostJson(new TaskListener<HttpWebResponse>(
                response =>
                {
                    Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.MatchComplete,
                        new { response?.StatusCode });
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    MatchResult matchResult = response.DeserializeBody<MatchResult>(true);
                    Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.MatchComplete, new { matchResult });
                    listener.OnComplete(matchResult);
                },
                e => { throw e; }
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
            Logger.Log(TraceLevel.Notice, testIds, Stage.General, StageType.UploadStart, new { mediaType, targetUrl });

            UploadCallback callback = new UploadCallback(listener, this, targetUrl, bytes, contentType, mediaType, testIds);
            callback.UploadDataAsync();
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
                    string locationUrlStr = response.Headers[HttpResponseHeader.Location];
                    Uri uri = new Uri(locationUrlStr);
                    return uri.Segments.Last();
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
                HttpWebRequest request = CreateUfgHttpWebRequest_("job-info");
                Logger.Log(TraceLevel.Info, testIds, Stage.Open, StageType.RequestSent, new { request.RequestUri });
                serializer_.Serialize(renderRequests, request.GetRequestStream());

                HttpRestClient.SendAsyncRequest(new TaskListener<HttpWebResponse>(
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
                    ), request, Logger);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Open, StageType.JobInfo, e, testIds);
                throw;
            }
        }

        public void SendLogs(LogSessionsClientEvents clientEvents)
        {
            EnsureHttpClient_();
            using (httpClient_.PostJson("api/sessions/log", clientEvents)) { }
        }

        public HttpWebRequest CreateUfgHttpWebRequest_(string url, WebProxy proxy = null, string fullAgentId = null,
            string method = "POST", string contentType = "application/json", string mediaType = "application/json")
        {
            RenderingInfo renderingInfo = GetRenderingInfo();
            return CreateUfgHttpWebRequest_(url, renderingInfo, proxy ?? Proxy, fullAgentId ?? AgentId, method, contentType, mediaType);
        }

        public HttpWebRequest CreateUfgHttpWebRequest_(string url, RenderingInfo renderingInfo, WebProxy proxy,
            string fullAgentId, string method = "POST", string contentType = "application/json", string mediaType = "application/json")
        {
            Uri uri = new Uri(renderingInfo.ServiceUrl, url);
            HttpRestClient restClient = CreateHttpRestClient(uri);
            HttpWebRequest request = (HttpWebRequest)restClient.WebRequestCreator.Create(uri);
            //HttpWebRequest request =  WebRequest.CreateHttp(uri); // TODO - replace with factory
            if (proxy != null) request.Proxy = proxy;
            request.ContentType = contentType;
            request.MediaType = mediaType;
            request.Method = method;
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
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
                using (HttpWebResponse response = httpClient_.GetJson(path))
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
            Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.UploadStart);
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

                Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.UploadResource,
                    new { CompressedDataSize = binData.Length });

                UploadData_(new TaskListener<string>(
                    r => listener.OnComplete(r),
                    ex => listener.OnFail(ex)
                ), binData, "application/octet-stream", "application/json", testIds);
            }
            catch (Exception ex)
            {
                throw new EyesException($"PostDomSnapshot failed: {ex.Message}", ex);
            }
            finally
            {
                Logger.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.UploadComplete);
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
            UploadData_(new TaskListener<string>(
                    r => listener.OnComplete(r),
                    ex => listener.OnFail(ex)
                ),
                screenshotBytes, "image/png", "image/png", testIds);
        }

        public virtual void CheckResourceStatus(TaskListener<bool?[]> taskListener, HashSet<string> testIds, string renderId, HashObject[] hashes)
        {
            renderId = renderId ?? "NONE";
            HttpWebRequest request = CreateUfgHttpWebRequest_($"/query/resources-exist?rg_render-id={renderId}");
            Logger.Log(TraceLevel.Info, testIds, Stage.ResourceCollection, StageType.CheckResource, new { hashes, renderId });
            serializer_.Serialize(hashes, request.GetRequestStream());
            SendUFGAsyncRequest_(taskListener, request);
        }

        protected virtual void SendUFGAsyncRequest_<T>(TaskListener<T> taskListener, HttpWebRequest request) where T : class
        {
            HttpRestClient.SendAsyncRequest(new TaskListener<HttpWebResponse>(
              response =>
              {
                  if (response == null)
                  {
                      throw new NullReferenceException("response is null");
                  }
                  taskListener.OnComplete(response.DeserializeBody<T>(true));
              },
              ex => taskListener.OnFail(ex)), request, Logger);
        }

        public Task<WebResponse> RenderPutResourceAsTask(string renderId, IVGResource resource)
        {
            ArgumentGuard.NotNull(resource, nameof(resource));
            byte[] content = resource.Content;
            ArgumentGuard.NotNull(content, nameof(resource.Content));

            string hash = resource.Sha256;
            string contentType = resource.ContentType;

            Logger.Log(TraceLevel.Info, resource.TestIds, Stage.Render,
                new { resourceHash = hash, resourceUrl = resource.Url, renderId });

            HttpWebRequest request = CreateUfgHttpWebRequest_($"/resources/sha256/{hash}?render-id={renderId}",
                method: "PUT", contentType: contentType, mediaType: contentType ?? "application/octet-stream");
            request.ContentLength = content.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(content, 0, content.Length);
            dataStream.Close();

            Task<WebResponse> task = request.GetResponseAsync();
            Logger.Verbose("future created.");
            return task;
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

            HttpWebRequest request = CreateUfgHttpWebRequest_("render");
            serializer_.Serialize(requests, request.GetRequestStream());

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

            HttpWebRequest request = CreateUfgHttpWebRequest_("render-status");
            request.ContinueTimeout = 1000;
            serializer_.Serialize(renderIds, request.GetRequestStream());

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

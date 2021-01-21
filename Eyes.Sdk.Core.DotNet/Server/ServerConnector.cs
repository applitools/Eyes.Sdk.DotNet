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
using System.Threading;
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
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(logger: Logger);
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

            HttpRestClient httpClient = httpClient_;
            if (httpClient.ServerUrl != url)
            {
                httpClient = httpClient_.Clone();
                httpClient.ServerUrl = url;
            }
            HttpWebResponse response = null;
            try
            {
                response = httpClient.Delete($"api/sessions/batches/{batchId}/close/bypointerid");
            }
            catch (Exception ex)
            {
                Logger.Log($"WARNING: Close session failed: {ex.Message}");
            }
            finally
            {
                response?.Close();
            }
        }


        public void MatchWindow(TaskListener<MatchResult> listener, MatchWindowData data)
        {
            ArgumentGuard.NotNull(data, nameof(data));

            if (data.AppOutput.ScreenshotBytes != null)
            {
                UploadImage(new TaskListener<string>(
                    returnedUrl =>
                    {
                        if (returnedUrl == null)
                        {
                            listener.OnFail(new EyesException($"{nameof(MatchWindow)} failed: could not upload image to storage service."));
                            return;
                        }
                        try
                        {
                            data.AppOutput.ScreenshotUrl = returnedUrl;
                            MatchWindowImpl_(listener, data);
                        }
                        catch (Exception ex)
                        {
                            throw new EyesException($"{nameof(MatchWindow)} failed: {ex.Message}", ex);
                        }
                    },
                    ex => listener.OnFail(ex)
                ), data.AppOutput.ScreenshotBytes);
            }
            else if (data.AppOutput.ScreenshotUrl != null)
            {
                MatchWindowImpl_(listener, data);
            }
            else
            {
                throw new EyesException("Failed to upload image.");
            }
        }

        private void MatchWindowImpl_(TaskListener<MatchResult> listener, MatchWindowData data)
        {
            string url = string.Format("api/sessions/running/{0}", data.RunningSession.Id);
            httpClient_.PostJson(new TaskListener<HttpWebResponse>(
                response =>
                {
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    listener.OnComplete(response.DeserializeBody<MatchResult>(true));
                },
                e => { throw e; }
                ), url, data);
        }

        /// <summary>
        /// Matches the current window with the currently expected window.
        /// </summary>
        /// <param name="data"></param>
        public virtual MatchResult MatchWindow(MatchWindowData data)
        {
            SyncTaskListener<MatchResult> sync = new SyncTaskListener<MatchResult>(null, e => throw e, Logger);
            MatchWindow(sync, data);
            return sync.Get();
        }

        private void UploadData_(TaskListener<string> listener, byte[] bytes, string contentType, string mediaType)
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
            Logger.Verbose("uploading {0} to {1}", mediaType, targetUrl);

            UploadCallback callback = new UploadCallback(listener, this, targetUrl, bytes, contentType, mediaType);
            callback.UploadDataAsync();
        }

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

        public virtual void GetJobInfo(TaskListener<IList<JobInfo>> listener, IList<IRenderRequest> browserInfos)
        {
            ArgumentGuard.NotNull(browserInfos, nameof(browserInfos));
            Logger.Verbose("called with {0}", StringUtils.Concat(browserInfos, ","));
            try
            {
                HttpWebRequest request = CreateUfgHttpWebRequest_("job-info");
                Logger.Verbose("sending /job-info request to {0}", request.RequestUri);
                serializer_.Serialize(browserInfos, request.GetRequestStream());

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
                        Logger.Verbose("request succeeded");
                        listener.OnComplete(jobInfos);
                    },
                    ex => listener.OnFail(ex)
                    ), request);
            }
            catch (Exception e)
            {
                Logger.Log("Error: {0}", e);
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
            Logger.Verbose("enter");
            if (renderingInfo_ == null)
            {
                renderingInfo_ = GetFromPath(renderingInfo_, "api/sessions/renderinfo", "Render Info");
            }
            Logger.Verbose("exit");
            return renderingInfo_;
        }

        private T GetFromPath<T>(T member, string path, string name)
        {
            Logger.Verbose("enter");

            Logger.Verbose("trying to get {0} from server ...", name);
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
            Logger.Verbose("exit");
            return member;
        }

        public virtual void PostDomCapture(TaskListener<string> listener, string domJson)
        {
            try
            {
                byte[] binData = Encoding.UTF8.GetBytes(domJson);

                using (MemoryStream compressedStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        zipStream.Write(binData, 0, binData.Length);
                    }
                    binData = compressedStream.ToArray();
                }

                UploadData_(new TaskListener<string>(
                    r => listener.OnComplete(r),
                    ex => listener.OnFail(ex)
                ), binData, "application/octet-stream", "application/json");
            }
            catch (Exception ex)
            {
                throw new EyesException($"PostDomSnapshot failed: {ex.Message}", ex);
            }
        }

        protected void EnsureHttpClient_()
        {
            if (httpClient_ != null && httpClient_.ServerUrl.Equals(ServerUrl) && !apiKeyChanged_ && !proxyChanged_)
            {
                return;
            }
            if (ApiKey == null)
            {
                throw new EyesException("ApiKey is null.");
            }
            Logger.Verbose("enter");
            HttpRestClient httpClient = CreateHttpRestClient(ServerUrl);
            httpClient.FormatRequestUri = uri => uri.AddUriQueryArg("apiKey", ApiKey);
            httpClient.Proxy = Proxy;

            httpClient.RequestCompleted += (s, args) =>
            {
                if ((int)args.Response.StatusCode >= 300)
                {
                    Logger.Log(args.ToString());
                }
                else
                {
                    Logger.Verbose(args.ToString());
                }
            };

            httpClient.RequestFailed += (s, args) =>
            {
                Logger.Log(args.ToString());
            };

            httpClient_ = httpClient;
            proxyChanged_ = false;
            apiKeyChanged_ = false;
        }

        public void UploadImage(TaskListener<string> listener, byte[] screenshotBytes)
        {
            UploadData_(new TaskListener<string>(
                    r => listener.OnComplete(r),
                    ex => listener.OnFail(ex)
                ),
                screenshotBytes, "image/png", "image/png");
        }

        public virtual void CheckResourceStatus(TaskListener<bool?[]> taskListener, string renderId, HashObject[] hashes)
        {
            HttpWebRequest request = CreateUfgHttpWebRequest_($"/query/resources-exist?rg_render-id={renderId}");
            Logger.Verbose("querying for existing resources for render id {0}", renderId);
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
              ex => taskListener.OnFail(ex)), request);
        }

        public Task<WebResponse> RenderPutResourceAsTask(string renderId, IVGResource resource)
        {
            ArgumentGuard.NotNull(resource, nameof(resource));
            byte[] content = resource.Content;
            ArgumentGuard.NotNull(content, nameof(resource.Content));

            string hash = resource.Sha256;
            string contentType = resource.ContentType;

            Logger.Verbose("resource hash: {0} ; url: {1} ; render id: {2}", hash, resource.Url, renderId);

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
            Logger.Verbose("called with {0}", StringUtils.Concat(requests, ","));
            string fullAgentId = AgentId;
            foreach (IRenderRequest renderRequest in requests)
            {
                renderRequest.AgentId = fullAgentId;
            }

            HttpWebRequest request = CreateUfgHttpWebRequest_("render");
            Logger.Verbose("sending /render request to {0}", request.RequestUri);
            serializer_.Serialize(requests, request.GetRequestStream());

            SendUFGAsyncRequest_(renderListener, request);
        }

        public void RenderStatusById(TaskListener<List<RenderStatusResults>> taskListener, IList<string> renderIds)
        {
            ArgumentGuard.NotNull(renderIds, nameof(renderIds));
            string idsAsString = string.Join(",", renderIds);
            Logger.Verbose("requesting visual grid server for render status of the following render ids: {0}", idsAsString);

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

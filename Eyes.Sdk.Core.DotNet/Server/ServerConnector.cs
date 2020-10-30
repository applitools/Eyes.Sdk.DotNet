using Applitools.Ufg;
using Applitools.Utils;
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

        private HttpRestClient httpClient_;
        private bool apiKeyChanged_ = true;
        private string apiKey_;
        private bool proxyChanged_ = false;
        private WebProxy proxy;
        private RenderingInfo renderingInfo_;
        private readonly JsonSerializer serializer_;

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
        protected Logger Logger { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts a new session.
        /// </summary>
        public RunningSession StartSession(SessionStartInfo startInfo)
        {
            return StartSessionInternal(startInfo);
        }

        protected virtual RunningSession StartSessionInternal(SessionStartInfo startInfo)
        {
            ArgumentGuard.NotNull(startInfo, nameof(startInfo));

            var body = new
            {
                StartInfo = startInfo
            };

            try
            {
                EnsureHttpClient_();
                using (HttpWebResponse response = httpClient_.PostJson("api/sessions/running", body))
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
                    return runningSession;
                }
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
        public TestResults EndSession(RunningSession runningSession, bool isAborted, bool save)
        {
            return EndSessionInternal(runningSession, isAborted, save);
        }

        protected virtual TestResults EndSessionInternal(RunningSession runningSession, bool isAborted, bool save)
        {
            ArgumentGuard.NotNull(runningSession, nameof(runningSession));

            var body = new
            {
                Aborted = isAborted,
                UpdateBaseline = save,
            };

            try
            {
                using (HttpWebResponse response = httpClient_.DeleteJson("api/sessions/running/" + runningSession.Id, body))
                {
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    return response.DeserializeBody<TestResults>(true);
                }
            }
            catch (Exception ex)
            {
                throw new EyesException($"EndSession failed: {ex.Message}", ex);
            }
        }

        public bool DontCloseBatches { get; } = "true".Equals(CommonUtils.GetEnvVar("APPLITOOLS_DONT_CLOSE_BATCHES"), StringComparison.OrdinalIgnoreCase);
        internal IHttpRestClientFactory HttpRestClientFactory { get; set; } = new DefaultHttpRestClientFactory();

        public virtual void CloseBatch(string batchId)
        {
            if (DontCloseBatches)
            {
                Logger.Log("APPLITOOLS_DONT_CLOSE_BATCHES environment variable set to true. Doing nothing.");
                return;
            }

            ArgumentGuard.NotNull(batchId, nameof(batchId));

            HttpWebResponse response = null;
            try
            {
                response = httpClient_.Delete($"api/sessions/batches/{batchId}/close/bypointerid");
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

        /// <summary>
        /// Matches the current window with the currently expected window.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="session"></param>
        public virtual MatchResult MatchWindow(RunningSession session, MatchWindowData data)
        {
            ArgumentGuard.NotNull(session, nameof(session));
            ArgumentGuard.NotNull(data, nameof(data));

            if (!TryUploadImage_(data))
            {
                throw new EyesException($"{nameof(MatchWindow)} failed: could not upload image to storage service.");
            }

            try
            {
                string url = string.Format("api/sessions/running/{0}", session.Id);
                using (HttpWebResponse response = httpClient_.PostJson(url, data))
                {
                    if (response == null)
                    {
                        throw new NullReferenceException("response is null");
                    }
                    return response.DeserializeBody<MatchResult>(true);
                }
            }
            catch (Exception ex)
            {
                throw new EyesException($"{nameof(MatchWindow)} failed: {ex.Message}", ex);
            }
        }

        private bool TryUploadImage_(MatchWindowData data)
        {
            if (data.AppOutput.ScreenshotUrl != null)
            {
                return true;
            }

            byte[] bytes = data.AppOutput.ScreenshotBytes;
            return (data.AppOutput.ScreenshotUrl = TryUploadData_(bytes, "image/png", "image/png")) != null;
        }

        private string TryUploadData_(byte[] bytes, string contentType, string mediaType)
        {
            RenderingInfo renderingInfo = GetRenderingInfo();

            if (renderingInfo != null && renderingInfo.ResultsUrl != null)
            {
                try
                {
                    string targetUrl = renderingInfo.ResultsUrl.AbsoluteUri;
                    Guid guid = Guid.NewGuid();
                    targetUrl = targetUrl.Replace("__random__", guid.ToString());
                    Logger.Verbose("uploading {0} to {1}", mediaType, targetUrl);

                    int retriesLeft = 3;
                    int wait = 500;
                    while (retriesLeft-- > 0)
                    {
                        try
                        {
                            int statusCode = UploadData_(bytes, renderingInfo, targetUrl, contentType, mediaType);
                            if (statusCode == 200 || statusCode == 201)
                            {
                                Logger.Verbose("upload {0} guid {1} complete.", mediaType, guid);
                                return targetUrl;
                            }
                            if (statusCode < 500)
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            if (retriesLeft == 0) throw;
                        }
                        Thread.Sleep(wait);
                        wait *= 2;
                        wait = Math.Min(10000, wait);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Error uploading " + mediaType + ": " + e);
                }
            }
            return null;
        }

        private int UploadData_(byte[] bytes, RenderingInfo renderingInfo, string targetUrl, string contentType, string mediaType)
        {
            HttpWebRequest request = WebRequest.CreateHttp(targetUrl);
            if (Proxy != null) request.Proxy = Proxy;
            request.ContentType = contentType;
            request.ContentLength = bytes.Length;
            request.MediaType = mediaType;
            request.Method = "PUT";
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bytes, 0, bytes.Length);
            dataStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            HttpStatusCode statusCode = response.StatusCode;
            Logger.Verbose("Upload Status Code: {0}", statusCode);
            response.Close();
            return (int)statusCode;
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

        public virtual async Task<List<JobInfo>> GetJobInfo(RenderRequest[] browserInfos)
        {
            ArgumentGuard.NotNull(browserInfos, nameof(browserInfos));
            Logger.Verbose("called with {0}", StringUtils.Concat(browserInfos, ","));
            try
            {
                RenderingInfo renderingInfo = GetRenderingInfo();

                HttpWebRequest request = CreateHttpWebRequest_("job-info", renderingInfo, Proxy, AgentId);
                Logger.Verbose("sending /job-info request to {0}", request.RequestUri);
                serializer_.Serialize(browserInfos, request.GetRequestStream());

                using (WebResponse response = await request.GetResponseAsync())
                {
                    Stream s = response.GetResponseStream();
                    string json = new StreamReader(s).ReadToEnd();
                    JObject[] jobInfosUnparsed = JsonConvert.DeserializeObject<JObject[]>(json);
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
                    return jobInfos;
                }
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

        public static HttpWebRequest CreateHttpWebRequest_(string url,
            RenderingInfo renderingInfo, WebProxy proxy, string fullAgentId)
        {
            Uri uri = new Uri(renderingInfo.ServiceUrl, url);
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            if (proxy != null) request.Proxy = proxy;
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Method = "POST";
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            request.Headers.Add("x-applitools-eyes-client", fullAgentId);
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

        public string PostDomCapture(string domJson)
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

                return TryUploadData_(binData, "application/octet-stream", "application/json");
            }
            catch (Exception ex)
            {
                throw new EyesException($"PostDomSnapshot failed: {ex.Message}", ex);
            }
        }

        private void EnsureHttpClient_()
        {
            if (httpClient_ != null && httpClient_.ServerUrl.Equals(ServerUrl) && !apiKeyChanged_ && !proxyChanged_)
            {
                return;
            }
            if (ApiKey == null)
            {
                throw new EyesException("ApiKey is null.");
            }
            Logger.Log("enter");
            //HttpRestClient httpClient = new HttpRestClient(ServerUrl, AgentId, json_);
            HttpRestClient httpClient = HttpRestClientFactory.Create(ServerUrl, AgentId, serializer_);
            httpClient.FormatRequestUri = uri => uri.AddUriQueryArg("apiKey", ApiKey);
            httpClient.AcceptLongRunningTasks = true;
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

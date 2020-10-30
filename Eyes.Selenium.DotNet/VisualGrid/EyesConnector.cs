using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Applitools.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Applitools.Selenium.VisualGrid
{
    public class EyesConnector : EyesBase, IUfgConnector
    {
        private readonly RenderBrowserInfo browserInfo_;
        private RenderingInfo renderInfo_;
        private HttpRestClient httpClient_;
        private readonly JsonSerializer serializer_ = JsonUtils.CreateSerializer();
        private RenderStatusResults renderStatusResult_;
        private string testName_;
        private string appName_;
        private Applitools.Configuration config_;
        private JobInfo jobInfo_;

        internal EyesConnector(Logger logger, RenderBrowserInfo browserInfo,
            Applitools.Configuration configuration, IServerConnectorFactory serverConnectorFactory)
            : base(serverConnectorFactory, logger)
        {
            browserInfo_ = browserInfo;
            config_ = configuration;
            UpdateServerConnector_();
        }

        public EyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration configuration)
            : base(logger)
        {
            browserInfo_ = browserInfo;
            config_ = configuration;
            UpdateServerConnector_();
        }

        #region Properties

        public override string TestName { get => testName_; set => testName_ = value; }
        protected override Applitools.Configuration Configuration => config_;
        public override string AppName { get => appName_; set => appName_ = value; }

        #endregion

        protected override string GetInferredEnvironment()
        {
            return null;
        }

        protected override object GetEnvironment_()
        {
            Logger.Verbose("enter");
            return GetJobInfo().EyesEnvironment;
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal) { return null; }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings)
        {
            return null;
        }

        protected override string GetTitle()
        {
            return null;
        }

        protected override Size GetViewportSize()
        {
            return Size.Empty;
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            Logger.Log("WARNING SetViewportSize() was called in Visual-Grid context");
        }

        public bool HideCaret { get; set; }

        public RenderingInfo GetRenderingInfo()
        {
            if (renderInfo_ != null)
            {
                Logger.Verbose("returning cached rendering info.");
                return renderInfo_;
            }
            Logger.Verbose("aquiring rendering info");
            ServerConnector.ApiKey = ApiKey;
            ServerConnector.ServerUrl = new Uri(ServerUrl);
            renderInfo_ = ServerConnector.GetRenderingInfo();
            UpdateRenderingServiceServerConnector();
            return renderInfo_;
        }

        public void SetRenderInfo(RenderingInfo renderingInfo)
        {
            renderInfo_ = renderingInfo;
            UpdateRenderingServiceServerConnector();
        }

        public async Task<List<JobInfo>> GetJobInfo(RenderRequest[] browserInfos)
        {
            ArgumentGuard.NotNull(browserInfos, nameof(browserInfos));
            Logger.Verbose("called with {0}", StringUtils.Concat(browserInfos, ","));
            try
            {
                HttpWebRequest request = CreateHttpWebRequest_("job-info");
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

        private void UpdateRenderingServiceServerConnector()
        {
            httpClient_ = new HttpRestClient(renderInfo_.ServiceUrl, FullAgentId, serializer_);
            httpClient_.Headers.Add("X-Auth-Token", renderInfo_.AccessToken);
        }

        public virtual List<RunningRender> Render(RenderRequest[] renderRequests)
        {
            return RenderAsync(renderRequests).Result;
        }

        public virtual async Task<List<RunningRender>> RenderAsync(RenderRequest[] renderRequests)
        {
            ArgumentGuard.NotNull(renderRequests, nameof(renderRequests));
            Logger.Verbose("called with {0}", StringUtils.Concat(renderRequests, ","));
            try
            {
                string fullAgentId = FullAgentId;
                foreach (RenderRequest renderRequest in renderRequests)
                {
                    renderRequest.AgentId = fullAgentId;
                }

                HttpWebRequest request = CreateHttpWebRequest_("render");
                Logger.Verbose("sending /render request to {0}", request.RequestUri);
                serializer_.Serialize(renderRequests, request.GetRequestStream());

                using (WebResponse response = await request.GetResponseAsync())
                {
                    Stream s = response.GetResponseStream();
                    string json = new StreamReader(s).ReadToEnd();
                    List<RunningRender> runningRenders = serializer_.Deserialize<List<RunningRender>>(json);
                    Logger.Verbose("request succeeded");
                    return runningRenders;
                }
            }
            catch (Exception e)
            {
                Logger.Log("exception in render: " + e);
            }

            return null;
        }

        private HttpWebRequest CreateHttpWebRequest_(string url)
        {
            RenderingInfo renderingInfo = GetRenderingInfo();
            Uri uri = new Uri(renderInfo_.ServiceUrl, url);
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            if (Proxy != null) request.Proxy = Proxy;
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Method = "POST";
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            request.Headers.Add("x-applitools-eyes-client", FullAgentId);
            return request;
        }

        public List<RenderStatusResults> RenderStatusById(string[] renderIds)
        {
            ArgumentGuard.NotNull(renderIds, nameof(renderIds));
            string idsAsString = string.Join(",", renderIds);
            Logger.Verbose("requesting visual grid server for render status of the following render ids: {0}", idsAsString);

            string json = null;
            try
            {
                HttpWebRequest request = CreateHttpWebRequest_("render-status");
                request.ContinueTimeout = 1000;
                serializer_.Serialize(renderIds, request.GetRequestStream());
                using (WebResponse response = request.GetResponse())
                {
                    Stream s = response.GetResponseStream();
                    json = new StreamReader(s).ReadToEnd();
                    List<RenderStatusResults> renderStatusResults = serializer_.Deserialize<List<RenderStatusResults>>(json);

                    Logger.Verbose("request succeeded");
                    foreach (RenderStatusResults renderStatusResult in renderStatusResults)
                    {
                        Logger.Verbose("render status result for id {0}: {1}", renderStatusResult?.RenderId, renderStatusResult?.Status);
                    }
                    return renderStatusResults;
                }
            }
            catch (JsonException e)
            {
                Logger.Log("error in JSON: {0}. exception in render status: {1}", json, e);
            }
            return null;
        }

        public MatchResult MatchWindow(Applitools.IConfiguration config, string resultImageURL, string domLocation, ICheckSettings checkSettings,
                                       IList<IRegion> regions, IList<VisualGridSelector[]> regionSelectors,
                                       Location location, RenderStatusResults results, string source)
        {
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            config_ = (Applitools.Configuration)config;

            MatchWindowTask matchWindowTask = new MatchWindowTask(Logger, ServerConnector, runningSession_, Configuration.MatchTimeout, this, null);

            ImageMatchSettings imageMatchSettings = MatchWindowTask.CreateImageMatchSettings(checkSettingsInternal, this);

            string tag = checkSettingsInternal.GetName();

            AppOutput appOutput = new AppOutput(tag, location, null, resultImageURL, domLocation, results.VisualViewport);
            AppOutputWithScreenshot appOutputWithScreenshot = new AppOutputWithScreenshot(appOutput, null);

            renderStatusResult_ = results;
            return matchWindowTask.PerformMatch(appOutputWithScreenshot, tag, false, checkSettingsInternal, imageMatchSettings, regions, regionSelectors, this, source, results.RenderId);
        }

        public void Open(Applitools.IConfiguration config)
        {
            Logger.Verbose("opening EyesConnector with {0} ({1})", browserInfo_, GetHashCode());
            config_ = (Configuration)config;
            AppName = config.AppName;
            TestName = config.TestName;
            //config_.SetViewportSize(deviceSize_ ?? browserInfo_.ViewportSize);
            //config_.SetBaselineEnvName(browserInfo_.BaselineEnvName);
            //Logger.Verbose("validating: {0} ({1})", Configuration.GetViewportSize(), GetHashCode());
            OpenBase();
        }

        public Task<WebResponse> RenderPutResourceAsTask(RunningRender runningRender, RGridResource resource)
        {
            ArgumentGuard.NotNull(runningRender, nameof(runningRender));
            ArgumentGuard.NotNull(resource, nameof(resource));
            byte[] content = resource.Content;
            ArgumentGuard.NotNull(content, nameof(resource.Content));

            string hash = resource.Sha256;
            string renderId = runningRender.RenderId;
            string contentType = resource.ContentType;

            Logger.Verbose("resource hash: {0} ; url: {1} ; render id: {2}", hash, resource.Url, renderId);

            RenderingInfo renderingInfo = GetRenderingInfo();
            Uri url = new Uri(renderingInfo.ServiceUrl, $"/resources/sha256/{hash}?render-id={renderId}");
            HttpWebRequest request = WebRequest.CreateHttp(url);
            if (Proxy != null) request.Proxy = Proxy;
            request.ContentType = contentType;
            request.ContentLength = content.Length;
            request.MediaType = contentType ?? "application/octet-stream";
            request.Method = "PUT";
            request.Headers.Add("X-Auth-Token", renderingInfo.AccessToken);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(content, 0, content.Length);
            dataStream.Close();

            Task<WebResponse> task = request.GetResponseAsync();
            Logger.Verbose("future created.");
            return task;
        }

        public PutFuture RenderPutResource(RunningRender runningRender, RGridResource resource)
        {
            Task<WebResponse> task = RenderPutResourceAsTask(runningRender, resource);
            return new PutFuture(task, resource, runningRender, this, Logger);
        }

        public ResourceFuture GetResource(Uri url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Proxy = Proxy;
            Task<WebResponse> task = request.GetResponseAsync();
            Logger?.Verbose("Downloading data from {0}", url);
            ResourceFuture result = new ResourceFuture(task, url, Logger);
            return result;
        }

        protected override string BaseAgentId => VisualGridEyes.GetBaseAgentId();

        protected override object GetAgentSetup()
        {
            Assembly eyesSeleniumAsm = this.GetType().Assembly;
            Assembly eyesCoreAsm = typeof(EyesBase).Assembly;
            string eyesSeleniumTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesSeleniumAsm);
            string eyesCoreTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesCoreAsm);
            return new
            {
                SeleniumSDKAssemblyData = new
                {
                    eyesSeleniumAsm.ImageRuntimeVersion,
                    eyesSeleniumTargetFramework
                },
                CoreSDKAssemblyData = new
                {
                    eyesCoreAsm.ImageRuntimeVersion,
                    eyesCoreTargetFramework
                },
                BrowserInfo = browserInfo_,
                FullAgentId,
                renderStatusResult_?.RenderId,
                renderStatusResult_?.Error
            };
        }

        public TestResults Close(bool throwEx, Applitools.IConfiguration config)
        {
            config_ = (Applitools.Configuration)config;
            return Close(throwEx);
        }

        public bool?[] CheckResourceStatus(string renderId, HashObject[] hashes)
        {
            using (HttpWebResponse response = httpClient_.PostJson($"/query/resources-exist?rg_render-id={renderId}", hashes))
            {
                return response.DeserializeBody<bool?[]>(true);
            }
        }

        public JobInfo GetJobInfo()
        {
            if (jobInfo_ == null)
            {
                RenderInfo renderInfo = new RenderInfo(browserInfo_.Width, browserInfo_.Height, default, null,
                    null, browserInfo_.EmulationInfo, browserInfo_.IosDeviceInfo);
                RenderRequest renderRequest = new RenderRequest(renderInfo, browserInfo_.Platform, browserInfo_.BrowserType);
                Task<List<JobInfo>> jobInfoTask = GetJobInfo(new RenderRequest[] { renderRequest });
                List<JobInfo> jobInfos = jobInfoTask.Result;
                if (jobInfos == null || jobInfos.Count == 0)
                {
                    throw new EyesException("Failed getting job info");
                }
                jobInfo_ = jobInfos[0];
            }
            return jobInfo_;
        }

        public string GetRenderer()
        {
            return GetJobInfo().Renderer;
        }
    }
}
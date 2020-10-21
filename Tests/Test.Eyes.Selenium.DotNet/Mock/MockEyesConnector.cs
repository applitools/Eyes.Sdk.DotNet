using Applitools.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.Mock
{
    class MockEyesConnector : EyesBase, IUfgConnector
    {
        private readonly RenderBrowserInfo browserInfo_;
        private readonly Applitools.Configuration config_;
        public IUfgConnector WrappedConnector { get; set; }

        public MockEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            browserInfo_ = browserInfo;
            config_ = config;
            ServerConnectorFactory = new MockServerConnectorFactory();

            userAgents_ = new Dictionary<BrowserType, string>()
            {
                { BrowserType.CHROME, "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/86.0.4240.75 Safari/537.36" },
                { BrowserType.CHROME_ONE_VERSION_BACK, "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/85.0.4183.83 Safari/537.36" },
                { BrowserType.CHROME_TWO_VERSIONS_BACK, "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/84.0.4147.89 Safari/537.36" },
                { BrowserType.FIREFOX, "Mozilla/5.0 (X11; Linux x86_64; rv:81.0) Gecko/20100101 Firefox/81.0" },
                { BrowserType.FIREFOX_ONE_VERSION_BACK, "Mozilla/5.0 (X11; Linux x86_64; rv:80.0) Gecko/20100101 Firefox/80.0" },
                { BrowserType.FIREFOX_TWO_VERSIONS_BACK, "Mozilla/5.0 (X11; Linux x86_64; rv:79.0) Gecko/20100101 Firefox/79.0" },
            };

            emulatedDevicesSizes_ = new Dictionary<DeviceName, DeviceSize>()
            {
                { DeviceName.iPhone_4, new DeviceSize(480, 320) },
                { DeviceName.iPhone_5SE, new DeviceSize(568, 320) },
                { DeviceName.Galaxy_S3, new DeviceSize(640, 360) },
                { DeviceName.Galaxy_S5, new DeviceSize(640, 360) }
            };

            iosDevicesSizes_ = new Dictionary<IosDeviceName, DeviceSize>()
            {
                { IosDeviceName.iPhone_11, new DeviceSize(896, 414) },
                { IosDeviceName.iPhone_11_Pro, new DeviceSize(812, 375) },
                { IosDeviceName.iPhone_11_Pro_Max, new DeviceSize(896, 414) },
                { IosDeviceName.iPhone_XS, new DeviceSize(812, 375) },
                { IosDeviceName.iPhone_X, new DeviceSize(812, 375) },
                { IosDeviceName.iPhone_XR, new DeviceSize(896, 414) },
                { IosDeviceName.iPhone_8, new DeviceSize(667, 375) },
                { IosDeviceName.iPhone_7, new DeviceSize(667, 375) },
                { IosDeviceName.iPad_Air_2, new DeviceSize(1024, 768) },
                { IosDeviceName.iPad_Pro_3, new DeviceSize(1366, 1024) },
                { IosDeviceName.iPad_7, new DeviceSize(1080, 810) },
            };
        }

        public RenderRequest[] LastRenderRequests { get; set; }

        protected override Applitools.Configuration Configuration => config_;

        public TestResults Close(bool throwEx, Applitools.IConfiguration config)
        {
            return new TestResults() { Status = TestResultsStatus.Passed };
        }

        public RenderingInfo GetRenderingInfo()
        {
            return new RenderingInfo();
        }

        public ResourceFuture GetResource(Uri url)
        {
            throw new NotImplementedException();
        }

        public MatchResult MatchWindow(Applitools.IConfiguration config, string resultImageURL, string domLocation,
            ICheckSettings checkSettings, IList<IRegion> regions, IList<VisualGridSelector[]> regionSelectors, Location location,
            RenderStatusResults results, string source)
        {
            MatchResult matchResult = WrappedConnector?.MatchWindow(config, resultImageURL, domLocation, checkSettings,
                regions, regionSelectors, location, results, source);

            return matchResult ?? new MatchResult() { AsExpected = true };
        }

        public void Open(Applitools.IConfiguration configuration)
        {
            OpenBase();
        }

        protected override void EnsureRunningSession()
        {
        }

        public List<RunningRender> Render(RenderRequest[] requests)
        {
            return RenderAsync(requests).Result;
        }

        public string RenderId { get; set; } = "47A4C2BD-0349-4232-B588-C9B9DA77498B";
        public string JobId { get; set; } = "A72E234C-58AA-4406-B8FD-8899FACEA147";
        public RectangleSize DeviceSize { get; private set; }

        public bool IsServerConcurrencyLimitReached => false;

        public async Task<List<RunningRender>> RenderAsync(RenderRequest[] requests)
        {
            LastRenderRequests = requests;
            List<RunningRender> runningRenders = new List<RunningRender>();
            foreach (RenderRequest request in requests)
            {
                RunningRender render = new RunningRender(RenderId, JobId, RenderStatus.Rendered, null, false);
                runningRenders.Add(render);
            }
            await Task.Delay(10);
            return runningRenders;
        }
        public Task<WebResponse> RenderPutResourceAsTask(RunningRender runningRender, RGridResource rGridResource)
        {
            throw new NotImplementedException();
        }

        public PutFuture RenderPutResource(RunningRender runningRender, RGridResource rGridResource)
        {
            throw new NotImplementedException();
        }

        private List<RenderStatusResults> renderStatusResults_ = null;
        public void SetRenderStatusResultsList(params RenderStatusResults[] resultsList)
        {
            renderStatusResults_ = new List<RenderStatusResults>(resultsList);
        }

        public List<RenderStatusResults> RenderStatusById(string[] renderIds)
        {
            if (renderStatusResults_ != null)
            {
                return renderStatusResults_;
            }

            List<RenderStatusResults> results = new List<RenderStatusResults>();
            foreach (string renderId in renderIds)
            {
                RenderStatusResults result = new RenderStatusResults()
                {
                    RenderId = renderId,
                    Status = RenderStatus.Rendered
                };
                results.Add(result);
            }
            return results;
        }

        public void SetDeviceSize(RectangleSize deviceSize)
        {
            DeviceSize = deviceSize;
        }

        public void SetRenderInfo(RenderingInfo renderingInfo)
        {
        }

        public void SetUserAgent(string userAgent)
        {
        }

        protected override string GetInferredEnvironment()
        {
            return nameof(MockEyesConnector);
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            throw new NotImplementedException();
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings)
        {
            throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            throw new NotImplementedException();
        }

        protected override Size GetViewportSize()
        {
            throw new NotImplementedException();
        }

        public bool?[] CheckResourceStatus(string renderId, HashObject[] hashes)
        {
            bool?[] arr = new bool?[hashes.Length];
            Array.Fill(arr, true);
            return arr;
        }

        protected override void SetViewportSize(RectangleSize size)
        {
        }
    }
}

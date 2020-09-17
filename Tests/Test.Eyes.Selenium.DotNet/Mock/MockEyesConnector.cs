using Applitools.Fluent;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.Mock
{
    class MockEyesConnector : EyesBase, IEyesConnector
    {
        private RenderBrowserInfo browserInfo_;
        private Applitools.Configuration config_;
        public IEyesConnector WrappedConnector { get; set; }

        public MockEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            browserInfo_ = browserInfo;
            config_ = config;
            ServerConnectorFactory = new MockServerConnectorFactory();
        }

        public RenderRequest[] LastRenderRequests { get; set; }

        protected override Applitools.Configuration Configuration => config_;

        public TestResults Close(bool throwEx, Applitools.IConfiguration config)
        {
            return new TestResults() { Status = TestResultsStatus.Passed };
        }

        public ResourceFuture CreateResourceFuture(RGridResource blob)
        {
            throw new NotImplementedException();
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

        protected override void SetViewportSize(RectangleSize size)
        {
        }
    }
}

using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestRenderingTask
    {
        [Test]
        public void TestFetchAllResources()
        {
            Logger logger = InitLogging(out string path);

            IDictionary<Uri, ResourceFuture> cache = new ConcurrentDictionary<Uri, ResourceFuture>();
            IDictionary<Uri, RGridResource> allBlobs = new ConcurrentDictionary<Uri, RGridResource>();
            HashSet<Uri> resourceUrls = new HashSet<Uri>()
            {
                new Uri("https://applitools.com/images/videos/step2.mp4"),
                new Uri("https://upload.wikimedia.org/wikipedia/commons/d/d8/NASA_Mars_Rover.jpg"),
                new Uri("https://www.wikipedia.org/portal/wikipedia.org/assets/img/Wikipedia-logo-v2.png"),
                new Uri("https://cdn.sstatic.net/Img/unified/sprites.svg?v=e5e58ae7df45"),
                new Uri("https://m.media-amazon.com/images/G/01/amazonglobal/images/Fuji/hero/English/SD10/2019/January/Fuji_EN_TallHero_ShipIL_Jan19_1X._CB458552189_.jpg"),
                new Uri("https://images-na.ssl-images-amazon.com/images/G/01/amazonglobal/images/Fuji/Dash/2018/December/Dash_M_AthleticShoes_dec18_1X._CB459955652_SY260_.jpg"),
                new Uri("https://target.scene7.com/is/image/Target/GUEST_9c641d16-d398-4902-8779-a688980cd199?wid=325&hei=325&qlt=80&fmt=webp"),
                new Uri("https://target.scene7.com/is/image/Target/GiftIdeas_CBimage_upload-190122_1548181114388?wid=328&hei=328&qlt=80&fmt=webp"),
                new Uri("https://applitools.com/images/artwork/homepage_banner.jpg"),
            };

            FileDebugResourceWriter debugResourceWriter = new FileDebugResourceWriter(path);
            RenderingTask renderingTask = new RenderingTask(logger, null, cache, null, null, debugResourceWriter);

            renderingTask.FetchAllResources_(allBlobs, resourceUrls);
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (resourceUrls.Count > 0 && stopwatch.ElapsedMilliseconds < 10000)
            {
                Thread.Sleep(100);
            }

            Assert.AreEqual(0, resourceUrls.Count);
        }

        [Test]
        public void TestParseScriptResult()
        {
            Logger logger = InitLogging(out string path);

            IDictionary<Uri, ResourceFuture> cache = new ConcurrentDictionary<Uri, ResourceFuture>();
            ConcurrentDictionary<Uri, RGridResource> allBlobs = new ConcurrentDictionary<Uri, RGridResource>();
            HashSet<Uri> resourceUrls = new HashSet<Uri>();

            FileDebugResourceWriter debugResourceWriter = new FileDebugResourceWriter(path);
            RenderingTask renderingTask = new RenderingTask(logger, null, cache, null, null, debugResourceWriter);

            VisualGridEyes eyes = new VisualGridEyes(new ConfigurationProviderForTesting());
            eyes.SetLogHandler(logger.GetILogHandler());

            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "http://applitools.github.io/demo/TestPages/FramesTestPage/";
            eyes.InitDriver(webDriver);
            try
            {
                string scriptResult = eyes.GetDomCaptureAndPollingScriptResult_();
                FrameData frameData = JsonConvert.DeserializeObject<FrameData>(scriptResult);
                renderingTask.ParseScriptResult_(frameData, allBlobs, resourceUrls);
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Test]
        public void TestPrepareDataForRG()
        {
            Logger logger = InitLogging(out string path);

            FileDebugResourceWriter debugResourceWriter = new FileDebugResourceWriter(path);

            VisualGridEyes eyes = new VisualGridEyes(new ConfigurationProviderForTesting());
            eyes.SetLogHandler(logger.GetILogHandler());
            IDictionary<Uri, ResourceFuture> cache = new ConcurrentDictionary<Uri, ResourceFuture>();

            RenderBrowserInfo renderBrowserInfo = new RenderBrowserInfo(400, 300, BrowserType.CHROME, "linux");
            List<VisualGridTask> taskList = new List<VisualGridTask>()
            {
                new VisualGridTask(TaskType.Check, logger, new RunningTest(renderBrowserInfo, logger))
            };
            RenderingInfo renderingInfo = new RenderingInfo() { AccessToken = "TestToken", ResultsUrl = new Uri("localhost:12345"), ServiceUrl = new Uri("localhost:54321") };
            RenderingTask renderingTask = new RenderingTask(logger, new CheckSettings(), cache, taskList, renderingInfo, debugResourceWriter);

            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "http://applitools.github.io/demo/TestPages/FramesTestPage/";
            eyes.InitDriver(webDriver);
            try
            {
                string scriptResult = eyes.GetDomCaptureScriptResult_();
                FrameData frameData = JsonConvert.DeserializeObject<FrameData>(scriptResult);
                RenderRequest[] renderRequests = renderingTask.PrepareDataForRG_(frameData);
                webDriver.Close();
            }
            finally
            {
                webDriver.Quit();
            }
        }


        //[Test]
        public void TestCreatePutFutures()
        {
            Logger logger = InitLogging(out string path);

            FileDebugResourceWriter debugResourceWriter = new FileDebugResourceWriter(path);

            Applitools.Selenium.VisualGrid.VisualGridEyes eyes = new Applitools.Selenium.VisualGrid.VisualGridEyes(new ConfigurationProviderForTesting());

            IDictionary<Uri, ResourceFuture> cache = new ConcurrentDictionary<Uri, ResourceFuture>();
            IDictionary<Uri, RGridResource> resources = new ConcurrentDictionary<Uri, RGridResource>();

            RenderingTask renderingTask = new RenderingTask(logger, new CheckSettings(), cache, null, null, debugResourceWriter);

            List<PutFuture> allPuts = null;
            RunningRender runningRender = null;

            renderingTask.CreatePutFutures_(allPuts, runningRender, resources);
        }

        private static Logger InitLogging(out string path, [CallerMemberName]string caller = null)
        {
            path = TestUtils.InitLogPath(caller);
            string logFilePath = Path.Combine(path, caller + ".log");

            Logger logger = new Logger();
            FileLogHandler logHandler = new FileLogHandler(logFilePath, false, true);
            logHandler.Open();
            logger.SetLogHandler(logHandler);
            return logger;
        }
    }
}

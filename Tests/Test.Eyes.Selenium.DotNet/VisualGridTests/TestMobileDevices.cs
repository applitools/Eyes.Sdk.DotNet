using Applitools.Metadata;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestMobileDevices : ReportingTestSuite
    {
        [Test]
        public void TestIosDeviceReportedResolutionOnFailure()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new LocalEyesConnectorFactory();

            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(new DesktopBrowserInfo(new RectangleSize(700, 460), BrowserType.CHROME));
            config.AddDeviceEmulation(DeviceName.Galaxy_S3);
            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_11_Pro));
            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_XR));
            config.SetBatch(TestDataProvider.BatchInfo);
            config.SetAppName("Visual Grid DotNet Tests");
            config.SetTestName("UFG Mobile Device No Result");
            eyes.SetConfiguration(config);

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://demo.applitools.com";
            try
            {
                eyes.Open(driver);
                eyes.Check(Target.Window());
                eyes.CloseAsync();
            }
            finally
            {
                driver.Quit();
                eyes.AbortAsync();
            }

            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(4, allTestResults.GetAllResults().Length);

            // Set results in array
            TestResults[] testResults = new TestResults[4];
            foreach (TestResultContainer resultsContainer in allTestResults)
            {
                RenderBrowserInfo browserInfo = resultsContainer.BrowserInfo;
                TestResults results = resultsContainer.TestResults;
                if (browserInfo.IosDeviceInfo == null && browserInfo.EmulationInfo == null)
                {
                    testResults[0] = results;
                    continue;
                }

                if (browserInfo.EmulationInfo != null)
                {
                    testResults[1] = results;
                    continue;
                }

                IosDeviceName deviceName = browserInfo.IosDeviceInfo.DeviceName;
                if (deviceName == IosDeviceName.iPhone_11_Pro)
                {
                    testResults[2] = results;
                    continue;
                }

                if (deviceName == IosDeviceName.iPhone_XR)
                {
                    testResults[3] = results;
                    continue;
                }

                Assert.Fail();
            }

            SessionResults chromeSessionResults = TestUtils.GetSessionResults(eyes.ApiKey, testResults[0]);
            string actualUserAgent = chromeSessionResults.StartInfo.Environment.Inferred;
            Dictionary<BrowserType, string> userAgents = eyes.visualGridEyes_.eyesConnector_.GetUserAgents();
            Assert.AreEqual("useragent: " + userAgents[BrowserType.CHROME], actualUserAgent);

            Assert.AreEqual(new RectangleSize(700, 460), testResults[0].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(360, 640), testResults[1].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(375, 812), testResults[2].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(414, 896), testResults[3].HostDisplaySize);
        }
    }

    class LocalEyesConnectorFactory : IEyesConnectorFactory
    {
        public IEyesConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            IEyesConnector eyesConnector = new LocalEyesConnector(browserInfo, config);
            return eyesConnector;
        }
    }

    class LocalEyesConnector : EyesConnector
    {
        public LocalEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration configuration)
            : base(browserInfo, configuration)
        {
            ServerConnector.ServerUrl = new Uri(ServerUrl);
            ServerConnector.ApiKey = ApiKey;
        }

        public override List<RunningRender> Render(RenderRequest[] renderRequests)
        {
            throw new Exception();
        }

        public override Task<List<RunningRender>> RenderAsync(RenderRequest[] renderRequests)
        {
            throw new Exception();
        }
    }

}

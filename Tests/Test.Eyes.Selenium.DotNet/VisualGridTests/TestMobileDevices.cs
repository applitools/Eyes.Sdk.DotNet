using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestMobileDevices : ReportingTestSuite
    {
        [Test]
        public void TestIosDeviceReportedResolutionOnFailure()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            EyesRunner runner = new VisualGridRunner(10, logHandler);
            Eyes eyes = new Eyes(runner);

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

            Assert.AreEqual(new RectangleSize(700, 460), testResults[0].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(360, 640), testResults[1].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(375, 812), testResults[2].HostDisplaySize);
            Assert.AreEqual(new RectangleSize(414, 896), testResults[3].HostDisplaySize);
        }
    }
}

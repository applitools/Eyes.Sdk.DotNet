using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestBrowserResize : ReportingTestSuite
    {
        [Test]
        public void BrowserSizeTest()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            //webDriver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/mobile.html";
            Eyes eyes = new Eyes(new MockServerConnectorFactory());
            eyes.SendDom = false;
            TestUtils.SetupLogging(eyes);
            eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                eyes.Open(webDriver, "Browser Size Test", "Browser Size Test", new Size(640, 480));
                //eyes.Check("Test 1", Target.Window());
                TestResults results1 = eyes.Close(false);

                eyes.Open(webDriver, "Browser Size Test", "Browser Size Test", new Size(800, 600));
                //eyes.Check("Test 2", Target.Window());
                TestResults results2 = eyes.Close(false);

                eyes.Open(webDriver, "Browser Size Test", "Browser Size Test", new Size(1024, 768));
                //eyes.Check("Test 3", Target.Window());
                TestResults results3 = eyes.Close(false);
            }
            finally
            {
                webDriver.Quit();
            }

            try
            {
                ChromeOptions options = new ChromeOptions();
                options.EnableMobileEmulation("iPhone 5/SE");
                webDriver = SeleniumUtils.CreateChromeDriver(options);
                //webDriver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/mobile.html";

                eyes.Open(webDriver, "Browser Size Test", "Browser Size Test");
                //eyes.Check("Test 4", Target.Window());
                TestResults results4 = eyes.Close(false);
            }
            finally
            {
                webDriver.Quit();
            }

            try
            {
                MockServerConnector server = (MockServerConnector)eyes.seleniumEyes_.ServerConnector;
                Assert.AreEqual(4, server.SessionIds.Count);
                Assert.AreEqual(new Size(640, 480), server.SessionsStartInfo[server.SessionIds[0]].Environment.DisplaySize.ToSize());
                Assert.AreEqual(new Size(800, 600), server.SessionsStartInfo[server.SessionIds[1]].Environment.DisplaySize.ToSize());
                Assert.AreEqual(new Size(1024, 768), server.SessionsStartInfo[server.SessionIds[2]].Environment.DisplaySize.ToSize());
                Assert.AreEqual(new Size(320, 568), server.SessionsStartInfo[server.SessionIds[3]].Environment.DisplaySize.ToSize());
            }
            finally
            {
                eyes.AbortIfNotClosed();
            }
        }
    }
}

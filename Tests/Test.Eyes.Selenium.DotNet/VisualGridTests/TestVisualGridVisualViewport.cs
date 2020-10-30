using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestVisualGridVisualViewport : ReportingTestSuite
    {
        [Test]
        public void TestUFGVisualViewport()
        {
            JsonSerializer serializer = JsonUtils.CreateSerializer();
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new Mock.MockEyesConnectorFactory();
            Configuration config = eyes.GetConfiguration();
            IosDeviceInfo iosDeviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_11_Pro);
            config.AddBrowser(iosDeviceInfo);
            eyes.SetConfiguration(config);
            try
            {
                eyes.Open(driver, "Eyes Selenium SDK", "Eyes Selenium SDK - UFG Visual Viewport Test");
                Mock.MockEyesConnector mockEyesConnector = (Mock.MockEyesConnector)eyes.visualGridEyes_.eyesConnector_;
                Mock.MockServerConnector mockServerConnector = new Mock.MockServerConnector(eyes.Logger, new Uri(eyes.ServerUrl));
                EyesConnector eyesConnector = new EyesConnector(eyes.Logger, new RenderBrowserInfo(iosDeviceInfo), config)
                {
                    runningSession_ = new RunningSession(),
                    ServerConnector = mockServerConnector
                };
                mockEyesConnector.WrappedConnector = eyesConnector;

                string inputJson = CommonUtils.ReadResourceFile("Test.Eyes.Selenium.DotNet.Resources.Misc.TestUFGVisualViewport_Input.json");
                RenderStatusResults renderStatusResults = serializer.Deserialize<RenderStatusResults>(inputJson);
                mockEyesConnector.SetRenderStatusResultsList(renderStatusResults);

                driver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/desktop.html";
                eyes.Check(Target.Window().Fully());
                eyes.Close();

                runner.GetAllTestResults();

                Assert.AreEqual(1, mockServerConnector.MatchWindowCalls.Count);
                MatchWindowData matchWindowData = mockServerConnector.MatchWindowCalls[0];
                AppOutput appOutput = matchWindowData.AppOutput;
                Assert.AreEqual(980, appOutput.Viewport.Width);
                Assert.AreEqual(1659, appOutput.Viewport.Height);

                Assert.AreEqual(375, mockEyesConnector.DeviceSize.Width);
                Assert.AreEqual(812, mockEyesConnector.DeviceSize.Height);
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}

using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestVisualGridApi : ReportingTestSuite
    {
        [Test]
        public void TestVisualGridOptions()
        {
            // We want VG mode
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new Mock.MockEyesConnectorFactory();

            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(800, 600, BrowserType.CHROME);
            config.SetVisualGridOptions(new VisualGridOption("option1", "value1"), new VisualGridOption("option2", false));
            config.SetBatch(TestDataProvider.BatchInfo);
            eyes.SetConfiguration(config);

            MockEyesConnector mockEyesConnector;

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/desktop.html";
            try
            {
                // First check - global + fluent config
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver);
                eyes.Check(Target.Window().VisualGridOptions(new VisualGridOption("option3", "value3"), new VisualGridOption("option4", 5)));
                eyes.Close();
                var expected1 = new Dictionary<string, object>()
                {
                    {"option1", "value1"}, {"option2", false}, {"option3", "value3"}, {"option4", 5}
                };
                var actual1 = mockEyesConnector.LastRenderRequests[0].Options;
                CollectionAssert.AreEquivalent(expected1, actual1);


                // Second check - only global
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver);
                eyes.CheckWindow();
                eyes.Close();
                var expected2 = new Dictionary<string, object>()
                {
                    {"option1", "value1"}, {"option2", false}
                };
                var actual2 = mockEyesConnector.LastRenderRequests[0].Options;
                CollectionAssert.AreEquivalent(expected2, actual2);

                // Third check - resetting
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver);
                config = eyes.GetConfiguration();
                config.SetVisualGridOptions(null);
                eyes.SetConfiguration(config);
                eyes.CheckWindow();
                eyes.Close();
                var actual3 = mockEyesConnector.LastRenderRequests[0].Options;
                Assert.IsNull(actual3);
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Close();
            }
        }
        
        [Test]
        public void TestVisualGridSkipList()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new Mock.MockEyesConnectorFactory();
         
            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(1050, 600, BrowserType.CHROME);
            config.SetBatch(TestDataProvider.BatchInfo);
            eyes.SetConfiguration(config);

            MockEyesConnector mockEyesConnector;
         
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/DomSnapshot/test-iframe.html";
            try
            {
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver);
               
                eyes.Check(Target.Window());
                string[] expectedUrls = new string[] { 
                    "https://applitools.github.io/demo/DomSnapshot/test.css", 
                    "https://applitools.github.io/demo/DomSnapshot/smurfs.jpg", 
                    "https://applitools.github.io/blabla", 
                    "https://applitools.github.io/demo/DomSnapshot/iframes/inner/smurfs.jpg",
                    "https://applitools.github.io/demo/DomSnapshot/test.html",
                    "https://applitools.github.io/demo/DomSnapshot/iframes/inner/test.html", 
                    "https://applitools.github.io/demo/DomSnapshot/iframes/frame.html"};
                CollectionAssert.AreEquivalent(expectedUrls, runner.CachedBlobsURLs.Keys);
                eyes.Check(Target.Window());
                eyes.Close();

                runner.GetAllTestResults();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Close();
            }
        }

        private static MockEyesConnector OpenEyesAndGetConnector_(Eyes eyes, Configuration config, IWebDriver driver)
        {
            eyes.Open(driver, "Mock app", "Mock Test");

            Mock.MockEyesConnector mockEyesConnector = (Mock.MockEyesConnector)eyes.visualGridEyes_.eyesConnector_;
            Mock.MockServerConnector mockServerConnector = new Mock.MockServerConnector(eyes.Logger, new Uri(eyes.ServerUrl));
            EyesConnector eyesConnector = new EyesConnector(config.GetBrowsersInfo()[0], config)
            {
                runningSession_ = new RunningSession(),
                ServerConnector = mockServerConnector
            };
            mockEyesConnector.WrappedConnector = eyesConnector;
            return mockEyesConnector;
        }
    }
}

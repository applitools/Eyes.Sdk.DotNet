using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestEkbVg
    {
        private EyesRunner visualGridRunner = null;
        private BatchInfo batchInfo;
        private Configuration suiteConfig;
        private const int concurrentSessions = 20;
        private const int viewPortWidth = 800;
        private const int viewPortHeight = 600;
        private RectangleSize localViewportSize = new RectangleSize(viewPortWidth, viewPortHeight);
        string appName = "testEKB3";
        string testName = "testEKB";
        BlockingCollection<IWebDriver> webDrivers = new BlockingCollection<IWebDriver>(concurrentSessions);

        //%%%% start before-class
        [OneTimeSetUp]
        public void BeforeSuite()
        {
            visualGridRunner = new VisualGridRunner(concurrentSessions);
            visualGridRunner.SetLogHandler(TestUtils.InitLogHandler(nameof(TestEkbVg), verbose: false));
            visualGridRunner.Logger.Log("enter");
            batchInfo = new BatchInfo("test EKB VG");
            suiteConfig = new Configuration();
            suiteConfig.AddBrowser(viewPortWidth, viewPortHeight, BrowserType.CHROME)
                    .SetHideScrollbars(true)
                    .SetHideCaret(true)
                    .SetWaitBeforeScreenshots(10000)
                    .SetAppName(appName)
                    .SetViewportSize(new RectangleSize(viewPortWidth, viewPortHeight))
                    .SetMatchTimeout(TimeSpan.FromSeconds(6))
                    .SetBatch(batchInfo);

            visualGridRunner.Logger.Log("creating {0} chrome drivers", webDrivers.BoundedCapacity);

            for (int i = 0; i < webDrivers.BoundedCapacity; ++i)
            {
                webDrivers.Add(SeleniumUtils.CreateChromeDriver());
            }

            visualGridRunner.Logger.Log("createdg {0} chrome drivers", webDrivers.Count);
        }
        //%%%% end before-class

        public static object[] dp()
        {
            string data = CommonUtils.ReadResourceFile("Customers.Test.Eyes.Selenium.VisualGrid.4.7.2b.txt");

            //string fileName = "./data/4.7.2b.txt";
            //string[] urls = File.ReadAllLines(fileName);
            string[] urls = data.Split('\n');

            string urlPrefix = "https://applitools.com";
            string urlSuffix = "?intercom=false";

            Regex regex1 = new Regex("^[^/]*", RegexOptions.Compiled);
            Regex regex2 = new Regex("\".*$", RegexOptions.Compiled);
            List<string> urlsList = new List<string>();
            foreach (string url in urls)
            {
                string str = url;
                str = regex1.Replace(str, "");
                str = regex2.Replace(str, "");
                urlsList.Add(urlPrefix + str + urlSuffix);
            }

            return urlsList.ToArray();
        }

        [Test]
        [TestCaseSource(nameof(dp))]
        public void Test(string testedUrl)
        {
            visualGridRunner.Logger.Log("entering test {0}", testedUrl);
            Eyes eyes;
            if (visualGridRunner != null)
            {
                eyes = new Eyes(visualGridRunner);
            }
            else
            {
                eyes = new Eyes();
            }
            Configuration testConfig = new Configuration(suiteConfig);
            testConfig.SetTestName("testEKB" + testedUrl);
            eyes.SetConfiguration(testConfig);
            eyes.ForceFullPageScreenshot = true;
            IWebDriver webDriver = null;
            try
            {
                if (webDrivers.TryTake(out webDriver, -1))
                {
                    eyes.Logger.Log("using existing web driver: {0} ({1})", webDriver, webDriver?.GetHashCode());
                }
            }
            catch (Exception e)
            {
                eyes.Logger.Log("Error: " + e);
            }

            try
            {
                IWebDriver driver = eyes.Open(webDriver, appName, testName + "-" + testedUrl, localViewportSize);
                eyes.Logger.Log("navigating to " + testedUrl);

                driver.Url = testedUrl;
                string script = "var d1 = document.querySelector('body > div.navbar-container');"
                              + "if (d1) {d1.style.position='relative'}";
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript(script);
                eyes.MatchLevel = MatchLevel.Strict;

                eyes.Check("Step1 - " + testedUrl, Target.Window());
                eyes.CloseAsync();
            }
            catch (Exception e)
            {
                eyes.Logger.Log("Error: " + e);
                if (webDriver != null)
                {
                    eyes.Logger.Log("calling webdriver.quit (1)");
                    webDriver.Quit();
                }
            }

            try
            {
                visualGridRunner.Logger.Log("returning webdriver to pool");
                webDrivers.Add(webDriver);
            }
            catch (Exception e)
            {
                eyes.Logger.Log("Error: " + e);
            }

            visualGridRunner.Logger.Log("last line of test");
        }

        [OneTimeTearDown]
        public void AfterSuite()
        {
            visualGridRunner.Logger.Log("After class");

            visualGridRunner.Logger.Log("iterating through all webdrivers in pool");
            foreach (IWebDriver webDriver in webDrivers)
            {
                try
                {
                    visualGridRunner.Logger.Log("calling webdriver.quit (2)");
                    webDriver.Quit();
                }
                catch (Exception e)
                {
                    visualGridRunner.Logger.Log("Error: " + e);
                }
            }

            if (visualGridRunner != null)
            {
                visualGridRunner.Logger.Log("calling GetAllTestResults()");
                TestResultsSummary allTestResults = visualGridRunner.GetAllTestResults();
            }
        }

    }
}

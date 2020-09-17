namespace Applitools.Selenium.Tests
{
    using Applitools.VisualGrid;
    using Metadata;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Safari;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Utils;
    using Region = Applitools.Utils.Geometry.Region;

    public abstract class TestSetup
    {
        //protected Eyes eyes_;
        protected ILogHandler logHandler_;
        //protected IWebDriver driver_;
        protected DriverOptions options_;
        private readonly bool useVisualGrid_;

        //protected ICapabilities capabilities_;
        //protected IWebDriver webdriver_;

        private string testSuitName_;
        private string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        protected string seleniumServerUrl = null;
        //protected Size testedPageSize = new Size(800, 600);
        protected Size testedPageSize = new Size(700, 460);

        private EyesRunner runner_;

        private HashSet<Region> expectedIgnoreRegions_ = new HashSet<Region>();
        private HashSet<Region> expectedStrictRegions_ = new HashSet<Region>();
        private HashSet<Region> expectedLayoutRegions_ = new HashSet<Region>();
        private HashSet<Region> expectedContentRegions_ = new HashSet<Region>();
        private HashSet<FloatingMatchSettings> expectedFloatingRegions_ = new HashSet<FloatingMatchSettings>();

        private string testNameSuffix_ = Environment.GetEnvironmentVariable("TEST_NAME_SUFFIX");

        protected bool CompareExpectedRegion { get; set; }

        public string TestedPageUrl { get => testedPageUrl; set { testedPageUrl = value; GetDriver().Url = value; } }

        public TestSetup(string testSuitName)
        {
            testSuitName_ = testSuitName + testNameSuffix_;
        }

        public TestSetup(string testSuitName, DriverOptions options, bool useVisualGrid = false)
        {
            testSuitName_ = testSuitName + testNameSuffix_;
            options_ = options;
            useVisualGrid_ = useVisualGrid;
        }

        #region expected regions

        protected void SetExpectedFloatingsRegions(params FloatingMatchSettings[] floatingMatchSettings)
        {
            expectedFloatingRegions_ = new HashSet<FloatingMatchSettings>(floatingMatchSettings);
        }

        protected void SetExpectedIgnoreRegions(params Region[] ignoreRegions)
        {
            expectedIgnoreRegions_ = new HashSet<Region>(ignoreRegions);
        }

        protected void SetExpectedLayoutRegions(params Region[] layoutRegions)
        {
            expectedLayoutRegions_ = new HashSet<Region>(layoutRegions);
        }

        protected void SetExpectedStrictRegions(params Region[] strictRegions)
        {
            expectedStrictRegions_ = new HashSet<Region>(strictRegions);
        }

        protected void SetExpectedContentRegions(params Region[] contentRegions)
        {
            expectedContentRegions_ = new HashSet<Region>(contentRegions);
        }

        #endregion

        private void Init_(string testName)
        {
            // Initialize the eyes SDK and set your private API key.
            Eyes eyes = InitEyes_(false);

            eyesByTestId_.Add(TestContext.CurrentContext.Test.ID, eyes);

            //testName += eyes.ForceFullPageScreenshot ? "_FPS" : string.Empty;

            RemoteWebDriver webDriver;
            string seleniumServerUrl = SetupSeleniumServer(testName);
            try
            {
                webDriver = new RemoteWebDriver(new Uri(seleniumServerUrl), options_);
            }
            catch
            {
                webDriver = (RemoteWebDriver)SeleniumUtils.CreateWebDriver(options_);
            }
            eyes.AddProperty("Selenium Session ID", webDriver.SessionId.ToString());

            eyes.AddProperty("ForceFPS", eyes.ForceFullPageScreenshot ? "true" : "false");
            eyes.AddProperty("Agent ID", eyes.FullAgentId);

            //IWebDriver webDriver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), capabilities_);

            SetupLogging(eyes, testName);

            eyes.Logger.Log("navigating to URL: " + TestedPageUrl);

            IWebDriver driver = eyes.Open(webDriver, testSuitName_, testName, testedPageSize);

            //string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            driver.Url = TestedPageUrl;

            eyes.Logger.Log(testName + ": " + TestDataProvider.BatchInfo.Name);

            driverByTestId_.Add(TestContext.CurrentContext.Test.ID, driver);
            webDriverByTestId_.Add(TestContext.CurrentContext.Test.ID, webDriver);
        }

        private IDictionary<string, Eyes> eyesByTestId_ = new ConcurrentDictionary<string, Eyes>();
        private IDictionary<string, IWebDriver> driverByTestId_ = new ConcurrentDictionary<string, IWebDriver>();
        private IDictionary<string, IWebDriver> webDriverByTestId_ = new ConcurrentDictionary<string, IWebDriver>();

        public Eyes GetEyes()
        {
            eyesByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out Eyes eyes);
            return eyes;
        }

        public IWebDriver GetDriver()
        {
            driverByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out IWebDriver driver);
            return driver;
        }

        public IWebDriver GetWebDriver()
        {
            webDriverByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out IWebDriver driver);
            return driver;
        }

        private Eyes InitEyes_(bool forceFullPageScreenshot)
        {
            //if (runner_ is VisualGridRunner && eyes_ != null) return;

            Eyes eyes = new Eyes(runner_);

            ClearExpectedRegionsSets();

            string serverUrl = Environment.GetEnvironmentVariable("APPLITOOLS_SERVER_URL");
            if (!string.IsNullOrEmpty(serverUrl))
            {
                eyes.ServerUrl = serverUrl;
            }

            eyes.ForceFullPageScreenshot = forceFullPageScreenshot;
            eyes.HideScrollbars = true;
            eyes.StitchMode = StitchModes.CSS;
            //eyes_.StitchMode = StitchModes.Scroll;
            //eyes_.MatchLevel = MatchLevel.Layout;
            eyes.SaveNewTests = false;
            eyes.Batch = TestDataProvider.BatchInfo;

            return eyes;
        }

        private string SetupSeleniumServer(string testName)
        {
            string seleniumServerUrl = this.seleniumServerUrl ?? Environment.GetEnvironmentVariable("SELENIUM_SERVER_URL");
            //if (seleniumServerUrl != null && seleniumServerUrl.Contains("ondemand.saucelabs.com", StringComparison.OrdinalIgnoreCase))
            //{
            //    DesiredCapabilities desiredCaps = (DesiredCapabilities)options_.ToCapabilities();
            //    desiredCaps.SetCapability("username", Environment.GetEnvironmentVariable("SAUCE_USERNAME"));
            //    desiredCaps.SetCapability("accesskey", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));
            //    desiredCaps.SetCapability("seleniumVersion", "3.12.0");

            //    if (options_.BrowserName == "chrome")
            //    {
            //        desiredCaps.SetCapability("chromedriverVersion", "2.37");
            //    }

            //    desiredCaps.SetCapability("name", testName + $" ({GetEyes().FullAgentId})");
            //}

            return seleniumServerUrl;
        }

        private void SetupLogging(Eyes eyes, string testName)
        {
            if (Environment.GetEnvironmentVariable("CI") == null)
            {
                string path = TestUtils.InitLogPath(testName);
                eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                {
                    Path = path,
                    Prefix = testName + "_"
                };
                logHandler_ = new FileLogHandler(Path.Combine(path, $"{testName}_{options_.PlatformName}.log"), true, true);
            }
            else
            {
                logHandler_ = new StdoutLogHandler(true);
                //logHandler_ = new TraceLogHandler(true);
            }

            if (logHandler_ != null)
            {
                eyes.SetLogHandler(logHandler_);
            }
        }

        private void ClearExpectedRegionsSets()
        {
            SetExpectedFloatingsRegions();
            SetExpectedIgnoreRegions();
            SetExpectedLayoutRegions();
            SetExpectedStrictRegions();
            SetExpectedContentRegions();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            runner_ = useVisualGrid_ ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestResultsSummary allResults = runner_?.GetAllTestResults();
        }

        [SetUp]
        public void SetUp()
        {
            Init_(TestContext.CurrentContext.Test.MethodName);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                TestResults results = GetEyes().Close();
                if (results != null)
                {
                    SessionResults sessionResults = Utils.TestUtils.GetSessionResults(GetEyes(), results);

                    if (sessionResults != null)
                    {
                        ActualAppOutput[] actualAppOutput = sessionResults.ActualAppOutput;
                        if (actualAppOutput.Length > 0)
                        {
                            ImageMatchSettings ims = actualAppOutput[0].ImageMatchSettings;
                            CompareRegions_(ims);
                        }
                    }
                    GetEyes().Logger.Log("Mismatches: " + results.Mismatches);
                }
            }
            catch (Exception ex)
            {
                GetEyes().Logger.Log("Exception: " + ex);
                throw;
            }
            finally
            {
                GetEyes().Abort();
                GetWebDriver().Quit();
            }
        }

        private void CompareRegions_(ImageMatchSettings ims)
        {
            if (!CompareExpectedRegion)
            {
                return;
            }

            if (expectedFloatingRegions_.Count > 0)
            {
                CollectionAssert.AreEqual(expectedFloatingRegions_, ims.Floating, "Floating regions lists differ");
            }

            CompareSimpleRegionsList_(ims.Ignore, expectedIgnoreRegions_, "Ignore");
            CompareSimpleRegionsList_(ims.Layout, expectedLayoutRegions_, "Layout");
            CompareSimpleRegionsList_(ims.Content, expectedContentRegions_, "Content");
            CompareSimpleRegionsList_(ims.Strict, expectedStrictRegions_, "Strict");
        }

        private static void CompareSimpleRegionsList_(Region[] actualRegions, HashSet<Region> expectedRegions, string type)
        {
            if (expectedRegions.Count > 0)
            {
                CollectionAssert.AreEqual(expectedRegions, actualRegions, "{0} regions lists differ", type);
            }
        }
    }
}

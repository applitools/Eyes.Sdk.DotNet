using Applitools.Metadata;
using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium.Tests
{
    public abstract class TestSetup : ReportingTestSuite
    {
        public class Expectations
        {
            public Dictionary<string, object> ExpectedProperties { get; set; } = new Dictionary<string, object>();
            public HashSet<Region> ExpectedIgnoreRegions { get; set; } = new HashSet<Region>();
            public HashSet<Region> ExpectedStrictRegions { get; set; } = new HashSet<Region>();
            public HashSet<Region> ExpectedLayoutRegions { get; set; } = new HashSet<Region>();
            public HashSet<Region> ExpectedContentRegions { get; set; } = new HashSet<Region>();
            public HashSet<FloatingMatchSettings> ExpectedFloatingRegions { get; set; } = new HashSet<FloatingMatchSettings>();
            public HashSet<AccessibilityRegionByRectangle> ExpectedAccessibilityRegions { get; internal set; } = new HashSet<AccessibilityRegionByRectangle>();
        }

        class SpecificTestContextRequirements
        {
            public SpecificTestContextRequirements(Eyes eyes, string testName)
            {
                Eyes = eyes;
                TestName = testName;
            }

            public Eyes Eyes { get; private set; }
            public IWebDriver WrappedDriver { get; set; }
            public IWebDriver WebDriver { get; set; }
            public Dictionary<int, Expectations> Expectations = new Dictionary<int, Expectations>();
            public string TestName { get; }
            public string TestNameAsFilename { get; set; }
            public string ExpectedVGOutput { get; set; }
            public WebDriverProvider WebDriverProvider { get; internal set; }
        }

        protected DriverOptions options_;
        protected readonly bool useVisualGrid_;
        protected readonly StitchModes stitchMode_;
        private string testSuitName_;
        protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        protected string seleniumServerUrl = null;
        protected Size testedPageSize = new Size(700, 460);

        private string testNameSuffix_ = Environment.GetEnvironmentVariable("TEST_NAME_SUFFIX");


        protected bool CompareExpectedRegion { get; set; } = true;

        private IDictionary<string, SpecificTestContextRequirements> testDataByTestId_ = new ConcurrentDictionary<string, SpecificTestContextRequirements>();

        public TestSetup(string testSuitName)
        {
            testSuitName_ = testSuitName + testNameSuffix_;
        }

        public TestSetup(string testSuitName, DriverOptions options, bool useVisualGrid = false)
        {
            testSuitName_ = testSuitName + testNameSuffix_;
            options_ = options;
            useVisualGrid_ = useVisualGrid;
            stitchMode_ = StitchModes.CSS;
            suiteArgs_.Add("mode", "VisualGrid");
        }

        public TestSetup(string testSuitName, DriverOptions options, StitchModes stitchMode = StitchModes.CSS)
        {
            testSuitName_ = testSuitName + testNameSuffix_;
            options_ = options;
            useVisualGrid_ = false;
            stitchMode_ = stitchMode;
            suiteArgs_.Add("mode", stitchMode.ToString());
        }

        #region expected regions

        private Expectations GetExpectationsAtIndex(int index, SpecificTestContextRequirements testData = null)
        {
            if (testData == null) testData = testDataByTestId_[TestContext.CurrentContext.Test.ID];
            if (!testData.Expectations.TryGetValue(index, out Expectations expectations))
            {
                expectations = new Expectations();
                testData.Expectations[index] = expectations;
            }
            return expectations;
        }

        protected void SetExpectedAccessibilityRegions(params AccessibilityRegionByRectangle[] accessibilityRegions)
        {
            SetExpectedAccessibilityRegions(0, accessibilityRegions);
        }

        protected void SetExpectedAccessibilityRegions(int index, params AccessibilityRegionByRectangle[] accessibilityRegions)
        {
            GetExpectationsAtIndex(index).ExpectedAccessibilityRegions = new HashSet<AccessibilityRegionByRectangle>(accessibilityRegions);
        }

        protected void SetExpectedFloatingRegions(params FloatingMatchSettings[] floatingMatchSettings)
        {
            SetExpectedFloatingRegions(0, floatingMatchSettings);
        }

        protected void SetExpectedFloatingRegions(int index, params FloatingMatchSettings[] floatingMatchSettings)
        {
            GetExpectationsAtIndex(index).ExpectedFloatingRegions = new HashSet<FloatingMatchSettings>(floatingMatchSettings);
        }

        protected void SetExpectedIgnoreRegions(params Region[] ignoreRegions)
        {
            SetExpectedIgnoreRegions(0, ignoreRegions);
        }

        protected void SetExpectedIgnoreRegions(int index, params Region[] ignoreRegions)
        {
            GetExpectationsAtIndex(index).ExpectedIgnoreRegions = new HashSet<Region>(ignoreRegions);
        }

        protected void SetExpectedLayoutRegions(params Region[] layoutRegions)
        {
            SetExpectedLayoutRegions(0, layoutRegions);
        }

        protected void SetExpectedLayoutRegions(int index, params Region[] layoutRegions)
        {
            GetExpectationsAtIndex(index).ExpectedLayoutRegions = new HashSet<Region>(layoutRegions);
        }

        protected void SetExpectedStrictRegions(params Region[] strictRegions)
        {
            SetExpectedStrictRegions(0, strictRegions);
        }

        protected void SetExpectedStrictRegions(int index, params Region[] strictRegions)
        {
            GetExpectationsAtIndex(index).ExpectedStrictRegions = new HashSet<Region>(strictRegions);
        }

        protected void SetExpectedContentRegions(params Region[] contentRegions)
        {
            SetExpectedContentRegions(0, contentRegions);
        }

        protected void SetExpectedContentRegions(int index, params Region[] contentRegions)
        {
            GetExpectationsAtIndex(index).ExpectedContentRegions = new HashSet<Region>(contentRegions);
        }

        #endregion

        private void Init_(string testName)
        {
            string testNameWithArguments = InitTestName_(ref testName);

            ILogHandler logHandler = TestUtils.InitLogHandler(testNameWithArguments);

            // Initialize the eyes SDK and set your private API key.
            Eyes eyes = InitEyes_(testName, logHandler);

            eyes.Logger.Log(TraceLevel.Notice, Stage.TestFramework, StageType.Start,
                new { TestName = TestContext.CurrentContext.Test.FullName });

            string seleniumServerUrl = SetupSeleniumServer(testName);
            bool isWellFormedUri = Uri.IsWellFormedUriString(seleniumServerUrl, UriKind.Absolute);

            RemoteWebDriver webDriver = SeleniumUtils.RetryCreateWebDriver(() =>
            {
                RemoteWebDriver rwDriver = null;
                if (isWellFormedUri)
                {
                    try
                    {
                        eyes.Logger.Log(TraceLevel.Info, Stage.TestFramework, StageType.Start,
                            new { message = $"Trying to create RemoteWebDriver on {seleniumServerUrl}" });
                        rwDriver = new RemoteWebDriver(new Uri(seleniumServerUrl), options_.ToCapabilities(), TimeSpan.FromMinutes(4));
                    }
                    catch (Exception e)
                    {
                        CommonUtils.LogExceptionStackTrace(eyes.Logger, Stage.TestFramework, StageType.Start, e,
                            testName, seleniumServerUrl);
                    }
                }

                if (rwDriver != null) return rwDriver;

                if (TestUtils.RUNS_ON_CI)
                {
                    if (options_.BrowserName.Equals(BrowserNames.Chrome, StringComparison.OrdinalIgnoreCase) ||
                        options_.BrowserName.Equals(BrowserNames.Firefox, StringComparison.OrdinalIgnoreCase))
                    {
                        rwDriver = (RemoteWebDriver)SeleniumUtils.CreateWebDriver(options_);
                    }
                }
                else
                {
                    rwDriver = (RemoteWebDriver)SeleniumUtils.CreateWebDriver(options_);
                }

                return rwDriver;
            });

            eyes.AddProperty("Selenium Session ID", webDriver.SessionId.ToString());

            eyes.AddProperty("ForceFPS", eyes.ForceFullPageScreenshot ? "true" : "false");
            eyes.AddProperty("Agent ID", eyes.FullAgentId);

            //IWebDriver webDriver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), capabilities_);

            IWebDriver driver;
            try
            {
                BeforeOpen(eyes);
                driver = eyes.Open(webDriver, testSuitName_, testName, testedPageSize);
            }
            catch
            {
                webDriver.Quit();
                throw;
            }

            //string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            driver.Navigate().GoToUrl(testedPageUrl);
            eyes.Logger.Log($"{testName} ({options_.BrowserName}) : {TestDataProvider.BatchInfo.Name}");

            testDataByTestId_[TestContext.CurrentContext.Test.ID].WrappedDriver = driver;
            testDataByTestId_[TestContext.CurrentContext.Test.ID].WebDriver = webDriver;
            testDataByTestId_[TestContext.CurrentContext.Test.ID].WebDriverProvider.SetDriver(driver);
        }

        private string InitTestName_(ref string testName)
        {
            string testNameWithArguments = testName;
            foreach (object argValue in TestContext.CurrentContext.Test.Arguments)
            {
                testNameWithArguments += "_" + argValue;
            }

            if (useVisualGrid_)
            {
                testName += "_VG";
                testNameWithArguments += "_VG";
            }
            else if (stitchMode_ == StitchModes.Scroll)
            {
                testName += "_Scroll";
                testNameWithArguments += "_Scroll";
            }

            return testNameWithArguments;
        }

        protected virtual void BeforeOpen(Eyes eyes) { }

        public void AddExpectedProperty(string propertyName, object expectedValue)
        {
            AddExpectedProperty(0, propertyName, expectedValue);
        }

        public void AddExpectedProperty(int index, string propertyName, object expectedValue)
        {
            Dictionary<string, object> expectedProps = GetExpectationsAtIndex(index).ExpectedProperties;
            expectedProps.Add(propertyName, expectedValue);
        }

        public Eyes GetEyes()
        {
            testDataByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out SpecificTestContextRequirements testData);
            Eyes eyes = testData?.Eyes;
            return eyes;
        }

        public IWebDriver GetDriver()
        {
            testDataByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out SpecificTestContextRequirements testData);
            IWebDriver driver = testData?.WrappedDriver;
            return driver;
        }

        public IWebDriver GetWebDriver()
        {
            testDataByTestId_.TryGetValue(TestContext.CurrentContext.Test.ID, out SpecificTestContextRequirements testData);
            IWebDriver driver = testData?.WebDriver;
            return driver;
        }

        private Eyes InitEyes_(string testName, ILogHandler logHandler, bool? forceFullPageScreenshot = null)
        {
            EyesRunner runner = null;
            string testNameAsFilename = TestUtils.SanitizeForFilename(TestContext.CurrentContext.Test.FullName);
            string expectedVGOutput = null;
            WebDriverProvider webDriverProvider = new WebDriverProvider();
            if (useVisualGrid_)
            {
                if (RUNS_ON_CI || USE_MOCK_VG)
                {
                    //eyes.Logger.Log("using VG mock eyes connector");
                    Assembly thisAssembly = Assembly.GetCallingAssembly();
                    Stream expectedOutputJsonStream = thisAssembly.GetManifestResourceStream("Test.Eyes.Selenium.DotNet.Resources.VGTests." + testNameAsFilename + ".json");
                    if (expectedOutputJsonStream != null)
                    {
                        using (StreamReader reader = new StreamReader(expectedOutputJsonStream))
                        {
                            expectedVGOutput = reader.ReadToEnd();
                        }
                        runner = new VisualGridRunner(10, null, new MockServerConnectorFactory(webDriverProvider), logHandler);
                    }
                }
            }

            runner = runner ?? (useVisualGrid_ ? (EyesRunner)new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler));

            Eyes eyes = new Eyes(runner);
            TestUtils.SetupDebugScreenshotProvider(eyes, testName);
            SpecificTestContextRequirements testContextReqs = new SpecificTestContextRequirements(eyes, testName);
            testDataByTestId_.Add(TestContext.CurrentContext.Test.ID, testContextReqs);
            testContextReqs.TestNameAsFilename = testNameAsFilename;
            testContextReqs.ExpectedVGOutput = expectedVGOutput;
            testContextReqs.WebDriverProvider = webDriverProvider;

            string serverUrl = Environment.GetEnvironmentVariable("APPLITOOLS_SERVER_URL");
            if (!string.IsNullOrEmpty(serverUrl))
            {
                eyes.ServerUrl = serverUrl;
            }

            if (forceFullPageScreenshot != null)
            {
                eyes.ForceFullPageScreenshot = forceFullPageScreenshot.Value;
            }

            eyes.HideScrollbars = true;
            eyes.StitchMode = stitchMode_;
            eyes.SaveNewTests = false;
            eyes.Batch = TestDataProvider.BatchInfo;

            return eyes;
        }

        private string SetupSeleniumServer(string testName)
        {
            if (TestUtils.RUNS_ON_TRAVIS)
            {
                if (options_.BrowserName != "chrome" && options_.BrowserName != "firefox")
                {
                    Dictionary<string, object> sauceOptions = new Dictionary<string, object>
                    {
                        ["username"] = TestDataProvider.SAUCE_USERNAME,
                        ["accesskey"] = TestDataProvider.SAUCE_ACCESS_KEY,
                        ["screenResolution"] = "1920x1080",
                        ["name"] = testName + $" ({GetEyes().FullAgentId})",
                        ["idleTimeout"] = 360
                    };

                    if (options_ is OpenQA.Selenium.IE.InternetExplorerOptions ieOptions)
                    {
                        ieOptions.AddAdditionalCapability("sauce:options", sauceOptions, true);
                        return TestDataProvider.SAUCE_SELENIUM_URL;
                    }

                    if (options_ is OpenQA.Selenium.Safari.SafariOptions safariOptions)
                    {
                        safariOptions.AddAdditionalCapability("sauce:options", sauceOptions);
                        return TestDataProvider.SAUCE_SELENIUM_URL;
                    }

                }
            }
            string seleniumServerUrl = this.seleniumServerUrl ?? Environment.GetEnvironmentVariable("SELENIUM_SERVER_URL");
            if (seleniumServerUrl != null)
            {
                if (seleniumServerUrl.ToLower().Contains("ondemand.saucelabs.com"))
                {
                    Dictionary<string, object> sauceOptions = new Dictionary<string, object>
                    {
                        ["username"] = TestDataProvider.SAUCE_USERNAME,
                        ["accesskey"] = TestDataProvider.SAUCE_ACCESS_KEY,
                        ["name"] = testName + $" ({GetEyes().FullAgentId})",
                        ["idleTimeout"] = 360
                    };
                    if (options_ is OpenQA.Selenium.Chrome.ChromeOptions chromeOptions)
                    {
                        chromeOptions.UseSpecCompliantProtocol = true;
                        chromeOptions.BrowserVersion = "77.0";
                        chromeOptions.AddAdditionalCapability("sauce:options", sauceOptions, true);
                    }
                }
                else if (seleniumServerUrl.ToLower().Contains("hub-cloud.browserstack.com"))
                {
                    Dictionary<string, object> browserstackOptions = new Dictionary<string, object>
                    {
                        ["userName"] = TestDataProvider.BROWSERSTACK_USERNAME,
                        ["accessKey"] = TestDataProvider.BROWSERSTACK_ACCESS_KEY,
                        ["name"] = testName + $" ({GetEyes().FullAgentId})"
                    };
                    if (options_ is OpenQA.Selenium.Chrome.ChromeOptions chromeOptions)
                    {
                        chromeOptions.UseSpecCompliantProtocol = true;
                        chromeOptions.BrowserVersion = "77.0";
                        chromeOptions.AddAdditionalCapability("bstack:options", browserstackOptions, true);
                    }
                }
            }
            return seleniumServerUrl;
        }

        [OneTimeSetUp]
        public new void OneTimeSetup()
        {
        }

        [SetUp]
        public new void SetUp()
        {
            Init_(TestContext.CurrentContext.Test.MethodName);
        }

        [TearDown]
        public void Teardown()
        {
            string testId = TestContext.CurrentContext.Test.ID;
            testDataByTestId_.TryGetValue(testId, out SpecificTestContextRequirements testData);
            testDataByTestId_.Remove(testId);
            Eyes eyes = testData?.Eyes;
            IWebDriver driver = testData?.WebDriver;
            try
            {
                TestResults results = eyes.Close();
                if (results != null)
                {
                    SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);

                    if (sessionResults != null)
                    {
                        ActualAppOutput[] actualAppOutput = sessionResults.ActualAppOutput;
                        for (int i = 0; i < actualAppOutput.Length; i++)
                        {
                            Metadata.ImageMatchSettings ims = actualAppOutput[i].ImageMatchSettings;
                            CompareRegions_(testData, ims, i);
                            CompareProperties_(testData, ims, i);
                        }
                    }

                    testData.Eyes.Logger.Log(TraceLevel.Notice, testData.TestName, Stage.TestFramework, StageType.Complete,
                        new { results.Mismatches });
                }
                if (testData?.Eyes.activeEyes_ is VisualGridEyes visualGridEyes &&
                    visualGridEyes.runner_.ServerConnector is Mock.MockServerConnector mockServerConnector)
                {
                    List<RenderRequest> requests = new List<RenderRequest>();
                    foreach (string requestJson in mockServerConnector.RenderRequests)
                    {
                        RenderRequest[] reqs = JsonConvert.DeserializeObject<RenderRequest[]>(requestJson);
                        requests.AddRange(reqs);
                    }
                    string serializedRequests = JsonUtils.CreateSerializer(indent: true).Serialize(requests);
                    if (!TestUtils.RUNS_ON_CI)
                    {
                        string directory = Path.Combine(TestUtils.LOGS_PATH, "DotNet", "VGResults");
                        File.WriteAllText(Path.Combine(directory, testData.TestNameAsFilename + ".json"), serializedRequests);
                    }
                    string expectedVGOutput = testData.ExpectedVGOutput?.Replace("{{agentId}}", eyes.FullAgentId);
                    Assert.AreEqual(expectedVGOutput, serializedRequests, "VG Request DOM JSON");
                }
            }
            catch (Exception ex)
            {
                eyes?.Logger?.GetILogHandler()?.Open();
                CommonUtils.LogExceptionStackTrace(GetEyes()?.Logger, Stage.TestFramework, StageType.Failed, ex, testId);
                throw;
            }
            finally
            {
                driver?.Quit();
                eyes?.Abort();
                eyes?.runner_.GetAllTestResults(false);
            }
        }

        protected override string GetTestName()
        {
            return testDataByTestId_[TestContext.CurrentContext.Test.ID].TestName;
        }

        private void CompareProperties_(SpecificTestContextRequirements testData, Metadata.ImageMatchSettings ims, int index)
        {
            Dictionary<string, object> expectedProps = GetExpectationsAtIndex(index, testData).ExpectedProperties;

            Type imsType = typeof(Metadata.ImageMatchSettings);
            foreach (KeyValuePair<string, object> kvp in expectedProps)
            {
                string propertyNamePath = kvp.Key;
                string[] properties = propertyNamePath.Split('.');

                Type currentType = imsType;
                object currentObject = ims;
                foreach (string propName in properties)
                {
                    PropertyInfo pi = currentType.GetProperty(propName);
                    currentObject = pi.GetValue(currentObject, null);
                    if (currentObject == null) break;
                    currentType = currentObject.GetType();
                }

                Assert.AreEqual(kvp.Value, currentObject);
            }
        }

        private void CompareRegions_(SpecificTestContextRequirements testData, Metadata.ImageMatchSettings ims, int index)
        {
            if (!CompareExpectedRegion)
            {
                return;
            }

            Expectations expectations = GetExpectationsAtIndex(index, testData);
            CompareAccessibilityRegionsList_(ims.Accessibility, expectations.ExpectedAccessibilityRegions, "Accessibility");
            CompareFloatingRegionsList_(ims.Floating, expectations.ExpectedFloatingRegions, "Floating");
            TestUtils.CompareSimpleRegionsList_(ims.Ignore, expectations.ExpectedIgnoreRegions, "Ignore");
            TestUtils.CompareSimpleRegionsList_(ims.Layout, expectations.ExpectedLayoutRegions, "Layout");
            TestUtils.CompareSimpleRegionsList_(ims.Content, expectations.ExpectedContentRegions, "Content");
            TestUtils.CompareSimpleRegionsList_(ims.Strict, expectations.ExpectedStrictRegions, "Strict");
        }


        private static void CompareFloatingRegionsList_(FloatingMatchSettings[] actualRegions, HashSet<FloatingMatchSettings> expectedRegions, string type)
        {
            HashSet<FloatingMatchSettings> expectedRegionsClone = new HashSet<FloatingMatchSettings>(expectedRegions);
            if (expectedRegions.Count > 0)
            {
                foreach (FloatingMatchSettings region in actualRegions)
                {
                    if (!expectedRegionsClone.Remove(region))
                    {
                        Assert.Fail("actual {0} region {1} not found in expected regions list", type, region);
                    }
                }
                Assert.IsEmpty(expectedRegionsClone, "not all expected regions found in actual regions list.", type);
            }
        }

        internal static void CompareAccessibilityRegionsList_(AccessibilityRegionByRectangle[] actualRegions, HashSet<AccessibilityRegionByRectangle> expectedRegions, string type)
        {
            HashSet<AccessibilityRegionByRectangle> expectedRegionsClone = new HashSet<AccessibilityRegionByRectangle>(expectedRegions);
            if (expectedRegions.Count > 0)
            {
                foreach (AccessibilityRegionByRectangle region in actualRegions)
                {
                    if (!expectedRegionsClone.Remove(region))
                    {
                        Assert.Fail("actual {0} region {1} not found in expected regions list", type, region);
                    }
                }
                Assert.IsEmpty(expectedRegionsClone, "not all expected regions found in actual regions list.", type);
            }
        }

    }
}

using System;
using System.Drawing;
using Applitools;
using Applitools.Selenium;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScreenOrientation = Applitools.VisualGrid.ScreenOrientation;

namespace Eyes.Selenium.E2ETests
{
    /// <summary>
    /// Tests for the ACME Bank demo app.
    /// </summary>
    public class VisualLocatorReproducer
    {
        private static readonly string driverPath_ = Environment.GetEnvironmentVariable("CHROMEWEBDRIVER");
        // Test control inputs to read once and share for all tests
        private static string applitoolsApiKey_;

        // Applitools objects to share for all tests
        private static BatchInfo batch_;
        private static Configuration config_;
        private static VisualGridRunner runner_;

        // Test-specific objects
        private ChromeDriver driver_;
        private Applitools.Selenium.Eyes eyes_;

        /// <summary>
        /// Sets up the configuration for running visual tests in the Ultrafast Grid.
        /// The configuration is shared by all tests in a test suite, so it belongs in a `OneTimeSetUp` method.
        /// If you have more than one test class, then you should abstract this configuration to avoid duplication.
        /// </summary>
        [OneTimeSetUp]
        public static void SetUpConfigAndRunner()
        {
            // Read the Applitools API key from an environment variable.
            applitoolsApiKey_ = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");

            // Create the runner for the Ultrafast Grid.
            // Concurrency refers to the number of visual checkpoints Applitools will perform in parallel.
            // Warning: If you have a free account, then concurrency will be limited to 1.
            runner_ = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));

            // Create a new batch for tests.
            // A batch is the collection of visual checkpoints for a test suite.
            // Batches are displayed in the dashboard, so use meaningful names.
            batch_ = new BatchInfo("Example: Selenium C# NUnit with the Ultrafast Grid");

            // Create a configuration for Applitools Eyes.
            config_ = new Applitools.Configuration();

            // Set the Applitools API key so test results are uploaded to your account.
            // If you don't explicitly set the API key with this call,
            // then the SDK will automatically read the `APPLITOOLS_API_KEY` environment variable to fetch it.
            config_.SetApiKey(applitoolsApiKey_);

            // Set the batch for the config.
            config_.SetBatch(batch_);

            // Add 3 desktop browsers with different viewports for cross-browser testing in the Ultrafast Grid.
            // Other browsers are also available, like Edge and IE.
            config_.AddBrowser(800, 600, BrowserType.CHROME);
            config_.AddBrowser(1600, 1200, BrowserType.FIREFOX);
            config_.AddBrowser(1024, 768, BrowserType.SAFARI);

            // Add 2 mobile emulation devices with different orientations for cross-browser testing in the Ultrafast Grid.
            // Other mobile devices are available, including iOS.
            config_.AddDeviceEmulation(DeviceName.Pixel_2, ScreenOrientation.Portrait);
            config_.AddDeviceEmulation(DeviceName.Nexus_10, ScreenOrientation.Landscape);
        }

        /// <summary>
        /// Sets up each test with its own ChromeDriver and Applitools Eyes objects.
        /// </summary>
        [SetUp]
        public void OpenBrowserAndEyes()
        {
            // Open the browser with the ChromeDriver instance.
            // Even though this test will run visual checkpoints on different browsers in the Ultrafast Grid,
            // it still needs to run the test one time locally to capture snapshots.
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("no-sandbox");
            driver_ = new ChromeDriver(driverPath_, options);
            
            // Set an implicit wait of 10 seconds.
            // For larger projects, use explicit waits for better control.
            // https://www.selenium.dev/documentation/webdriver/waits/
            // The following call works for Selenium 4:
            driver_.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Create the Applitools Eyes object connected to the VisualGridRunner and set its configuration.
            eyes_ = new Applitools.Selenium.Eyes(runner_);
            eyes_.SetConfiguration(config_);
            eyes_.SaveNewTests = true;

            // Open Eyes to start visual testing.
            // It is a recommended practice to set all four inputs:
            string testName = TestContext.CurrentContext.Test.Name;
            eyes_.Open(
                driver_,                         // WebDriver object to "watch"
                "ACME Bank Web App",            // The name of the app under test
                testName,                       // The name of the test case
                new Size(1024, 768));           // The viewport size for the local browser
        }

        /// <summary>
        /// This test covers login for the Applitools demo site, which is a dummy banking app.
        /// The interactions use typical Selenium WebDriver calls,
        /// but the verifications use one-line snapshot calls with Applitools Eyes.
        /// If the page ever changes, then Applitools will detect the changes and highlight them in the dashboard.
        /// Traditional assertions that scrape the page for text values are not needed here.
        /// </summary>
        [Test]
        public void LogIntoBankAccount()
        {
            // Load the login page.
            driver_.Navigate().GoToUrl("https://demo.applitools.com");

            var locatorDict = eyes_.Locate("avatar", "twitter", "circles");
            //var locatorDict = Eyes.Locate(new string[] { "avatar", "twitter", "circles" });
            Console.WriteLine(locatorDict.Count);

            foreach (var item in locatorDict)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }

            // Verify the full login page loaded correctly.
            eyes_.Check(Target.Window().Fully().WithName("Login page"));

            // Perform login.
            // ReSharper disable once StringLiteralTypo
            driver_.FindElement(By.Id("username")).SendKeys("applibot");
            driver_.FindElement(By.Id("password")).SendKeys("I<3VisualTests");
            driver_.FindElement(By.Id("log-in")).Click();

            // Verify the full main page loaded correctly.
            // This snapshot uses LAYOUT match level to avoid differences in closing time text.
            eyes_.Check(Target.Window().Fully().WithName("Main page").Layout());
        }

        /// <summary>
        /// Concludes the test by quitting the browser and closing Eyes.
        /// </summary>
        [TearDown]
        public void CleanUpTest()
        {
            // Quit the WebDriver instance.
            driver_.Quit();

            // Close Eyes to tell the server it should display the results.
            eyes_.CloseAsync();

            // Warning: `Eyes.CloseAsync()` will NOT wait for visual checkpoints to complete.
            // You will need to check the Applitools dashboard for visual results per checkpoint.
            // Note that "unresolved" and "failed" visual checkpoints will not cause the NUnit test to fail.

            // If you want the NUnit test to wait synchronously for all checkpoints to complete, then use `Eyes.Close()`.
            // If any checkpoints are unresolved or failed, then `Eyes.Close()` will make the NUnit test fail.
        }

        /// <summary>
        /// Prints the final summary report for the test suite.
        /// </summary>
        [OneTimeTearDown]
        public static void PrintResults()
        {
            // Close the batch and report visual differences to the console.
            // Note that it forces NUnit to wait synchronously for all visual checkpoints to complete.
            TestResultsSummary allTestResults = runner_.GetAllTestResults();
            Console.WriteLine(allTestResults);
        }
    }
}

using Applitools.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Applitools.VisualGrid.Demo
{
    class ApplitoolsTutorial
    {
        public static void Main()
        {
            ApplitoolsTutorial program = new ApplitoolsTutorial();
            program.Run();
        }

        private VisualGridRunner runner;
        private Eyes eyes;
        private IWebDriver webDriver;

        private void Run()
        {
            Init();

            TestLoginPage();
            TestDashboardPage();

            ValidateResults();
        }

        private void Init()
        {
            // Create a new webdriver
            webDriver = new ChromeDriver();

            // Navigate to the url we want to test
            webDriver.Url = "https://demo.applitools.com";

            // ⭐️ Note to see visual bugs, run the test using the above URL for the 1st run.
            //but then change the above URL to https://demo.applitools.com/index_v2.html (for the 2nd run)

            // Create a runner with concurrency of 10
            runner = new VisualGridRunner(10);

            // Set StdOut log handler with regular verbosity.
            runner.SetLogHandler(new StdoutLogHandler(false));

            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            eyes = new Eyes(runner);

            // Create configuration object
            Selenium.Configuration conf = new Selenium.Configuration();

            //conf.SetApiKey("APPLITOOLS_API_KEY");    // Set the Applitools API KEY here or as an environment variable "APPLITOOLS_API_KEY"
            conf.SetTestName("C# VisualGrid demo 2")   // Set test name
                .SetAppName("Demo app");             // Set app name

            // Add browsers with different viewports
            conf.AddBrowser(800, 600, BrowserType.CHROME);
            conf.AddBrowser(700, 500, BrowserType.FIREFOX);
            conf.AddBrowser(1200, 800, BrowserType.IE_10);
            conf.AddBrowser(1600, 1200, BrowserType.IE_11);
            conf.AddBrowser(1024, 768, BrowserType.EDGE);

            // Add iPhone 4 device emulation in Portrait mode
            conf.AddDeviceEmulation(DeviceName.iPhone_4, ScreenOrientation.Portrait);

            // Set the configuration object to eyes
            eyes.SetConfiguration(conf);
        }

        private void TestLoginPage()
        {
            // Call Open on eyes to initialize a test session
            eyes.Open(webDriver);

            // check the login page
            eyes.Check(Target.Window().Fully().WithName("Login page"));

            eyes.CloseAsync();
        }

        private void TestDashboardPage()
        {
            // Call Open on eyes to initialize a test session
            eyes.Open(webDriver);

            webDriver.FindElement(By.Id("log-in")).Click();

            // Check the app page
            eyes.Check(Target.Window().Fully().WithName("Dashboard page"));

            eyes.CloseAsync();
        }

        private void ValidateResults()
        {
            // Close the browser
            webDriver.Quit();

            Console.WriteLine("Collecting results...");
            
            //Wait and collect all test results
            TestResultsSummary allTestResults = runner.GetAllTestResults();
            Console.WriteLine(allTestResults);
        }
    }
}

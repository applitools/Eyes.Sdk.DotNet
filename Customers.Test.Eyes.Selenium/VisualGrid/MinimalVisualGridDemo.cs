using Applitools.Selenium;
using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;

namespace Applitools.VisualGrid.Demo
{
    public class MinimalVisualGridDemo
    {
        public static void Main()
        {
            MinimalVisualGridDemo program = new MinimalVisualGridDemo();
            program.Run();
        }

        private void Run()
        {
            // Create a new webdriver
            IWebDriver webDriver = new ChromeDriver();

            // Navigate to the url we want to test
            webDriver.Url = "https://applitools.com/helloworld";

            // Create a runner with concurrency of 10
            VisualGridRunner runner = new VisualGridRunner(10);

            runner.SetLogHandler(TestUtils.InitLogHandler(nameof(MinimalVisualGridDemo)));

            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            Eyes eyes = new Eyes(runner);

            // Create SeleniumConfiguration.
            Selenium.Configuration sconf = new Selenium.Configuration();

            // Set test name
            sconf.SetTestName("Visual Grid Demo");

            // Set app name
            sconf.SetAppName("Visual Grid Demo");

            // Add browsers
            sconf.AddBrowser(800, 600, BrowserType.CHROME);
            sconf.AddBrowser(700, 500, BrowserType.FIREFOX);
            sconf.AddBrowser(1200, 800, BrowserType.IE_10);
            sconf.AddBrowser(1200, 800, BrowserType.IE_11);
            sconf.AddBrowser(1600, 1200, BrowserType.EDGE);

            //// Add iPhone 4 device emulation
            //ChromeEmulationInfo iphone4 = new ChromeEmulationInfo(DeviceName.iPhone_4, ScreenOrientation.Portrait);
            //sconf.AddDeviceEmulation(iphone4);

            // Add custom mobile device emulation
            //EmulationDevice customMobile = new EmulationDevice(width: 1024, height: 768, deviceScaleFactor: 2);
            //sconf.AddDeviceEmulation(customMobile);

            sconf.AddDeviceEmulation(DeviceName.iPhone_5SE, ScreenOrientation.Landscape);
            sconf.AddDeviceEmulation(DeviceName.Galaxy_S5);

            //sconf.AddDeviceEmulation(800, 640);

            // Set the configuration object to eyes
            eyes.SetConfiguration(sconf);

            // Call Open on eyes to initialize a test session
            eyes.Open(webDriver);

            // Add 2 checks
            eyes.Check(Target.Window().WithName("Step 1 - Viewport").Ignore(By.CssSelector(".primary")));
            eyes.Check(Target.Window().Fully().WithName("Step 1 - Full Page").Floating(By.CssSelector(".primary"), 10, 20, 30, 40).Floating(By.TagName("button"), 1, 2, 3, 4));

            webDriver.FindElement(By.TagName("button")).Click();

            // Add 2 checks
            eyes.Check(Target.Window().WithName("Step 2 - Viewport"));
            eyes.Check(Target.Window().Fully().WithName("Step 2 - Full Page"));

            // Close the browser
            webDriver.Quit();

            // Close eyes and collect results
            TestResults results = eyes.Close();
            TestResultsSummary allTestResults = runner.GetAllTestResults();
        }
    }
}

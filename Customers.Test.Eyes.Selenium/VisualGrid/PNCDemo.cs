using Applitools.Selenium;
using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;

namespace Applitools.VisualGrid.Demo
{
    public class PNCDemo
    {
        public static void Main()
        {
            PNCDemo program = new PNCDemo();
            program.Run();
        }

        private void Run()
        {
            // Create a new webdriver
            IWebDriver webDriver = new ChromeDriver();

            // Navigate to the url we want to test
            webDriver.Url = "https://pnc.com/";

            // Create a runner with concurrency of 10
            VisualGridRunner runner = new VisualGridRunner(10);

            runner.SetLogHandler(TestUtils.InitLogHandler(nameof(PNCDemo)));

            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            Eyes eyes = new Eyes(runner);

            // Create SeleniumConfiguration.
            Selenium.Configuration sconf = new Selenium.Configuration();

            // Add browsers
            sconf.AddDeviceEmulation(DeviceName.Galaxy_S5);
            foreach (BrowserType b in Enum.GetValues(typeof(BrowserType)))
            {
                sconf.AddBrowser(700, 500, b);
                sconf.AddBrowser(800, 600, b);
                sconf.AddBrowser(900, 700, b);
                sconf.AddBrowser(1200, 800, b);
                sconf.AddBrowser(1600, 1200, b);
            }

            //sconf.AddDeviceEmulation(800, 640);

            // Set the configuration object to eyes
            eyes.SetConfiguration(sconf);

            // Call Open on eyes to initialize a test session
            eyes.Open(webDriver, "PNC Demo", "PNC Demo", new System.Drawing.Size(1000, 700));

            eyes.Check("PNC", Target.Window().Fully().SendDom()
                      .Layout(By.CssSelector("#main-header > div.highlighted-topics-tout-container-parsys.limited-parsys"))
                      .Layout(By.CssSelector("#main-header > div.promo-parsys.limited-parsys"))
                      .Ignore(By.CssSelector("#oo-feedback > img")));

            // Close the browser
            webDriver.Quit();

            // Close eyes and collect results
            TestResults results = eyes.Close();
            TestResultsSummary allTestResults = runner.GetAllTestResults();
        }
    }
}

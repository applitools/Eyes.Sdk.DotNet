using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    //[Parallelizable(ParallelScope.All)]
    public class TestHelloWorld : ReportingTestSuite
    {
        //[TestCase(true), TestCase(false)]
        public void HelloWorldTest(bool useVisualGrid)
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.com/helloworld";

            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();

            runner.SetLogHandler(TestUtils.InitLogHandler());
            Eyes eyes = new Eyes(runner);

            Configuration sconf = eyes.GetConfiguration();

            string suffix = useVisualGrid ? "_VG" : "";
            // Set test name
            sconf.SetTestName("Hello World Demo" + suffix);

            // Set app name
            sconf.SetAppName("Hello World Demo");

            // Add browsers
            sconf.AddBrowser(800, 600, BrowserType.CHROME);
            sconf.AddBrowser(700, 500, BrowserType.FIREFOX);
            sconf.AddBrowser(1200, 800, BrowserType.IE_10);
            sconf.AddBrowser(1200, 800, BrowserType.IE_11);
            sconf.AddBrowser(1600, 1200, BrowserType.EDGE);

            // Set the configuration object to eyes
            eyes.SetConfiguration(sconf);

            try
            {
                // Call Open on eyes to initialize a test session
                eyes.Open(webDriver);

                // Add 2 checks
                eyes.Check(Target.Window().WithName("Step 1 - Viewport").Ignore(By.CssSelector(".primary")));
                eyes.Check(Target.Window().Fully().WithName("Step 1 - Full Page")
                    .Floating(By.CssSelector(".primary"), 10, 20, 30, 40)
                    .Floating(By.TagName("button"), 1, 2, 3, 4));

                webDriver.FindElement(By.TagName("button")).Click();

                // Add 2 checks
                eyes.Check(Target.Window().WithName("Step 2 - Viewport"));
                eyes.Check(Target.Window().Fully().WithName("Step 2 - Full Page"));

                // Close eyes and collect results
                eyes.Close();
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }
    }
}

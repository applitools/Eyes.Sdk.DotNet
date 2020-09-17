namespace Applitools.Selenium.Tests
{
    using System;
    using System.Drawing;
    using Applitools.Ui;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Safari;
    using System.IO;
    using Applitools.Utils;

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApplitoolsTest
    {
        private static void SetupLogging(Eyes eyes, string testName)
        {
            ILogHandler logHandler;
            if (Environment.GetEnvironmentVariable("CI") == null)
            {
                string path = TestUtils.InitLogPath(testName);
                eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider()
                {
                    Path = path,
                    Prefix = testName + "_"
                };
                logHandler = new FileLogHandler(Path.Combine(path, $"{testName}.log"), true, true);
            }
            else
            {
                logHandler = new StdoutLogHandler(true);
                //logHandler_ = new TraceLogHandler(true);
            }

            if (logHandler != null)
            {
                eyes.SetLogHandler(logHandler);
            }
        }

        [Test]
        public void TestHome()
        {
            //FirefoxOptions options = new FirefoxOptions();
            //if (TestSetup.RUN_HEADLESS)
            //{
            //    options.AddArgument("--headless");
            //}
            //IWebDriver webdriver = new FirefoxDriver(options);

            ChromeOptions options = new ChromeOptions();
            if (SeleniumUtils.RUN_HEADLESS)
            {
                options.AddArgument("headless");
            }
            IWebDriver webdriver = new ChromeDriver(options);

            var eyes = new Eyes();
            eyes.BranchName = "demo";
            eyes.StitchMode = StitchModes.CSS;
            SetupLogging(eyes, nameof(TestHome));
            try
            {
                IWebDriver driver = eyes.Open(webdriver, "www.applitools.com", "Home", new Size(1006, 677));

                driver.Url = "https://www.applitools.com";
                //eyes.Check("Home (window)", Target.Window().ScrollRootElement(By.CssSelector("div.horizontal-page")).Fully());
                eyes.Check("Home (region)", Target.Region(By.CssSelector("div.page")).Fully());
                //eyes.Check("Home", Target.Window().ScrollRootElement(By.CssSelector("div.page")).Fully());

                eyes.Close();

                driver.FindElement(By.LinkText("Features")).Click();
                eyes.Check("Features", Target.Window().ScrollRootElement(By.CssSelector("div.page")).Fully().Timeout(TimeSpan.FromSeconds(5)));

                CollectionAssert.AreEqual(
                    new[] { "Features", "CUSTOMERS" },
                    eyes.InRegion(By.TagName("h1")).And(By.LinkText("Customers")).GetText());

                driver.FindElement(By.LinkText("Free Trial")).Click();
                eyes.Check("Sign Up", Target.Window().Fully());

                eyes.Close();
                /**/
            }
            finally
            {
                eyes.Abort();
                webdriver.Quit();
            }
        }

        [Test]
        public void IgnoreDisplacementsTest()
        {
            Eyes eyes = new Eyes();
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            eyes.ForceFullPageScreenshot = true;

            Configuration conf = eyes.GetConfiguration();

            conf.IgnoreDisplacements = true;

            eyes.SetConfiguration(conf);

            eyes.Open(driver, "Ignore displacments", "test", new Size(800, 600));

            driver.Url = "https://applitools.com/helloworld";

            //eyes.Check("step", Target.Window().IgnoreDisplacements(true));
            eyes.Check("step", Target.Window());
            eyes.Check("step", Target.Window());
            eyes.Close(false);
            driver.Quit();
        }
    }
}

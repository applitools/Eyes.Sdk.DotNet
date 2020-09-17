using Applitools;
using Applitools.Selenium;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Drawing;
using Configuration = Applitools.Selenium.Configuration;

namespace Elad
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class UnitTest4
    {
        [Test]
        public void TestProperties()
        {
            Eyes eyes = new Eyes();

            eyes.AppName = "app name";
            eyes.TestName = "test name";
            eyes.BaselineBranchName = "baseline branch name";
            eyes.ParentBranchName = "parent branch name";
            eyes.BranchName = "branch name";
            eyes.BaselineEnvName = "baseline env name";
            eyes.EnvironmentName = "env name";
            eyes.HostApp = "host app";
            eyes.HostOS = "windows";
            eyes.AgentId = "some agent id";

            string fullAgentId = eyes.FullAgentId;

            eyes.IgnoreCaret = true;
            eyes.HideCaret = true;
#pragma warning disable CS0618 // Type or member is obsolete
            eyes.SaveFailedTests = true;
#pragma warning restore CS0618 // Type or member is obsolete
            eyes.SaveNewTests = true;
            eyes.SendDom = true;
            eyes.SaveDiffs = true;

            eyes.StitchOverlap = 20;

            eyes.Batch = new BatchInfo();
#pragma warning disable CS0612 // Type or member is obsolete
            eyes.FailureReports = FailureReports.Immediate;
#pragma warning restore CS0612 // Type or member is obsolete
            eyes.MatchTimeout = TimeSpan.FromSeconds(30);

            eyes.ViewportSize = new RectangleSize(1000, 600);
            eyes.ViewportSize = new Size(1000, 600);

            eyes.MatchLevel = MatchLevel.Strict;

        }

        [Test]
        public void TestConfiguration()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options);
            Eyes eyes = new Eyes();
            try
            {
                Configuration conf = new Configuration();
                conf.SetServerUrl("https://testeyes.applitools.com").SetApiKey("HSNghRv9zMtkhcmwj99injSggnd2a8zUY390OiyNRWoQ110");
                eyes.Open(driver, "test configuration", "test server url 1", new Size(800, 600));
                TestResults results1 = eyes.Close();
                eyes.SetConfiguration(conf);
                eyes.Open(driver, "test configuration", "test server url 2", new Size(800, 600));
                TestResults results2 = eyes.Close();

                StringAssert.DoesNotStartWith("https://testeyesapi.applitools.com", results1.ApiUrls.Session);
                StringAssert.StartsWith("https://testeyesapi.applitools.com", results2.ApiUrls.Session);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Test]
        public void ServiceTitan()
        {
            Eyes eyes = new Eyes();

            eyes.SendDom = true;

            IWebDriver driver = new ChromeDriver();

            driver.Url = "https://qa.servicetitan.com/Auth/Login";
            driver.FindElement(By.CssSelector("#username")).SendKeys("applitools");
            driver.FindElement(By.CssSelector("#password")).SendKeys("1234");
            driver.FindElement(By.CssSelector("#loginButton")).Click();

            eyes.Open(driver, "ServiceTitan", "test ServiceTitan", new Size(1000, 800));
            eyes.Check("step1", Target.Window().Fully());

            eyes.Close(false);
            driver.Quit();
        }

        //private static final String URL = 

        [Test]
        public void ScreenshotAfterScrollOnIE()
        {

            //DesiredCapabilities caps = DesiredCapabilities.edge();
            //caps.setCapability("platform", "Windows 10");
            //caps.setCapability("version", "18.17763");
            //caps.setCapability("username", System.getenv("SAUCE_USERNAME"));
            //caps.setCapability("accesskey", System.getenv("SAUCE_ACCESS_KEY"));
            //caps.setCapability("name", "Michael's");

            //        capabilities.setCapability("app", "https://applitools.bintray.com/Examples/app-debug.apk");
            //
            //        capabilities.setCapability("appPackage", "com.applitoolstest");
            //        capabilities.setCapability("appActivity", "com.applitoolstest.ScrollActivity");
            //        capabilities.setCapability("newCommandTimeout", 600);


            Eyes eyes = new Eyes();
            eyes.WaitBeforeScreenshots = 1500;
            eyes.SetLogHandler(new StdoutLogHandler(true));

            //        eyes.setSendDom(true);

            IWebDriver driver = new EdgeDriver();

            driver.Url = "https://www.ns.nl/dagje-uit/wandelen"; //"https://www.ns.nl/flex";

            try
            {
                IWebDriver eyesDriver = eyes.Open(driver, "NS.nl", "Applitools test - Screenshot after scroll", new Size(800, 640));

                var targetLocator = eyesDriver.SwitchTo();


                try
                {
                    targetLocator.Frame("r42CookieBar");

                    eyesDriver.FindElement(By.CssSelector("a.button.accept")).Click();

                    eyesDriver.SwitchTo().ParentFrame();
                }
                catch (Exception e)
                {
                    eyes.Logger.Log("Error: " + e);
                }



                eyes.Check("Full results - OK", Target.Window().Fully());

                IWebElement provincieFilter = driver.FindElement(By.CssSelector("form.filter ns-filter#provincies"));
                provincieFilter.FindElements(By.CssSelector("ul li"))[0].Click();

                // Workaround
                //((JavascriptExecutor) driver).executeScript("return window.scrollTo(0,0)");

                eyes.Check("Filtered results - Top of page is missing in Edge", Target.Window().Fully());

                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}

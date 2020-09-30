using Applitools.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using Applitools.Tests.Utils;
using Applitools.Ufg;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CustomersPagesVGTest
    {
        [Test]
        public void TestPNC()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyes(driver, runner, "https://www.pnc.com/", 1000, 700, false);
            try
            {
                eyes.Check("PNC", Target.Window().Fully().SendDom()
                    .Layout(By.CssSelector("#main-header > div.highlighted-topics-tout-container-parsys.limited-parsys"))
                    .Layout(By.CssSelector("#main-header > div.promo-parsys.limited-parsys"))
                    .Ignore(By.CssSelector("#oo-feedback > img")));
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        [Test]
        public void TestVanguard()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyes(driver, runner, "https://investor.vanguard.com/home/", 1000, 700, false);
            Thread.Sleep(2000);
            try
            {
                eyes.Check("Vanguard",
                    Target
                    .Window()
                    .Fully()
                    .Strict()
                    .SendDom(false)
                    .Layout(By.ClassName("vgc-feedbackLink"))
                    );
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        [Test]
        public void TestJustEat()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyesWithLogger(runner, verbose: true, writeResources: true);
            //ConfigureSingleBrowser(eyes);
            ConfigureMultipleBrowsers(eyes);
            OpenEyes(driver, "https://www.just-eat.co.uk/", eyes, 1000, 700);
            try
            {
                //Close the cookie notification footer
                driver.FindElement(By.XPath("//*[@data-test-id='cookieBanner-close-button']")).Click();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //Driver for Uber ad shows randomly. Remove this element so it doesn't affect our tests
                //IWebElement uberContainer = _driver.FindElementByClassName("ex1140 l-container");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                //IWebElement element = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.ex1140.l-container")));

                js.ExecuteScript("var e = document.querySelector('div.ex1140.l-container'); if (e) e.hidden = true;");

                eyes.CheckWindow("Homepage");
                eyes.CheckWindow("another test");

                //Search by Postal Code
                driver.FindElement(By.Name("postcode")).SendKeys("EC4M7RA");
                driver.FindElement(By.XPath("//button[@data-test-id='find-restaurants-button']")).Click();

                //Deal with time of day issues -- Sometimes it asks if you want take away 
                driver.FindElement(By.ClassName("closeButton")).Click();

                //Narrow the search to just first in the list (helps when running before the restaurant is open
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.c-searchFilterSection-filterList  span:nth-child(1)"))).Click();

                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div[data-test-id=searchresults] section:nth-child(1)"))).Click();

                //Open the Show More link
                IList<IWebElement> showMoreLink = driver.FindElements(By.Id("showMoreText"));
                if (showMoreLink.Count > 0)
                {
                    Actions actions = new Actions(driver);
                    actions.MoveToElement(showMoreLink[0]).Click().Perform();
                }

                eyes.CheckWindow();
                eyes.CheckRegion(By.ClassName("menuDescription"), "Menu Description");

                //eyes.ForceFullPageScreenshot = false;

                //Check the top Food Allergy link
                driver.FindElement(By.XPath("//div[@id='basket']//button[contains(text(), 'If you or someone')]")).Click();
                eyes.CheckWindow("last");

                eyes.CheckRegion(By.XPath("//div[@data-ft='allergenModalDefault']"), "Food Allergy - Top", false);
                driver.FindElement(By.XPath("//button[text()='Close']")).Click();

                //Scroll to the bottom of the page to check the second Food Allergy link
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

                //retryingFindClick(By.XPath("//div[@id='menu']//button[contains(text(), 'If you or someone')]"));
                //eyes.CheckElement(By.XPath("//div[@data-ft='allergenModalDefault']"), "Food Allergy - Bottom");
                //eyes.CheckRegion(By.XPath("//div[@data-ft='allergenModalDefault']"), "Food Allergy - Bottom", false);
                //eyes.CheckWindow("Food Allergy - Bottom");
                //driver.FindElement(By.XPath("//button[text()='Close']")).Click();/**/
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        [Test]
        public void TestPandalytics()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyesWithLogger(runner, verbose: true, writeResources: true);
            //ConfigureSingleBrowser(eyes);
            ConfigureMultipleBrowsers(eyes);
            try
            {
                EyesWebDriver eyesWebDriver = (EyesWebDriver)OpenEyes(driver, "https://pandalytics-sandbox.instructure.com", eyes, 1000, 700);
                driver.FindElement(By.Id("pseudonym_session_unique_id")).SendKeys("Aayla_Secura@pandalytictestdata.edu");
                driver.FindElement(By.Id("pseudonym_session_password")).SendKeys("password");
                driver.FindElement(By.ClassName("Button--login")).Click();

                eyes.Check("Dashboard page", Target.Window().Fully());

                driver.Url = "https://pandalytics-sandbox.instructure.com/courses/17/external_tools/63";

                Thread.Sleep(1000);
                eyes.Check("LTI tool", Target.Frame("tool_content").Fully());
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        //[TestCase("https://www.verizonwireless.com/", "Verizon Wireless")]
        //[TestCase("https://www.kayak.com/", "Kayak")]
        [TestCase("https://www.humana.com", "Humana Login Page")]
        public void TestWebsiteThatRequireScrolling(string website, string name)
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyesWithLogger(runner, writeResources: true, verbose: true, testMethod: name);
            ConfigureMultipleBrowsers(eyes);

            try
            {
                EyesWebDriver eyesWebDriver = (EyesWebDriver)OpenEyes(driver, website, eyes, 1000, 700, name); ;
                EyesRemoteWebElement html = (EyesRemoteWebElement)eyesWebDriver.FindElement(By.TagName("html"));
                eyesWebDriver.ExecuteAsyncScript(@"var currScrollPosition = 0;
                    let cb = arguments[arguments.length - 1]
                    var interval = setInterval(function() {
                    let scrollPosition = document.documentElement.scrollTop;
                    currScrollPosition += 300;
                    window.scrollTo(0, currScrollPosition);
                    if (scrollPosition === document.documentElement.scrollTop)
                    {
                        clearInterval(interval);
                        window.scrollTo(0, 0);
                        cb()
                    }
                    }, 100); ");
                eyes.Check(name, Target.Window().Fully());
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        //[TestCase("https://test.asosservices.com/river-island/river-island-button-cowl-neck-jumper/prd/1775492?&x-site-origin=webmasterpdpstub.asosdevelopment.com", "ASOS")]
        //[TestCase("https://www.progressive.com/home/home/", "Progressive")]
        //[TestCase("https://www.walgreens.com/", "Walgreens")]
        //[TestCase("https://carbon.sage.com/components/button-toggle-group", "Sage")]
        //[TestCase("https://www.scotiabank.com/ca/en/personal.html", "Scotia Bank")]
        //[TestCase("https://www.cigna.com/individuals-families/", "Cigna")]
        //[TestCase("https://www.southwest.com/", "SouthWest")]
        //[TestCase("https://www.att.com/", "AT&T")]
        //[TestCase("https://www.cvshealth.com/", "CVS Health")]
        //[TestCase("https://www.chewy.com/", "Chewy")]
        //[TestCase("https://www.github.com", "GitHub")]
        //[TestCase("https://www.espn.com/", "ESPN")]
        //[TestCase("https://caesars.com/", "Caesars")]
        //[TestCase("https://www.bostonglobe.com/", "Boston Globe")]
        //[TestCase("https://myregus.com/", "My Regus")]
        //[TestCase("https://www.ihg.com/hotels/us/en/find-hotels/hotel/list?qDest=Niagra%20Falls,%20NY,%20United%20States&qCiMy=82019&qCiD=7&qCoMy=82019&qCoD=9&qAdlt=1&qChld=0&qRms=1&qRtP=6CBARC&qAkamaiCC=MX&qSrt=sDD&qBrs=re.ic.in.vn.cp.vx.hi.ex.rs.cv.sb.cw.ma.ul.ki.va&srb_u=0&qRad=30&qRdU=mi", "IHG")]
        //[TestCase("https://www.turncar.com/", "TurnCar")]
        [TestCase("https://developer.mozilla.org/en-US/docs/Web/API/Canvas_API/Tutorial/Drawing_shapes", "Mozilla")]
        public void TestWebsite(string website, string name)
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = InitEyes(driver, runner, website, 1000, 700, writeResources: true, verbose: true, testMethod: name);
            //Thread.Sleep(3000);
            try
            {
                eyes.Check(name, Target.Window().Fully());
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        private Eyes InitEyes(IWebDriver driver, EyesRunner runner, string url, int width, int height, bool writeResources = false, bool verbose = true, [CallerMemberName] string testMethod = null)
        {
            Eyes eyes = InitEyesWithLogger(runner, writeResources, verbose, testMethod);

            ConfigureMultipleBrowsers(eyes);

            OpenEyes(driver, url, eyes, width, height, testMethod);
            return eyes;
        }

        private static IWebDriver OpenEyes(IWebDriver driver, string url, Eyes eyes, int width, int height, [CallerMemberName] string testMethod = null)
        {
            IWebDriver eyesWebDriver = eyes.Open(driver, nameof(CustomersPagesVGTest), testMethod, new Size(width, height));
            driver.Url = url;
            return eyesWebDriver;
        }

        private static Eyes InitEyesWithLogger(EyesRunner runner, bool writeResources = false, bool verbose = true, [CallerMemberName] string testMethod = null)
        {
            string logPath = TestUtils.InitLogPath(testMethod);
            if (writeResources && runner is VisualGridRunner visualGridRunner)
            {
                visualGridRunner.DebugResourceWriter = new FileDebugResourceWriter(logPath);
            }
            Eyes eyes = new Eyes(runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler(testMethod, logPath, verbose));
            eyes.Batch = TestDataProvider.BatchInfo;
            return eyes;
        }

        private static void ConfigureSingleBrowser(Eyes eyes)
        {
            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(1200, 800, BrowserType.CHROME);
            eyes.SetConfiguration(config);
        }

        private static void ConfigureMultipleBrowsers(Eyes eyes)
        {
            Configuration config = eyes.GetConfiguration();
            config.AddDeviceEmulation(DeviceName.Galaxy_S5);
            foreach (BrowserType b in Enum.GetValues(typeof(BrowserType)))
            {
                //config.AddBrowser(700, 500, b);
                config.AddBrowser(800, 600, b);
                //config.AddBrowser(900, 700, b);
                config.AddBrowser(1200, 800, b);
                config.AddBrowser(1600, 1200, b);
            }

            //config.AddBrowser(700, 500,   BrowserType.CHROME);
            //config.AddBrowser(900, 700,   BrowserType.CHROME);
            //config.AddBrowser(1200, 800,  BrowserType.CHROME);
            //config.AddBrowser(1600, 1200, BrowserType.CHROME);

            //config.AddBrowser(700, 500,   BrowserType.FIREFOX);
            //config.AddBrowser(900, 700,   BrowserType.FIREFOX);
            //config.AddBrowser(1200, 800,  BrowserType.FIREFOX);
            //config.AddBrowser(1600, 1200, BrowserType.FIREFOX);

            //config.AddBrowser(700, 500,   BrowserType.IE_10);
            //config.AddBrowser(900, 700,   BrowserType.IE_10);
            //config.AddBrowser(1200, 800,  BrowserType.IE_10);
            //config.AddBrowser(1600, 1200, BrowserType.IE_10);

            //config.AddBrowser(700, 500,   BrowserType.IE_11);
            //config.AddBrowser(900, 700,   BrowserType.IE_11);
            //config.AddBrowser(1200, 800,  BrowserType.IE_11);
            //config.AddBrowser(1600, 1200, BrowserType.IE_11);

            //config.AddBrowser(700, 500,   BrowserType.EDGE);
            //config.AddBrowser(900, 700,   BrowserType.EDGE);
            //config.AddBrowser(1200, 800,  BrowserType.EDGE);
            //config.AddBrowser(1600, 1200, BrowserType.EDGE);
            eyes.SetConfiguration(config);
        }


        [Test]
        public void Trello_1544()
        {
            //EyesRunner runner = new VisualGridRunner(10);
            EyesRunner runner = new ClassicRunner();

            Eyes eyes = new Eyes(runner);

            Configuration configuration = eyes.GetConfiguration();
            configuration.AddBrowser(800, 600, BrowserType.CHROME);
            configuration.AddBrowser(1600, 1200, BrowserType.CHROME);
            eyes.SetConfiguration(configuration);

            eyes.SetLogHandler(new NunitLogHandler(true));
            eyes.MatchTimeout = TimeSpan.FromSeconds(0);
            IWebDriver driver = new ChromeDriver();

            try
            {

                eyes.Open(driver, "Trello 1544 PNC", "Intermittent Coded Regions", new Size(800, 600));
                driver.Url = "https://www.pnc.com/en/personal-banking.html";

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#container > div.hero-login-wrapper")));

                eyes.Check(Target.Window().Fully()
                    .Layout(By.CssSelector("#container > div.hero-login-wrapper"))
                    .Layout(By.CssSelector("#main-header > div.header-wrapper"))
                );
                eyes.Close(false);
            }
            finally
            {
                driver.Quit();
                runner.GetAllTestResults();
            }
        }
    }
}

using Applitools.Cropping;
using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
//using TestUtils = Applitools.Selenium.Tests.ApplitoolsTest;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class CustomerPagesTest2
    {
        [Test]
        public void Ticket31566()
        {
            var driver = new ChromeDriver();

            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            try
            {
                eyes.Open(driver, "Demo C# app", "Login Window", new Size(1300, 800));
                driver.Url = "https://cvi-ui-demo.coxautoinventory-np.com/inventory";
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#login input[name='username']")));
                driver.FindElement(By.CssSelector("#login input[name='username']")).SendKeys("uiauto");
                driver.FindElement(By.CssSelector("#login input[name='password']")).SendKeys("password3");
                driver.FindElement(By.CssSelector("#login input[name='login']")).Click();

                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#root > div > ul > li:nth-child(1) > a"))).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#Add-Vehicle"))).Click();

                eyes.Check("TAG NAME", Target.Region(By.CssSelector(".modal-content")).ScrollRootElement(By.CssSelector(".modal-dialog")));
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Ticket31566_CVS_Mobile()
        {
            ChromeOptions options = new ChromeOptions();
            ChromeMobileEmulationDeviceSettings mobileSettings = new ChromeMobileEmulationDeviceSettings();
            mobileSettings.PixelRatio = 4;
            mobileSettings.Width = 360;
            mobileSettings.Height = 740;
            mobileSettings.UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.137 Mobile Safari/537.36";
            options.EnableMobileEmulation(mobileSettings);
            IWebDriver driver = new ChromeDriver(options);

            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            try
            {
                eyes.Open(driver, "Demo C# app", "CVS Mobile");
                eyes.StitchMode = StitchModes.Scroll;
                driver.Url = "https://www.cvs.com/mobile/mobile-cvs-pharmacy/";
                eyes.Check("CVS!", Target.Window().Fully());
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }



        [Test]
        public void Trello_1091()
        {
            IWebDriver chromeDriver = new ChromeDriver();

            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            try
            {
                IWebDriver driver = eyes.Open(chromeDriver, "Datorama", "Dashboard", new Size(1300, 800));
                driver.Url = "https://ci-platform.datorama.com/1655137/visualize/401393/page/523292";
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                IWebElement loginField = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("dato-input[data-auto-id=login_email] input")));

                loginField.SendKeys("applitools.debug@gmail.com");
                driver.FindElement(By.CssSelector("dato-input[data-auto-id=login_password] input")).SendKeys("Aa123456");
                driver.FindElement(By.CssSelector("dato-button[data-auto-id=login_button] button")).Click();

                IWebElement dashboardElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("da-dashboard-content > div")));

                eyes.WaitBeforeScreenshots = 3000;
                eyes.Check("Datorama Dashboard", Target.Region(By.TagName("da-grid-stack")).ScrollRootElement(dashboardElement).Fully());
                eyes.Close();
            }
            finally
            {
                chromeDriver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_1532()
        {
            IWebDriver webDriver = new ChromeDriver();
            ClassicRunner runner = new ClassicRunner();
            webDriver.Url = "https://www.humana.com/medicare/medicare-supplement-plans/plan-g";

            Eyes eyes = new Eyes(runner);

            eyes.SetLogHandler(new StdoutLogHandler(true));
            Configuration conf = new Configuration();

            conf.SetTestName("#31659")
                .SetAppName("Humana")
                .SetViewportSize(new Size(1200, 800));

            eyes.SetConfiguration(conf);

            eyes.StitchMode = StitchModes.CSS;
            //eyes.HideCaret = false;
            eyes.Open(webDriver);

            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            IReadOnlyCollection<IWebElement> TabsInPage = webDriver.FindElements(By.XPath("//button[@role='tab']"));

            if (TabsInPage.Count > 0)
            {
                By layout = By.CssSelector("#anchor-main-content > section:nth-child(2) > div > div > div.col-12.col-sm-12.col-md-4");
                By ignore = By.CssSelector("#searchbox");

                eyes.Check(Target.Window().Fully().WithName("Login page").Layout(layout).Ignore(ignore)); //This is fine

                TabsInPage.ElementAt(0).Click();
                eyes.Check(Target.Window().Fully().WithName("Login page").Layout(layout).Ignore(ignore)); //This is wrong

                TabsInPage.ElementAt(1).Click();
                eyes.Check(Target.Window().Fully().WithName("Login page").Layout(layout).Ignore(ignore));
            }

            webDriver.Quit();
            eyes.CloseAsync();

            TestResultsSummary allTestResults = runner.GetAllTestResults();

            TestContext.Progress.WriteLine(allTestResults);

        }

        [Test]
        public void YeezysTest()
        {
            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            //ChromeOptions options = new ChromeOptions();
            //options.AddArguments("ignore-certificate-errors");

            //string browserstackURL = "https://hub-cloud.browserstack.com/wd/hub";

            //DesiredCapabilities capabilities = new DesiredCapabilities();
            //Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            //browserstackOptions.Add("osVersion", "9.0");
            //browserstackOptions.Add("deviceName", "Google Pixel 3 XL");
            //browserstackOptions.Add("realMobile", "true");
            //browserstackOptions.Add("local", "false");
            //browserstackOptions.Add("userName", Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"));
            //browserstackOptions.Add("accessKey", Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"));
            //capabilities.SetCapability("bstack:options", browserstackOptions);

            //IWebDriver driver = new RemoteWebDriver(new Uri(browserstackURL), capabilities);

            ChromeMobileEmulationDeviceSettings mobileSettings = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (Linux; Android 9; Pixel 3 XL) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.99 Mobile Safari/537.36",
                Width = 412,
                Height = 693,
                PixelRatio = 3.5
            };

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.EnableMobileEmulation(mobileSettings);
            IWebDriver driver = new ChromeDriver(chromeOptions);

            try
            {
                Configuration config = new Configuration();
                config.SetAppName("Zach's Demo c# App");
                //config.setViewportSize(new RectangleSize(1600, 900));
                config.SetTestName("YEEEZY");
                //config.setForceFullPageScreenshot(true);
                //config.setApiKey(System.getenv("APPLITOOLS_API_KEY_BUG"));
                eyes.SetConfiguration(config);

                //eyes.HideScrollbars = false;
                //eyes.HideCaret = false;
                eyes.SendDom = false;
                eyes.MatchTimeout = new TimeSpan(0);
                //eyes.setForceFullPageScreenshot(true);
                //eyes.SetSaveDebugScreenshots(true);

                eyes.StitchMode = StitchModes.CSS;

                driver = eyes.Open(driver);

                driver.Url = "https://www.adidas.co.uk/yeezy";
                //eyes.Check(Target.Window().Fully());
                eyes.Check(Target.Region(By.CssSelector(".theme-light")).ScrollRootElement(By.CssSelector("#app")).Fully());
                eyes.Close();
            }
            finally
            {
                driver?.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_1540_Humana_medicare()
        {
            string securityToken = "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI5MjIzOWFkNi1iNzQ5LTQ3MWMtOGM4ZS0wYWRkYTVkODZmNjMifQ.eyJqdGkiOiI1OGViNGVmZi0xZTg4LTRjMTMtYmNlYS1mMmM3ZjNmZGNjMGMiLCJleHAiOjAsIm5iZiI6MCwiaWF0IjoxNTgzMTc2NTAzLCJpc3MiOiJodHRwczovL2F1dGgucGVyZmVjdG9tb2JpbGUuY29tL2F1dGgvcmVhbG1zL21vYmlsZWNsb3VkLXBlcmZlY3RvbW9iaWxlLWNvbSIsImF1ZCI6Imh0dHBzOi8vYXV0aC5wZXJmZWN0b21vYmlsZS5jb20vYXV0aC9yZWFsbXMvbW9iaWxlY2xvdWQtcGVyZmVjdG9tb2JpbGUtY29tIiwic3ViIjoiMjBiYzU2YTAtZmY2MS00MmQwLWExZTEtYWFmM2FkODg0NjM4IiwidHlwIjoiT2ZmbGluZSIsImF6cCI6Im9mZmxpbmUtdG9rZW4tZ2VuZXJhdG9yIiwibm9uY2UiOiJjYjBjNTFmNC0wNTI1LTQ4MTktYmJiNy1jZDE0Y2Y5OWEwNTciLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiI2ZDA1OTUwOS04ZmIyLTQ5NGYtYWQ2OS03M2IzNDNhNGYyYzgiLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsib2ZmbGluZV9hY2Nlc3MiLCJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlcG9ydGl1bSI6eyJyb2xlcyI6WyJleGVjdXRpb25fYWRtaW4iXX0sImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIG9mZmxpbmVfYWNjZXNzIn0.RFzFUBFn6hil4-f5mpvcXNmgp9GNf8luWgYHke9PNNk";
            DesiredCapabilities capabilities = new DesiredCapabilities("", "", Platform.CurrentPlatform);
            capabilities.SetCapability("securityToken", securityToken);
            capabilities.SetCapability("deviceName", "HT81Z1A00865");
            capabilities.SetCapability("user", "moshe.milman@applitools.com");
            capabilities.SetCapability("waitForAvailableLicense", true);
            Uri remoteAddress = new Uri("https://mobilecloud.perfectomobile.com/nexperience/perfectomobile/wd/hub");
            RemoteWebDriver driver = new RemoteWebDriver(remoteAddress, capabilities);

            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            eyes.CutProvider = new UnscaledFixedCutProvider(210, 123, 0, 0);
            TestUtils.SetupLogging(eyes);
            eyes.MatchTimeout = TimeSpan.FromSeconds(0);
            try
            {
                eyes.Open(driver, "Humana", "medicare");

                driver.Url = "https://www.humana.com/medicare";
                driver.ExecuteScript(@"var e = document.querySelector('.phone-widget-wrapper.phone-widget-expanded');
e.parentElement.removeChild(e);
document.querySelector('div.copy-container > div > ol > li:nth-child(3) > a').style.overflowWrap = 'break-word';");
                eyes.Check(Target.Window().Fully());

                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }
        [Test]
        public void Trello_1723_Humana_medicare_on_IE11()
        {
            IWebDriver driver = new InternetExplorerDriver();
            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            eyes.MatchTimeout = TimeSpan.FromSeconds(0);
            try
            {
                eyes.Open(driver, "Humana", "medicare", new Size(1200, 800));
                eyes.StitchMode = StitchModes.CSS;
                driver.Url = "https://www.humana.com/medicare";
                //                driver.ExecuteScript(@"var e = document.querySelector('.phone-widget-wrapper.phone-widget-expanded');
                //e.parentElement.removeChild(e);
                //document.querySelector('div.copy-container > div > ol > li:nth-child(3) > a').style.overflowWrap = 'break-word';");
                eyes.Check(Target.Window().Fully());

                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Ticket_1586_GovIL()
        {
            ClassicRunner runner = new ClassicRunner();
            var driver = new ChromeDriver();
            var eyes = new Eyes(runner);
            eyes.SetLogHandler(new StdoutLogHandler(true));
            try
            {
                eyes.Open(driver, "GovIL", "Full page test", new Size(1000, 600));

                driver.Url = "https://govforms.gov.il/mw/forms/CommitteesReport%40health.gov.il";
                eyes.HideScrollbars = false;
                driver.ExecuteScript("document.documentElement.style.overflow='hidden'; document.documentElement.style.transform='translate(0px, 0px)';document.body.style.overflow='visible';");
                eyes.Check("step name", Target.Window().Fully());

                eyes.CloseAsync();
            }
            finally
            {
                driver.Quit();
                runner.GetAllTestResults();
            }
        }

        [Test]
        public void Ticket_1561_GovIL()
        {
            var driver = new ChromeDriver();
            var eyes = new Eyes();
            eyes.SetLogHandler(new StdoutLogHandler());
            try
            {
                eyes.Open(driver, "GovIL", "#31542", new Size(1680, 895));
                driver.Url = "https://www.gov.il/he/departments/policies/26122019";
                eyes.Check("Step 1", Target.Region(By.CssSelector("#accordion-files")));
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Taxcom()
        {
            ChromeOptions options = new ChromeOptions();
            options.EnableMobileEmulation("iPhone 6");
            var driver = new ChromeDriver(options);

            var eyes = new Eyes();
            eyes.SetLogHandler(new StdoutLogHandler(true));
            //eyes.StitchMode = StitchModes.CSS;
            eyes.MatchTimeout = TimeSpan.FromSeconds(0);
            eyes.StitchMode = StitchModes.Scroll;
            //eyes.SendDom = false;

            driver.Url = "https://taxcom.ru/centr/";

            try
            {
                eyes.WaitBeforeScreenshots = 600;
                var eyesDriver = eyes.Open(driver, "Taxcom", "#29531");
                driver.ExecuteScript("document.querySelector('a.scrollToTop').style.visibility='hidden'");
                eyes.Check(Target.Window().Fully());
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }


        [TestCase(StitchModes.CSS)]
        [TestCase(StitchModes.Scroll)]
        public void Trello_1668_Humana(StitchModes stitchMode)
        {
            var driver = new ChromeDriver();
            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes, nameof(Trello_1668_Humana) + $" ({stitchMode})");
            driver.Url = "file:///C:/Users/USER/Websites/Humana/PaymentDetails/humana_payment_details.html";
            //driver.Url = "https://billing.humana.com/PayNow";
            try
            {
                eyes.StitchMode = stitchMode;
                eyes.Batch = new BatchInfo();
                eyes.Open(driver, "humana", $"pay now ({stitchMode})", new Size(1300, 600));
                driver.FindElement(By.Id("accountNumber")).SendKeys("41221821");
                eyes.AddProperty("Stitch Mode", eyes.StitchMode.ToString());
                eyes.Check(Target.Window().Fully()
                    //.ScrollRootElement(By.TagName("body"))
                    );

                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_1951_Humana()
        {
            var driver = new ChromeDriver();
            var eyes = new Eyes();
            eyes.SendDom = false;
            TestUtils.SetupLogging(eyes);
            driver.Url = "https://www.humana.com/provider/news/health-research-library";
            try
            {
                eyes.Batch = new BatchInfo();
                eyes.Open(driver, "humana", "health research library", new Size(1200, 800));
                IWebElement firstIFrame = driver.FindElement(By.XPath("HTML[1]/BODY[1]/IFRAME[1]"));
                //eyes.Check(Target.Window());
                eyes.Check(Target.Window().Fully());
                //eyes.Check(Target.Region(firstIFrame));
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_1676_LoopNet_32073()
        {
            var driver = new ChromeDriver();
            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            try
            {
                eyes.Open(driver, "Ticket32073", "region", new Size(1200, 500));
                driver.Url = "https://www.loopnet.com";

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#top > section > header > nav > ul > li:nth-child(1) > a"))).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#login > section > div > div.column-06.divide-east > div.field-group.margin-top-lg > input"))).SendKeys("testuserforapplitools@mailinator.com");
                driver.FindElement(By.CssSelector("#login > section > div > div.column-06.divide-east > div:nth-child(5) > input")).SendKeys("Loopnet123");
                driver.FindElement(By.CssSelector("#login > section > div > div.column-06.divide-east > div:nth-child(7) > div > button")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#top > section > header > nav > ul > li:nth-child(1) > div > button")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".master > .main-header #csgp-menu-icon"))).Click();
                //wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ListingManager_LN_Lister_enUS > a"))).Click();
                //menuIcon.Click();

                //eyes.Check(Target.Region(By.ClassName("top-menu")));
                eyes.Check(Target.Region(By.Id("csgp-nav-menu-options")).Fully().WithName("side menu"));
                eyes.Close(false);
            }
            finally
            {
                driver.Quit();
                eyes.Abort();
            }
        }

        [Test]
        public void Trello_1722_LampsPlus()
        {
            Eyes eyes = new Eyes();
            eyes.StitchMode = StitchModes.CSS;
            TestUtils.SetupLogging(eyes);

            IWebDriver driver = new ChromeDriver();

            try
            {
                eyes.Open(driver, "LampsPlus", "Bug", new Size(800, 600));

                driver.Url = "https://www.lampsplus.com/products/juno-close-up-3-light-white-par30-floating-canopy-track-kit__68e68.html";

                // Visual checkpoint #1 - Check the login page.
                //eyes.CheckWindow("Login Page");
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
                var element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("pdAddToCart")));

                // This will create a test with two test steps.
                driver.FindElement(By.Id("pdAddToCart")).Click();

                element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cartPromotionalCode")));
                driver.FindElement(By.Id("cartPromotionalCode")).Click();
                //element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("promoInput")));

                eyes.Check("Window", Target.Window().Fully().Ignore(By.Id("cartId")));
                eyes.Check("Region", Target.Region(By.Id("cartOverview")).Fully().Ignore(By.Id("cartId")));

                // End the test.
                eyes.Close(false);
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }

        [TestCase("3.6.0")]
        [TestCase("3.141.0")]
        [TestCase("3.141.59")]
        public void IEDriverTest(string driverVersion)
        {
            var runner = new ClassicRunner();
            var driver = new InternetExplorerDriver(@"C:\Users\USER\Downloads\IEDrivers\" + driverVersion);

            var eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);

            try
            {
                eyes.Open(driver, "IE", "IEDriverServer version " + driverVersion);
                driver.Url = "https://applitools.com";
                eyes.Check(Target.Window());
                eyes.Close();
            }
            finally
            {
                runner.GetAllTestResults();
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void IEDriverTest_SauceLabs()
        {
            var sauceOptions = new Dictionary<string, object>();
            sauceOptions.Add("iedriverVersion", "3.141.0");
            sauceOptions.Add("username", TestDataProvider.SAUCE_USERNAME);
            sauceOptions.Add("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);
            sauceOptions.Add("name", nameof(IEDriverTest_SauceLabs));
            var options = new InternetExplorerOptions();
            options.PlatformName = "Windows 10";
            options.BrowserVersion = "latest";
            options.AddAdditionalCapability("sauce:options", sauceOptions, true);

            RemoteWebDriver driver = new RemoteWebDriver(new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options.ToCapabilities(), TimeSpan.FromMinutes(3));

            var eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            eyes.SendDom = false;
            try
            {
                eyes.Open(driver, "IE", "IEDriverServer version 3.141.0 (SauceLabs)");
                driver.Url = "https://applitools.com";
                eyes.Check(Target.Window());
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void TestViewportOnPixel2()
        {
            Eyes eyes = null;
            IWebDriver webDriver = null;
            try
            {
                //ChromeMobileEmulationDeviceSettings mobileSettings = new ChromeMobileEmulationDeviceSettings()
                //{
                //    UserAgent = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.129 Mobile Safari/537.36",
                //    Width = 411,
                //    Height = 731,
                //    PixelRatio = 2.65
                //};

                ChromeOptions chromeOptions = new ChromeOptions();
                //chromeOptions.EnableMobileEmulation(mobileSettings);
                chromeOptions.EnableMobileEmulation("Pixel 2");
                webDriver = SeleniumUtils.CreateChromeDriver(chromeOptions);

                eyes = new Eyes();
                Configuration configuration = eyes.GetConfiguration();
                configuration.SetAppName("TestMobileEmulation").SetTestName(nameof(TestViewportOnPixel2));
                eyes.SetConfiguration(configuration);
                eyes.Open(webDriver);

                webDriver.Url = "https://applitools.github.io/demo/MobileEmulation/font.html?type=zilla-slab";

                eyes.Check(Target.Window());
                eyes.Close();
            }
            finally
            {
                eyes?.AbortIfNotClosed();
                webDriver?.Quit();
            }
        }

        [Test]
        public void Trello_1661_Garmin_31839()
        {
            EyesRunner runner = new ClassicRunner();

            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            try
            {

                eyes.Open(driver, "Ticket31839", "Test", new Size(1200, 700));
                driver.Url = "https://www.garmin.com/en-US/account/create/";
                driver.FindElement(By.Id("emailMatch")).SendKeys("someemail@mailinator.com");
                eyes.Check("TAG NAME", Target.Region(By.CssSelector("#content > section > div")).Fully()
                    //.ScrollRootElement(By.TagName("body"))
                    );
                eyes.Close();

            }
            finally
            {
                driver.Close();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_988_Vitality()
        {
            string name = "Trello 988 Vitality";
            Eyes eyes = new Eyes();
            eyes.HideScrollbars = false;
            TestUtils.SetupLogging(eyes);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            try
            {
                eyes.Open(driver, name, "Test", new Size(1200, 700));
                driver.Url = "https://www-stacq.on2pp5.co.uk/health-insurance/call-back/";
                driver.ExecuteJavaScript("document.body.style.overflow='hidden'; document.body.style.transform='translate(0,0)';");
                eyes.Check(name, Target.Window().Fully());//.ScrollRootElement(By.TagName("body")));
                eyes.Close();
            }
            finally
            {
                driver.Close();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void Trello_1846_Duda_32648()
        {
            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            eyes.SendDom = false;
            eyes.MatchTimeout = TimeSpan.Zero;
            eyes.ForceFullPageScreenshot = true;

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                IWebDriver innerDriver = eyes.Open(driver, "Duda", "Duda Issue 3", new Size(937, 904));
                innerDriver.Url = "https://editor-sandbox.duda.co/login";
                innerDriver.FindElement(By.CssSelector("#j_username")).SendKeys("support@applitools.com");
                innerDriver.FindElement(By.CssSelector("body > div.signupBodyInner > div.signupBox > div > form > div:nth-child(2) > input[type=password]")).SendKeys("qwer7890");
                innerDriver.FindElement(By.CssSelector("body > div.signupBodyInner > div.signupBox > div > form > input")).Click();
                Thread.Sleep(1000);
                innerDriver.FindElement(By.CssSelector("div._draggable.freestylePopupBody.ui-draggable > div > div > div > span > span > svg")).Click();
                innerDriver.SwitchTo().Frame(0);
                innerDriver.FindElement(By.CssSelector("#trial-action")).Click();
                Thread.Sleep(1000);
                innerDriver.SwitchTo().DefaultContent();
                //By frame =  By.ClassName("OldDashboard-iframe-2bj");
                innerDriver.SwitchTo().Frame(0);
                By scrollMain = By.CssSelector("div.scrollable--main");
                By scrollInner = By.CssSelector("div.scrollableInner");
                By region = By.CssSelector("#accountPlanPopupWrapper > div > div > div > div.accountPlanContent > div > div > div");
                IWebElement scrollMainElem = driver.FindElement(scrollMain);
                IWebElement scrollInnerElem = driver.FindElement(scrollInner);
                IWebElement regionElem = driver.FindElement(region);
                eyes.Check("frame fully ", Target.Region(scrollInner).ScrollRootElement(scrollInner).Fully());
                innerDriver.SwitchTo().DefaultContent();
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }
    }
}

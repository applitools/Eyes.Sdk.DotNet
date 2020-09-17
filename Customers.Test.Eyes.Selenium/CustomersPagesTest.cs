using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CustomersPagesTest : TestSetup
    {
        public CustomersPagesTest() : base(nameof(CustomersPagesTest), new ChromeOptions(), false)
        {
            testedPageSize = new Size(1200, 800);
        }

        [Test]
        public void TestOrbis()
        {
            TestedPageUrl = "https://www.orbis.com/jp/institutional/about-us/press-room";
            GetEyes().Check("Orbis Full Window", Target.Window().Fully().Layout(By.CssSelector("div[test-id=press-articles-0] > progressive-display > div")));
        }

        [Test]
        public void TestNicorette()
        {
            TestedPageUrl = "https://www.nicorette.es/productos";
            GetEyes().Check("Nicorette Full Window", Target.Window().Fully().Layout(By.CssSelector(".view-products > .view-content")));
        }

        [Test]
        public void TestIngBank()
        {
            TestedPageUrl = "file:///C:/temp/Secure%20Banking%20-%20ING.html";
            GetEyes().CheckWindow("test1");
            GetEyes().Check("test3", Target.Window().Fully());
        }

        [Test]
        public void TestAwwwards()
        {
            TestedPageUrl = "https://www.awwwards.com/websites/single-page/";
            //GetEyes().StitchMode = StitchModes.Scroll;
            int originalWait = GetEyes().WaitBeforeScreenshots;
            GetEyes().WaitBeforeScreenshots = 1000;
            GetDriver().FindElement(By.ClassName("cc-close")).Click();
            GetEyes().Check("Awwwards", Target.Region(By.CssSelector("#content")).Fully().Ignore(By.CssSelector(".box-photo")).Layout());
            GetEyes().WaitBeforeScreenshots = originalWait;
        }

        [Test]
        public void TestAmazonSpecialPage()
        {
            TestedPageUrl = "https://www.amazon.com/stream/cd7be774-51ef-4dfe-8e97-1fdec7357113/ref=strm_theme_kitchen?asCursor=WyIxLjgiLCJ0czEiLCIxNTM1NTQyMjAwMDAwIiwiIiwiUzAwMTc6MDpudWxsIiwiUzAwMTc6MjoxIiwiUzAwMTc6MDotMSIsIiIsIiIsIjAiLCJzdWI0IiwiMTUzNTU5NDQwMDAwMCIsImhmMS1zYXZlcyIsIjE1MzU2MDE2MDAwMDAiLCJ2MSIsIjE1MzU2MDUyMDQyMzgiLCIiLCIwIiwidjEiLCIxNTM1NTg1NDAwMDAwIl0%3D&asCacheIndex=0&asYOffset=-321&asMod=1";
            GetEyes().SendDom = true;
            GetEyes().Check("Amazon", Target.Window().Fully());
            GetEyes().SendDom = false;
        }

        [Test]
        public void TestAmazonSpecialPage_2()
        {
            TestedPageUrl = "https://www.amazon.com/b/ref=s9_acss_bw_tt_x_fhp2b_w?node=7147443011&pf_rd_m=ATVPDKIKX0DER&pf_rd_s=merchandised-search-3&pf_rd_r=JPH9TXMED80YR5WFWRT2&pf_rd_t=101&pf_rd_p=ad09ad93-017c-41bc-a874-2779beb5f1e9&pf_rd_i=7141123011";
            GetEyes().Check("Amazon", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestApplitoolsHomePage()
        {
            TestedPageUrl = "https://www.applitools.com";
            GetEyes().Check("Applitools Home Page", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestBookingSpecialPage()
        {
            TestedPageUrl = "https://www.booking.com/city/us/new-york.en-gb.html?label=gen173nr-1FCAEoggJCAlhYSDNYBGhqiAEBmAEuwgEKd2luZG93cyAxMMgBDNgBAegBAfgBC5ICAXmoAgM;sid=ce4701a88873eed9fbb22893b9c6eae4;city=20088325;from_idr=1;lp_index2sr=1;ilp=1";
            GetEyes().StitchMode = StitchModes.Scroll;
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("document.querySelector('div.sb-searchbox-sticky').style.display='none';");
            GetEyes().Check("Booking", Target.Window().ScrollRootElement(By.TagName("html"))
                .Ignore(By.CssSelector(".static_map"), By.CssSelector(".bui-card__image-container"))
                .Layout(By.CssSelector(".bui-card__content"))
                .Fully().SendDom(false));
        }

        [Test]
        public void TestStaplesSpecialPage()
        {
            TestedPageUrl = "https://www.staples.com/Staples-Manila-File-Folders-Letter-3-Tab-Assorted-Position-100-Box/product_116657";
            GetEyes().Check("Staples", Target.Window().ScrollRootElement(By.TagName("html"))
                .Layout(
                    By.CssSelector(".product-details > a"),
                    By.CssSelector(".product-details > .product-reviews"),
                    By.Id("TurnToReviewsContent")
                )
                .Ignore(
                    By.CssSelector("iframe[data-google-container-id]")
                )
                .Fully().SendDom());
        }

        [Test]
        public void TestGamesWorkshopSpecialPage()
        {
            TestedPageUrl = "https://www.games-workshop.com/en-US/The-Beast-Arises-Omnibus-1-2018";
            GetEyes().SendDom = true;
            GetEyes().Check("Games Workshop", Target.Window().ScrollRootElement(By.TagName("html")).Fully());
            GetEyes().SendDom = false;
        }

        [Test]
        public void TestDomCapture()
        {
            TestedPageUrl = "https://applitools.github.io/demo/TestPages/DomTest/dom_capture.html";
            GetEyes().Check("DOM Capture Test", Target.Window().ScrollRootElement(By.TagName("html")).Fully().SendDom());
        }

        [Test]
        public void TestHomeNetAuto()
        {
            TestedPageUrl = "https://www-integration01-iol.homenetauto.com/login";
            GetEyes().Check("Home Net Auto", Target.Window().Fully().ScrollRootElement(By.TagName("body")).SendDom());
        }

        [Test]
        public void TestVince()
        {
            TestedPageUrl = "https://vince-www.coxautoinv-np.com/";
            WebDriverWait wait = new WebDriverWait(GetDriver(), new TimeSpan(0, 0, 30));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.LinkText("Login"))).Click();
            GetDriver().FindElement(By.CssSelector("input[name=username]")).SendKeys("uiauto");
            GetDriver().FindElement(By.CssSelector("input[name=password]")).SendKeys("password2");
            GetDriver().FindElement(By.CssSelector("input[name=login]")).Click();
            GetDriver().Url = "https://vince-www.coxautoinv-np.com/better-fake-vdp/RlVTSU9OOkRFQUxFUjoxMTY5ODI0/e94818a6-4ea9-4e3e-865e-3ac9c5a79fe6";
            IWebElement costFeesFrame = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("route-read-only-cost-fees")));
            GetDriver().SwitchTo().Frame(costFeesFrame);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("edit-costs-fees-button"))).Click();

            GetDriver().SwitchTo().DefaultContent();
            IWebElement costFeesEditFrame = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("route-edit-cost-fees")));
            GetDriver().SwitchTo().Frame(costFeesEditFrame);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cost-fees")));

            GetDriver().SwitchTo().DefaultContent();
            GetEyes().StitchMode = StitchModes.CSS;
            //GetEyes().Check("Vince Costs and Fees Edit Modal", Target.Region(By.ClassName("modal-content")).Fully().SendDom(false));
            GetEyes().Check("Vince Costs and Fees Edit Modal", Target.Region(By.Id("vince-editor")).Fully().SendDom(false));
        }

        [Test]
        public void TestIonicFramework()
        {
            TestedPageUrl = "https://ionicframework.com/docs/api/infinite-scroll/";
            IWebElement element = GetDriver().FindElement(By.CssSelector("body > docs-root > stencil-router > docs-menu > stencil-route-switch"));
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("arguments[0].scrollTop = 0;", element);
            GetEyes().Check("Ionic Framework Docs Navigation", Target.Region(element).Fully());
        }

        [Test]
        public void TestW3Schools()
        {
            TestedPageUrl = "https://www.w3schools.com/html";
            GetEyes().Check("W3Schools HTML", Target.Window().Fully());
        }

        [Test]
        public void TestW3Schools_CssSideMenu()
        {
            TestedPageUrl = "https://www.w3schools.com/cssreF/pr_pos_overflow.asp";
            testedPageSize = new Size(1000, 800);
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("var menu = document.querySelector('#leftmenuinnerinner'); menu.scrollTop=0;");
            GetEyes().Check("CSS Side Menu", Target.Region(By.CssSelector("#leftmenuinnerinner")).Fully());
        }

        [Test]
        public void TestServiceTitan()
        {
            TestedPageUrl = "https://qa.servicetitan.com/";
            testedPageSize = new Size(1000, 700);

            GetDriver().FindElement(By.CssSelector("#username")).SendKeys("applitools");
            GetDriver().FindElement(By.CssSelector("#password")).SendKeys("1234");

            GetDriver().FindElement(By.CssSelector("#loginButton")).Click();

            WebDriverWait wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#navbar > div.ui.inverted.menu.extra-navigation > a:nth-child(4) > i"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#settingsMenu > button"))).Click();

            GetEyes().Check("ServiceTitan", Target.Region(By.CssSelector("#settingsMenu")).Fully());
        }

        [Test]
        public void TestServiceTitanLogin()
        {
            TestedPageUrl = "https://qa.servicetitan.com/Auth/Login";
            testedPageSize = new Size(1000, 800);

            GetEyes().SendDom = true;
            GetDriver().FindElement(By.CssSelector("input[name=username]")).SendKeys("applitools");
            GetDriver().FindElement(By.CssSelector("input[name=password]")).SendKeys("Atools1234");
            WebDriverWait wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type=submit]"))).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("global-loading")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("global-loading")));

            GetEyes().Check("step1", Target.Window().Fully());
        }

        [Test]
        public void TestBankOfAmerica()
        {
            TestedPageUrl = "https://cashproonline.bankofamerica.com/AuthenticationFrameworkWeb/cpo/login/public/loginMain.faces";
            testedPageSize = new Size(1200, 800);
            GetEyes().Check("Bank of America", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestLampsPlus()
        {
            TestedPageUrl = "https://www.lampsplus.com/products/s_schonbek/page_4/";
            testedPageSize = new Size(1200, 800);
            GetEyes().StitchMode = StitchModes.CSS;
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("var i=0; var h = document.documentElement.scrollHeight; var step = document.documentElement.clientHeight;" +
            "var intervalId = 0; intervalId = window.setInterval(function(){ i += step; window.scrollTo(0, i); if (i > h) {" +
            "window.clearInterval(intervalId);" +
            "window.scrollTo(0,0);" +
            "}}, 400); ");
            GetEyes().Check("Lamps Plus", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestNYTimes()
        {
            TestedPageUrl = "https://www.nytimes.com/";
            testedPageSize = new Size(1200, 800);
            GetEyes().StitchMode = StitchModes.CSS;
            IWebElement app = GetDriver().FindElement(By.Id("app"));
            for (int i = 0; i < app.Size.Height; i += 700)
            {
                ((IJavaScriptExecutor)GetDriver()).ExecuteScript($"var app = document.querySelector('#app'); app.style.transform = 'translate(0px, -{i}px)';");
            }
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("var app = document.querySelector('#app'); app.style.transform = 'translate(0px, 0px)';");
            GetEyes().Check("NY Times", Target.Window().ScrollRootElement(By.Id("app")).Fully().SendDom(false));
        }

        [Test]
        public void TestSarinePartialScroll()
        {
            TestedPageUrl = "http://api.sarine.com/viewer/v1/9NUKVEJ568E/NWPZMUVJVRX";
            GetEyes().HideScrollbars = false;
            GetDriver().FindElement(By.Id("menu-item-carousel")).Click();
            WebDriverWait wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(30));
            IWebElement frame = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#stl > iframe")));
            EyesRemoteWebElement eyesFrame = new EyesRemoteWebElement(GetEyes().Logger, (EyesWebDriver)GetDriver(), frame);
            Rectangle region = eyesFrame.GetClientBounds();
            Thread.Sleep(1000);
            GetEyes().Check(Target.Window().WithName("sarine partial scroll").Fully(false).Ignore(region));
        }

        [Test]
        public void TestSarineLanguageSelect()
        {
            Logger logger = GetEyes().Logger;

            TestedPageUrl = "https://api.sarine.com/viewer/v1/1UH9WF1D3B/KELAJWM5O2";
            try
            {
                WebDriverWait wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(30));

                By langBtnSelector = By.CssSelector("div#languageBar b");
                wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();

                int langs = GetDriver().FindElements(By.CssSelector("div.selectric-items li")).Count;
                logger.Verbose("found {0} language buttons", langs);

                for (int i = 0; i < langs; ++i)
                {
                    logger.Verbose("2.1");
                    if (!GetDriver().FindElement(By.ClassName("selectric-items")).Displayed)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();
                    }

                    IWebElement lang = wait.Until(ExpectedConditions.ElementExists(By.CssSelector($"div.selectric-items li[data-index='{i}']")));
                    string langText = lang.Text;

                    logger.Verbose("2.2");
                    wait.Until(ExpectedConditions.ElementToBeClickable(lang)).Click();

                    logger.Verbose("2.3");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".viewer.loupeRealView")));

                    logger.Verbose("2.4");
                    var languagesMenu = GetDriver().FindElements(By.ClassName("selectric-items"));
                    if (languagesMenu != null && languagesMenu.Count > 0 && languagesMenu[0].Displayed)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();
                    }
                    logger.Verbose("2.5");
                    GetEyes().Check(langText, Target.Window().SendDom(false)
                        .Ignore(
                            By.CssSelector(".viewer.loupeRealView"),
                            By.CssSelector("[data-sarine-report=\"updated::MM-DD-YYYY\"]")));
                }
            }
            catch (Exception e)
            {
                logger.Log("Error: " + e);
            }
        }

        [Test]
        public void TestApplitoolsHompage()
        {
            TestedPageUrl = "https://applitools.com";
            GetEyes().StitchMode = StitchModes.CSS;
            GetEyes().Check("Applitools Full Window CSS", Target.Window().Fully());
        }
    }
}

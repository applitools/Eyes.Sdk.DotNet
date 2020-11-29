using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    public class SeleniumTest
    {
        [Test]
        public void TestSelenium()
        {
            var driver = new ChromeDriver();
            driver.Url = "data:text/html,<div style='height:100px;overflow:scroll' id='el'><div style='padding-top:10px;height:200px;'>aaa</div></div><script>document.documentElement.style.overflow = 'hidden'; el.scrollTop = 20;</script>";
            IWebElement el = driver.FindElement(By.CssSelector("div>div"));
            //driver.Url = "https://www.awwwards.com/websites/single-page/";
            //driver.ExecuteScript("")
            //IWebElement el = driver.FindElement(By.CssSelector("#content"));
            Console.Out.WriteLine(el.Location);
            driver.Quit();
        }

        [Test]
        public void TestSarineLanguageSelect()
        {
            var driver = new ChromeDriver();
            driver.Url = "https://api.sarine.com/viewer/v1/1UH9WF1D3B/KELAJWM5O2";
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                By langBtnSelector = By.CssSelector("div#languageBar b");
                wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();

                int langs = driver.FindElements(By.CssSelector("div.selectric-items li")).Count;
                string logPath = TestUtils.InitLogPath();
                for (int i = 0; i < langs; ++i)
                {
                    if (!driver.FindElement(By.ClassName("selectric-items")).Displayed)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();
                    }

                    IWebElement lang = wait.Until(ExpectedConditions.ElementExists(By.CssSelector($"div.selectric-items li[data-index='{i}']")));
                    string langText = lang.Text;

                    wait.Until(ExpectedConditions.ElementToBeClickable(lang)).Click();

                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".viewer.loupeRealView")));

                    var languagesMenu = driver.FindElements(By.ClassName("selectric-items"));
                    if (languagesMenu != null && languagesMenu.Count > 0 && languagesMenu[0].Displayed)
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(langBtnSelector)).Click();
                    }
                    Thread.Sleep(2000);
                    var screenshot = driver.GetScreenshot();
                    screenshot.SaveAsFile(Path.Combine(logPath, $"screenshot_{langText}.png"));
                }
            }
            finally
            {
                driver.Quit();
            }
        }

        [Test]
        public void Attentia()
        {
            // Open a Chrome browser.
            var driver = new ChromeDriver();

            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            eyes.SendDom = false;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            //Navigate to the URL
            driver.Url = "https://uat-dots.attentia.be";

            try
            {
                //Start the test
                var eyesDriver = eyes.Open(driver, "Attentia", "#29053", new Size(1200, 800));

                WebDriverWait wait = new WebDriverWait(eyesDriver, TimeSpan.FromSeconds(30));
                IWebElement userName = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#userNameInput")));
                eyes.CheckWindow("step 1", true);
                userName.SendKeys("angelinodogan@mailinator.com");
                eyes.CheckWindow("step 2", true);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#passwordInput"))).SendKeys("Azerty_03");
                eyes.CheckWindow("step 3", true);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#submitButton"))).Click();
                eyes.CheckWindow("step 4", true);

                //eyes.Check(Target.Frame("appFrame").Fully());

                //Close the test
                eyes.Close();
            }
            catch (Exception e)
            {
                eyes.Logger.Log("Error: " + e);
            }
            finally
            {
                // Close the browser.
                driver.Quit();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.Abort();
            }
        }

        [Test]
        public void McKinsey()
        {
            var driver = new ChromeDriver();

            var eyes = new Eyes();
            eyes.SendDom = false;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            driver.Url = "https://www.mckinsey.com/";

            try
            {
                IWebDriver eyesDriver = eyes.Open(driver, "McKinsey", "McKinsey");//, new Size(1200, 800));
                var ccDismiss = driver.FindElements(By.CssSelector(".cc-dismiss"));
                if (ccDismiss.Count > 0)
                {
                    ccDismiss[0].Click();
                }

                IList<IWebElement> sections = driver.FindElements(By.CssSelector(".pager>a[data-section]"));
                sections[0].Click();
                Thread.Sleep(300);
                eyes.Check(Target.Window());

                sections[1].Click();
                Thread.Sleep(300);
                eyes.Check(Target.Window());

                sections[2].Click();
                Thread.Sleep(300);
                eyes.Check(Target.Window());

                ((IJavaScriptExecutor)eyesDriver).ExecuteScript(
                    "document.querySelector('#skipToMain').removeChild(document.querySelector('#sectionScroll'));" +
                    "document.querySelector('#form1').removeChild(document.querySelector('header'));" +
                    "document.querySelector('body').style.overflow='auto';");
                eyes.Check(Target.Window().Fully()
                    //.ScrollRootElement(By.TagName("body"))
                    );

                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.Abort();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BeatsByDre_HoverElement(bool bCheckHover)
        {

            string testName = "Hover Element";

            if (bCheckHover == true)
                testName += " - On";
            else
                testName += " - Off";

            Eyes eyes = new Eyes();

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            string logPath = TestUtils.InitLogPath();
            eyes.DebugScreenshotProvider = new FileDebugScreenshotProvider() { Path = logPath };
            try
            {
                IWebDriver eyesDriver = eyes.Open(driver, testName, testName, new Size(1200, 600));

                driver.Url = "https://www.beatsbydre.com/support/headphones/studio3-wireless";

                ((ChromeDriver)driver).ExecuteScript("window.scrollBy(0,400)");

                By selector = By.CssSelector(
                        "#maincontent > div:nth-child(1) > div.supportContent.parbase.section > div > div > div.selector.topics > div.boxes > a:nth-child(1) > div > div.content");
                
                eyes.CheckWindow("Window", true);

                //eyes.StitchMode = StitchModes.CSS;
                // Hover effect
                if (bCheckHover)
                {
                    IWebElement we = eyesDriver.FindElement(selector);

                    Actions action = new Actions(eyesDriver);

                    action.MoveToElement(we).Perform();
                }
                // End

                eyes.Check("Region", Target.Region(selector));

                eyes.Close();

            }
            finally
            {

                eyes.Abort();
                driver.Quit();
            }
        }
    }
}

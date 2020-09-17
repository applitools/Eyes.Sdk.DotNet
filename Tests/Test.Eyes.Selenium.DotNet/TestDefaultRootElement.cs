using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    public class TestDefaultRootElement : ReportingTestSuite
    {
        [Test]
        public void TestCheckDefaultElementBiggerBody()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                driver.Url = "data:text/html,<html style=\"height:100vh\"><body><div style=\"height: 200vh\">Test</div></body></html>";
                IWebElement root = EyesSeleniumUtils.GetDefaultRootElement(driver);
                Assert.AreEqual("body", root.TagName);

                driver.Url = "data:text/html,<!doctype HTML><html style=\"height:100vh\"><body><div style=\"height: 200vh\">Test</div></body></html>";
                root = EyesSeleniumUtils.GetDefaultRootElement(driver);
                Assert.AreEqual("html", root.TagName);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}

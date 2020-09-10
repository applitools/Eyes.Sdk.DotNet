using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    public class TestCodedRegions : ReportingTestSuite
    {
        [Test]
        public void TestSimpleCodedRegions()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Manage().Window.Size = new Size(740, 600);
            webDriver.Url = "data:text/html,<!DOCTYPE html><html><body><header style='height: 50px'>header</header><main style='position: relative;'><section><div style='position: relative; border: 3px blue solid; height: 30px;'>box</div></section></main></body></html>";

            try
            {
                IWebElement headerElement = webDriver.FindElement(By.CssSelector("header"));
                IWebElement mainElement = webDriver.FindElement(By.CssSelector("main"));
                IWebElement boxElement = webDriver.FindElement(By.CssSelector("div"));
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)webDriver;
                Rectangle headerRect = EyesRemoteWebElement.GetVisibleElementRect(headerElement, jsExecutor);
                Rectangle mainRect = EyesRemoteWebElement.GetVisibleElementRect(mainElement, jsExecutor);
                Rectangle boxRect = EyesRemoteWebElement.GetVisibleElementRect(boxElement, jsExecutor);
                Assert.AreEqual(new Rectangle(8, 8, 724, 50), headerRect);
                Assert.AreEqual(new Rectangle(8, 58, 724, 36), mainRect);
                Assert.AreEqual(new Rectangle(8, 58, 724, 36), boxRect);
            }
            finally
            {
                webDriver.Close();
            }
        }
    }
}

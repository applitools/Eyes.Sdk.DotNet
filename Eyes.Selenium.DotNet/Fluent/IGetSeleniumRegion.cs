using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    internal interface IGetSeleniumRegion
    {
        IList<IWebElement> GetElements(IWebDriver driver);
    }
}
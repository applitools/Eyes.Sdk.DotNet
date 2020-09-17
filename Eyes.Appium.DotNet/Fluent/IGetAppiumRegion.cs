using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Appium.Fluent
{
    internal interface IGetAppiumRegion
    {
        IList<IWebElement> GetElements(IWebDriver driver);
    }
}
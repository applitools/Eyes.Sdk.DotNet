using System.Collections.Generic;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal interface IGetAppiumRegion
    {
        IList<IWebElement> GetElements(IWebDriver driver);
    }
}
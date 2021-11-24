using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    internal interface IGetSeleniumRegion<IWebElement, IWebDriver>
    {
        IList<IWebElement> GetElements(IWebDriver driver);
    }
}
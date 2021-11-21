using Applitools.OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium
{
    public interface IEyesWebDriverAdapter
    {
        string GetElementId(Logger logger, IWebElement webElement);

        EyesWebElement WrapElementIfRemote(Logger logger, EyesWebDriver driver, IWebElement elementToWrap);

        void TryAddCommand(string commandName, string method, string resourcePath);

        IDictionary<string, object> GetSessionDetails();
        string GetTagName(Logger logger, IWebElement webElement);
    }
}

using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium
{
    public interface IEyesWebDriverAdapter
    {
        string GetElementId(Logger logger, IWebElement webElement);

        IWebElement WrapElementIfRemote(Logger logger, EyesWebDriver driver, IWebElement elementToWrap);
        void TryAddCommand(string commandName, string method, string resourcePath);
        IDictionary<string, object> GetSessionDetails();
    }
}

using System.Drawing;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
    public interface ISeleniumEyes : IEyes
    {
        IWebDriver Open(IWebDriver webDriver);
        IWebDriver Open(IWebDriver driver, string appName, string testName, Size viewportSize);
        TestResults Close(bool throwEx);
        EyesWebDriver GetDriver();
    }
}
using OpenQA.Selenium;

namespace Applitools.Selenium.Fluent
{
    interface IScrollRootElementContainer
    {
        IWebElement GetScrollRootElement();
        By GetScrollRootSelector();
    }
}

using Applitools.OpenQA.Selenium;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumFrameCheckTarget : IScrollRootElementContainer
    {
        int? GetFrameIndex();
        string GetFrameNameOrId();
        IWebElement GetFrameReference();
        By GetFrameSelector();
    }
}

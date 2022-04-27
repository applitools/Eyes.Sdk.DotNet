using OpenQA.Selenium;

namespace Applitools.Appium
{
    interface IAppiumFrameCheckTarget : IScrollRootElementContainer
    {
        int? GetFrameIndex();
        string GetFrameNameOrId();
        IWebElement GetFrameReference();
        By GetFrameSelector();
    }
}

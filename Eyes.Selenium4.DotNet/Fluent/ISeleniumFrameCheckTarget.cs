namespace Applitools.Selenium.Fluent
{
    using OpenQA.Selenium;

    interface ISeleniumFrameCheckTarget : IScrollRootElementContainer
    {
        int? GetFrameIndex();
        string GetFrameNameOrId();
        IWebElement GetFrameReference();
        By GetFrameSelector();
    }
}

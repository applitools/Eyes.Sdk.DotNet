using OpenQA.Selenium;

namespace Applitools
{
    internal interface IScrollRootElementContainer
    {
        IWebElement GetScrollRootElement();
        By GetScrollRootSelector();
    }

    internal interface ITargetContainer
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();
    }
}

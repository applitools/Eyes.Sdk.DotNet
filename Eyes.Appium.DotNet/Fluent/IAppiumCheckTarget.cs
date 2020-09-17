using OpenQA.Selenium;

namespace Applitools.Appium.Fluent
{
    interface IAppiumCheckTarget //: IScrollRootElementContainer
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();
    }
}
using Applitools.Selenium.Common;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : ISeleniumCheckTargetCommon<IWebDriver, IWebElement>, IScrollRootElementContainer
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();

        IList<FrameLocator> GetFrameChain();
    }
}
using Applitools.Selenium.Common;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : ISeleniumCheckTargetCommon, IScrollRootElementContainer
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();

        IList<FrameLocator> GetFrameChain();

        CheckState State { get; set; }
    }
}
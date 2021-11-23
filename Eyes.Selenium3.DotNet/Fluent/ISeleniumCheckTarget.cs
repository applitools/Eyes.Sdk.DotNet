using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : IScrollRootElementContainer
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();

        IList<FrameLocator> GetFrameChain();

        CheckState State { get; set; }

        List<int> GetLayoutBreakpoints();

        bool GetLayoutBreakpointsEnabled();

        bool? GetUseCookies();
    }
}
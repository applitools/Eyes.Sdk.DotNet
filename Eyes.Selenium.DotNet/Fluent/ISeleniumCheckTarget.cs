using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : ITargetContainer, IScrollRootElementContainer
    {
        IList<FrameLocator> GetFrameChain();

        CheckState State { get; set; }

        List<int> GetLayoutBreakpoints();

        bool? GetLayoutBreakpointsEnabled();

        bool? GetUseCookies();
    }
}
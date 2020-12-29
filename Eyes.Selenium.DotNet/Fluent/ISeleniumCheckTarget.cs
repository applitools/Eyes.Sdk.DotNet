using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : IScrollRootElementContainer//, IImplicitInitialization
    {
        By GetTargetSelector();

        IWebElement GetTargetElement();

        IList<FrameLocator> GetFrameChain();

        CheckState State { get; set; }
    }
}
using System.Collections.Generic;

namespace Applitools.Selenium.Fluent
{
    interface ISeleniumCheckTarget : ITargetContainer, IScrollRootElementContainer
    {
        IList<FrameLocator> GetFrameChain();

        CheckState State { get; set; }

        bool? GetUseCookies();
    }
}
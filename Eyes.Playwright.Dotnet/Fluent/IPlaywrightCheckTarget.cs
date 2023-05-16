using System.Collections.Generic;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;

namespace Applitools.Playwright.Fluent
{
    public interface IPlaywrightCheckTarget
    {
        IList<FrameLocator> GetFrameChain();
        Reference GetScrollRootElement();
    }
}
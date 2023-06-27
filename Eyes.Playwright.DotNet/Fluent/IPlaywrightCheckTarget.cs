using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;

namespace Applitools.Playwright.Fluent
{
    public interface IPlaywrightCheckTarget : ICheckSettingsInternal
    {
        IList<FrameLocator> GetFrameChain();
        Reference GetScrollRootElement();
        Reference GetTargetElement();
    }
}
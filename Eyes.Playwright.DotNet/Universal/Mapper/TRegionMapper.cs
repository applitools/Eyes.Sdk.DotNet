using Applitools.Fluent;
using Applitools.Playwright.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Mapper
{
    public class TRegionMapper
    {
        // public static TRegion ToTRegionFromCheckSettings(ICheckSettings checkSettings, Refer refer, Reference root)
        // {
        //     if (!(checkSettings is PlaywrightCheckSettings playwrightCheckSettings))
        //     {
        //         return null;
        //     }
        //
        //     Reference targetElement = playwrightCheckSettings.TargetElement;
        //     if (targetElement != null)
        //     {
        //         if (targetElement is Element element)
        //         {
        //             IElementHandle elementHandle = element.ElementHandle;
        //             element.ApplitoolsRefId = refer.Ref(elementHandle, root);
        //         }
        //         else if (targetElement is Selector selector)
        //         {
        //             if (selector.Locator != null)
        //             {
        //                 ILocator locator = selector.Locator;
        //                 selector.ApplitoolsRefId = refer.Ref(locator, root);
        //             }
        //         }
        //
        //         return targetElement;
        //     }
        //
        //     Region region = playwrightCheckSettings.TargetRegion;
        //     if (region != null)
        //     {
        //         return RectangleRegionMapper.toRectangleRegionDto(region);
        //     }
        //
        //     return null;
        // }

        public static TRegion ToTRegionDtoFromSRE(Reference scrollRootElement, Refer refer, Reference root)
        {
            if (scrollRootElement == null)
            {
                return null;
            }

            if (scrollRootElement is Element element)
            {
                IElementHandle elementHandle = element.ElementHandle;
                scrollRootElement.ApplitoolsRefId = refer.Ref(elementHandle, root);
            }
            else if (scrollRootElement is Selector selector)
            {
                if (selector.Locator != null)
                {
                    ILocator locator = selector.Locator;
                    scrollRootElement.ApplitoolsRefId = refer.Ref (locator, root);
                }
            }
            return scrollRootElement;
        }
    }
}
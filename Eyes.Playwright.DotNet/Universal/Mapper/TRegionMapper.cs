using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Mapper
{
    public static class TRegionMapper
    {
        public static TRegion ToTRegionDtoFromSre(Reference scrollRootElement, Refer refer, Reference root)
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
using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    /// <summary>
    /// Used internally to represent a ref of an accessibility element
    /// </summary>
    public class AccessibilityRegionBySelector : Selector, IGetAccessibilityRegion, IGetAccessibilityRegionType
    {
        public AccessibilityRegionType AccessibilityRegionType { get; }

        public AccessibilityRegionBySelector(string selector, AccessibilityRegionType type)
            : base(selector)
        {
            AccessibilityRegionType = type;
        }

        public AccessibilityRegionBySelector(ILocator locator, AccessibilityRegionType type) : base(locator)
        {
            AccessibilityRegionType = type;
        }

        public TAccessibilityRegion ToRegion()
        {
            return new ElementAccessibilityRegion
            {
                Type = AccessibilityRegionType,
                Region = new RegionElement
                {
                    //ElementId = base.ElementHandle
                }
            };
        }
    }
}
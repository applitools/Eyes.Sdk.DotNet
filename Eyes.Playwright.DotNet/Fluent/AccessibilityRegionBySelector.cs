using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Microsoft.Playwright;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright.Fluent
{
    /// <summary>
    /// Used internally to represent a ref of an accessibility element
    /// </summary>
    public class AccessibilityRegionBySelector : Selector, IGetAccessibilityRegion, IGetAccessibilityRegionType, IPlaywrightReference<TAccessibilityRegion>
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

        TAccessibilityRegion IGetAccessibilityRegion.ToRegion()
        {
            return new SelectorAccessibilityRegion()
            {
                Type = AccessibilityRegionType,
                Region = new RegionSelector
                {
                    //ElementId = base.ElementHandle
                }
            };
        }

        TAccessibilityRegion IPlaywrightReference<TAccessibilityRegion>.ToRegion(Reference root, Refer refer)
        {
            return new SelectorAccessibilityRegion()
            {
                Type = AccessibilityRegionType,
                Region = GetRegionSelector(root, refer)
            };
        }
    }
}
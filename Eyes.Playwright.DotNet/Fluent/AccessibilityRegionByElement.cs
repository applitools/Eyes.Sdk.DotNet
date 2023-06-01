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
    public class AccessibilityRegionByElement : Element, IGetAccessibilityRegion, IGetAccessibilityRegionType, IPlaywrightReference<TAccessibilityRegion>
    {
        public AccessibilityRegionType AccessibilityRegionType { get; }

        public AccessibilityRegionByElement(IElementHandle element, AccessibilityRegionType type) 
            : base(element)
        {
            AccessibilityRegionType = type;
        }

        public new TAccessibilityRegion ToRegion()
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

        TAccessibilityRegion IPlaywrightReference<TAccessibilityRegion>.ToRegion(Reference root, Refer refer)
        {
            return new ElementAccessibilityRegion
            {
                Type = AccessibilityRegionType,
                Region = new RegionElement
                {
                    ElementId = refer.Ref(ElementHandle, root)
                }
            };
        }
    }
}
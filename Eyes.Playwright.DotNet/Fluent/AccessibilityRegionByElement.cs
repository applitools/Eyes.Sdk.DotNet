using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    /// <summary>
    /// Used internally to represent a ref of an accessibility element
    /// </summary>
    public class AccessibilityRegionByElement : Element, IGetAccessibilityRegion, IGetAccessibilityRegionType
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
    }
}
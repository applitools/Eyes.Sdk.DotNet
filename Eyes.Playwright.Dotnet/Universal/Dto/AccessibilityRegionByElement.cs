using Applitools.Fluent;

namespace Applitools.Playwright.Universal.Dto
{
    public class AccessibilityRegionByElement : TAccessibilityRegion, IGetRegion
    {
        public Element Region { get; set; }
    }
}
using Applitools.Fluent;

namespace Applitools.Playwright.Universal.Dto
{
    public class AccessibilityRegionBySelector : TAccessibilityRegion, IGetRegion
    {
        public Selector Region { get; set; }
    }
}
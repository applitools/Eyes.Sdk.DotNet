using Applitools.Fluent;

namespace Applitools.Playwright.Universal.Dto
{
    public class FloatingRegionBySelector : TFloatingRegion, IGetRegion
    {
        public Selector Region { get; set; }
    }
}
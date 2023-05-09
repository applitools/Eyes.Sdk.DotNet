using Applitools.Fluent;

namespace Applitools.Playwright.Universal.Dto
{
    public class FloatingRegionByElement : TFloatingRegion, IGetRegion
    {
        public Element Region { get; set; }
    }
}
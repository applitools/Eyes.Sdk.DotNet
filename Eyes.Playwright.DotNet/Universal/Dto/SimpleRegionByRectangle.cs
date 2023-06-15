using System.Drawing;
using Applitools.Universal;

namespace Applitools.Playwright.Universal.Dto
{
    public class SimpleRegionByRectangle : Applitools.SimpleRegionByRectangle, IPlaywrightReference<CodedRegionReference>
    {
        public SimpleRegionByRectangle(Rectangle region, Padding padding = null, string regionId = null) : base(region, padding, regionId)
        {
        }

        public SimpleRegionByRectangle(Point location, Size size) : base(location, size)
        {
        }

        CodedRegionReference IPlaywrightReference<CodedRegionReference>.ToRegion(Reference root, Refer refer)
        {
            return base.ToRegion();
        }
    }
}
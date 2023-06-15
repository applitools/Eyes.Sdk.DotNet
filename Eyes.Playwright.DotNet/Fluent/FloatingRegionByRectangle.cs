using System.Drawing;
using Applitools.Universal;

namespace Applitools.Playwright.Universal.Dto
{
    public class FloatingRegionByRectangle : Applitools.FloatingRegionByRectangle, IPlaywrightReference<TFloatingRegion>
    {
        public FloatingRegionByRectangle(
            Rectangle rect, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset) 
            : base(rect, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset)
        {
        }

        public FloatingRegionByRectangle(
            Point location,
            Size size, 
            int maxUpOffset = 0,
            int maxDownOffset = 0,
            int maxLeftOffset = 0,
            int maxRightOffset = 0) 
            : base(location, size, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset)
        {
        }

        TFloatingRegion IPlaywrightReference<TFloatingRegion>.ToRegion(Reference root, Refer refer)
        {
            return base.ToRegion();
        }
    }
}
using System;
using Applitools.Utils.Geometry;
using Applitools.Positioning;

namespace Applitools.Selenium.Positioning
{
    internal class SafariRegionPositionCompensation : IRegionPositionCompensation
    {
        public Region CompensateRegionPosition(Region region, double pixelRatio)
        {

            if (pixelRatio == 1.0)
            {
                return region;
            }

            if (region.Width <= 0 || region.Height <= 0)
            {
                return Region.Empty;
            }

            System.Drawing.Rectangle rect = region.ToRectangle();
            rect.Offset(0, (int)Math.Ceiling(pixelRatio));
            return new Region(rect);
        }
    }
}
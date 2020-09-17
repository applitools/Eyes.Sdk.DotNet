using System;
using Applitools.Utils.Geometry;
using Applitools.Positioning;

namespace Applitools.Selenium.Positioning
{
    internal class InternetExplorerRegionPositionCompensation : IRegionPositionCompensation
    {
        public Region CompensateRegionPosition(Region region, double pixelRatio)
        {
            System.Drawing.Rectangle rect = region.ToRectangle();
            rect.Offset(0, (int)Math.Floor(pixelRatio - 0.0001));
            return new Region(rect);
        }
    }
}
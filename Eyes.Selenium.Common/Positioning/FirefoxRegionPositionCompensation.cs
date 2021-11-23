using System;
using Applitools.Utils.Geometry;
using Applitools.Positioning;
using Applitools.Selenium.Common;

namespace Applitools.Selenium.Positioning
{
    internal class FirefoxRegionPositionCompensation : IRegionPositionCompensation
    {
        private readonly ISeleniumEyesCommon eyes_;
        protected Logger logger_;

        public FirefoxRegionPositionCompensation(ISeleniumEyesCommon eyes,  Logger logger)
        {
            eyes_ = eyes;
            logger_ = logger;
        }

        public Region CompensateRegionPosition(Region region, double pixelRatio)
        {
            if (pixelRatio == 1.0)
            {
                return region;
            }

            if (eyes_.GetFrameChainLength() > 0)
            {
                return region;
            }

            System.Drawing.Rectangle rect = region.ToRectangle();
            rect.Offset(0, -(int)Math.Ceiling(pixelRatio / 2));

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return Region.Empty;
            }

            Region compensatedRegion = new Region(rect);
            logger_.Verbose($"compensating for firefox: {region} ==> {compensatedRegion}");

            return compensatedRegion;
        }
    }
}
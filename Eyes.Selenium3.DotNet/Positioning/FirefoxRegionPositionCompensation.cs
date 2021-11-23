using System;
using Applitools.Utils.Geometry;
using Applitools.Positioning;

namespace Applitools.Selenium.Positioning
{
    internal class FirefoxRegionPositionCompensation : IRegionPositionCompensation
    {
        private SeleniumEyes eyes;
        private Logger logger;

        public FirefoxRegionPositionCompensation(SeleniumEyes eyes, Logger logger)
        {
            this.eyes = eyes;
            this.logger = logger;
        }

        public Region CompensateRegionPosition(Region region, double pixelRatio)
        {
            if (pixelRatio == 1.0)
            {
                return region;
            }

            EyesWebDriver eyesWebDriver = eyes.GetDriver();
            FrameChain frameChain = eyesWebDriver.GetFrameChain();
            if (frameChain.Count > 0)
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
            logger.Verbose($"compensating for firefox: {region} ==> {compensatedRegion}");

            return compensatedRegion;
        }
    }
}
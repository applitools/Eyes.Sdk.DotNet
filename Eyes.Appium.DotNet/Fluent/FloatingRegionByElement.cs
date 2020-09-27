using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium.Fluent
{
    internal class FloatingRegionByElement : IGetFloatingRegion, IGetAppiumRegion, IGetFloatingRegionOffsets
    {
        private int maxDownOffset_;
        private int maxLeftOffset_;
        private int maxRightOffset_;
        private int maxUpOffset_;

        private IWebElement element_;

        public FloatingRegionByElement(IWebElement element, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            element_ = element;

            maxUpOffset_ = maxUpOffset;
            maxDownOffset_ = maxDownOffset;
            maxLeftOffset_ = maxLeftOffset;
            maxRightOffset_ = maxRightOffset;
        }

        int IGetFloatingRegionOffsets.MaxLeftOffset => maxLeftOffset_;

        int IGetFloatingRegionOffsets.MaxUpOffset => maxUpOffset_;

        int IGetFloatingRegionOffsets.MaxRightOffset => maxRightOffset_;

        int IGetFloatingRegionOffsets.MaxDownOffset => maxDownOffset_;

        public IList<FloatingMatchSettings> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            double scaleRatio = ((Eyes)eyesBase).GetScaleRatioForRegions();
            Region r = new Region(element_.Location, element_.Size);
            r = r.Scale(scaleRatio);
            Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            return new FloatingMatchSettings[] {
                new FloatingMatchSettings()
                {
                    Left = pTag.X,
                    Top = pTag.Y,
                    Width = r.Width,
                    Height = r.Height,
                    MaxLeftOffset = maxLeftOffset_,
                    MaxUpOffset = maxUpOffset_,
                    MaxRightOffset = maxRightOffset_,
                    MaxDownOffset = maxDownOffset_
                }
            };
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }


    }
}
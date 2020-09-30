using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium.Fluent
{
    internal class AccessibilityRegionByElement : IGetAccessibilityRegion, IGetAppiumRegion, IGetAccessibilityRegionType
    {
        private readonly AccessibilityRegionType regionType_;
        private IWebElement element_;

        public AccessibilityRegionByElement(IWebElement element, AccessibilityRegionType regionType)
        {
            element_ = element;
            regionType_ = regionType;
        }

        AccessibilityRegionType IGetAccessibilityRegionType.AccessibilityRegionType => regionType_;

        public IList<AccessibilityRegionByRectangle> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            double scaleRatio = ((Eyes)eyesBase).GetScaleRatioForRegions();
            Region r = new Region(element_.Location, element_.Size);
            r = r.Scale(scaleRatio);
            Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            return new AccessibilityRegionByRectangle[] {
                new AccessibilityRegionByRectangle(new Rectangle(pTag, r.Size), regionType_)
            };
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }


    }
}
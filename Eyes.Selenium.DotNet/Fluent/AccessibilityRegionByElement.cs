using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Selenium.Fluent
{
    internal class AccessibilityRegionByElement : IGetAccessibilityRegion, IGetSeleniumRegion, IGetAccessibilityRegionType
    {
        private readonly AccessibilityRegionType regionType_;
        private IWebElement element_;

        public AccessibilityRegionByElement(IWebElement element, AccessibilityRegionType regionType)
        {
            element_ = element;
            regionType_ = regionType;
        }

        AccessibilityRegionType IGetAccessibilityRegionType.AccessibilityRegionType => regionType_;

        public IList<AccessibilityRegionByRectangle> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot)
        {
            Rectangle r = EyesSeleniumUtils.GetVisibleElementBounds(element_);
            Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            return new AccessibilityRegionByRectangle[] {
                new AccessibilityRegionByRectangle(new Rectangle(pTag, r.Size), regionType_)
            };
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }


    }
}
using OpenQA.Selenium;
using System.Drawing;
using Applitools.Utils.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium.Fluent
{
    internal class AccessibilityRegionBySelector : IGetAccessibilityRegion, IGetAppiumRegion, IGetAccessibilityRegionType
    {
        private readonly AccessibilityRegionType regionType_;
        private readonly By selector_;

        public AccessibilityRegionBySelector(By selector, AccessibilityRegionType regionType)
        {
            selector_ = selector;
            regionType_ = regionType;
        }

        AccessibilityRegionType IGetAccessibilityRegionType.AccessibilityRegionType => regionType_;

        public IList<AccessibilityRegionByRectangle> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            ReadOnlyCollection<IWebElement> elements = ((Eyes)eyesBase).GetDriver().FindElements(selector_);
            IList<AccessibilityRegionByRectangle> retVal = new List<AccessibilityRegionByRectangle>();
            double scaleRatio = ((Eyes)eyesBase).GetScaleRatioForRegions();
            foreach (IWebElement element in elements)
            {
                Region r = new Region(element.Location, element.Size);
                r = r.Scale(scaleRatio);
                Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
                retVal.Add(new AccessibilityRegionByRectangle(new Rectangle(pTag, r.Size), regionType_));
            }
            return retVal;
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
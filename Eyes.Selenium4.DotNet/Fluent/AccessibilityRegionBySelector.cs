using OpenQA.Selenium;
using System.Drawing;
using Applitools.Utils.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;

namespace Applitools.Selenium.Fluent
{
    internal class AccessibilityRegionBySelector : IGetAccessibilityRegion, IGetSeleniumRegion, IGetAccessibilityRegionType
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
            ReadOnlyCollection<IWebElement> elements = ((SeleniumEyes)eyesBase).GetDriver().FindElements(selector_);
            IList<AccessibilityRegionByRectangle> retVal = new List<AccessibilityRegionByRectangle>();
            foreach (IWebElement element in elements)
            {
                Rectangle r = EyesSeleniumUtils.GetVisibleElementBounds(element);
                Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
                retVal.Add(new AccessibilityRegionByRectangle(new Rectangle(pTag, r.Size), regionType_));
            }
            return retVal;
        }

        public TAccessibilityRegion ToRegion()
        {
            return new SelectorAccessibilityRegion
            {
                Type = regionType_,
                Region = new RegionSelector
                {
                    Type = selector_.Mechanism,
                    Selector = selector_.Criteria
                }
            };
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
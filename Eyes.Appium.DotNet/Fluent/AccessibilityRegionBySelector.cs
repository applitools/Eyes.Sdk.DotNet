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
            throw new System.NotImplementedException();
        }

        public TAccessibilityRegion ToRegion()
        {
            throw new System.NotImplementedException();
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
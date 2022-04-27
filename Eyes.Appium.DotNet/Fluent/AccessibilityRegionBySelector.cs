using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class AccessibilityRegionBySelector : IGetAccessibilityRegion, IGetAppiumRegion, IGetAccessibilityRegionType
    {
        private readonly AccessibilityRegionType regionType_;
        private readonly By selector_;
        private readonly ISelectorTransformer selectorTransformer_;

        public AccessibilityRegionBySelector(By selector, AccessibilityRegionType regionType)
        {
            selector_ = selector;
            regionType_ = regionType;
            selectorTransformer_ = new AppiumSelectorTransformer();
        }

        AccessibilityRegionType IGetAccessibilityRegionType.AccessibilityRegionType => regionType_;
        
        public TAccessibilityRegion ToRegion()
        {
            return new SelectorAccessibilityRegion
            {
                Type = regionType_,
                Region = selectorTransformer_.GetRegionSelector(selector_)
            };
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
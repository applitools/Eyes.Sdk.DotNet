using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Applitools.Selenium.Fluent
{
    internal class AccessibilityRegionBySelector : IGetAccessibilityRegion, IGetSeleniumRegion, IGetAccessibilityRegionType
    {
        private readonly AccessibilityRegionType regionType_;
        private readonly By selector_;
        private readonly SelectorTransformer selectorTransformer_;

        public AccessibilityRegionBySelector(By selector, AccessibilityRegionType regionType)
        {
            selector_ = selector;
            regionType_ = regionType;
            selectorTransformer_ = new SelectorTransformer();
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

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
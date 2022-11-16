using OpenQA.Selenium;
using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.Utils;

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
        
        public TAccessibilityRegion ToRegion()
        {
            return new ElementAccessibilityRegion
            {
                Type = regionType_,
                Region = new RegionElement
                {
                    ElementId = element_.GetElementId()
                }
            };
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { element_ };
        }
    }
}
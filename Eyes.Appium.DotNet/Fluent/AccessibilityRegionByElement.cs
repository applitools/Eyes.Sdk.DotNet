using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Appium
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
        
        public TAccessibilityRegion ToRegion()
        {
            return new ElementAccessibilityRegion
            {
                Type = regionType_,
                Region = new RegionElement
                {
                    ElementId = element_.GetPrivateFieldValue<string>("elementId")
                }
            };
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { element_ };
        }
    }
}
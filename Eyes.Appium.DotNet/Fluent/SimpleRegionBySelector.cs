using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class SimpleRegionBySelector : IGetRegions, IGetAppiumRegion
    {
        private readonly By selector_;
        private readonly Padding padding_;
        private readonly string regionId_;
        private readonly ISelectorTransformer selectorTransformer_;
        
        public SimpleRegionBySelector(By by, Padding padding = null, string regionId = null)
        {
            selector_ = by;
            padding_ = padding;
            regionId_ = regionId;
            selectorTransformer_ = new AppiumSelectorTransformer();
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
        
        public CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = selectorTransformer_.GetRegionSelector(selector_),
                Padding = padding_,
                RegionId = regionId_
            };
        }
    }
}

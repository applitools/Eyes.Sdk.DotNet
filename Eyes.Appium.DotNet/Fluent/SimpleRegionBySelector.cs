using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;
using Applitools.Selenium;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class SimpleRegionBySelector : IGetRegions, IGetAppiumRegion
    {
        private readonly By selector_;
        private readonly ISelectorTransformer selectorTransformer_;
        
        public SimpleRegionBySelector(By by)
        {
            selector_ = by;
            selectorTransformer_ = new AppiumSelectorTransformer();
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
        
        public TRegion ToRegion()
        {
            return selectorTransformer_.GetRegionSelector(selector_);
        }
    }
}

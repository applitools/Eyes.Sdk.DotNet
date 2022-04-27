namespace Applitools.Selenium.Fluent
{
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class SimpleRegionBySelector : IGetRegions, IGetSeleniumRegion
    {
        private readonly By selector_;
        private readonly ISelectorTransformer selectorTransformer_;
        
        public SimpleRegionBySelector(By by)
        {
            selector_ = by;
            selectorTransformer_ = new SeleniumSelectorTransformer();
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
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

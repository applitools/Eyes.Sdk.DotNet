namespace Applitools.Selenium.Fluent
{
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class SimpleRegionBySelector : IGetRegions, IGetSeleniumRegion
    {
        private readonly By _selector;
        private readonly Padding _padding;
        private readonly string _regionId;
        private readonly ISelectorTransformer _selectorTransformer;
        
        public SimpleRegionBySelector(By by, Padding padding = null, string regionId = null)
        {
            _selector = by;
            _padding = padding;
            _regionId = regionId;
            _selectorTransformer = new SeleniumSelectorTransformer();
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(_selector);
            return elements;
        }
        
        public CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = _selectorTransformer.GetRegionSelector(_selector),
                Padding = _padding,
                RegionId = _regionId
            };
        }
    }
}

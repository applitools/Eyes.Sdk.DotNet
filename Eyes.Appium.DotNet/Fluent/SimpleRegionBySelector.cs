using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class SimpleRegionBySelector : IGetRegions, IGetAppiumRegion
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
            _selectorTransformer = new AppiumSelectorTransformer();
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
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

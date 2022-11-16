using Applitools.Utils;

namespace Applitools.Selenium.Fluent
{
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Collections.Generic;

    internal class SimpleRegionByElement : IGetRegions, IGetSeleniumRegion
    {
        private readonly IWebElement _element;
        private readonly Padding _padding;
        private readonly string _regionId;

        public SimpleRegionByElement(IWebElement element, Padding padding = null, string regionId = null)
        {
            _element = element;
            _padding = padding;
            _regionId = regionId;
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { _element };
        }
        
        public CodedRegionReference ToRegion()
        {
            var elementId = _element.GetElementId();

            return new CodedRegionReference
            {
                Region = new RegionElement
                {
                    ElementId = elementId
                },
                Padding = _padding,
                RegionId = _regionId
            };
        }
    }
}

using Applitools.Utils;

namespace Applitools.Selenium.Fluent
{
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Collections.Generic;

    internal class SimpleRegionByElement : IGetRegions, IGetSeleniumRegion
    {
        private IWebElement element_;

        public SimpleRegionByElement(IWebElement element)
        {
            element_ = element;
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }
        
        public TRegion ToRegion()
        {
            var elementId = element_.GetElementId();
            


            return new RegionElement // ?
            {
               // ElementId = elementId
            };
        }
    }
}

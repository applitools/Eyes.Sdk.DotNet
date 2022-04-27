using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class SimpleRegionByElement : IGetRegions, IGetAppiumRegion
    {
        private IWebElement element_;

        public SimpleRegionByElement(IWebElement element)
        {
            element_ = element;
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { element_ };
        }
        
        public TRegion ToRegion()
        {
            var elementId = element_.GetPrivateFieldValue<string>("elementId");

            return new RegionElement
            {
                ElementId = elementId
            };
        }
    }
}

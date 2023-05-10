using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.Appium.Utils;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class SimpleRegionByElement : IGetRegions, IGetAppiumRegion
    {
        private readonly IWebElement element_;
        private readonly Padding padding_;
        private readonly string regionId_;

        public SimpleRegionByElement(IWebElement element, Padding padding = null, string regionId = null)
        {
            element_ = element;
            padding_ = padding;
            regionId_ = regionId;
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { element_ };
        }
        
        public CodedRegionReference ToRegion()
        {
            var elementId = element_.GetElementId();

            return new CodedRegionReference
            {
                Region = new RegionElement
                {
                    ElementId = elementId
                },
                Padding = padding_,
                RegionId = regionId_
            };
        } 
    }
}

using System.Collections.Generic;
using Applitools.Fluent;
using Eyes.Appium.DotNet.Utils;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class FloatingRegionByElement : IGetFloatingRegion, IGetAppiumRegion, IGetFloatingRegionOffsets
    {
        private int maxDownOffset_;
        private int maxLeftOffset_;
        private int maxRightOffset_;
        private int maxUpOffset_;

        private IWebElement element_;

        public FloatingRegionByElement(IWebElement element, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            element_ = element;

            maxUpOffset_ = maxUpOffset;
            maxDownOffset_ = maxDownOffset;
            maxLeftOffset_ = maxLeftOffset;
            maxRightOffset_ = maxRightOffset;
        }

        int IGetFloatingRegionOffsets.MaxLeftOffset => maxLeftOffset_;

        int IGetFloatingRegionOffsets.MaxUpOffset => maxUpOffset_;

        int IGetFloatingRegionOffsets.MaxRightOffset => maxRightOffset_;

        int IGetFloatingRegionOffsets.MaxDownOffset => maxDownOffset_;
        
        public TFloatingRegion ToRegion()
        {
            return new ElementFloatingRegion
            {
                MaxDownOffset = maxDownOffset_,
                MaxUpOffset = maxUpOffset_,
                MaxLeftOffset = maxLeftOffset_,
                MaxRightOffset = maxRightOffset_,
                Region = new RegionElement
                {
                    ElementId = element_.GetElementId("elementId")
                }
            };
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement> { element_ };
        }
    }
}
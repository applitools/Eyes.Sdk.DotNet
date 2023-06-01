using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;

namespace Applitools.Selenium.Fluent
{
    internal class FloatingRegionBySelector : IGetFloatingRegion, IGetSeleniumRegion, IGetFloatingRegionOffsets
    {
        private readonly SeleniumSelectorTransformer selectorTransformer_;
        private readonly int maxDownOffset_;
        private readonly int maxLeftOffset_;
        private readonly int maxRightOffset_;
        private readonly int maxUpOffset_;

        private readonly By selector_;

        public FloatingRegionBySelector(By selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            selector_ = selector;
            selectorTransformer_ = new SeleniumSelectorTransformer();
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
            return new SelectorFloatingRegion
            {
                Region = selectorTransformer_.GetRegionSelector(selector_),
                Offset = new Padding(maxLeftOffset_, maxUpOffset_, maxRightOffset_, maxDownOffset_)
            };
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
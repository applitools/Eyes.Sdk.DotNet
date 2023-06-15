using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;
using Applitools.Selenium;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    internal class FloatingRegionBySelector : IGetFloatingRegion, IGetAppiumRegion, IGetFloatingRegionOffsets
    {
        private readonly SeleniumSelectorTransformer selectorTransformer_;
        private int maxDownOffset_;
        private int maxLeftOffset_;
        private int maxRightOffset_;
        private int maxUpOffset_;

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

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
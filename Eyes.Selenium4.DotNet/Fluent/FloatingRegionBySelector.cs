using OpenQA.Selenium;
using System.Drawing;
using Applitools.Utils.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Applitools.Fluent;

namespace Applitools.Selenium.Fluent
{
    internal class FloatingRegionBySelector : IGetFloatingRegion, IGetSeleniumRegion, IGetFloatingRegionOffsets
    {
        private int maxDownOffset_;
        private int maxLeftOffset_;
        private int maxRightOffset_;
        private int maxUpOffset_;

        private readonly By selector_;

        public FloatingRegionBySelector(By selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            selector_ = selector;

            maxUpOffset_ = maxUpOffset;
            maxDownOffset_ = maxDownOffset;
            maxLeftOffset_ = maxLeftOffset;
            maxRightOffset_ = maxRightOffset;
        }

        int IGetFloatingRegionOffsets.MaxLeftOffset => maxLeftOffset_;

        int IGetFloatingRegionOffsets.MaxUpOffset => maxUpOffset_;

        int IGetFloatingRegionOffsets.MaxRightOffset => maxRightOffset_;

        int IGetFloatingRegionOffsets.MaxDownOffset => maxDownOffset_;

        public IList<FloatingMatchSettings> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            ReadOnlyCollection<IWebElement> elements = ((SeleniumEyes)eyesBase).GetDriver().FindElements(selector_);
            IList<FloatingMatchSettings> retVal = new List<FloatingMatchSettings>();
            foreach (IWebElement element in elements)
            {
                Rectangle r = EyesSeleniumUtils.GetVisibleElementBounds(element);
                Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

                retVal.Add(
                    new FloatingMatchSettings()
                    {
                        Left = pTag.X,
                        Top = pTag.Y,
                        Width = r.Width,
                        Height = r.Height,
                        MaxLeftOffset = maxLeftOffset_,
                        MaxUpOffset = maxUpOffset_,
                        MaxRightOffset = maxRightOffset_,
                        MaxDownOffset = maxDownOffset_
                    }
                );
            }
            return retVal;
        }

        public TFloatingRegion ToRegion()
        {
            return new SelectorFloatingRegion
            {
                MaxDownOffset = maxDownOffset_,
                MaxUpOffset = maxUpOffset_,
                MaxLeftOffset = maxLeftOffset_,
                MaxRightOffset = maxRightOffset_,
                Region = new RegionSelector
                {
                    Type = selector_.Mechanism,
                    Selector = selector_.Criteria
                }
            };
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }
    }
}
using Applitools.Fluent;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
	public class OcrRegion : OcrRegionBase
	{
        private readonly IWebElement element_;
        private readonly By selector_;

        public OcrRegion(Region? region)
        {
            Region(region);
        }
        
        public OcrRegion(By selector)
        {
            selector_ = selector;
        }

        public OcrRegion(IWebElement element)
        {
            element_ = element;
        }

        internal By GetSelector()
        {
            return selector_;
        }

        internal IWebElement GetWebElement()
        {
            return element_;
        }
    }
}
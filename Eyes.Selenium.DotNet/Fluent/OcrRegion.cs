using Applitools.Fluent;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
	public class OcrRegion : OcrRegionBase
	{
        private readonly Region _region;
        private readonly IWebElement _element;
        private readonly By _selector;
        public TRegion Region { get; set; }

        public OcrRegion(By selector)
        {
            _selector = selector;
        }

        public OcrRegion(IWebElement element)
        {
            _element = element;
        }

        public OcrRegion(Region region)
        {
            _region = region;
        }

        internal By GetSelector()
        {
            return _selector;
        }

        internal IWebElement GetWebElement()
        {
            return _element;
        }

        internal Region GetRegion()
        {
            return _region;
        }
    }
}
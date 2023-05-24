using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright
{
    public class OcrRegion : OcrRegionBase
    {
        private IElementHandle element_;

        private ILocator locator_;

        private string selector_;

        public OcrRegion(IElementHandle element)
        {
            element_ = element;
        }

        public OcrRegion(ILocator locator)
        {
            locator_ = locator;
        }

        public OcrRegion(Region region)
        {
            Region(region);
        }

        public OcrRegion(string selector)
        {
            selector_ = selector;
        }
        
        public OcrRegion Element(IElementHandle element)
        {
            element_ = element;
            return this;
        }

        internal IElementHandle GetElement()
        {
            return element_;
        }
        
        public OcrRegion Locator(ILocator locator)
        {
            locator_ = locator;
            return this;
        }

        internal ILocator GetLocator()
        {
            return locator_;
        }
        
        public OcrRegion Selector(string selector)
        {
            selector_ = selector;
            return this;
        }

        internal string GetSelector()
        {
            return selector_;
        }
        
        public new OcrRegion Region(Region? region)
        {
            base.Region(region);
            return this;
        }
    }
}
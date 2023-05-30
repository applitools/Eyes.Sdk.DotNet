using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Selenium
{
    public class OcrRegion : OcrRegionBase
    {
        private readonly IElementHandle element_;
        private readonly ILocator locator_;
        private readonly string selector_;

        #region ctors

        public OcrRegion(IElementHandle element)
        {
            element_ = element;
        }

        public OcrRegion(ILocator locator)
        {
            locator_ = locator;
        }

        public OcrRegion(string selector)
        {
            selector_ = selector;
        }
        
        public OcrRegion(Region? region)
        {
            Region(region);
        }

        #endregion
        
        internal IElementHandle GetElement()
        {
            return element_;
        }

        internal ILocator GetLocator()
        {
            return locator_;
        }
        
        internal string GetSelector()
        {
            return selector_;
        }
    }
}
using Applitools.Fluent;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Selenium
{
    public class OcrRegion : OcrRegionBase
    {
        private readonly Region region_;
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
        
        public OcrRegion(Region region)
        {
            region_ = region;
        }

        #endregion
        
        internal IElementHandle GetElement()
        {
            return element_;
        }

        internal Region GetRegion()
        {
            return region_;
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
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Locators
{
    public class OcrRegion : OcrRegionBase
    {
        public IElementHandle Element { get; set; }

        public ILocator Locator { get; set; }

        public string Selector { get; set; }

        public Region Region { get; set; }

        public OcrRegion(IElementHandle element)
        {
            Element = element;
        }

        public OcrRegion(ILocator locator)
        {
            Locator = locator;
        }

        public OcrRegion(Region region)
        {
            Region = region;
        }

        public OcrRegion(string selector)
        {
            Selector = selector;
        }
    }
}
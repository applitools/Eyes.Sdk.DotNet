using Applitools.Fluent;

namespace Applitools.Playwright
{
    public class OcrRegionTransformer : IOcrRegionTransformer
    {
        public TRegion GetRegion(OcrRegion ocrRegion)
        {
            var selector = ocrRegion.GetSelector();
            if (selector != null)
            {
                return selectorTransformer_.GetRegionSelector(selector);
            }

            var element = ocrRegion.GetWebElement();
            if (element != null)
            {
                return new RegionElement
                {
                    ElementId = element.GetElementId()
                };
            }

            var region = ocrRegion.GetRegion();
            return new RegionRectangle
            {
                X = region.Left,
                Y = region.Top,
                Height = region.Height,
                Width = region.Width
            };
        }
    }
}
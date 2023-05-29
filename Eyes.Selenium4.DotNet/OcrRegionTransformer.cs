using Applitools.Fluent;
using Applitools.Utils;

namespace Applitools.Selenium
{
    public class OcrRegionTransformer : IOcrRegionTransformer
    {
        private readonly ISelectorTransformer selectorTransformer_;

        public OcrRegionTransformer(ISelectorTransformer selectorTransformer)
        {
            selectorTransformer_ = selectorTransformer;
        }

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
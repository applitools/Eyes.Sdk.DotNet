using Applitools.Fluent;
using Applitools.Utils;

namespace Applitools.Selenium
{
    public class OcrRegionTransformer : IOcrRegionTransformer
    {
        private readonly SelectorTransformer _selectorTransformer;

        public OcrRegionTransformer(SelectorTransformer selectorTransformer)
        {
            _selectorTransformer = selectorTransformer;
        }

        public TRegion GetRegion(OcrRegion ocrRegion)
        {
            var selector = ocrRegion.GetSelector();
            if (selector != null)
            {
                return _selectorTransformer.GetRegionSelector(selector.ToString());
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
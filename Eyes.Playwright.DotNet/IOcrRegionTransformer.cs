using Applitools.Fluent;

namespace Applitools.Playwright
{
    public interface IOcrRegionTransformer
    {
        TRegion GetRegion(OcrRegion ocrRegion);
    }
}
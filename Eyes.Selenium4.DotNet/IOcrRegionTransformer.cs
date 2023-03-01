using Applitools.Fluent;

namespace Applitools.Selenium
{
    public interface IOcrRegionTransformer
    {
        TRegion GetRegion(OcrRegion ocrRegion);
    }
}
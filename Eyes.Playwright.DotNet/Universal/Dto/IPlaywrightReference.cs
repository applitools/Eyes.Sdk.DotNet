using Applitools.Universal;

namespace Applitools.Playwright.Universal.Dto
{
    public interface IPlaywrightReference<out T>
    {
        T ToRegion(Reference root, Refer refer);
    }
}
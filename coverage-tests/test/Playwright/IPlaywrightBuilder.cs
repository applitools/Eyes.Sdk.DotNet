using Microsoft.Playwright;

namespace Applitools.Generated.Playwright.Tests
{
    public interface IPlaywrightBuilder
    {
        IPage Build(IPlaywright playwright, bool headless, bool legacy, bool executionGrid);
    }
}
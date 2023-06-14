using Microsoft.Playwright;

namespace Applitools.Generated.Playwright.Tests
{
    public class ChromeBuilder : IPlaywrightBuilder
    {
        public IPage Build(IPlaywright playwright, bool headless, bool legacy, bool executionGrid)
        {
            IBrowser browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    IgnoreDefaultArgs = headless ? new[] { "--hide-scrollbars" } : null
                }
            ).GetAwaiter().GetResult();
            return browser.NewPageAsync().GetAwaiter().GetResult();
        }
    }
}
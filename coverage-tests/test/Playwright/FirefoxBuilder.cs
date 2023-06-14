using Microsoft.Playwright;

namespace Applitools.Generated.Playwright.Tests
{
    public class FirefoxBuilder : IPlaywrightBuilder
    {
        public IPage Build(IPlaywright playwright, bool headless, bool legacy, bool executionGrid)
        {
            IBrowser browser = playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    IgnoreDefaultArgs = headless ? new[] { "--hide-scrollbars" } : null
                }
            ).GetAwaiter().GetResult();
            return browser.NewPageAsync().GetAwaiter().GetResult();
        }
    }
}
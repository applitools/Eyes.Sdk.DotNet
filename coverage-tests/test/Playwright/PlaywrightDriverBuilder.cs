using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Applitools.Generated.Playwright.Tests
{
    public class PlaywrightDriverBuilder
    {
        private bool headless_ = true;
        private bool legacy_ = false;
        private bool executionGrid_ = false;
        private string browser_ = "chrome";
        private IPlaywright playwright_;

        private static readonly Dictionary<string, IPlaywrightBuilder> playwrightBuilders_ = new()
        {
            { "chrome", new ChromeBuilder() },
            { "firefox", new FirefoxBuilder() }
        };

        public PlaywrightDriverBuilder Headless(bool headless)
        {
            headless_ = headless;
            return this;
        }

        public PlaywrightDriverBuilder Browser(string browser)
        {
            browser_ = browser;
            return this;
        }

        public PlaywrightDriverBuilder Legacy(bool legacy)
        {
            legacy_ = legacy;
            return this;
        }

        public PlaywrightDriverBuilder ExecutionGrid(bool executionGrid)
        {
            executionGrid_ = executionGrid;
            return this;
        }

        public IPage Build()
        {
            async Task<IPage> BuildAsync()
            {
                playwright_ = await Microsoft.Playwright.Playwright.CreateAsync();
                if (playwrightBuilders_.TryGetValue(browser_, out IPlaywrightBuilder builder))
                {
                    return builder.Build(playwright_, headless_, legacy_, executionGrid_);
                }

                throw new Exception($"builder for {browser_} not found");
            }

            return BuildAsync().GetAwaiter().GetResult();
        }

        public void Quit()
        {
            playwright_?.Dispose();
        }
    }
}
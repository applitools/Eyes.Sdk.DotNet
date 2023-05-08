using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Selector : Reference
    {
        public string Type { get; set; }
        
        [JsonIgnore]
        public ILocator Locator { get; set; }

        public string SelectorPath { get; set; }

        public Padding Padding { get; set; }

        public string RegionId { get; set; }

        public Selector()
        {
        }

        public Selector(string selector)
        {
            SelectorPath = selector;
        }

        public Selector(ILocator locator)
        {
            Type = "selector";
            Locator = locator;
        }
    }
}
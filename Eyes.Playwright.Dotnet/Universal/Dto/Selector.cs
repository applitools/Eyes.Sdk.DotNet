using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Selector : Reference
    {
        public string Type { get; set; }
        
        [JsonIgnore]
        public ILocator Locator { get; set; }

        public string SelectorName { get; set; }

        public Padding Padding { get; set; }

        public string RegionId { get; set; }

        public Selector()
        {
        }

        public Selector(string selector)
        {
            SelectorName = selector;
        }

        public Selector(ILocator locator)
        {
            Type = "selector";
            Locator = locator;
        }
    }
}
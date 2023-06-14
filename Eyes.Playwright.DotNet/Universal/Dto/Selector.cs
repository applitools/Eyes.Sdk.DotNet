using Applitools.Fluent;
using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Selector : Reference, IPlaywrightReference<CodedRegionReference>
    {
        public string Type { get; set; }
        
        [JsonIgnore]
        public ILocator Locator { get; set; }

        [JsonProperty("selector")]
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

        CodedRegionReference IPlaywrightReference<CodedRegionReference>.ToRegion(Reference root, Refer refer)
        {
            return new CodedRegionReference
            {
                Region = GetRegionSelector(root, refer),
                Padding = Padding,
                RegionId = ApplitoolsRefId
            };
        }

        protected RegionSelector GetRegionSelector(Reference root, Refer refer)
        {
            RegionSelector regSel;
            if (Locator != null)
            {
                regSel = new RegionSelector
                {
                    Type = "locator",
                    Selector = refer.Ref(Locator, root)
                };
            }
            else
            {
                regSel = new RegionSelector
                {
                    Selector = SelectorPath
                };
            }

            return regSel;
        }
    }
}
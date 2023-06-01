using Applitools.Fluent;
using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Element : Reference, IPlaywrightReference<CodedRegionReference>
    {
        [JsonIgnore]
        public IElementHandle ElementHandle { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("padding")]
        public Padding Padding { get; set; }
        
        [JsonProperty("regionId")]
        public string RegionId { get; set; }

        public Element()
        {
            Type = "element";
        }

        public Element(IElementHandle element) : this()
        {
            ElementHandle = element;
        }
        
        CodedRegionReference IPlaywrightReference<CodedRegionReference>.ToRegion(Reference root, Refer refer)
        {
            ApplitoolsRefId = refer.Ref(ElementHandle, root);
            return new CodedRegionReference
            {
                Region = new Element
                {
                    ApplitoolsRefId = ApplitoolsRefId
                },
                Padding = Padding,
                RegionId = ApplitoolsRefId
            };
        }
    }
}
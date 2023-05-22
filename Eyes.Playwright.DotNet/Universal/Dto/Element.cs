using System.Text.Json.Serialization;
using Applitools.Fluent;
using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Element : Reference
    {
        [Newtonsoft.Json.JsonIgnore]
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
        
        public override CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = new RegionElement
                {
                    ElementId = ApplitoolsRefId
                },
                Padding = Padding,
                RegionId = ApplitoolsRefId
            };
        }
    }
}
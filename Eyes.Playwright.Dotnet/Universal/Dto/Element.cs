using Applitools.Fluent;
using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Element : Reference
    {
        [JsonIgnore]
        public IElementHandle ElementHandle { get; set; }

        public string Type { get; set; }
        public Padding Padding { get; set; }
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
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

        public string GetElementType() => Type;

        public void SetElementType(string type)
        {
            Type = type;
        }

        public IElementHandle GetElementHandle() => ElementHandle;

        public void SetElementHandle(IElementHandle elementHandle)
        {
            ElementHandle = elementHandle;
        }

        public Padding GetPadding() => Padding;

        public void SetPadding(Padding padding)
        {
            Padding = padding;
        }

        public string GetRegionId() => RegionId;

        public void SetRegionId(string regionId)
        {
            RegionId = regionId;
        }
    }
}
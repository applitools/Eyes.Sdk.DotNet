using Applitools.Fluent;
using Newtonsoft.Json;

namespace Applitools.Universal
{
    public class Reference : TRegion, IGetRegions
    {
        [JsonProperty("applitools-ref-id")] 
        public string ApplitoolsRefId { get; set; }

        public Reference() { }

        public Reference(string applitoolsRefId) 
        {
            ApplitoolsRefId = applitoolsRefId;
        }

        public override string ToString() 
        {
            return "Reference{" +
                "applitoolsRefId='" + ApplitoolsRefId + '\'' +
                '}';
        }

        public virtual CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = new RegionElement
                {
                    ElementId = ApplitoolsRefId
                },
                Padding = null,
                RegionId = ApplitoolsRefId
            };
        }
    }
}
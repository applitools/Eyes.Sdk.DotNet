using Applitools.Fluent;
using Newtonsoft.Json;

namespace Applitools.Universal
{
    public class Reference : TRegion, IGetRegion
    {

        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId {get;set;}

        public Reference() {
        }

        public Reference(string applitoolsRefId) {
            ApplitoolsRefId = applitoolsRefId;
        }

        public override string ToString() {
            return "Reference{" +
                "applitoolsRefId='" + ApplitoolsRefId + '\'' +
                '}';
        }

    }
}
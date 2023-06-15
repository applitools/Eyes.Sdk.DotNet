using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IosVersion
    {
        [EnumMember(Value = "latest")] LATEST,
        [EnumMember(Value = "latest-1")] ONE_VERSION_BACK,
    }
}

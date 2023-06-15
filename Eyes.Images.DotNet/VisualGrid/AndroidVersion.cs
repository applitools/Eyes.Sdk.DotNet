using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AndroidVersion
    {
        [EnumMember(Value = "latest")] LATEST,
        [EnumMember(Value = "latest-1")] ONE_VERSION_BACK,
        [EnumMember(Value = "canary")] CANARY,
    }
}
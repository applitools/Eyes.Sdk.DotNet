using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Stage
    {
        General,
        Open,
        Check,
        Close,
        Render,
        ResourceCollection,
        Locate
    }
}

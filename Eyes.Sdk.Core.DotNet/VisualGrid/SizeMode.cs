using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter), typeof(DashSeparatedNamingStrategy))]
    public enum SizeMode
    {
        Viewport,
        FullPage,
        Selector,
        Region
    }
}
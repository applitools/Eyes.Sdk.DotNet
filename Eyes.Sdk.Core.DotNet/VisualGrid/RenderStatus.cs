using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter), typeof(DashSeparatedNamingStrategy))]
    public enum RenderStatus
    {
        None,
        NeedMoreResources,
        Rendering,
        Rendered,
        Error,
        NeedMoreDom
    }
}
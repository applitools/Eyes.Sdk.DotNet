using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RenderStatus
    {
        [EnumMember(Value = "none")] None,
        [EnumMember(Value = "need-more-resources")] NeedMoreResources,
        [EnumMember(Value = "rendering")] Rendering,
        [EnumMember(Value = "rendered")] Rendered,
        [EnumMember(Value = "error")] Error,
        [EnumMember(Value = "need-more-dom")] NeedMoreDom
    }
}
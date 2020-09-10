using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SizeMode
    {
        [EnumMember(Value = "viewport")]
        Viewport,
        
        [EnumMember(Value = "full-page")]
        FullPage,
        
        [EnumMember(Value = "selector")]
        Selector,
        
        [EnumMember(Value = "region")]
        Region,

        [EnumMember(Value = "full-selector")]
        FullSelector
    }
}
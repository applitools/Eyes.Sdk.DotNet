using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TraceLevel
    {
        Debug = 0,
        Info = 1,
        Notice = 2,
        Warn = 3,
        Error = 4
    }
}

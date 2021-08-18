using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TestResultsStatus
    {
        Passed,
        Unresolved,
        Failed,
        NotOpened
    }
}

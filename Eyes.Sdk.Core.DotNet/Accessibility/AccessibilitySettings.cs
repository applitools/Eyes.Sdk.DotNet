using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public class AccessibilitySettings
    {
        public AccessibilitySettings(AccessibilityLevel level, AccessibilityGuidelinesVersion version)
        {
            Level = level;
            GuidelinesVersion = version;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public AccessibilityLevel Level { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Version")]
        public AccessibilityGuidelinesVersion GuidelinesVersion { get; set; }
    }
}

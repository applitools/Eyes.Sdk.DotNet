using Newtonsoft.Json;

namespace Applitools
{
    public class SessionAccessibilityStatus
    {
        [JsonProperty("status")]
        public AccessibilityStatus Status { get; set; }

        [JsonProperty("level")]
        public AccessibilityLevel Level { get; set; }

        [JsonProperty("version")]
        public AccessibilityGuidelinesVersion Version { get; set; }
    }

    /// <summary>
    /// Accessibility status.
    /// </summary>
    public enum AccessibilityStatus
    {
        /// <summary>
        /// Session has passed accessibility validation.
        /// </summary>
        Passed,

        /// <summary>
        /// Session hasn't passed accessibility validation.
        /// </summary>
        Failed,
    }
}
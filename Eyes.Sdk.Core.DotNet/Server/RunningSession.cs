using Newtonsoft.Json;

namespace Applitools
{
    /// <summary>
    /// Encapsulates data for the session currently running in the agent.
    /// </summary>
    public class RunningSession
    {
        [JsonProperty("isNew")]
        public bool? isNewSession_ = null;
       
        [JsonIgnore]
        public bool IsNewSession { get { return isNewSession_ ?? false; } }

        public string Id { get; set; }

        public string BaselineId { get; set; }

        public string BatchId { get; set; }

        public string SessionId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "Serialization")]
        public string Url { get; set; }

        public RenderingInfo RenderingInfo { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Id;
        }
    }
}

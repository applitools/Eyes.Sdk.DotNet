using Newtonsoft.Json;

namespace Applitools.Tests.Utils
{
    public partial class TestResultReportSummary
    {
        [JsonProperty("group")]
        public string Group => "images";

        [JsonIgnore]
        public bool IsGenerated => false;
    }
}

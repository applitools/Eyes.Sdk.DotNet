using Newtonsoft.Json;

namespace Applitools.VisualGrid.Model
{
    public class AttributeData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}='{Value}'";
        }
    }
}
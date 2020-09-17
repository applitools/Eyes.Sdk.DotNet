using Newtonsoft.Json;

namespace Applitools.VisualGrid
{
    public class VisualGridSelector
    {
        public VisualGridSelector(string xpath, object category)
        {
            Selector = xpath;
            Category = category;
        }
        public string Selector { get; }

        [JsonIgnore]
        public object Category { get; }

        public string Type => "xpath";
    }
}
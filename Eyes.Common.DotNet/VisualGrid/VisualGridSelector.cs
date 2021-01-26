using Newtonsoft.Json;
using System;

namespace Applitools.VisualGrid
{
    public class VisualGridSelector : IEquatable<VisualGridSelector>
    {
        public VisualGridSelector(string xpath, object category)
        {
            Selector = xpath;
            Category = category;
        }

        public VisualGridSelector() { }
        public string Selector { get; set; }

        [JsonIgnore]
        public object Category { get; }

        public string Type => "xpath";

        public bool Equals(VisualGridSelector other)
        {
            if (other == null) return false;
            return Selector == other.Selector;
        }
    }
}
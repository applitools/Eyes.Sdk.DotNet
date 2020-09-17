using Newtonsoft.Json;
using System.Collections.Generic;

namespace Applitools.VisualGrid.Model
{
    public class CdtData
    {
        [JsonProperty("nodeType")]
        public int NodeType { get; set; }

        [JsonProperty("childNodeIndexes")]
        public List<int> ChildNodeIndexes { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }

        [JsonProperty("nodeValue")]
        public string NodeValue { get; set; }

        [JsonProperty("attributes")]
        public List<AttributeData> Attributes { get; set; }

        [JsonProperty("shadowRootIndex")]
        public int? ShadowRootIndex { get; set; }

        public override string ToString()
        {
            return $"{NodeName ?? "NodeType: " + NodeType} (Value: '{NodeValue}' ; ChildNodeIndexes.Count: {ChildNodeIndexes?.Count} ; Attributes.Count: {Attributes?.Count})";
        }
    }

}
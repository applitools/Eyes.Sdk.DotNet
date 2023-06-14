using System.Collections;
using System.Collections.Generic;

namespace Applitools
{
    public struct PropertyData
    {
        public PropertyData(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static PropertyData FromDictionary(Dictionary<string, string> keyVal)
        {
            if (keyVal != null &&
                keyVal.TryGetValue("name", out var name) &&
                keyVal.TryGetValue("value", out var value))
            {
                return new PropertyData(name, value);
            }

            return new PropertyData();
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string> { { "name", Name }, { "value", Value } };
        }
    }
}
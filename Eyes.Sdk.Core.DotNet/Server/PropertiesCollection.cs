using System.Collections;
using System.Collections.Generic;

namespace Applitools
{
    public class PropertiesCollection : IEnumerable<PropertyData>
    {
        private List<PropertyData> properties_ = new List<PropertyData>();

        public void Add(string name, string value)
        {
            PropertyData pd = new PropertyData(name, value);
            properties_.Add(pd);
        }

        public void Clear()
        {
            properties_.Clear();
        }

        public IEnumerator<PropertyData> GetEnumerator()
        {
            return ((IEnumerable<PropertyData>)properties_).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PropertyData>)properties_).GetEnumerator();
        }
    }
}
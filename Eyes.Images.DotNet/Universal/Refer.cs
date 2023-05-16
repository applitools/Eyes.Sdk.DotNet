using System.Collections.Generic;

namespace Applitools.Universal
{
    public abstract class Refer
    {
        public static readonly string APPLITOOLS_REF_ID = "applitools-ref-id";

        protected Dictionary<string, object> References = new Dictionary<string, object>();
        protected Dictionary<string, HashSet<Reference>> Relations = new Dictionary<string, HashSet<Reference>>();

        public abstract void Destroy(Reference root);
    }
}

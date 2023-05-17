using System;
using System.Collections.Generic;
using Applitools.Universal;

namespace Applitools.Playwright.Universal
{
    public class Refer : Applitools.Universal.Refer
    {
        private readonly Dictionary<string, object> references_ = new Dictionary<string, object>();

        private readonly Dictionary<string, HashSet<Reference>> relations_ =
            new Dictionary<string, HashSet<Reference>>();

        public Refer()
        {
        }

        ///<summary>store the driver's ref.</summary>
        ///<returns>the ref GUID</returns>
        public string Ref(object value, Reference root)
        {
            if (value == null)
            {
                return null;
            }

            string refId = Guid.NewGuid().ToString();
            references_.Add(refId, value);

            if (root != null)
            {
                if (!relations_.TryGetValue(root.ApplitoolsRefId, out HashSet<Reference> childRefs))
                {
                    relations_[root.ApplitoolsRefId] = childRefs = new HashSet<Reference>();
                }

                childRefs.Add(new Reference(refId));
            }

            return refId;
        }
        
        ///<summary>get a ref from store</summary>
        ///<param name="ref">the ref</param>
        /// <returns>the ref</returns>
        public object Deref(object @ref)
        {
            if (IsRef(@ref))
            {
                Reference reference = (Reference)@ref;
                references_.TryGetValue(reference.ApplitoolsRefId, out object result);
                return result;
            }

            return @ref;
        }

        public override void Destroy(Reference root)
        {
            if (!IsRef(root))
            {
                return;
            }

            relations_.TryGetValue(root.ApplitoolsRefId, out HashSet<Reference> childRefs);
            if (childRefs != null)
            {
                foreach (Reference childRef in childRefs)
                {
                    Destroy(childRef);
                }

                relations_.Remove(root.ApplitoolsRefId);
            }

            references_.Remove(root.ApplitoolsRefId);
        }

        private static bool IsRef(object @ref)
        {
            return @ref is Reference reference && reference.ApplitoolsRefId != null;
        }
    }
}
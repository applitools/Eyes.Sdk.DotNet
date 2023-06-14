using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Applitools.Universal;

namespace Applitools.Playwright.Universal
{
    public class Refer : Applitools.Universal.Refer
    {
        private readonly ConcurrentDictionary<string, object> references_ = new ConcurrentDictionary<string, object>();

        private readonly ConcurrentDictionary<string, HashSet<Reference>> relations_ =
            new ConcurrentDictionary<string, HashSet<Reference>>();

        ///<summary>store the driver's ref.</summary>
        ///<returns>the ref GUID</returns>
        public string Ref(object value, Reference root)
        {
            if (value == null)
            {
                return null;
            }

            string refId = Guid.NewGuid().ToString();
            references_.TryAdd(refId, value);

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

                relations_.TryRemove(root.ApplitoolsRefId, out _);
            }

            references_.TryRemove(root.ApplitoolsRefId, out _);
        }

        private static bool IsRef(object @ref)
        {
            return @ref is Reference reference && reference.ApplitoolsRefId != null;
        }
    }
}
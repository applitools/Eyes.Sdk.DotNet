using Applitools.Commands;
using Applitools.Fluent;

namespace Applitools.Universal
{
    public class ContextReferenceDto : IFrame
    {
        public object Frame { get; set; }
        public TRegion ScrollRootElement { get; set; }
    }
}
using Applitools.Fluent;

namespace Applitools.Commands
{
    public interface IElementReference
    {

    }

    public class TElement : IElementReference
    {
        public string ElementId { get; set; }

        public TElement()
        {

        }

        public TElement(string elementId)
        {
            ElementId = elementId;
        }
    }

    public class TSelector : IElementReference
    {
        public string Type { get; set; }
        public string Selector { get; set; }

        public TSelector()
        {
            
        }

        public TSelector(string type, string selector)
        {
            Type = type;
            Selector = selector;
        }
    }

    public interface IFrame{}

    public class FrameNameOrId : IFrame
    {
        public string Frame { get; set; }
    }
    
    public class FrameSelector : IFrame
    {
        public RegionSelector Frame { get; set; }
    }

    public class FrameElement : IFrame
    {
        public string ElementId { get; set; }
    }

    public class FrameScrollRootElement : IFrame
    {
        public IElementReference ScrollRootElement { get; set; }
    }
}
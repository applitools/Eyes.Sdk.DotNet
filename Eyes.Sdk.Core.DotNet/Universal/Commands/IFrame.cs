namespace Applitools.Selenium.Commands
{
    public interface IFrame{}

    public class FrameNameOrId : IFrame
    {
        public string Frame { get; set; }
    }

    public class FrameSelector : IFrame
    {
        public string Type { get; set; }
        public string Selector { get; set; }
    }

    public class FrameElement : IFrame
    {
        public string ElementId { get; set; }
    }
}
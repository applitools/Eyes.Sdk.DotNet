namespace Applitools.Playwright.Universal.Dto
{
    public class FrameLocator
    {
        public string FrameNameOrId { get; set; }
        public int? FrameIndex { get; set; }
        public Element FrameElement { get; set; }
        public Selector FrameSelector { get; set; }
        public Element ScrollRootElement { get; set; }
        public Selector ScrollRootSelector { get; set; }
    }
}
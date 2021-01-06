using Applitools.Utils.Geometry;

namespace Applitools.VisualGrid
{
    public class RenderStatusResults : IRenderStatusResults
    {
        public RenderStatus Status { get; set; } = RenderStatus.None;
        public string DomLocation { get; set; }
        public string UserAgent { get; set; }
        public string ImageLocation { get; set; }
        public string OS { get; set; }
        public string Error { get; set; }
        public RectangleSize DeviceSize { get; set; }
        public RectangleSize VisualViewport { get; set; }
        public VGRegion[] SelectorRegions { get; set; }
        public string RenderId { get; set; }
        public Location ImagePositionInActiveFrame { get; set; }

        public override string ToString()
        {
            return "RenderStatusResults{" +
                    $"renderId={RenderId}, " +
                    $"status={Status}, " +
                    $"domLocation='{DomLocation}', " +
                    $"userAgent='{UserAgent}', " +
                    $"imageLocation='{ImageLocation}', " +
                    $"os='{OS}', " +
                    $"error='{Error}', " +
                    $"selectorRegions={SelectorRegions}, " +
                    $"deviceSize={DeviceSize}, " +
                    $"visualViewport={VisualViewport}" +
                    "}";
        }

        public bool IsEmpty()
        {
            return (SelectorRegions == null || SelectorRegions.Length == 0) &&
                    Status == RenderStatus.None &&
                    RenderId == null &&
                    ImageLocation == null &&
                    Error == null &&
                    OS == null &&
                    UserAgent == null &&
                    DeviceSize.IsEmpty() &&
                    SelectorRegions == null;

        }

    }
}
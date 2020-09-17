using Applitools.VisualGrid.Model;

namespace Applitools.Selenium.VisualGrid
{
    public enum CaptureStatusEnum
    {
        WIP,
        SUCCESS,
        ERROR
    };

    public class CaptureStatus
    {
        public CaptureStatusEnum Status { get; set; }
        public string Error { get; set; }
        public FrameData Value { get; set; }

        public override string ToString()
        {
            return $"Status: {Status} - {Error}{Value}";
        }
    }
}
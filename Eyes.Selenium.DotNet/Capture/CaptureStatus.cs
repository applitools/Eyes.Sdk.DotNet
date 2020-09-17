namespace Applitools.Selenium.Capture
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
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Status: {Status} - {Error}{Value}";
        }
    }
}
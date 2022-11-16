namespace Applitools
{
    public interface ICloseConfig
    {
        bool? ThrowErr { get; }
        bool? UpdateBaselineIfNew { get; }
        bool? UpdateBaselineIfDifferent { get; }
    }
    
    public class CloseConfig : ICloseConfig
    {
        public bool? ThrowErr { get; set; }
        public bool? UpdateBaselineIfNew { get; set; }
        public bool? UpdateBaselineIfDifferent { get; set; }
    }
}
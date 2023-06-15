using Applitools.Utils.Geometry;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class BaselineEnv
    {
        public string Os { get; set; }
        public string OsInfo { get; set; }
        public string HostingApp { get; set; }
        public string HostingAppInfo { get; set; }
        public string DeviceName { get; set; }
        public RectangleSize ViewportSize { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
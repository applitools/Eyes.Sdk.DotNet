using Applitools.Utils.Geometry;

namespace Applitools
{
    /// <summary>
    /// The environment in which the AUT is executing.
    /// </summary>
    public class AppEnvironment
    {
        /// <summary>
        /// Creates a new <see cref="AppEnvironment"/> instance.
        /// </summary>
        public AppEnvironment()
        {
        }

        /// <summary>
        /// Creates a new AppEnvironment instance.
        /// </summary>
        public AppEnvironment(string operatingSystem, string hostingApp, RectangleSize displaySize)
        {
            OS = operatingSystem;
            HostingApp = hostingApp;
            DisplaySize = displaySize;
        }

        /// <summary>
        /// Gets/Sets the OS hosting the application_ under test or <c>null</c> if
        /// unknown.
        /// </summary>
        public string OS { get; set; }

        /// <summary>
        /// Information inferred from the execution environment or {@code null} if no
        /// information could be inferred.
        /// </summary>
        public string Inferred { get; set; }

        /// <summary>
        /// Gets/Sets the application_ hosting the application_ under test or <c>null</c>
        /// if unknown.
        /// </summary>
        public string HostingApp { get; set; }

        /// <summary>
        /// Gets/Sets the display size of the application_ or <c>null</c> if unknown.
        /// </summary>
        public RectangleSize DisplaySize { get; set; }

        /// <summary>
        /// Gets/Sets the device on which the test has run.
        /// </summary>
        public string DeviceInfo { get; set; }

        public override string ToString()
        {
            return "[OS = " + (OS == null ? "?" : "'" + OS + "'")
                + " HostingApp = " + (HostingApp == null ? "?" : "'" + HostingApp + "'")
                + " DisplaySize = " + DisplaySize + "]";
        }
    }
}

namespace Applitools.Correlate.Capture
{
    using System;
    using System.Security;
    using Applitools.Utils.Config;

    /// <summary>
    /// Captures window images and hid events.
    /// </summary>
    public interface IGuiCapturer : IConfigurable<string>, IDisposable
    {
        /// <summary>
        /// Start monitoring active applications.
        /// </summary>
        /// <param name="monitorHid">Whether or not to monitor HID actions</param>
        [SecurityCritical]
        void StartMonitoring(bool monitorHid);

        /// <summary>
        /// Stop monitoring active applications.
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Returns a <see cref="GuiCapture"/> instance of the currently active window or
        /// <c>null</c> if the active window should not be monitored.
        /// The caller is responsible for disposing of the capture.
        /// </summary>
        /// <param name="standalone">
        /// Whether this is a standalone capture or part of a burst
        /// </param>
        [SecurityCritical]
        GuiCapture Capture(bool standalone = false);
    }
}

namespace Applitools.Correlate.Capture
{
    using System;
    using System.Drawing;
    using System.Security;
    using System.Windows.Forms;
    using Applitools.Utils.Geometry;
    using Applitools.Utils.Gui;

    /// <summary>
    /// Low level Api for capturing GUI events and entities.
    /// </summary>
    public interface ILowLevelCaptureApi : IGuiMonitor
    {
        /// <summary>
        /// Starts monitoring.
        /// </summary>
        [SecurityCritical]
        void StartMonitoring(bool monitorHid);

        /// <summary>
        /// Gets a handle to the window located at the input point (relative to the desktop) 
        /// or <c>IntPtr.Zero</c> if no such window exists
        /// </summary>
        IntPtr GetWindowFromPoint(Point point);

        /// <summary>
        /// Gets the details of the input window.
        /// </summary>
        WindowCaptureInfo GetWindowInfo(IntPtr window);

        /// <summary>
        /// Gets the root or root owner window of the input window.
        /// </summary>
        IntPtr GetAncestorWindow(IntPtr window, bool owner);

        /// <summary>
        /// Captures the contents of the input region of the input window.
        /// </summary>
        /// <param name="window">Window to capture</param>
        /// <param name="clientArea">Whether to capture only the client area</param>
        /// <param name="padding">
        /// Inner window padding defining the region to capture or <c>null</c> to capture the
        /// entire window
        /// </param>
        /// <param name="fromScreen">Whether to capture screen pixels or attempt to draw
        /// window to the returned bitmap
        /// </param>
        /// <param name="redrawWait">
        /// Delay in milliseconds to allow window background drawing to complete before 
        /// capturing.
        /// </param>
        [SecurityCritical]
        Bitmap CaptureWindow(
            IntPtr window, 
            bool clientArea, 
            RectangularMargins padding, 
            bool fromScreen = true, 
            int redrawWait = 0);

        /// <summary>
        /// Gets the currently active window or <c>IntPtr.Zero</c> if there is no active
        /// window.
        /// </summary>
        IntPtr GetActiveWindow();

        /// <summary>
        /// Places <c>window1</c> above <c>window2</c> without activating <c>window1</c>
        /// </summary>
        bool PlaceWindowAbove(IntPtr window1, IntPtr window2);

        /// <summary>
        /// Makes the input window the active window.
        /// </summary>
        bool ActivateWindow(IntPtr window);
        
        /// <summary>
        /// Returns <c>true</c> if and only if the input window is visible
        /// </summary>
        bool IsWindowVisible(IntPtr window);

        /// <summary>
        /// Creates a Form that cannot be activated.
        /// </summary>
        Form CreateNonActiveForm();
    }
}

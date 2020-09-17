namespace Applitools.Correlate.Capture
{
    using System;
    using System.Drawing;
    using Applitools.Utils;
    using Applitools.Utils.Geometry;

    /// <summary>
    /// Information related to a captured window.
    /// </summary>
    public class WindowCaptureInfo
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="WindowCaptureInfo"/> instance.
        /// </summary>
        public WindowCaptureInfo(
            IntPtr handle,
            int process,
            int thread,
            string caption,
            string className,
            Rectangle window,
            Rectangle client)
        {
            Handle = handle;
            Process = process;
            Thread = thread;
            Caption = caption ?? string.Empty;
            ClassName = className ?? string.Empty;
            Window = window;
            Client = client;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The window handle
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// The ID of the process owning the window
        /// </summary>
        public int Process { get; }

        /// <summary>
        /// The ID of the thread owning the window
        /// </summary>
        public int Thread { get; }

        /// <summary>
        /// The window caption.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// The window class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The window area coordinates relative to the screen
        /// </summary>
        public Rectangle Window { get; }

        /// <summary>
        /// The window client area (the area delimited by the window borders and caption) 
        /// coordinates relative to the screen
        /// </summary>
        public Rectangle Client { get; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Handle}@{Process} ({ClassName}) {GeometryUtils.ToString(Window)} '{Caption}'";
        }

        #endregion
    }
}

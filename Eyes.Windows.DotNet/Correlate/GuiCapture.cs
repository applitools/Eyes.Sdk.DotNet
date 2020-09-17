namespace Applitools.Correlate
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Applitools.Utils;
    using Applitools.Utils.Geometry;
    using Applitools.Utils.Gui;
    
    /// <summary>
    /// The result of GUI capturing consisting of a window and a sequence of HID events.
    /// </summary>
    public sealed class GuiCapture : ICloneable, IDisposable
    {
        #region Fields

        private static readonly GuiEventArgs[] NoEvents_ = new GuiEventArgs[0];

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="GuiCapture"/> instance.
        /// </summary>
        /// <param name="windowTitle">Window title or <c>null</c> if unknown</param>
        /// <param name="windowImage">Window image</param>
        /// <param name="windowLocation">Window location relative to desktop</param>
        /// <param name="hidEvents">Hid events or <c>null</c> if there are none</param>
        /// <param name="windowId">Matching model window id or <c>null</c> if unknown</param>
        /// <param name="ignore">An optional list of regions to ignore.</param>
        public GuiCapture(
            string windowTitle,
            Bitmap windowImage, 
            Point windowLocation,
            IEnumerable<GuiEventArgs> hidEvents,
            string windowId = null,
            IEnumerable<IRegion> ignore = null) : this(windowTitle, windowImage)
        {
            WindowLocation = windowLocation;
            HidEvents = hidEvents ?? new GuiEventArgs[0];
            WindowId = windowId;
            Ignore = ignore ?? new IRegion[0];
        }

        /// <summary>
        /// Creates a new <see cref="GuiCapture"/> instance.
        /// </summary>
        public GuiCapture(string windowTitle, Bitmap windowImage)
        {
            ArgumentGuard.NotNull(windowImage, nameof(windowImage));

            WindowTitle = windowTitle;
            WindowImage = windowImage;
            HidEvents = NoEvents_;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The captured window title or <c>null</c> if unknown.
        /// </summary>
        public string WindowTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// The locator (e.g., URL) of the window if available.
        /// </summary>
        public string WindowLocator
        {
            get;
            set;
        }

        /// <summary>
        /// The captured window image.
        /// </summary>
        public Bitmap WindowImage
        {
            get;
            private set;
        }

        /// <summary>
        /// The top-left corner of <see cref="WindowImage"/> relative to the screen.
        /// </summary>
        public Point WindowLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// The sequence of HID events that occurred since the last capture.
        /// </summary>
        public IEnumerable<GuiEventArgs> HidEvents
        {
            get;
            private set;
        }

        /// <summary>
        /// The id of a model window matching this capture or <c>null</c> if no such model 
        /// window is known.
        /// </summary>
        public string WindowId { get; set; }

        /// <summary>
        /// Regions of <see cref="WindowImage"/> that can be ignored and do not indicate window 
        /// controls (i.e., window images).
        /// </summary>
        public IEnumerable<IRegion> Ignore { get; set; }
        
        #endregion

        #region Methods

        /// <inheritdoc />
        public object Clone()
        {
            return Clone(true);
        }

        /// <summary>
        /// Clones this object.
        /// </summary>
        /// <param name="image">
        /// If <c>false</c> the image is not cloned and set to <c>null</c></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability", 
            "CA2000:Dispose objects before losing scope",
            Justification = "Clone owned by caller")]
        public object Clone(bool image)
        {
            return new GuiCapture(
                WindowTitle,
                WindowImage == null || !image ? null : (Bitmap)WindowImage.Clone(),
                WindowLocation,
                new List<GuiEventArgs>(HidEvents),
                WindowId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            WindowImage.DisposeIfNotNull();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "'{0}' '{1}' '{2}' [{3}]".Fmt(
                WindowTitle,
                GeometryUtils.ToString(new Rectangle(WindowLocation, WindowImage.Size)),
                WindowId,
                StringUtils.Concat(HidEvents, ", "));
        }

        #endregion
    }
}

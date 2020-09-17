namespace Applitools.Utils.Gui
{
    using System;

    /// <summary>
    /// Window event arguments.
    /// </summary>
    public class WindowEventArgs : GuiEventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="WindowEventArgs"/> instance.
        /// </summary>
        public WindowEventArgs(DateTimeOffset timestamp, GuiEventTypes eventType, IntPtr handle)
            : base(timestamp, eventType)
        { 
            Window = handle; 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Window handle.
        /// </summary>
        public IntPtr Window
        {
            get;
            private set;
        }

        #endregion

        #region Methods
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "{0} {1}".Fmt(EventType, Window);
        }

        #endregion
    }
}

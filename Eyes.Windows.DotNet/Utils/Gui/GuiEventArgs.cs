namespace Applitools.Utils.Gui
{
    using System;

    /// <summary>
    /// Base class for <see cref="IGuiMonitor.GuiEvent"/> event arguments.
    /// </summary>
    public class GuiEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="GuiEventArgs"/> instance.
        /// </summary>
        public GuiEventArgs(DateTimeOffset timestamp, GuiEventTypes eventType)
        {
            EventType = eventType;
            Timestamp = timestamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of this event.
        /// </summary>
        public GuiEventTypes EventType
        {
            get;
            private set;
        }

        /// <summary>
        /// The time when this event occurred.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the event is a keyboard related event.
        /// </summary>
        public bool IsKeyboardEvent
        {
            get { return 0 != (GuiEventTypes.Keyboard & EventType); }
        }

        /// <summary>
        /// Whether the event is a mouse related event.
        /// </summary>
        public bool IsMouseEvent
        {
            get { return 0 != (GuiEventTypes.Mouse & EventType); }
        }

        /// <summary>
        /// Whether the event is a window event.
        /// </summary>
        public bool IsWindowEvent
        {
            get { return 0 != (GuiEventTypes.Window & EventType); }
        }

        #endregion
    }
}

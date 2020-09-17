namespace Applitools.Utils.Gui
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Mouse event arguments.
    /// </summary>
    public class MouseEventArgs : GuiEventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="MouseEventArgs"/> instance.
        /// </summary>
        public MouseEventArgs(
            DateTimeOffset timestamp,
            GuiEventTypes eventType, 
            MouseButtons button, 
            int clicks, 
            int x, 
            int y, 
            int delta)
            : base(timestamp, eventType)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        #endregion

        #region Properties

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.Button" />
        public MouseButtons Button
        {
            get;
            private set;
        }

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.Clicks" />
        public int Clicks
        {
            get;
            private set;
        }

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.X" />
        public int X
        {
            get;
            set;
        }

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.Y" />
        public int Y
        {
            get;
            set;
        }

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.Location" />
        public Point Location
        {
            get { return new Point(X, Y); }
        }

        /// <inheritdoc cref="System.Windows.Forms.MouseEventArgs.Delta" />
        public int Delta
        {
            get;
            private set;
        }

        /// <summary>
        /// The size of the window relative to which the coordinate of the mouse are provided
        /// or <see cref="Size.Empty"/> if the coordinates absolute.
        /// </summary>
        public Size WindowSize
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0}({1} x {2} [{3}])".Fmt(EventType, Button, Clicks, Location);
        }

        #endregion
    }
}

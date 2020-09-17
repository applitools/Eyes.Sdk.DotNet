namespace Applitools
{
    using Applitools.Utils;
    using Applitools.Utils.Geometry;

    /// <summary>
    /// Mouse action
    /// </summary>
    public enum MouseAction
    {
        None,
        Click,
        RightClick,
        DoubleClick,
        Move,
        Down,
        Up,
    }

    /// <summary>
    /// Encapsulates a mouse trigger.
    /// </summary>
    public class MouseTrigger : Trigger
    {
        public MouseTrigger(
            MouseAction mouseAction,
            Region control,
            Location location)
        {
            ArgumentGuard.NotNull(control, nameof(control));
            ArgumentGuard.NotNull(location, nameof(location));

            MouseAction = mouseAction;
            Control = control;
            Location = location;
        }
        
        /// <inheritdoc />
        public override TriggerTypes TriggerType 
        { 
            get { return TriggerTypes.Mouse; } 
        }

        /// <summary>
        /// Gets the location of the action relative to the top left corner of 
        /// <see cref="Control"/> or <c>null</c> if unknown.
        /// </summary>
        public Location Location
        {
            get;
            private set;
        }

        public MouseAction MouseAction
        {
            get;
            private set;
        }

        /// <summary>
        /// The region of the control that was clicked. If no control is known, specify a region
        /// with zero size.
        /// </summary>
        public Region Control
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0} [{1}] {2}".Fmt(MouseAction, Control, Location);
        }
    }
}

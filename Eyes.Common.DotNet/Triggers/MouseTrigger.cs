using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
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
            Location location) : base(control)
        {
            ArgumentGuard.NotNull(location, nameof(location));

            MouseAction = mouseAction;
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

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0} [{1}] {2}".Fmt(MouseAction, Control, Location);
        }
    }
}

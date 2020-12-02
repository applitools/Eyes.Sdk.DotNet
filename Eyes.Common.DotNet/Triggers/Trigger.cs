﻿using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    /// <summary>
    /// Trigger types.
    /// </summary>
    public enum TriggerTypes
    {
        /// <summary>
        /// Unknown trigger
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A mouse trigger
        /// </summary>
        Mouse = 1,

        /// <summary>
        /// A text trigger
        /// </summary>
        Text = 2,

        /// <summary>
        /// A keyboard trigger.
        /// </summary>
        Keyboard = 3,
    }

    /// <summary>
    /// A base class for model transition triggers.
    /// </summary>
    public abstract class Trigger
    {
        public Trigger(Region control)
        {
            ArgumentGuard.NotNull(control, nameof(control));
            Control = control;
        }

        /// <summary>
        /// The concrete trigger type.
        /// </summary>
        public abstract TriggerTypes TriggerType { get; }

        public Region Control
        {
            get;
            private set;
        }
    }
}

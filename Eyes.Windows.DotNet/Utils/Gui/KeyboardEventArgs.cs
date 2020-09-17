namespace Applitools.Utils.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Applitools.Utils;

    /// <summary>
    /// Keyboard event arguments
    /// </summary>
    public class KeyboardEventArgs : GuiEventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="KeyboardEventArgs"/> instance.
        /// </summary>
        public KeyboardEventArgs(
            DateTimeOffset timestamp,
            GuiEventTypes eventType, 
            Keys key, 
            Keys modifiers, 
            char character = (char)0)
            : base(timestamp, eventType)
        {
            Handled = false;
            Key = key;
            Modifiers = modifiers;
            Char = character;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the event is handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// The changed key.
        /// </summary>
        public Keys Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Pressed key modifiers (i.e., <c>Shift</c>, <c>Control</c>, etc.).
        /// </summary>
        public Keys Modifiers
        {
            get;
            private set;
        }

        /// <summary>
        /// The resulting character or <c>(char)0</c> if not applicable.
        /// </summary>
        public char Char
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the shift key is pressed.
        /// </summary>
        public bool Shift 
        { 
            get { return 0 != (Keys.Shift & Modifiers); } 
        }

        /// <summary>
        /// Whether the <c>Control</c> key is pressed.
        /// </summary>
        public bool Control 
        { 
            get { return 0 != (Keys.Control & Modifiers); } 
        }

        /// <summary>
        /// Whether the <c>Alt</c> key is pressed.
        /// </summary>
        public bool Alt 
        { 
            get { return 0 != (Keys.Alt & Modifiers); } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a <see cref="KeyCombination"/> instance representing the key combination
        /// of this keyboard event.
        /// </summary>
        public KeyCombination GetKeyCombination()
        {
            return new KeyCombination(Key, Modifiers);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var character = Char == 0 ? string.Empty : "'{0}'".Fmt(Char);
            return "{0}({1} [{2}]{3})".Fmt(EventType, Key, Modifiers, character);
        }

        #endregion
    }
}

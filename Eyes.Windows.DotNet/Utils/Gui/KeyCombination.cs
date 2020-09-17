namespace Applitools.Utils.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// A combination of a keyboard key and zero or more modifiers.
    /// </summary>
    public class KeyCombination
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="KeyCombination"/> instance.
        /// </summary>
        public KeyCombination()
        {
        }

        /// <summary>
        /// Creates a new <see cref="KeyCombination"/> consisting of the input key.
        /// </summary>
        public KeyCombination(Keys key)
        {
            Key = key;
        }

        /// <summary>
        /// Creates a new <see cref="KeyCombination"/> consisting of the input key and modifiers.
        /// </summary>
        public KeyCombination(Keys key, Keys modifiers) : this(key)
        {
            Modifiers = modifiers;
        }

        /// <summary>
        /// Creates a new <see cref="KeyCombination"/> instance that represents the key 
        /// combination encoded in the input string.
        /// </summary>
        public KeyCombination(string str)
        {
            if (!TryParseKeyCombination(str, out KeyCombination parsed))
            {
                throw new FormatException("Invalid key combination format: {0}".Fmt(str));
            }

            Key = parsed.Key;
            Modifiers = parsed.Modifiers;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Pressed key.
        /// </summary>
        public Keys Key { get; set; }

        /// <summary>
        /// Pressed modifiers (e.g., Shift, Alt, etc.).
        /// </summary>
        public Keys Modifiers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the input key combination.
        /// </summary>
        public static bool TryParseKeyCombination(
            string str,
            out KeyCombination combination)
        {
            ArgumentGuard.NotNull(str, nameof(str));

            combination = new KeyCombination();
            if (string.IsNullOrWhiteSpace(str))
            {
                return true;
            }

            str = str.Replace('+', ',');

            if (!Enum.TryParse(str, out Keys parsed))
            {
                combination = null;
                return false;
            }

            combination.Modifiers = parsed & Keys.Modifiers;
            combination.Key = parsed & ~Keys.Modifiers;

            return true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var combination = new List<Keys>();
            if ((Modifiers & Keys.Alt) != 0)
            {
                combination.Add(Keys.Alt);
            }

            if ((Modifiers & Keys.Control) != 0)
            {
                combination.Add(Keys.Control);
            }

            if ((Modifiers & Keys.Shift) != 0)
            {
                combination.Add(Keys.Shift);
            }

            if (Key != Keys.None)
            {
                combination.Add(Key);
            }

            return StringUtils.Concat(combination, " + ");
        }

        #endregion
    }
}

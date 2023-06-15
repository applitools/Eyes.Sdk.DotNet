using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    /// <summary>
    /// Encapsulates a text trigger.
    /// </summary>
    public class TextTrigger : Trigger
    {
        public TextTrigger(
            Region control,
            string text) : base(control)
        {
            ArgumentGuard.NotNull(text, nameof(text));

            Text = text;
        }

        /// <inheritdoc />
        public override TriggerTypes TriggerType 
        { 
            get { return TriggerTypes.Text; } 
        }

        public string Text
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Text [{0}] '{1}'".Fmt(Control, Text);
        }
    }
}

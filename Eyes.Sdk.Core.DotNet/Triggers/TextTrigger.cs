namespace Applitools
{
    using Applitools.Utils;
    using Applitools.Utils.Geometry;

    /// <summary>
    /// Encapsulates a text trigger.
    /// </summary>
    public class TextTrigger : Trigger
    {
        public TextTrigger(
            Region control,
            string text)
        {
            ArgumentGuard.NotNull(control, nameof(control));
            ArgumentGuard.NotNull(text, nameof(text));

            Control = control;
            Text = text;
        }

        /// <inheritdoc />
        public override TriggerTypes TriggerType 
        { 
            get { return TriggerTypes.Text; } 
        }

        public Region Control
        {
            get;
            private set;
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

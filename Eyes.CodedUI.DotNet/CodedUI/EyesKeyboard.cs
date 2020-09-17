[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI
{
    using System.Drawing;
    using System.Windows.Input;
    using Applitools.Utils;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Region = Applitools.Utils.Geometry.Region;
    
    internal class EyesKeyboard : Keyboard
    {
        #region Fields

        private Eyes eyes_;
        private Keyboard wrapped_;

        #endregion

        #region Constructors

        private EyesKeyboard(Logger logger, Eyes eyes)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(eyes, nameof(eyes));

            Logger = logger;
            this.eyes_ = eyes;
            this.wrapped_ = Keyboard.Instance;
        }

        #endregion

        #region Properties

        protected Logger Logger { get; private set; }

        #endregion

        #region Methods

        public static void Attach(Logger logger, Eyes eyes)
        {
            Keyboard.Instance = new EyesKeyboard(logger, eyes);
        }

        protected override void SendKeysImplementation(
            UITestControl control, 
            string text, 
            ModifierKeys modifierKeys, 
            bool isEncoded, 
            bool isUnicode)
        {
            if (control != null && IsRelevantControl_(control))
            {
                var controlBounds = control.BoundingRectangle;
                var trigger = CreateTrigger_(controlBounds, text);
                if (trigger != null)
                {
                    eyes_.UserInputs.Add(trigger);
                    Logger.Verbose(trigger.ToString());
                }
            }
            if (wrapped_ is EyesKeyboard eyesKeyboard)
            {
                eyesKeyboard.SendKeysImplementation(
                    control, text, modifierKeys, isEncoded, isUnicode);
            }
            else
            {
                base.SendKeysImplementation(
                    control, text, modifierKeys, isEncoded, isUnicode);
            }
        }

        private TextTrigger CreateTrigger_(Rectangle control, string text)
        {
            if (!eyes_.ScreenToLastScreenshot(control, out Region controlRegion))
            {
                return null;
            }

            return new TextTrigger(controlRegion, text);
        }

        private bool IsRelevantControl_(UITestControl control)
        {
            return eyes_.IsRelevantControl(control);
        }

        #endregion
    }
}

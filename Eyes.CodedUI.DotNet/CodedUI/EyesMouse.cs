[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI
{
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Applitools.Utils;
    using Applitools.Utils.Geometry;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Region = Applitools.Utils.Geometry.Region;

    internal class EyesMouse : Mouse
    {
        #region Fields

        private readonly Eyes eyes_;
        private readonly Mouse wrapped_;

        #endregion

        #region Constructors

        private EyesMouse(Logger logger, Eyes eyes)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(eyes, nameof(eyes));

            Logger = logger;
            eyes_ = eyes;
            wrapped_ = Mouse.Instance;
        }

        #endregion

        #region Properties

        protected Logger Logger { get; private set; }

        #endregion

        #region Methods

        public static void Attach(Logger logger, Eyes eyes)
        {
            Mouse.Instance = new EyesMouse(logger, eyes);
        }

        protected override void ClickImplementation(
            UITestControl control, MouseButtons button, ModifierKeys modifierKeys, Point where)
        {
            MouseAction action;
            switch (button)
            {
                case MouseButtons.Left: action = MouseAction.Click; break;
                case MouseButtons.Right: action = MouseAction.RightClick; break;
                default:action = MouseAction.None; break;
            }

            AddMouseTrigger_(control, action, where);

            if (wrapped_ is EyesMouse eyesMouse)
            {
                eyesMouse.ClickImplementation(control, button, modifierKeys, where);
            }
            else
            {
                base.ClickImplementation(control, button, modifierKeys, where);
            }
        }

        protected override void DoubleClickImplementation(
            UITestControl control, MouseButtons button, ModifierKeys modifierKeys, Point where)
        {
            AddMouseTrigger_(control, MouseAction.DoubleClick, where);

            if (wrapped_ is EyesMouse eyesMouse)
            {
                eyesMouse.DoubleClickImplementation(control, button, modifierKeys, where);
            }
            else
            {
                base.DoubleClickImplementation(control, button, modifierKeys, where);
            }
        }

        protected override void StartDraggingImplementation(
            UITestControl control, MouseButtons button, ModifierKeys modifierKeys, Point where)
        {
            AddMouseTrigger_(control, MouseAction.Down, where);

            if (wrapped_ is EyesMouse eyesMouse)
            {
                eyesMouse.StartDraggingImplementation(control, button, modifierKeys, where);
            }
            else
            {
                base.StartDraggingImplementation(control, button, modifierKeys, where);
            }
        }

        protected override void StopDraggingImplementation(
            UITestControl control, Point where, bool isDisplacement)
        {
            AddMouseTrigger_(control, MouseAction.Up, where);

            if (wrapped_ is EyesMouse eyesMouse)
            {
                eyesMouse.StopDraggingImplementation(control, where, isDisplacement);
            }
            else
            {
                base.StopDraggingImplementation(control, where, isDisplacement);
            }
        }

        protected override void MoveImplementation(UITestControl control, Point where)
        {
            AddMouseTrigger_(control, MouseAction.Move, where);

            if (wrapped_ is EyesMouse eyesMouse)
            {
                eyesMouse.MoveImplementation(control, where);
            }
            else
            {
                base.MoveImplementation(control, where);
            }
        }

        private void AddMouseTrigger_(
            UITestControl control, MouseAction mouseAction, Point where)
        {
            if (control == null)
            {
                AddAbsoluteMouseTrigger_(mouseAction, where);
            }
            else
            {
                AddControlMouseTrigger_(control, mouseAction, where);
            }
        }

        private void AddAbsoluteMouseTrigger_(MouseAction mouseAction, Point onScreen)
        {
            var trigger = CreateAbsoluteTrigger_(mouseAction, onScreen);
            if (trigger != null)
            {
                eyes_.UserInputs.Add(trigger);
                Logger.Verbose(trigger.ToString());
            }
        }

        private void AddControlMouseTrigger_(
            UITestControl control, MouseAction mouseAction, Point inControl)
        {
            if (!IsRelevantControl_(control))
            {
                return;
            }

            var controlBounds = control.BoundingRectangle;
            var trigger = CreateTrigger_(mouseAction, controlBounds, inControl);
            if (trigger != null)
            {
                eyes_.UserInputs.Add(trigger);
                Logger.Verbose(trigger.ToString());
            }
        }

        private MouseTrigger CreateAbsoluteTrigger_(
            MouseAction mouseAction, Point onScreen)
        {
            if (!eyes_.ScreenToLastScreenshot(onScreen, out Point onScreenshot))
            {
                return null;
            }

            return new MouseTrigger(
                mouseAction, new Region(0, 0, 0, 0), new Location(onScreenshot));
        }

        private MouseTrigger CreateTrigger_(
            MouseAction mouseAction, Rectangle controlBounds, Point inControl)
        {
            if (!eyes_.ScreenToLastScreenshot(controlBounds, out Region controlRegion))
            {
                return null;
            }

            if (inControl.X == -1 && inControl.Y == -1)
            {
                inControl.X = controlBounds.Width / 2;
                inControl.Y = controlBounds.Height / 2;
            }

            return new MouseTrigger(mouseAction, controlRegion, new Location(inControl));
        }

        private bool IsRelevantControl_(UITestControl control)
        {
            return eyes_.IsRelevantControl(control);
        }

        #endregion
    }
}

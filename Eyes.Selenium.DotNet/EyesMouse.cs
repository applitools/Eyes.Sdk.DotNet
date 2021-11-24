using System;
using System.Drawing;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;

namespace Applitools.Selenium
{
    /// <summary>
    /// A wrapper class for Selenium's Mouse class. It adds saving of mouse events
    /// so they can be sent to the agent later on.
    /// </summary>
    [Obsolete]
    public class EyesMouse : IMouse
    {
        private readonly EyesWebDriver eyesDriver_;
        private readonly IMouse mouse_;

        private Location mouseLocation_;

        public EyesMouse(Logger logger, EyesWebDriver eyesDriver, IMouse mouse)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(eyesDriver, nameof(eyesDriver));
            ArgumentGuard.NotNull(mouse, nameof(mouse));

            Logger = logger;
            eyesDriver_ = eyesDriver;
            mouse_ = mouse;
            mouseLocation_ = new Location(0, 0);
        }

        /// <summary>
        /// Message logger.
        /// </summary>
        protected Logger Logger { get; private set; }

        public void Click(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("Click({0})", location);

            MoveIfNeeded(where);
            AddMouseTrigger_(MouseAction.Click);

            Logger.Verbose("Click(): Location is {0}", mouseLocation_);
            mouse_.Click(where);
        }

        public void DoubleClick(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("DoubleClick({0})", location);

            MoveIfNeeded(where);
            AddMouseTrigger_(MouseAction.DoubleClick);

            Logger.Verbose("DoubleClick(): Location is {0}", mouseLocation_);
            mouse_.DoubleClick(where);
        }

        public void MouseDown(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("MouseDown({0})", location);

            MoveIfNeeded(where);
            AddMouseTrigger_(MouseAction.Down);

            Logger.Verbose("MouseDown(): Location is {0}", mouseLocation_);
            mouse_.MouseDown(where);
        }

        public void MouseUp(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("MouseUp(" + location + ")");

            MoveIfNeeded(where);
            AddMouseTrigger_(MouseAction.Up);

            Logger.Verbose("MouseUp(): Location is " + mouseLocation_);
            mouse_.MouseUp(where);
        }

        public void MouseMove(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("MouseMove(" + location + ")");

            if (location != null)
            {
                mouseLocation_ = new Location(Math.Max(0, location.X), Math.Max(0, location.Y));
                AddMouseTrigger_(MouseAction.Move);
            }

            mouse_.MouseMove(where);
        }

        public void MouseMove(ICoordinates where, int offsetX, int offsetY)
        {
            Location location = Location_(where);
            Logger.Verbose("MouseMove(" + location + ", " + offsetX + ", " + offsetY + ")");

            int newX, newY;
            if (location != null)
            {
                newX = location.X + offsetX;
                newY = location.Y + offsetY;
            }
            else
            {
                newX = mouseLocation_.X + offsetX;
                newY = mouseLocation_.Y + offsetY;
            }

            if (newX < 0)
            {
                newX = 0;
            }

            if (newY < 0)
            {
                newY = 0;
            }

            mouseLocation_ = new Location(newX, newY);
            AddMouseTrigger_(MouseAction.Move);

            mouse_.MouseMove(where, offsetX, offsetY);
        }

        public void ContextClick(ICoordinates where)
        {
            Location location = Location_(where);
            Logger.Verbose("ContextClick(" + location + ")");

            MoveIfNeeded(where);
            AddMouseTrigger_(MouseAction.RightClick);

            Logger.Verbose("ContextClick(): Location is " + mouseLocation_);
            mouse_.ContextClick(where);
        }

        /// <summary>
        /// Moves the mouse according to the coordinates, if required.
        /// </summary>
        /// <param name="where">The coordinates to move to. If null,
        /// mouse position does not changes. </param>
        protected void MoveIfNeeded(ICoordinates where)
        {
            if (where != null)
            {
                MouseMove(where);
            }
        }

        private static Location Location_(ICoordinates where)
        {
            if (where == null)
            {
                return null;
            }

            Point p = where.LocationInDom;
            return new Location(p.X, p.Y);
        }

        private void AddMouseTrigger_(MouseAction action)
        {
            eyesDriver_.UserActionsEyes.AddMouseTrigger(action, null, mouseLocation_.ToPoint());
        }
    }
}

using Applitools.Utils;
using System.Drawing;

namespace Applitools.Selenium
{
    internal class NullPositionProvider : IPositionProvider
    {
        public Point GetCurrentPosition()
        {
            return Point.Empty;
        }

        public Size GetEntireSize()
        {
            return Size.Empty;
        }

        public PositionMemento GetState()
        {
            return new NullPositionMemento();
        }

        public void RestoreState(PositionMemento state) { }

        public Point SetPosition(Point pos)
        {
            return pos;
        }
    }
}
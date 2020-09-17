namespace Applitools.Utils
{
    using System.Drawing;

    public class ScrollPositionMemento : PositionMemento
    {
        #region Properties

        public override int X { get { return Position.X; } }
        public override int Y { get { return Position.Y; } }

        public Point Position { get; private set; }

        #endregion


        #region Constructors

        public ScrollPositionMemento(Point p)
        {
            Position = p;
        }

        #endregion
    }
}

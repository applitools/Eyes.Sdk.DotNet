using System.Drawing;

namespace Applitools.Utils
{
    public class CssTranslatePositionMemento : PositionMemento
    {
        #region Properties

        public string Transform { get; private set; }

        public override int X { get { return Position.X; } }
        public override int Y { get { return Position.Y; } }

        public Point Position { get; set; }

        #endregion

        #region Constructors

        public CssTranslatePositionMemento(string transform, Point position)
        {
            Transform = transform;
            Position = position;
        }

        #endregion
    }
}

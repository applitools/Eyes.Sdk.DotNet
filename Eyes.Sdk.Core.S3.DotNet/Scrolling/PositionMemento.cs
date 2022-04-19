namespace Applitools.Utils
{
    public abstract class PositionMemento
    {
        public abstract int X { get; }
        public abstract int Y { get; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}

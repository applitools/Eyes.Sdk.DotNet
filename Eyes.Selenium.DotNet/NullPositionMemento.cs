using Applitools.Utils;

namespace Applitools.Selenium
{
    internal class NullPositionMemento : PositionMemento
    {
        public override int X => 0;

        public override int Y => 0;
    }
}
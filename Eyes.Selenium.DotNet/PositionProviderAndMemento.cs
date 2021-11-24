using Applitools.Utils;
using System.Drawing;

namespace Applitools.Selenium
{
    internal class PositionProviderAndMemento
    {

        public PositionProviderAndMemento(IPositionProvider positionProvider, PositionMemento positionMemento, FrameChain frames, Point currentScrollPosition)
        {
            Provider = positionProvider;
            Memento = positionMemento;
            Frames = frames;
            CurrentScrollPosition = currentScrollPosition;
        }

        public IPositionProvider Provider { get; private set; }
        public PositionMemento Memento { get; private set; }
        public FrameChain Frames { get; private set; }
        public Point CurrentScrollPosition { get; set; }

        public void RestoreState()
        {
            Provider.RestoreState(Memento);
        }
    }
}
using Applitools.VisualGrid.Model;

namespace Applitools.VisualGrid
{
    public interface IDebugResourceWriter
    {
        void Write(RGridResource value);
        void Write(FrameData value);
    }
}
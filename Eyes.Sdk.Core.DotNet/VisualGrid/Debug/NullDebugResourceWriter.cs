using Applitools.VisualGrid.Model;

namespace Applitools.VisualGrid
{
    public class NullDebugResourceWriter : IDebugResourceWriter
    {
        public static NullDebugResourceWriter Instance { get; } = new NullDebugResourceWriter();

        public void Write(RGridResource value)
        {
            // Do nothing
        }

        public void Write(FrameData value)
        {
            // Do nothing
        }
    }
}

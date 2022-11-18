namespace Applitools
{
    public class Config
    {
        public IOpenConfig Open { get; }
        public IScreenshotConfig Screenshot { get; }
        public ICheckConfig Check { get; }
        public ICloseConfig Close { get; }

        public Config(IOpenConfig open, IScreenshotConfig screenshot, ICheckConfig check, ICloseConfig close)
        {
            Open = open;
            Screenshot = screenshot;
            Check = check;
            Close = close;
        }
    }
}
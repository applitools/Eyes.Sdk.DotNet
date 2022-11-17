namespace Applitools
{
    public class Config
    {
        public IOpenConfig Open { get; }
        public ICheckConfig Check { get; }
        public ICloseConfig Close { get; }

        public Config(IOpenConfig open, ICheckConfig check, ICloseConfig close)
        {
            Open = open;
            Check = check;
            Close = close;
        }
    }
}
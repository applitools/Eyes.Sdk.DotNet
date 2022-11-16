namespace Applitools
{
    public class Config
    {
        public IOpenConfig OpenConfig { get; }
        public ICheckConfig CheckConfig { get; }
        public ICloseConfig CloseConfig { get; }

        public Config(IOpenConfig openConfig, ICheckConfig checkConfig, ICloseConfig closeConfig)
        {
            OpenConfig = openConfig;
            CheckConfig = checkConfig;
            CloseConfig = closeConfig;
        }
    }
}
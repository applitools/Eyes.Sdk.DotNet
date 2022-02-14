namespace Applitools.Utils
{
    public static class ScrollPositionProviderFactory
    {
        public static IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor)
        {
            switch (stitchMode)
            {
                case StitchModes.Scroll: return new ScrollPositionProvider(logger, executor);
                case StitchModes.CSS: return new CssTranslatePositionProvider(logger, executor);
                default:
                    return null;
            }
        }
    }
}

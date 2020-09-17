using Applitools.Utils;
using OpenQA.Selenium;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Applitools.Selenium.Scrolling
{
    public static class SeleniumPositionProviderFactory
    {
        private static ConcurrentDictionary<string, IPositionProvider> positionProviders_ = new ConcurrentDictionary<string, IPositionProvider>();
        public static IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, IWebElement scrollRootElement, UserAgent userAgent = null)
        {
            return positionProviders_.GetOrAdd(scrollRootElement + "_" + stitchMode,
                (e) => CreatePositionProvider(logger, stitchMode, executor, scrollRootElement, userAgent));
        }

        public static IPositionProvider CreatePositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, IWebElement scrollRootElement, UserAgent userAgent = null)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(executor, nameof(executor));

            switch (stitchMode)
            {
                case StitchModes.CSS: return new CssTranslatePositionProvider(logger, executor, scrollRootElement);
                case StitchModes.Scroll:
                    if (userAgent != null && userAgent.Browser.Equals(BrowserNames.Edge))
                        return new EdgeBrowserScrollPositionProvider(logger, executor, scrollRootElement);
                    //else
                    return new ScrollPositionProvider(logger, executor, scrollRootElement);
                default:
                    return null;
            }
        }

        public static IPositionProvider TryGetPositionProviderForElement(IWebElement scrollRootElement, StitchModes stitchMode)
        {
            positionProviders_.TryGetValue(scrollRootElement + "_" + stitchMode, out IPositionProvider positionProvider);
            return positionProvider;
        }
    }
}

using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Applitools.Selenium.Scrolling
{
    public static class SeleniumPositionProviderFactory
    {
        private static readonly ConcurrentDictionary<string, IPositionProvider> positionProviders_ = new ConcurrentDictionary<string, IPositionProvider>();
        public static IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, RemoteWebDriver driver, IWebElement scrollRootElement, UserAgent userAgent = null)
        {
            string id = scrollRootElement.GetHashCode() + "_" + stitchMode + "_" + driver.SessionId;
            logger.Log(TraceLevel.Debug, Stage.General, new { PositionProviderId = id });
            return positionProviders_.GetOrAdd(id,
                (e) => CreatePositionProvider(logger, stitchMode, executor, scrollRootElement, id, userAgent));
        }

        private static IPositionProvider CreatePositionProvider(Logger logger, StitchModes stitchMode,
            IEyesJsExecutor executor, IWebElement scrollRootElement, string positionProviderId, UserAgent userAgent = null)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(executor, nameof(executor));

            logger.Log(TraceLevel.Debug, Stage.General,
                new { message = "position provider does not exist in dictionary", positionProviderId });

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

        public static IPositionProvider TryGetPositionProviderForElement(IWebElement scrollRootElement, StitchModes stitchMode,
            RemoteWebDriver driver, Logger logger)
        {
            string id = scrollRootElement.GetHashCode() + "_" + stitchMode + "_" + driver.SessionId;
            logger.Log(TraceLevel.Debug, Stage.General, new { PositionProviderId = id });
            positionProviders_.TryGetValue(id, out IPositionProvider positionProvider);
            return positionProvider;
        }
    }
}

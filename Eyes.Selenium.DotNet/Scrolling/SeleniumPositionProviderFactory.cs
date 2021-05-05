using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Concurrent;

namespace Applitools.Selenium.Scrolling
{
    public class SeleniumPositionProviderFactory
    {
        internal static SeleniumPositionProviderFactory GetInstance(SeleniumEyes eyes)
        {
            if (eyes.PositionProviderFactory == null)
            {
                eyes.PositionProviderFactory = new SeleniumPositionProviderFactory();
            }
            return eyes.PositionProviderFactory;
        }

        private readonly ConcurrentDictionary<string, IPositionProvider> positionProviders_ = new ConcurrentDictionary<string, IPositionProvider>();
        public IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, IWebElement scrollRootElement, UserAgent userAgent = null)
        {
            string id = scrollRootElement.GetHashCode() + "_" + stitchMode;
            logger.Log(TraceLevel.Debug, Stage.General, new { PositionProviderId = id });
            return positionProviders_.GetOrAdd(id,
                (e) => CreatePositionProvider(logger, stitchMode, executor, scrollRootElement, id, userAgent));
        }

        private IPositionProvider CreatePositionProvider(Logger logger, StitchModes stitchMode,
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

        public IPositionProvider TryGetPositionProviderForElement(IWebElement scrollRootElement, StitchModes stitchMode,
            RemoteWebDriver driver, Logger logger)
        {
            string id = scrollRootElement.GetHashCode() + "_" + stitchMode + "_" + driver.SessionId;
            logger.Log(TraceLevel.Debug, Stage.General, new { PositionProviderId = id });
            positionProviders_.TryGetValue(id, out IPositionProvider positionProvider);
            return positionProvider;
        }
    }
}

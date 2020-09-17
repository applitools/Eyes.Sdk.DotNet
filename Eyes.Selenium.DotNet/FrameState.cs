using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium
{
    /// <summary>
    /// A data class to hold position of a frame in both Scroll and CSS.
    /// </summary>
    internal class FrameState
    {
        public FrameState(EyesWebDriver driver, IWebElement scrolledElement, IPositionProvider cssPositionProvider, IPositionProvider scrollPositionProvider, string overflow, FrameChain frameChain)
        {
            driver_ = driver;
            scrolledElement_ = scrolledElement;
            
            cssPositionProvider_ = cssPositionProvider;
            cssMemento_ = cssPositionProvider_.GetState();
            
            scrollPositionProvider_ = scrollPositionProvider;
            scrollMemento_ = scrollPositionProvider_.GetState();

            overflow_ = overflow;
            frameChain_ = frameChain;
        }

        private EyesWebDriver driver_;
        private IWebElement scrolledElement_;
        private string overflow_;
        private FrameChain frameChain_;
        private IPositionProvider cssPositionProvider_;
        private IPositionProvider scrollPositionProvider_;
        private PositionMemento cssMemento_;
        private PositionMemento scrollMemento_;

        public void Restore()
        {
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();
            switchTo.Frames(frameChain_);

            cssPositionProvider_.RestoreState(cssMemento_);
            scrollPositionProvider_.RestoreState(scrollMemento_);
            driver_.ExecuteScript($"arguments[0].style.overflow='{overflow_}'", scrolledElement_);
        }

        internal static FrameState GetCurrentFrameState(EyesWebDriver driver, IWebElement scrolledElement)
        {
            IJavaScriptExecutor jsExecutor = driver;
            Logger logger = driver.Eyes.Logger;
            UserAgent userAgent = UserAgent.ParseUserAgentString(driver.GetUserAgent());
            
            IPositionProvider cssPositionProvider = SeleniumPositionProviderFactory.GetPositionProvider(logger, StitchModes.CSS, driver, scrolledElement, userAgent);
            IPositionProvider scrollPositionProvider = SeleniumPositionProviderFactory.GetPositionProvider(logger, StitchModes.Scroll, driver, scrolledElement, userAgent);
            string overflow = (string)jsExecutor.ExecuteScript("return arguments[0].style.overflow", scrolledElement);
            FrameChain frameChain = driver.GetFrameChain().Clone();
            FrameState frameState = new FrameState(driver, scrolledElement, cssPositionProvider, scrollPositionProvider, overflow, frameChain);
            return frameState;
        }
    }
}
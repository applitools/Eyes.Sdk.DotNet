using System;

namespace Applitools.Selenium
{
    internal class CaretVisibilityProvider
    {
        private Logger logger_;
        private EyesWebDriver driver_;
        private Configuration configuration_;

        object activeElement_ = null;
        private FrameChain frameChain_;

        private const string HIDE_CARET = "var activeElement = document.activeElement; activeElement && activeElement.blur(); return activeElement;";

        public CaretVisibilityProvider(Logger logger, EyesWebDriver driver, Configuration configuration)
        {
            logger_ = logger;
            driver_ = driver;
            configuration_ = configuration;
        }

        internal void HideCaret()
        {
            if (configuration_.HideCaret)
            {
                frameChain_ = driver_.GetFrameChain().Clone();
                logger_.Verbose("Hiding caret. driver_.FrameChain.Count: {0}", frameChain_.Count);
                activeElement_ = driver_.ExecuteScript(HIDE_CARET);
            }
        }

        internal void RestoreCaret()
        {
            if (configuration_.HideCaret && activeElement_ != null)
            {
                logger_.Verbose("Restoring caret. driver_.FrameChain.Count: {0}", driver_.GetFrameChain().Count);
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(frameChain_);
                driver_.ExecuteScript("arguments[0].focus();", activeElement_);
            }
        }
    }
}
using OpenQA.Selenium;
using System;

namespace Applitools.Selenium
{
    internal class ScrollbarsVisibilityProvider
    {
        private Logger logger_;
        private EyesWebDriver driver_;
        private Configuration configuration_;

        private string originalOverflow_;
        private IWebElement scrollRootElement_;
        private FrameChain frameChain_;

        public ScrollbarsVisibilityProvider(Logger logger, EyesWebDriver driver, Configuration configuration)
        {
            logger_ = logger;
            driver_ = driver;
            configuration_ = configuration;
        }

        internal void TryHideScrollbars(bool stitchContent, IWebElement scrollRootElement)
        {
            if (configuration_.HideScrollbars)
            {
                scrollRootElement_ = scrollRootElement;
                frameChain_ = driver_.GetFrameChain().Clone();
                if (frameChain_.Count > 0)
                {
                    FrameChain fc = frameChain_.Clone();

                    // for the target frame, we only wish to remove scrollbars when in "fully" mode.
                    if (stitchContent)
                    {
                        Frame frame = fc.Peek();
                        frame.HideScrollbars(driver_);
                    }

                    RemoveScrollbarsFromParentFrames_(logger_, fc, driver_);
                }
                else
                {
                    logger_.Verbose("hiding scrollbars of element: {0}", scrollRootElement);
                    originalOverflow_ = EyesSeleniumUtils.SetOverflow("hidden", driver_, scrollRootElement);
                }

                logger_.Verbose("switching back to original frame");
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(frameChain_);

                logger_.Verbose("done hiding scrollbars.");
            }
            else
            {
                logger_.Verbose("no need to hide scrollbars.");
            }
        }
        private static void RemoveScrollbarsFromParentFrames_(Logger logger, FrameChain fc, EyesWebDriver driver)
        {
            logger.Verbose("enter");
            driver.SwitchTo().ParentFrame();
            fc.Pop();
            Frame frame = fc.Peek();

            while (fc.Count > 0)
            {
                logger.Verbose("fc.Count = {0}", fc.Count);
                frame.HideScrollbars(driver);
                driver.SwitchTo().ParentFrame();
                fc.Pop();
                frame = fc.Peek();
            }

            logger.Verbose("exit");
        }

        internal void TryRestoreScrollbars()
        {
            if (configuration_.HideScrollbars)
            {
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(frameChain_);
                FrameChain fc = frameChain_.Clone();
                if (fc.Count > 0)
                {
                    while (fc.Count > 0)
                    {
                        Frame frame = fc.Pop();
                        frame.ReturnToOriginalOverflow(driver_);
                        EyesWebDriverTargetLocator.ParentFrame(logger_, driver_.RemoteWebDriver.SwitchTo(), fc);
                    }
                }
                else
                {
                    if (originalOverflow_ != null)
                    {
                        logger_.Verbose("returning overflow of element to its original value: {0}", scrollRootElement_);
                        EyesSeleniumUtils.SetOverflow(originalOverflow_, driver_, scrollRootElement_);
                    }
                }
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(frameChain_);
                logger_.Verbose("done restoring scrollbars.");
            }
            else
            {
                logger_.Verbose("no need to restore scrollbars.");
            }
        }
    }
}
using Applitools.Selenium.Fluent;
using Applitools.Utils;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Selenium
{
    internal class PageState
    {
        private EyesWebDriver driver_;
        private Logger logger_;
        private List<FrameState> frameStates_;
        private FrameChain originalFrameChain_;
        private StitchModes stitchMode_;
        private UserAgent userAgent_;

        public PageState(Logger logger, EyesWebDriver driver, StitchModes stitchMode, UserAgent userAgent)
        {
            logger_ = logger;
            driver_ = driver;
            stitchMode_ = stitchMode;
            userAgent_ = userAgent;
        }

        internal void PreparePage(ISeleniumCheckTarget seleniumCheckTarget, Configuration config, IWebElement userDefinedSRE)
        {
            frameStates_ = new List<FrameState>();
            originalFrameChain_ = driver_.GetFrameChain().Clone();

            if (seleniumCheckTarget.GetTargetElement() != null ||
                seleniumCheckTarget.GetTargetSelector() != null ||
                seleniumCheckTarget.GetFrameChain().Count > 0)
            {
                PrepareParentFrames_();
            }

            SaveCurrentFrameState_(frameStates_, driver_, userDefinedSRE);
            TryHideScrollbarsInFrame(config, driver_, userDefinedSRE);

            int switchedToFrameCount = SwitchToTargetFrame_(seleniumCheckTarget, config, frameStates_, userDefinedSRE);
            logger_.Verbose(nameof(switchedToFrameCount) + ": {0}", switchedToFrameCount);
        }

        private void PrepareParentFrames_()
        {
            if (originalFrameChain_.Count == 0) return;

            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();
            FrameChain fc = originalFrameChain_.Clone();
            while (fc.Count > 0)
            {
                switchTo.ParentFrame();
                Frame currentFrame = fc.Pop();
                IWebElement rootElement = EyesSeleniumUtils.GetCurrentFrameScrollRootElement(driver_, null);
                SaveCurrentFrameState_(frameStates_, driver_, rootElement);
                MaximizeTargetFrameInCurrentFrame_(currentFrame.Reference, rootElement);
            }
            frameStates_.Reverse();
            switchTo.Frames(originalFrameChain_);
        }

        internal void RestorePageState()
        {
            frameStates_.Reverse();
            foreach (FrameState state in frameStates_)
            {
                state.Restore();
            }
            ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrameChain_);
        }

        internal int SwitchToTargetFrame_(ISeleniumCheckTarget checkTarget, Configuration config, 
            List<FrameState> frameStates, IWebElement userDefinedSRE)
        {
            IList<FrameLocator> frameChain = checkTarget.GetFrameChain();
            foreach (FrameLocator frameLocator in frameChain)
            {
                IWebElement frameElement = EyesSeleniumUtils.FindFrameByFrameCheckTarget(frameLocator, driver_);
                MaximizeTargetFrameInCurrentFrame_(frameElement, userDefinedSRE);
                SwitchToFrame_(frameElement, frameLocator, config, frameStates);
            }
            return frameChain.Count;
        }

        private void MaximizeTargetFrameInCurrentFrame_(IWebElement frameElement, IWebElement userDefinedSRE)
        {
            IWebElement currentFrameSRE = EyesSeleniumUtils.GetCurrentFrameScrollRootElement(driver_, userDefinedSRE);

            IPositionProvider positionProvider = SeleniumEyes.GetPositionProviderForScrollRootElement_(logger_, driver_,
                stitchMode_, userAgent_, currentFrameSRE);

            Rectangle frameRect = EyesRemoteWebElement.GetClientBoundsWithoutBorders(frameElement, driver_, logger_);
            if (stitchMode_ == StitchModes.Scroll)
            {
                Point pageScrollPosition = positionProvider.GetCurrentPosition();
                frameRect.Offset(pageScrollPosition);
            }
            positionProvider.SetPosition(frameRect.Location);
        }

        private void SwitchToFrame_(IWebElement frameElement,
            ISeleniumFrameCheckTarget frameTarget, Configuration config, List<FrameState> frameStates)
        {
            ITargetLocator switchTo = driver_.SwitchTo();

            switchTo.Frame(frameElement);
            IWebElement rootElement = SeleniumEyes.GetScrollRootElementFromSREContainer(frameTarget, driver_);
            Frame frame = driver_.GetFrameChain().Peek();
            frame.ScrollRootElement = rootElement;
            SaveCurrentFrameState_(frameStates, driver_, rootElement);
            TryHideScrollbarsInFrame(config, driver_, rootElement);
            frame.ScrollRootElementInnerBounds = EyesRemoteWebElement.GetClientBoundsWithoutBorders(rootElement, driver_, logger_);
        }

        private static void TryHideScrollbarsInFrame(Configuration config, EyesWebDriver driver, IWebElement rootElement)
        {
            if (config.HideScrollbars)
            {
                EyesSeleniumUtils.SetOverflow("hidden", driver, rootElement);
            }
        }

        private static void SaveCurrentFrameState_(List<FrameState> frameStates, EyesWebDriver driver, IWebElement rootElement)
        {
            FrameState frameState = FrameState.GetCurrentFrameState(driver, rootElement);
            frameStates.Add(frameState);
        }
    }
}
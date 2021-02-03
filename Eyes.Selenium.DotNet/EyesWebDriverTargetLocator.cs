using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Applitools.Selenium
{
    internal class EyesWebDriverTargetLocator : ITargetLocator
    {
        private readonly Logger logger_;
        private readonly EyesWebDriver driver_;
        private readonly IPositionProvider scrollPositionProvider_;
        private readonly ITargetLocator targetLocator_;

        private readonly SeleniumJavaScriptExecutor jsExecutor_;
        private PositionMemento defaultContentPositionMemento_;

        public EyesWebDriverTargetLocator(EyesWebDriver driver, Logger logger, ITargetLocator targetLocator)
        {
            driver_ = driver;
            logger_ = logger;
            targetLocator_ = targetLocator;
            jsExecutor_ = new SeleniumJavaScriptExecutor(driver_);
            if (driver.Eyes != null)
            {
                scrollPositionProvider_ = SeleniumPositionProviderFactory.GetPositionProvider(
                    logger_, StitchModes.Scroll, jsExecutor_, driver.Eyes.GetCurrentFrameScrollRootElement(), driver.Eyes.userAgent_);
            }
        }

        #region ITargetLocator members

        public IWebElement ActiveElement()
        {
            IWebElement element = targetLocator_.ActiveElement();
            if (!(element is RemoteWebElement remoteWebElement))
            {
                throw new EyesException("Not a remote web element!");
            }
            return new EyesRemoteWebElement(logger_, driver_, remoteWebElement);
        }

        public IAlert Alert()
        {
            return targetLocator_.Alert();
        }

        public IWebDriver DefaultContent()
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            driver_.GetFrameChain().Clear();
            targetLocator_.DefaultContent();
            return driver_;
        }

        public IWebDriver Frame(IWebElement frameElement)
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            WillSwitchToFrame_(frameElement);
            targetLocator_.Frame(frameElement);
            return driver_;
        }

        public IWebDriver Frame(string nameOrId)
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called, new { nameOrId });
            // Finding the target element so we can report it.
            // We use find elements(plural) to avoid exception when the element
            // is not found.
            ReadOnlyCollection<IWebElement> frames = driver_.FindElementsByName(nameOrId);
            if (frames.Count == 0)
            {
                // If there are no frames by that name, we'll try the id
                frames = driver_.FindElementsById(nameOrId);
                if (frames.Count == 0)
                {
                    // No such frame, bummer
                    throw new NoSuchFrameException($"No frame with name or id '{nameOrId}' exists!");
                }
            }
            WillSwitchToFrame_(frames[0]);
            targetLocator_.Frame(nameOrId);
            return driver_;
        }

        public IWebDriver Frame(int frameIndex)
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called, new { frameIndex });
            // Finding the target element so and reporting it using onWillSwitch.
            ReadOnlyCollection<IWebElement> frames = driver_.FindElements(By.CssSelector("frame, iframe"));
            if (frameIndex >= frames.Count)
            {
                throw new NoSuchFrameException(string.Format("Frame index [{0}] is invalid!", frameIndex));
            }
            IWebElement targetFrame = frames[frameIndex];
            WillSwitchToFrame_(targetFrame);
            targetLocator_.Frame(frameIndex);
            return driver_;
        }

        private void WillSwitchToFrame_(IWebElement targetFrame)
        {
            ArgumentGuard.NotNull(targetFrame, nameof(targetFrame));
            if (!(targetFrame is EyesRemoteWebElement eyesFrame))
            {
                eyesFrame = new EyesRemoteWebElement(logger_, driver_, targetFrame);
            }

            Size ds = targetFrame.Size;

            SizeAndBorders sizeAndBorders = eyesFrame.SizeAndBorders;
            RectangularMargins borderWidths = sizeAndBorders.Borders;
            Size clientSize = sizeAndBorders.Size;

            Rectangle bounds = eyesFrame.GetClientBounds();

            Point contentLocation = new Point(bounds.X + borderWidths.Left, bounds.Y + borderWidths.Top);
            Point originalLocation = eyesFrame.ScrollPosition;

            Frame frame = new Frame(logger_, targetFrame,
                    contentLocation,
                    new Size(ds.Width, ds.Height),
                    clientSize,
                    originalLocation,
                    bounds,
                    borderWidths,
                    jsExecutor_);

            driver_.GetFrameChain().Push(frame);
        }

        public IWebDriver ParentFrame()
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called, 
                new { frameChainSize = driver_.GetFrameChain().Count });
            if (driver_.GetFrameChain().Count != 0)
            {
                Frame frame = driver_.GetFrameChain().Pop();
                //frame.ReturnToOriginalPosition(driver_);
                ParentFrame(logger_, targetLocator_, driver_.GetFrameChain());
            }
            return driver_;
        }


        public static void ParentFrame(Logger logger, ITargetLocator targetLocator, FrameChain frameChainToParent)
        {
            logger.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            try
            {
                targetLocator.ParentFrame();
            }
            catch
            {
                targetLocator.DefaultContent();
                foreach (Frame frame in frameChainToParent)
                {
                    targetLocator.Frame(frame.Reference);
                }
            }
        }

        public IWebDriver Window(string windowName)
        {
            driver_.GetFrameChain().Clear();
            targetLocator_.Window(windowName);
            return driver_;
        }
        #endregion

        /// <summary>
        /// Switches into every frame in the frame chain. This is used as way to
        /// switch into nested frames (while considering scroll) in a single call.
        /// </summary>
        /// <param name="frameChain">The path to the frame to switch to.</param>
        /// <returns>The WebDriver with the switched context.</returns>
        public IWebDriver Frames(FrameChain frameChain)
        {
            FrameChain currentFrameChain = driver_.GetFrameChain();
            if (frameChain == currentFrameChain)
            {
                throw new ArgumentException("given " + nameof(frameChain) + " is the same instance as the one in the driver! Perhaps `.Clone()` is missing?");
            }

            if (FrameChain.IsSameFrameChain(currentFrameChain, frameChain))
            {
                logger_.Log(TraceLevel.Debug, null, Stage.General, 
                    new { message = "given frame chain equals current frame chain. returning." });
                return driver_;
            }

            logger_.Log(TraceLevel.Debug, null, Stage.General, new { frameChainSize = frameChain?.Count ?? 0 });
            if (currentFrameChain.Count > 0)
            {
                this.DefaultContent();
            }

            if (frameChain != null && frameChain.Count > 0)
            {
                foreach (Frame frame in frameChain)
                {
                    this.Frame(frame.Reference);
                    Frame newFrame = driver_.GetFrameChain().Peek();
                    newFrame.ScrollRootElement = frame.ScrollRootElement;
                }
            }
            return driver_;
        }

        /// <summary>
        /// Switches into every frame in the list. This is used as way to switch into nested frames in a single call.
        /// </summary>
        /// <param name="framesPath">The path to the frame to check. This is a list of frame names/IDs (where each frame is nested in the previous frame).</param>
        /// <returns>The WebDriver with the switched context.</returns>
        public IWebDriver Frames(string[] framesPath)
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            ITargetLocator targetLocator = driver_.SwitchTo();
            foreach (string frameNameOrId in framesPath)
            {
                targetLocator.Frame(frameNameOrId);
            }
            return driver_;
        }

        public IWebDriver FramesDoScroll(FrameChain frameChain)
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            ITargetLocator targetLocator = driver_.SwitchTo();
            targetLocator.DefaultContent();
            IPositionProvider scrollProvider = new ScrollPositionProvider(logger_, jsExecutor_, driver_.Eyes.GetCurrentFrameScrollRootElement());
            defaultContentPositionMemento_ = scrollProvider.GetState();
            foreach (Frame frame in frameChain)
            {
                Point frameLocation = frame.Location;
                scrollProvider.SetPosition(frameLocation);
                targetLocator.Frame(frame.Reference);
                Frame newFrame = driver_.GetFrameChain().Peek();
                newFrame.ScrollRootElement = frame.ScrollRootElement;
                newFrame.ScrollRootElementInnerBounds = frame.ScrollRootElementInnerBounds;
            }

            return driver_;
        }

        internal void ResetScroll()
        {
            logger_.Log(TraceLevel.Debug, null, Stage.General, StageType.Called);
            if (defaultContentPositionMemento_ != null)
            {
                scrollPositionProvider_?.RestoreState(defaultContentPositionMemento_);
            }
        }
    }

}

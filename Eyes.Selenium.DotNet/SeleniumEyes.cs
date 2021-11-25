using Applitools.Capture;
using Applitools.Cropping;
using Applitools.Fluent;
using Applitools.Selenium.Capture;
using Applitools.Selenium.Common;
using Applitools.Selenium.Fluent;
using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using Applitools.Utils.Cropping;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CssTranslatePositionProvider = Applitools.Selenium.Scrolling.CssTranslatePositionProvider;
using Region = Applitools.Utils.Geometry.Region;
using ScrollPositionProvider = Applitools.Selenium.Scrolling.ScrollPositionProvider;

namespace Applitools.Selenium
{
    /// <summary>
    /// Applitools Eyes Selenium DotNet API.
    /// </summary>
    internal class SeleniumEyes : SeleniumEyesBase<IWebElement, IWebDriver, ISeleniumCheckTarget>, IEyes, ISeleniumEyes, IUserActionsEyes, ISeleniumEyesCommon
    {
        #region Fields

        private EyesWebDriver driver_;
        private bool dontGetTitle_;

        private readonly SeleniumPositionProviderFactory positionProviderFactory_ = new SeleniumPositionProviderFactory();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public SeleniumEyes(ISeleniumConfigurationProvider configurationProvider, Uri serverUrl, ClassicRunner runner)
            : base(configurationProvider, serverUrl, runner)
        {
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public SeleniumEyes(ISeleniumConfigurationProvider configurationProvider, ClassicRunner runner)
            : base(configurationProvider, runner)
        {
        }
      
        protected override IWebDriver Driver => driver_;
        
        protected override IWebElement GetDefaultRootElement() => EyesSeleniumUtils.GetDefaultRootElement(driver_);

        #endregion Constructors

        #region Methods

        protected override void InitDriverImpl_(IWebDriver driver)
        {
            if (driver is RemoteWebDriver remoteDriver)
            {
                driver_ = new EyesWebDriver(Logger, this, this, remoteDriver);
            }
            else if (driver is EyesWebDriver eyesDriver)
            {
                driver_ = eyesDriver;
            }
            else
            {
                string errMsg = $"Driver is not a RemoteWebDriver ({driver.GetType().Name})";
                Logger.Log(TraceLevel.Error, Stage.Open, new { errMsg });
                throw new EyesException(errMsg);
            }
        }

        protected override void EnsureCorrectFrame(CheckState<IWebElement> state)
        {
            FrameChain frameChain = driver_.GetFrameChain();
            if (state.FrameToSwitchTo != null &&
                (frameChain.Count == 0 ||
                state.FrameToSwitchTo != frameChain.Last().ReferenceObj))
            {
                driver_.SwitchTo().Frame(state.FrameToSwitchTo);
            }
        }

        protected override EyesScreenshot CreateScreenshot(Bitmap image) =>
            new EyesWebDriverScreenshot(Logger, driver_, image);

        protected override IPositionProvider CreateScrollPositionProvider(IWebElement scrollRootElement) =>
            new ScrollPositionProvider(Logger, driver_, scrollRootElement);

        protected override IPositionProvider CreateElementPositionProvider(IWebElement scrollRootElement) =>
            new ElementPositionProvider(Logger, driver_, scrollRootElement);

        protected override IPositionProvider CreateCssTranslatePositionProvider(IWebElement scrollRootElement) =>
            new CssTranslatePositionProvider(Logger, driver_, scrollRootElement);

        /// <summary>
        /// Get the viewport size using the driver. Call this method if for some reason you don't 
        /// want to call <see cref="Open(IWebDriver, string, string, Size)"/> (or one of its variants) yet.
        /// </summary>
        /// <param name="driver">The driver to use for getting the viewport.</param>
        /// <returns>The viewport size of the current context.</returns>
        public static Size GetViewportSize(IWebDriver driver)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            return EyesSeleniumUtils.GetViewportSize(new Logger(), driver);
        }

        /// <summary>
        /// Set the viewport size using the driver. Call this method if for some reason you don't 
        /// want to call <see cref="Open(IWebDriver, string, string, Size)"/> (or one of its variants) yet.
        /// </summary>
        /// <param name="driver">The driver to use for setting the viewport.</param>
        /// <param name="size">The required viewport size.</param>
        public static void SetViewportSize(IWebDriver driver, Size size)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.IsValidState(!size.IsEmpty, "Empty viewport size");
            EyesSeleniumUtils.SetViewportSize(new Logger(), driver, new RectangleSize(size));
        }

        protected override IPageStateCommon<IWebDriver, IWebElement> CreatePageState()
        {
            return new PageState(Logger, driver_, StitchMode, userAgent_);
        }

        protected override string GetCurrentUrl() => driver_.Url;

        protected override IEyesJsExecutor GetJsExecutor() => new SeleniumJavaScriptExecutor(driver_);

        protected override string GetUserAgent() => driver_.GetUserAgent();

        protected override object GetWrappedWebDriver() => driver_.RemoteWebDriver;

        protected override IDictionary<string, object> GetSessionDetails() => driver_.SessionDetails;
     
        protected override FrameChain GetCurrentFrameChain() => driver_.GetFrameChain();

        protected override string TryCaptureDom()
        {
            DomCapture domCapture = new DomCapture(Logger, driver_, userAgent_);
            string domJson = domCapture.GetFullWindowDom(TestId);
            Logger.Log(TraceLevel.Info, TestId, Stage.Check, StageType.DomScript, new { domJsonLength = domJson.Length });
            return domJson;
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegionBase InRegion(Rectangle region)
        {
            return InRegionBase(() => region);
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegion InRegion(By selector)
        {
            ArgumentGuard.NotNull(selector, nameof(selector));
            userDefinedSRE_ = EyesSeleniumUtils.GetDefaultRootElement(driver_);
            PositionProvider = GetPositionProvider(Logger, StitchMode,
                jsExecutor_, userDefinedSRE_, userAgent_);

            var element = driver_.RemoteWebDriver.FindElement(selector);
            return new InRegion(driver_, InRegionBase(() => new Rectangle(element.Location, element.Size)));
        }


        private FullPageCaptureAlgorithm CreateFullPageCaptureAlgorithm(ScaleProviderFactory scaleProviderFactory, RenderingInfo renderingInfo)
        {
            ISizeAdjuster sizeAdjuster = Capture.Utils.GetImageSizeAdjuster(userAgent_, jsExecutor_);
            return new FullPageCaptureAlgorithm(
                Logger, TestId, regionPositionCompensation_, WaitBeforeScreenshots,
                DebugScreenshotProvider,
                (image) => new EyesWebDriverScreenshot(Logger, driver_, image,
                    EyesWebDriverScreenshot.ScreenshotTypeEnum.VIEWPORT, Point.Empty),
                scaleProviderFactory, CutProvider, Config_.StitchOverlap, ImageProvider,
                renderingInfo.MaxImageHeight, renderingInfo.MaxImageArea, sizeAdjuster);
        }

        protected override IGetRegions CreateSimpleRegionByElement(IWebElement element) => new SimpleRegionByElement(element);

        private void InitPositionProvidersForCheckElement_(bool isScrollableElement, IWebElement targetElement, CheckState state)
        {
            // User might still call "fully" on a non-scrollable element, adjust the position provider accordingly.
            if (isScrollableElement)
            {
                state.StitchPositionProvider = new ElementPositionProvider(Logger, driver_, targetElement);
            }
            else // Not a scrollable element but an element enclosed within a scroll-root-element 
            {
                IWebElement scrollRootElement = GetCurrentFrameScrollRootElement();
                if (StitchMode == StitchModes.CSS)
                {
                    state.StitchPositionProvider = new CssTranslatePositionProvider(Logger, driver_, targetElement);
                    state.OriginPositionProvider = new NullPositionProvider();
                }
                else
                {
                    state.StitchPositionProvider = new ScrollPositionProvider(Logger, driver_, scrollRootElement);
                }
            }
            Logger.Verbose(nameof(isScrollableElement) + "? {0}", isScrollableElement);
        }

        protected override void CheckElement_(ICheckSettingsInternal checkSettingsInternal, IWebElement targetElement,
            Rectangle? targetRegion, CheckState<IWebElement> state)
        {
            IList<FrameLocator> frameLocators = ((ISeleniumCheckTarget)checkSettingsInternal).GetFrameChain();
            if (frameLocators.Count > 0)
            {
                Logger.Verbose("Target.Frame(frame).Region(element).Fully(false)");
            }
            else
            {
                Logger.Verbose("Target.Region(element).Fully(false)");
            }
            FrameChain currentFrameChain = driver_.GetFrameChain().Clone();
            Rectangle bounds = EyesRemoteWebElement.GetClientBounds(targetElement, driver_, Logger);
            Point el = EyesRemoteWebElement.GetOffsetPosition(targetElement, driver_, Logger);
            if (!state.EffectiveViewport.Contains(bounds))
            {
                Point visualOffset = GetFrameChainOffset_(currentFrameChain);
                bounds.Offset(visualOffset);
                IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
                IWebElement currentFrame = driver_.FindElement(By.TagName("html"));
                if (!currentFrameSRE.Equals(currentFrame))
                {
                    el.Offset(EyesRemoteWebElement.GetScrollPosition(currentFrame, driver_, Logger));
                }
                IPositionProvider currentFramePositionProvider = GetPositionProviderForScrollRootElement_(currentFrameSRE);
                Point currentFramePosition = currentFramePositionProvider.GetCurrentPosition();
                bounds.Offset(currentFramePosition);
                currentFramePositionProvider.SetPosition(bounds.Location);

                Region actualElementBounds = EyesRemoteWebElement.GetClientBounds(targetElement, driver_, Logger);
                actualElementBounds = actualElementBounds.Offset(visualOffset.X, visualOffset.Y);

                Point actualFramePosition = bounds.Location - (Size)actualElementBounds.Location;
                bounds.Location -= (Size)actualFramePosition;
            }
            state.OriginLocation = bounds.Location;

            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();
            FrameChain fcClone = currentFrameChain.Clone();

            while (!state.EffectiveViewport.IntersectsWith(bounds) && fcClone.Count > 0)
            {
                fcClone.Pop();
                switchTo.ParentFrame();
                IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
                IPositionProvider currentFramePositionProvider = GetPositionProviderForScrollRootElement_(currentFrameSRE);
                Point currentFramePosition = currentFramePositionProvider.GetCurrentPosition();
                bounds.Offset(currentFramePosition);
                Point actualFramePosition = currentFramePositionProvider.SetPosition(bounds.Location);
                bounds.Location -= (Size)actualFramePosition;
            }

            switchTo.Frames(currentFrameChain);

            Rectangle crop = ComputeCropRectangle(bounds, targetRegion) ?? bounds;
            CheckWindowBase(crop, checkSettingsInternal, source: driver_.Url);
        }

        protected override void CheckFullRegion_(ICheckSettingsInternal checkSettingsInternal, Rectangle targetRegion, CheckState<IWebElement> state)
        {
            if (((ISeleniumCheckTarget)checkSettingsInternal).GetFrameChain().Count > 0)
            {
                Logger.Verbose("Target.Frame(frame).Region(x,y,w,h).Fully(true)");
            }
            else
            {
                Logger.Verbose("Target.Region(x,y,w,h).Fully(true)");
            }
            FrameChain currentFrameChain = driver_.GetFrameChain().Clone();
            IFrame currentFrame = currentFrameChain.Peek();
            if (currentFrame != null)
            {
                Rectangle currentFrameBoundsWithoutBorders = Region.RemoveBorders(currentFrame.Bounds, currentFrame.BorderWidths);
                state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, currentFrameBoundsWithoutBorders);
                IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
                Size currentSREScrollSize = EyesRemoteWebElement.GetScrollSize(currentFrameSRE, driver_, Logger);
                state.FullRegion = new Rectangle(state.EffectiveViewport.Location, currentSREScrollSize);
            }
            else
            {
                Point visualOffset = GetFrameChainOffset_(currentFrameChain);
                targetRegion.Offset(visualOffset);
            }
            CheckWindowBase(targetRegion, checkSettingsInternal, source: driver_.Url);
        }

        protected override void CheckFullFrame_(ICheckSettingsInternal checkSettingsInternal, CheckState<IWebElement> state)
        {
            Logger.Verbose("Target.Frame(frame).Fully(true)");
            FrameChain currentFrameChain = driver_.GetFrameChain().Clone();
            Point visualOffset = GetFrameChainOffset_(currentFrameChain);
            IFrame currentFrame = currentFrameChain.Peek();
            state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, new Rectangle(visualOffset, currentFrame.InnerSize));

            IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
            Size currentSREScrollSize = EyesRemoteWebElement.GetScrollSize(currentFrameSRE, driver_, Logger);
            state.FullRegion = new Rectangle(state.EffectiveViewport.Location, currentSREScrollSize);

            //string originalOverflow = EyesSeleniumUtils.SetOverflow("hidden", driver_, currentFrameSRE);

            CheckWindowBase(null, checkSettingsInternal, source: driver_.Url);

            //EyesSeleniumUtils.SetOverflow(originalOverflow, driver_, currentFrameSRE);
        }

        internal protected override IWebElement GetScrollRootElementFromSREContainer(ISeleniumCheckTarget scrollRootElementContainer, IWebDriver driver)
        {
            IWebElement scrollRootElement = TryGetUserDefinedSREFromSREContainer(scrollRootElementContainer, driver);
            if (scrollRootElement == null)
            {
                scrollRootElement = EyesSeleniumUtils.GetDefaultRootElement(driver);
            }
            return scrollRootElement;
        }

        protected override IWebElement TryGetUserDefinedSREFromSREContainer(ISeleniumCheckTarget scrollRootElementContainer, IWebDriver driver)
        {
            IWebElement scrollRootElement = scrollRootElementContainer.GetScrollRootElement();
            if (scrollRootElement == null)
            {
                By scrollRootSelector = scrollRootElementContainer.GetScrollRootSelector();
                if (scrollRootSelector != null)
                {
                    scrollRootElement = driver.FindElement(scrollRootSelector);
                }
            }
            return scrollRootElement;
        }

        internal override IWebElement GetCurrentFrameScrollRootElement()
        {
            return EyesSeleniumUtils.GetCurrentFrameScrollRootElement(driver_, userDefinedSRE_);
        }

        public override void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor)
        {
            Rectangle control = Rectangle.Empty;
            if (element != null)
            {
                control = new Rectangle(element.Location, element.Size);

                var ts = $"{action} [{GeometryUtils.ToString(control)}] {cursor}";

                if (LastScreenshotBoundsProvider == null)
                {
                    Logger.Verbose("Ignoring {0} (no screenshot)", ts);
                    return;
                }

                var sb = LastScreenshotBoundsProvider.LastScreenshotBounds;
                cursor.Offset(control.Location);
                if (!sb.Contains(cursor))
                {
                    Logger.Verbose("Ignoring {0} (out of bounds)", ts);
                    return;
                }

                control.Intersect(sb);
                if (control == Rectangle.Empty)
                {
                    cursor.Offset(-sb.X, -sb.Y);
                }
                else
                {
                    cursor.Offset(-control.X, -control.Y);
                    control.Offset(-sb.X, -sb.Y);
                }
            }

            var trigger = new MouseTrigger(action, new Region(control), new Location(cursor));
            UserInputs.Add(trigger);

            Logger.Verbose("Added {0}", trigger);
        }

        public override void AddKeyboardTrigger(IWebElement element, string text)
        {
            if (LastScreenshotBoundsProvider == null)
            {
                Logger.Verbose("Ignoring {0} (no screenshot)", text);
                return;
            }

            Rectangle control = Rectangle.Empty;
            if (element is RemoteWebElement)
            {
                element = new EyesRemoteWebElement(Logger, driver_, element);
                control = ((EyesRemoteWebElement)element).GetBounds().ToRectangle();
            }

            if (control != Rectangle.Empty)
            {
                // If we know where the text goes to and it's outside the bounds of the image, 
                // don't show it.
                var sb = LastScreenshotBoundsProvider.LastScreenshotBounds;
                control.Intersect(sb);
                if (control == Rectangle.Empty)
                {
                    Logger.Verbose("Ignoring {0} (out of bounds)", text);
                    return;
                }
                else
                {
                    // Even after we intersected the control, we need to make sure it's location
                    // is based on the last screenshot location (remember it might be offsetted).
                    control.Offset(-sb.X, -sb.Y);
                }
            }

            var trigger = new TextTrigger(new Region(control), text);
            UserInputs.Add(trigger);

            Logger.Verbose("Added {0}", trigger);
        }

        protected override Size GetViewportSize()
        {
            Size vpSize;
            if (ImageProvider is MobileScreenshotImageProvider mobileScreenshotProvider)
            {
                using (Bitmap mobileScreenshot = mobileScreenshotProvider.GetImage())
                {
                    vpSize = mobileScreenshot.Size;
                }
                vpSize.Width = (int)Math.Round(vpSize.Width / DevicePixelRatio);
                vpSize.Height = (int)Math.Round(vpSize.Height / DevicePixelRatio);
            }
            else
            {
                vpSize = EyesSeleniumUtils.GetViewportSize(Logger, driver_);
            }

            Logger.Verbose("vpSize: {0}", vpSize);
            return vpSize;
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            FrameChain originalFrame = driver_.GetFrameChain().Clone();
            if (originalFrame.Count > 0)
            {
                driver_.SwitchTo().DefaultContent();
            }
            try
            {
                EyesSeleniumUtils.SetViewportSize(Logger, driver_, size);
                SetEffectiveViewportSize(Config_.ViewportSize);
            }
            catch (EyesSetViewportSizeException eyesSetViewportSizeException)
            {
                // Just in case the user catches this error
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrame);

                if (!eyesSetViewportSizeException.IsRoundingError)
                {
                    throw new TestFailedException("Failed to set the viewport size");
                }
            }
            if (originalFrame.Count > 0)
            {
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrame);
            }
        }

        protected override void SetEffectiveViewportSize(RectangleSize size)
        {
            effectiveViewport_ = new Region(Point.Empty, size.ToSize());
            Logger.Verbose("setting effective viewport size to {0}", size);
        }

        protected override ScaleProviderFactory GetScaleProviderFactory_()
        {
            IWebElement element = EyesSeleniumUtils.GetDefaultRootElement(driver_);
            Size entireSize = EyesRemoteWebElement.GetEntireSize(element, driver_, Logger);
            return new ContextBasedScaleProviderFactory(entireSize,
                    GetViewportSize(), DevicePixelRatio, ScaleProvider, SetScaleProvider);
        }

        protected override EyesScreenshot GetFrameOrElementScreenshot(ScaleProviderFactory scaleProviderFactory, CheckState<IWebElement> state)
        {
            EyesWebDriverScreenshot result;
            bool switchedFrame = false;
            FrameChain originalFrameChain = driver_.GetFrameChain().Clone();
            Point originalViewportLocation = effectiveViewport_.Location;
            if (state.EffectiveViewport.IsEmpty)
            {
                switchedFrame = true;
                IFrame frame;
                FrameChain frameChain = driver_.GetFrameChain();
                while ((frame = frameChain.Peek()) != null)
                {
                    Rectangle frameBounds = new Rectangle(frame.Location, frame.InnerSize);
                    if (!effectiveViewport_.Contains(frameBounds))
                    {
                        driver_.SwitchTo().ParentFrame();
                        frameBounds = BringRegionToView(frameBounds, effectiveViewport_.Location);
                        effectiveViewport_.Intersect(frameBounds);
                    }
                }
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrameChain);
                state.EffectiveViewport = effectiveViewport_.ToRectangle();
            }

            RenderingInfo renderingInfo = ServerConnector.GetRenderingInfo();
            FullPageCaptureAlgorithm algo = CreateFullPageCaptureAlgorithm(scaleProviderFactory, renderingInfo);

            Logger.Verbose("Check frame/element requested");

            IPositionProvider positionProvider = state.StitchPositionProvider;
            IPositionProvider originPositionProvider = state.OriginPositionProvider;

            if (positionProvider == null)
            {
                Logger.Verbose(nameof(positionProvider) + " is null, updating it.");
                IWebElement scrollRootElement = GetCurrentFrameScrollRootElement();
                positionProvider = GetPositionProviderForScrollRootElement_(Logger, driver_, StitchMode, userAgent_, scrollRootElement);
            }

            if (originPositionProvider == null)
            {
                originPositionProvider = new NullPositionProvider();
            }

            jsExecutor_.ExecuteScript(SET_DATA_APPLITOOLS_SCROLL_ATTR, (positionProvider as ISeleniumPositionProvider)?.ScrolledElement);
            Bitmap entireFrameOrElement = algo.GetStitchedRegion(state.EffectiveViewport, state.FullRegion, positionProvider, originPositionProvider, state.StitchOffset);

            Logger.Verbose("Building screenshot object...");
            Point frameLocationInScreenshot = new Point(-state.FullRegion.Left, -state.FullRegion.Top);
            result = new EyesWebDriverScreenshot(Logger, driver_, entireFrameOrElement, entireFrameOrElement.Size, frameLocationInScreenshot);
            state.OriginLocation = state.FullRegion.Location;
            if (switchedFrame)
            {
                IFrame frame;
                FrameChain frameChain = driver_.GetFrameChain();
                while ((frame = frameChain.Peek()) != null)
                {
                    driver_.SwitchTo().ParentFrame();
                    BringRegionToView(new Rectangle(frame.OriginalLocation, frame.InnerSize), originalViewportLocation, false);
                }
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrameChain);
            }
            return result;
        }

        protected override EyesScreenshot GetViewportScreenshot(ScaleProviderFactory scaleProviderFactory, CheckState<IWebElement> state)
        {
            Thread.Sleep(WaitBeforeScreenshots);
            IWebElement scrolledElement = GetCurrentFrameScrollRootElement();
            if (state.StitchContent)
            {
                jsExecutor_.ExecuteScript(SET_DATA_APPLITOOLS_SCROLL_ATTR, scrolledElement);
            }

            EyesWebDriverScreenshot result;
            if (state.EffectiveViewport.IsEmpty)
            {
                IFrame frame;
                FrameChain originalFrameChain = driver_.GetFrameChain().Clone();
                FrameChain frameChain = driver_.GetFrameChain();
                Point originalViewportLocation = effectiveViewport_.Location;
                while ((frame = frameChain.Peek()) != null)
                {
                    Rectangle frameBounds = new Rectangle(frame.Location, frame.InnerSize);
                    if (!effectiveViewport_.Contains(frameBounds))
                    {
                        driver_.SwitchTo().ParentFrame();
                        frameBounds = BringRegionToView(frameBounds, effectiveViewport_.Location);
                        effectiveViewport_.Intersect(frameBounds);
                    }
                }
                result = GetScaledAndCroppedScreenshot_(scaleProviderFactory);
                if (effectiveViewport_.Width < result.Image.Width || effectiveViewport_.Height < result.Image.Height)
                {
                    result = (EyesWebDriverScreenshot)result.GetSubScreenshot(effectiveViewport_, false);
                }
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrameChain);
                frameChain = driver_.GetFrameChain();
                while ((frame = frameChain.Peek()) != null)
                {
                    driver_.SwitchTo().ParentFrame();
                    BringRegionToView(new Rectangle(frame.OriginalLocation, frame.InnerSize), originalViewportLocation, false);
                }
                ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(originalFrameChain);
            }
            else
            {
                result = GetScaledAndCroppedScreenshot_(scaleProviderFactory);
            }
            return result;
        }

        internal protected override IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, IWebElement scrollRootElement, UserAgent userAgent = null)
        {
            return positionProviderFactory_.GetPositionProvider(logger, stitchMode, executor, scrollRootElement, userAgent);
        }

        private IPositionProvider GetPositionProviderForScrollRootElement_(IWebElement scrollRootElement)
        {
            return GetPositionProviderForScrollRootElement_(Logger, driver_, StitchMode, userAgent_, scrollRootElement);
        }

        internal static IPositionProvider GetPositionProviderForScrollRootElement_(Logger logger, EyesWebDriver driver, StitchModes stitchMode, UserAgent ua, IWebElement scrollRootElement)
        {
            IPositionProvider positionProvider = driver.Eyes.positionProviderFactory_.TryGetPositionProviderForElement(
                scrollRootElement, stitchMode, driver.RemoteWebDriver, logger);
            if (positionProvider == null)
            {
                logger.Verbose("creating a new position provider.");
                IWebElement defaultSRE = EyesSeleniumUtils.GetDefaultRootElement(driver);
                if (scrollRootElement.Equals(defaultSRE))
                {
                    positionProvider = driver.Eyes.GetPositionProvider(logger, stitchMode,
                        driver, scrollRootElement, ua);
                }
                else
                {
                    positionProvider = new ElementPositionProvider(logger, driver, scrollRootElement);
                }
            }
            logger.Verbose("position provider: {0}", positionProvider);
            return positionProvider;
        }

        private EyesWebDriverScreenshot GetScaledAndCroppedScreenshot_(ScaleProviderFactory scaleProviderFactory)
        {
            Bitmap screenshotImage = ImageProvider.GetImage();

            IScaleProvider scaleProvider = scaleProviderFactory.GetScaleProvider(screenshotImage.Width);
            ICutProvider cutProvider = CutProvider;
            if (scaleProvider.ScaleRatio != 1.0)
            {
                Bitmap scaledImage = BasicImageUtils.ScaleImage(screenshotImage, scaleProvider);
                screenshotImage.Dispose();
                screenshotImage = scaledImage;
                DebugScreenshotProvider.Save(screenshotImage, "scaled");
                cutProvider.Scale(scaleProvider.ScaleRatio);
            }

            if (!(cutProvider is NullCutProvider))
            {
                Bitmap croppedImage = cutProvider.Cut(screenshotImage);
                screenshotImage.Dispose();
                screenshotImage = croppedImage;
                DebugScreenshotProvider.Save(screenshotImage, "cut");
            }

            EyesWebDriverScreenshot result = new EyesWebDriverScreenshot(Logger, driver_, screenshotImage);
            return result;
        }

        protected override string GetTitle()
        {
            if (!dontGetTitle_)
            {
                try
                {
                    return driver_.Title;
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, ex, TestId);
                    dontGetTitle_ = true;
                }
            }

            return string.Empty;
        }

        protected override string GetInferredEnvironment()
        {
            var userAgent = userAgent_?.OriginalUserAgentString ?? driver_.GetUserAgent();
            if (userAgent != null)
            {
                return "useragent:" + userAgent;
            }

            return null;
        }

        public EyesWebDriver GetDriver()
        {
            return driver_;
        }

        protected override IWebElement GetTargetElement(ISeleniumCheckTarget seleniumCheckTarget)
        {
            ArgumentGuard.NotNull(seleniumCheckTarget, nameof(seleniumCheckTarget));
            By targetSelector = seleniumCheckTarget.GetTargetSelector();
            IWebElement targetElement = seleniumCheckTarget.GetTargetElement();
            if (targetElement == null && targetSelector != null)
            {
                targetElement = this.driver_.FindElement(targetSelector);
            }
            return targetElement;
        }

        protected override void GetWebDriverInfo(out object remoteWebDriver,
                    out HtmlElementInfo html, out HtmlElementInfo body,
                    out string scrollRootElement, out string sessionId, out object capabilities)
        {
            remoteWebDriver = driver_.RemoteWebDriver;
            var h = (EyesRemoteWebElement)driver_.FindElement(By.TagName("html"));
            var b = (EyesRemoteWebElement)driver_.FindElement(By.TagName("body"));
            html = new HtmlElementInfo { Id = h.ToString(), ClientSize = h.ClientSize, ScrollSize = h.ScrollSize, Size = h.Size };
            body = new HtmlElementInfo { Id = b.ToString(), ClientSize = b.ClientSize, ScrollSize = b.ScrollSize, Size = b.Size };
            scrollRootElement = userDefinedSRE_?.ToString();
            sessionId = GetSessionId();
            capabilities = driver_.RemoteWebDriver.Capabilities;
        }

        protected override string GetSessionId() => driver_.RemoteWebDriver.SessionId.ToString();

        public override int GetFrameChainLength()
        {
            return driver_.GetFrameChain().Count;
        }

        public override int GetFrameChainLength(ISeleniumCheckTarget seleniumCheckTarget)
        {
            return seleniumCheckTarget.GetFrameChain().Count;
        }

        #endregion Methods
    }
}

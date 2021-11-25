using Applitools.Capture;
using Applitools.Fluent;
using Applitools.Positioning;
using Applitools.Selenium.Capture;
using Applitools.Selenium.Positioning;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium.Common
{
    internal abstract class SeleniumEyesBase<IWebElement, IWebDriver, ISeleniumCheckTarget> :
        RunningTest, IEyes, IUserActionsEyes<IWebElement>, ISeleniumEyesCommon
    {
        protected const string SET_DATA_APPLITOOLS_SCROLL_ATTR = "var e = arguments[0]; if (e != null) e.setAttribute('data-applitools-scroll','true');";
        protected const string HIDE_CARET = "var activeElement = document.activeElement; activeElement && activeElement.blur(); return activeElement;";

        protected ISeleniumConfigurationProvider configProvider_;
        protected static readonly double UnknownDevicePixelRatio_ = 0;
        protected static readonly double DefaultDevicePixelRatio_ = 1;

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public SeleniumEyesBase(ISeleniumConfigurationProvider configurationProvider, Uri serverUrl, ClassicRunner runner)
            : base(runner)
        {
            ArgumentGuard.NotNull(serverUrl, nameof(serverUrl));
            configProvider_ = configurationProvider;
            ServerUrl = serverUrl.ToString();
            Init_();
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public SeleniumEyesBase(ISeleniumConfigurationProvider configurationProvider, ClassicRunner runner)
            : base(runner)
        {
            configProvider_ = configurationProvider;
            runner_ = runner;
            Init_();
        }

        private void Init_()
        {
            Config_.SetHideScrollbars(true);
            DevicePixelRatio = UnknownDevicePixelRatio_;
            WaitBeforeScreenshots = 100;
            DefaultMatchSettings.UseDom = false;
            ServerConnector.SdkName = FullAgentId;
            ServerConnector.AgentId = FullAgentId;
        }

        protected override Applitools.Configuration Configuration => configProvider_.GetConfiguration();

        protected Configuration Config_ => configProvider_.GetConfiguration();

        #endregion Constructors

        #region Properties

        public override string ServerUrl { get => base.ServerUrl ?? runner_.ServerUrl; set => base.ServerUrl = value; }

        /// <summary>
        /// Forces a full page screenshot (by scrolling and stitching) if the browser only 
        /// supports viewport screenshots).
        /// </summary>
        public bool ForceFullPageScreenshot { get => Config_.IsForceFullPageScreenshot ?? false; set => Config_.SetForceFullPageScreenshot(value); }
        public StitchModes StitchMode { get => Config_.StitchMode; set => Config_.SetStitchMode(value); }
        public int WaitBeforeScreenshots { get => Config_.WaitBeforeScreenshots; set => Config_.SetWaitBeforeScreenshots(value); }
        public bool HideScrollbars { get => Config_.HideScrollbars; set => Config_.SetHideScrollbars(value); }
        public bool HideCaret { get => Config_.HideCaret; set => Config_.SetHideCaret(value); }

        public override string ApiKey { get => base.ApiKey ?? runner_.ApiKey; set => base.ApiKey = value; }
        public override ProxySettings Proxy { get => base.Proxy ?? runner_.Proxy; set => base.Proxy = value; }
        public IImageProvider ImageProvider { get; private set; }

        protected bool? isDisabled_;
        protected object lastCheckSettings_;
        protected object lastCheckSettingsInternal_;
        protected IEyesJsExecutor jsExecutor_;
        internal UserAgent userAgent_;

        protected abstract IWebDriver Driver { get; }

        protected Region effectiveViewport_;
        protected IWebElement userDefinedSRE_;
        protected IRegionPositionCompensation regionPositionCompensation_;

        internal FrameChain OriginalFC { get; set; }

        public IDictionary<string, object> CachedSessionDetails { get; protected set; }

        public override bool IsDisabled { get => isDisabled_ ?? runner_.IsDisabled; set => isDisabled_ = value; }

        public IPositionProvider CurrentFramePositionProvider { get; protected set; }

        public void SetScrollToRegion(bool shouldScroll) { }

        public double DevicePixelRatio { get; set; }

        public string PlatformName { get; protected set; }

        #endregion Properties

        /// <summary>
        /// Starts a new test.
        /// </summary>
        /// <param name="driver">The web driver that controls the browser 
        /// hosting the application under test.</param>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The required browser's viewport size 
        /// (i.e., the visible part of the document's body) or <c>Size.Empty</c>
        /// to allow any viewport size.</param>
        public IWebDriver Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            Config_.SetAppName(appName);
            Config_.SetTestName(testName);
            Config_.SetViewportSize(new RectangleSize(viewportSize));
            return Open(driver);
        }

        /// <summary>
        /// Starts a new test.
        /// </summary>
        /// <param name="driver">The web driver that controls the browser 
        /// hosting the application under test.</param>
        public IWebDriver Open(IWebDriver driver)
        {
            InitDriver_(driver);

            string uaString = GetUserAgent();
            if (uaString != null)
            {
                Logger.Verbose("User-Agent: {0}", uaString);
                userAgent_ = UserAgent.ParseUserAgentString(uaString, true);
            }

            InitDevicePixelRatio_();

            jsExecutor_ = GetJsExecutor();

            ImageProvider = ImageProviderFactory.GetImageProvider(userAgent_, this, Logger, Driver);
            regionPositionCompensation_ = RegionPositionCompensationFactory.GetRegionPositionCompensation(userAgent_, this, Logger);

            OpenBase();
            DevicePixelRatio = UnknownDevicePixelRatio_;
            //PositionProvider = SeleniumScrollPositionProviderFactory.GetPositionProvider(StitchMode, jsExecutor_, scrollRootElement_);
            string batchId = Batch.Id;
            runner_.AddBatch(batchId, this);

            return Driver;
        }

        protected abstract IEyesJsExecutor GetJsExecutor();
        protected abstract string GetUserAgent();

        protected void InitDriver_(IWebDriver driver)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));

            InitDriverImpl_(driver);

            Type driverType = GetWrappedWebDriver().GetType();
            Logger.Log(TraceLevel.Info, Stage.Open, new { driverType });
            MethodInfo getPlatformNameMI = driverType.GetProperty("PlatformName")?.GetGetMethod();
            MethodInfo getSessionDetailsMI = driverType.GetProperty("SessionDetails")?.GetGetMethod();

            if (getPlatformNameMI != null)
            {
                PlatformName = (string)getPlatformNameMI.Invoke(GetWrappedWebDriver(), null);
            }
            else
            {
                IEyesWebDriverCommon commonDriver_ = (IEyesWebDriverCommon)Driver;
                PlatformName = (commonDriver_.GetSessionDetail("platformName") as string) ?? (commonDriver_.GetSessionDetail("platform") as string);
            }

            if (getSessionDetailsMI != null)
            {
                InitSessionDetails_(getSessionDetailsMI);
            }
            else
            {
                InitSessionDetailsUsingInternalImplementation_();
            }

            Logger.Log(TraceLevel.Info, Stage.Open,
                new { message = "Initialized Web Driver.", seleniumSessionId = GetSessionId(), PlatformName });
        }

        protected abstract void InitDriverImpl_(IWebDriver driver);

        private void InitSessionDetails_(MethodInfo getSessionDetailsMI)
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = (IDictionary<string, object>)getSessionDetailsMI.Invoke(GetWrappedWebDriver(), null);
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = getSessionDetailsMI.Invoke(GetWrappedWebDriver(), null)
                    });
            }
        }

        protected abstract object GetWrappedWebDriver();
        protected abstract IDictionary<string, object> GetSessionDetails();

        private void InitSessionDetailsUsingInternalImplementation_()
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = GetSessionDetails();
                if (CachedSessionDetails == null) break;
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = GetSessionDetails()
                    });
            }
        }

        protected ScaleProviderFactory UpdateScalingParams()
        {
            ScaleProviderFactory factory;
            if (ScaleProvider is NullScaleProvider)
            {
                InitDevicePixelRatio_();

                Logger.Verbose("Setting scale provider...");
                try
                {
                    factory = GetScaleProviderFactory_();
                }
                catch (Exception)
                {
                    // This can happen in Appium for example.
                    Logger.Verbose("Failed to set ContextBasedScaleProvider. Using FixedScaleProvider instead...");
                    factory = new FixedScaleProviderFactory(1.0 / DevicePixelRatio, SetScaleProvider);
                }
                Logger.Verbose("Done!");
            }
            else
            {
                factory = new ScaleProviderIdentityFactory(ScaleProvider, (sp) => { });
            }

            return factory;
        }

        protected abstract ScaleProviderFactory GetScaleProviderFactory_();

        protected void InitDevicePixelRatio_()
        {
            Logger.Verbose("Trying to extract device pixel ratio...");
            try
            {
                object dpr = JSBrowserCommands.WithReturn.GetDevicePixelRatio(jsExecutor_);
                DevicePixelRatio = double.Parse(dpr.ToString());
            }
            catch (Exception)
            {
                Logger.Verbose("Failed to extract device pixel ratio! Using default.");
                DevicePixelRatio = DefaultDevicePixelRatio_;
            }
            Logger.Verbose("Device pixel ratio: {0}", DevicePixelRatio);
        }

        /// <summary>
        /// Takes a snapshot of the application under test, where the capture area and settings
        /// are given by <paramref name="checkSettings"/>.
        /// </summary>
        /// <param name="checkSettings">A settings object defining the capture area and parameters.
        /// Created fluently using the <see cref="Target"/> static class.</param>
        public void Check(ICheckSettings checkSettings)
        {
            CheckImpl_(checkSettings);
        }

        internal protected void CheckImpl_(ICheckSettings checkSettings)
        {
            try
            {
                ArgumentGuard.IsValidState(IsOpen, "Eyes not open");
                ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));

                Logger.Verbose("URL: {0}", GetCurrentUrl());
                ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
                ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;
                lastCheckSettings_ = checkSettingsInternal.ToSerializableDictionary();
                lastCheckSettingsInternal_ = checkSettingsInternal;

                CheckState<IWebElement> state = InitCheckState_(checkSettings, checkSettingsInternal, seleniumCheckTarget);

                Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();

                Logger.Verbose("setting " + nameof(userDefinedSRE_) + " ...");
                userDefinedSRE_ = TryGetUserDefinedSREFromSREContainer(seleniumCheckTarget, Driver);
                IWebElement scrollRootElement = userDefinedSRE_;
                if (scrollRootElement == null)
                {
                    scrollRootElement = GetDefaultRootElement();
                }
                Logger.Verbose(nameof(userDefinedSRE_) + " set to {0}", userDefinedSRE_?.ToString() ?? "null");
                Logger.Verbose(nameof(scrollRootElement) + " set to {0}", scrollRootElement);

                CurrentFramePositionProvider = null;
                PositionProvider = GetPositionProvider(Logger, StitchMode, jsExecutor_, scrollRootElement, userAgent_);
                CaretVisibilityProvider caretVisibilityProvider = new CaretVisibilityProvider(Logger, Driver, Config_);

                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)Driver.SwitchTo();
                IPageStateCommon<IWebDriver, IWebElement> pageState = CreatePageState();
                pageState.PreparePage((ISeleniumCheckTargetCommon<IWebDriver, IWebElement>)seleniumCheckTarget, Config_, scrollRootElement);

                FrameChain frameChainAfterSwitchToTarget = GetCurrentFrameChain().Clone();

                state.EffectiveViewport = ComputeEffectiveViewport(frameChainAfterSwitchToTarget, viewportSize_);
                // new Rectangle(Point.Empty, viewportSize_);
                IWebElement targetElement = GetTargetElementFromSettings_(seleniumCheckTarget);

                caretVisibilityProvider.HideCaret();

                //////////////////////////////////////////////////////////////////

                // Cases:
                // Target.Region(x,y,w,h).Fully(true) - TODO - NOT TESTED!
                // Target.Region(x,y,w,h).Fully(false)
                // Target.Region(element).Fully(true)
                // Target.Region(element).Fully(false)
                // Target.Frame(frame).Fully(true)
                // Target.Frame(frame).Region(x,y,w,h).Fully(true)
                // Target.Frame(frame).Region(x,y,w,h).Fully(false) - TODO - NOT TESTED!
                // Target.Frame(frame).Region(element).Fully(true)
                // Target.Frame(frame).Region(element).Fully(false) - TODO - NOT TESTED!
                // Target.Window().Fully(true)
                // Target.Window().Fully(false)

                // Algorithm:
                // 1. Save current page state
                // 2. Switch to frame
                // 3. Maximize desired region or element visibility
                // 4. Capture desired region of element
                // 5. Go back to original frame
                // 6. Restore page state

                if (targetElement != null)
                {
                    if (state.StitchContent)
                    {
                        CheckFullElement_(checkSettingsInternal, targetElement, targetRegion, state);
                    }
                    else
                    {
                        // TODO Verify: if element is outside the viewport, we should still capture entire (outer) bounds
                        CheckElement_(checkSettingsInternal, targetElement, targetRegion, state);
                    }
                }
                else if (targetRegion != null)
                {
                    // Coordinates should always be treated as "Fully" in case they get out of the viewport.
                    bool originalFully = state.StitchContent;
                    state.StitchContent = true;
                    CheckFullRegion_(checkSettingsInternal, targetRegion.Value, state);
                    state.StitchContent = originalFully;
                }
                else if (GetFrameChainLength(seleniumCheckTarget) > 0)
                {
                    if (state.StitchContent)
                    {
                        CheckFullFrame_(checkSettingsInternal, state);
                    }
                    else
                    {
                        Logger.Log(TraceLevel.Notice, TestId, Stage.Check, StageType.Called,
                            new { message = "Target.Frame(frame).Fully(false)" });
                        Logger.Log(TraceLevel.Warn, TestId, Stage.Check, StageType.Called,
                            new { message = "This shouldn't have been called, as it is covered by `CheckElement_(...)`" });
                    }
                }
                else
                {
                    if (state.StitchContent)
                    {
                        CheckFullWindow_(checkSettingsInternal, state, scrollRootElement);
                    }
                    else
                    {
                        state.OriginLocation = Point.Empty;// PositionProvider.GetCurrentPosition();
                        CheckWindow_(checkSettingsInternal);
                    }
                }

                caretVisibilityProvider.RestoreCaret();

                pageState.RestorePageState();
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, ex);
                throw new EyesException("Error", ex);
            }
        }

        protected abstract IPageStateCommon<IWebDriver, IWebElement> CreatePageState();

        protected abstract void CheckElement_(ICheckSettingsInternal checkSettingsInternal, IWebElement targetElement, Rectangle? targetRegion, CheckState<IWebElement> state);

        protected virtual void CheckFullRegion_(ICheckSettingsInternal checkSettingsInternal, Rectangle targetRegion, CheckState<IWebElement> state)
        {
            if (GetFrameChainLength((ISeleniumCheckTarget)checkSettingsInternal) > 0)
            {
                Logger.Verbose("Target.Frame(frame).Region(x,y,w,h).Fully(true)");
            }
            else
            {
                Logger.Verbose("Target.Region(x,y,w,h).Fully(true)");
            }
            FrameChain currentFrameChain = GetCurrentFrameChain().Clone();
            IFrame currentFrame = currentFrameChain.Peek();
            if (currentFrame != null)
            {
                Rectangle currentFrameBoundsWithoutBorders = Region.RemoveBorders(currentFrame.Bounds, currentFrame.BorderWidths);
                state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, currentFrameBoundsWithoutBorders);
                IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
                Size currentSREScrollSize = EyesSeleniumUtilsCommon.GetScrollSize(currentFrameSRE, jsExecutor_, Logger);
                state.FullRegion = new Rectangle(state.EffectiveViewport.Location, currentSREScrollSize);
            }
            else
            {
                Point visualOffset = GetFrameChainOffset_(currentFrameChain);
                targetRegion.Offset(visualOffset);
            }
            CheckWindowBase(targetRegion, checkSettingsInternal, GetCurrentUrl());
        }

        protected virtual void CheckFullFrame_(ICheckSettingsInternal checkSettingsInternal, CheckState<IWebElement> state)
        {
            Logger.Verbose("Target.Frame(frame).Fully(true)");
            FrameChain currentFrameChain = GetCurrentFrameChain().Clone();
            Point visualOffset = GetFrameChainOffset_(currentFrameChain);
            IFrame currentFrame = currentFrameChain.Peek();
            state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, new Rectangle(visualOffset, currentFrame.InnerSize));

            IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
            Size currentSREScrollSize = EyesSeleniumUtilsCommon.GetScrollSize(currentFrameSRE, jsExecutor_, Logger);
            state.FullRegion = new Rectangle(state.EffectiveViewport.Location, currentSREScrollSize);

            //string originalOverflow = EyesSeleniumUtils.SetOverflow("hidden", Driver, currentFrameSRE);

            CheckWindowBase(null, checkSettingsInternal, GetCurrentUrl());

            //EyesSeleniumUtils.SetOverflow(originalOverflow, Driver, currentFrameSRE);
        }
       
        protected abstract FrameChain GetCurrentFrameChain();
        protected abstract IWebElement GetDefaultRootElement();

        internal protected abstract IWebElement GetScrollRootElementFromSREContainer(ISeleniumCheckTarget seleniumCheckTarget, IWebDriver driver);
        protected abstract IWebElement TryGetUserDefinedSREFromSREContainer(ISeleniumCheckTarget seleniumCheckTarget, IWebDriver driver);

        internal protected abstract IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor jsExecutor_, IWebElement scrollRootElement, UserAgent userAgent_);

        protected static Rectangle? ComputeCropRectangle(Rectangle fullRect, Rectangle? cropRect)
        {
            if (cropRect == null) return null;
            Rectangle crop = fullRect;
            Rectangle cropRectValue = cropRect.Value;
            cropRectValue.Offset(crop.Location);
            crop.Intersect(cropRectValue);
            return crop;
        }

        internal CheckState<IWebElement> InitCheckState_(ICheckSettings checkSettings,
            ICheckSettingsInternal checkSettingsInternal, ISeleniumCheckTarget seleniumCheckTarget)
        {
            CheckState<IWebElement> state = new CheckState<IWebElement>();
            ((ISeleniumCheckTargetCommon<IWebDriver, IWebElement>)seleniumCheckTarget).State = state;
            state.StitchContent = checkSettingsInternal.GetStitchContent() ?? Config_.IsForceFullPageScreenshot ?? false;

            ICheckSettingsInternal clonedCheckSettings = (ICheckSettingsInternal)checkSettings.Clone();

            // Ensure frame is not used as a region
            ((ISeleniumCheckTargetCommon<IWebDriver, IWebElement>)checkSettings).SanitizeSettings(Logger, Driver, state);
            return state;
        }

        private void CheckWindow_(ICheckSettingsInternal checkSettingsInternal)
        {
            Logger.Verbose("Target.Window()");

            CheckWindowBase(null, checkSettingsInternal, GetCurrentUrl());
        }

        protected abstract string GetCurrentUrl();

        private void CheckFullWindow_(ICheckSettingsInternal checkSettingsInternal, CheckState<IWebElement> state,
            IWebElement scrollRootElement)
        {
            Logger.Verbose("Target.Window().Fully(true)");

            InitPositionProvidersForCheckWindow_(state, scrollRootElement);

            CheckWindowBase(null, checkSettingsInternal, GetCurrentUrl());
        }

        private void InitPositionProvidersForCheckWindow_(CheckState<IWebElement> state, IWebElement scrollRootElement)
        {
            if (StitchMode == StitchModes.Scroll)
            {
                state.StitchPositionProvider = CreateScrollPositionProvider(scrollRootElement);
            }
            else // Stitch mode == CSS
            {
                if (userDefinedSRE_ != null)
                {
                    state.StitchPositionProvider = CreateElementPositionProvider(userDefinedSRE_);
                }
                else
                {
                    state.StitchPositionProvider = CreateCssTranslatePositionProvider(scrollRootElement);
                    state.OriginPositionProvider = CreateScrollPositionProvider(scrollRootElement);
                }
            }
        }

        protected abstract IPositionProvider CreateScrollPositionProvider(IWebElement scrollRootElement);
        protected abstract IPositionProvider CreateElementPositionProvider(IWebElement userDefinedSRE_);
        protected abstract IPositionProvider CreateCssTranslatePositionProvider(IWebElement scrollRootElement);

        private Rectangle ComputeEffectiveViewport(FrameChain frameChain, RectangleSize initialSize)
        {
            Rectangle viewport = new Rectangle(Point.Empty, initialSize);
            if (userDefinedSRE_ != null)
            {
                Rectangle sreInnerBounds = EyesSeleniumUtilsCommon.GetClientBoundsWithoutBorders(userDefinedSRE_, jsExecutor_, Logger);
                viewport.Intersect(sreInnerBounds);
            }
            Point offset = Point.Empty;
            foreach (IFrame frame in frameChain)
            {
                offset.Offset(frame.Location);
                Rectangle frameViewport = new Rectangle(offset, frame.InnerSize);
                viewport.Intersect(frameViewport);
                Rectangle frameSreInnerBounds = frame.ScrollRootElementInnerBounds;
                if (frameSreInnerBounds.Size.IsEmpty)
                {
                    continue;
                }
                frameSreInnerBounds.Offset(offset);
                viewport.Intersect(frameSreInnerBounds);
            }
            return viewport;
        }

        // This is the most commonly called method, yielding most problems :-)
        private void CheckFullElement_(ICheckSettingsInternal checkSettingsInternal, IWebElement targetElement,
            Rectangle? targetRegion, CheckState<IWebElement> state)
        {
            if (GetFrameChainLength((ISeleniumCheckTarget)checkSettingsInternal) > 0)
            {
                Logger.Verbose("Target.Frame(frame)...Region(element).Fully(true)");
            }
            else
            {
                Logger.Verbose("Target.Region(element).Fully(true)");
            }

            // Hide scrollbars
            string orignalOverflow = EyesSeleniumUtilsCommon.SetOverflow("hidden", jsExecutor_, targetElement);

            // Get element's scroll size and bounds
            Size scrollSize = EyesSeleniumUtilsCommon.GetScrollSize(targetElement, jsExecutor_, Logger);
            Rectangle elementBounds = EyesSeleniumUtilsCommon.GetClientBounds(targetElement, jsExecutor_, Logger);
            Rectangle elementInnerBounds = EyesSeleniumUtilsCommon.GetClientBoundsWithoutBorders(targetElement, jsExecutor_, Logger);
            string positionStyle = EyesSeleniumUtilsCommon.GetComputedStyle("position", targetElement, jsExecutor_);

            Point el = EyesSeleniumUtilsCommon.GetOffsetPosition(targetElement, Driver, Logger);
            el.Offset(elementInnerBounds.X - elementBounds.X, elementInnerBounds.Y - elementBounds.Y);
            IWebElement currentFrame = Driver.FindElement(By.TagName("html"));
            if (userDefinedSRE_ != null && !userDefinedSRE_.Equals(currentFrame))
            {
                el.Offset(EyesSeleniumUtilsCommon.GetScrollPosition(currentFrame, jsExecutor_, Logger));
            }
            state.OriginLocation = el;
            bool isScrollableElement = scrollSize.Height > elementInnerBounds.Height || scrollSize.Width > elementInnerBounds.Width;

            if (isScrollableElement)
            {
                elementBounds = elementInnerBounds;
            }
            InitPositionProvidersForCheckElement_(isScrollableElement, targetElement, state);

            Point originalElementLocation = elementBounds.Location;
            if (!"fixed".Equals(positionStyle, StringComparison.OrdinalIgnoreCase) && !state.EffectiveViewport.Contains(elementBounds))
            {
                elementBounds = BringRegionToView(elementBounds, state.EffectiveViewport.Location);
                if (StitchMode == StitchModes.CSS)
                {
                    if (isScrollableElement)
                    {
                        elementBounds = EyesSeleniumUtilsCommon.GetClientBoundsWithoutBorders(targetElement, jsExecutor_, Logger);
                    }
                    else
                    {
                        elementBounds = EyesSeleniumUtilsCommon.GetClientBounds(targetElement, jsExecutor_, Logger);
                    }
                }
            }

            Rectangle fullElementBounds = elementBounds;
            fullElementBounds.Width = Math.Max(fullElementBounds.Width, scrollSize.Width);
            fullElementBounds.Height = Math.Max(fullElementBounds.Height, scrollSize.Height);
            FrameChain currentFrameChain = GetCurrentFrameChain().Clone();
            Point screenshotOffset = GetFrameChainOffset_(currentFrameChain);
            Logger.Verbose(nameof(screenshotOffset) + ": {0}", screenshotOffset);
            fullElementBounds.Offset(screenshotOffset);

            state.FullRegion = fullElementBounds;

            // Now we have a 2-step part:
            // 1. Intersect the SRE and the effective viewport.
            if (StitchMode == StitchModes.CSS && userDefinedSRE_ != null)
            {
                Rectangle viewportInScreenshot = state.EffectiveViewport;
                Size elementTranslationSize = (Size)originalElementLocation - (Size)elementBounds.Location;
                state.EffectiveViewport = new Rectangle(viewportInScreenshot.Location, viewportInScreenshot.Size - elementTranslationSize);
            }

            // In CSS stitch mode, if the element is not scrollable but the SRE is (i.e., "Modal" case), 
            // we move the SRE to (0,0) but then we translate the element itself to get the full contents. 
            // However, in Scroll stitch mode, we scroll the SRE itself to the get full contents, and it 
            // already has an offset caused by "BringRegionToView", so we should consider this offset.
            if (StitchMode == StitchModes.Scroll && !isScrollableElement)
            {
                IWebElement sre = GetCurrentFrameScrollRootElement();
                state.StitchOffset = (Size)EyesSeleniumUtilsCommon.GetScrollPosition(sre, jsExecutor_, Logger);
            }

            // 2. Intersect the element and the effective viewport
            Rectangle elementBoundsInScreenshotCoordinates = elementBounds;
            elementBoundsInScreenshotCoordinates.Offset(screenshotOffset);
            state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, elementBoundsInScreenshotCoordinates);


            Rectangle? crop = ComputeCropRectangle(fullElementBounds, targetRegion);
            CheckWindowBase(crop, checkSettingsInternal, source: GetCurrentUrl());

            EyesSeleniumUtilsCommon.SetOverflow(orignalOverflow, jsExecutor_, targetElement);
        }

        protected static Point GetFrameChainOffset_(FrameChain frameChain)
        {
            Point offset = Point.Empty;
            foreach (IFrame frame in frameChain)
            {
                offset.Offset(frame.Location);
            }
            return offset;
        }

        internal abstract IWebElement GetCurrentFrameScrollRootElement();

        protected Rectangle BringRegionToView(Rectangle bounds, Point viewportLocation, bool compensateSRE = true)
        {
            IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
            IPositionProvider currentFramePositionProvider = GetPositionProvider(
                Logger, StitchMode, jsExecutor_, currentFrameSRE, userAgent_);
            Point initialFramePosition = currentFramePositionProvider.GetCurrentPosition();
            Point boundsLocation = bounds.Location;
            Point newFramePosition = boundsLocation - (Size)viewportLocation;

            // This comes to compensate scrolling of SRE.
            if (StitchMode == StitchModes.Scroll && compensateSRE)
            {
                newFramePosition += (Size)initialFramePosition;
            }

            Point framePositionAfterSet = currentFramePositionProvider.SetPosition(newFramePosition);
            bounds.Location += (Size)initialFramePosition - (Size)framePositionAfterSet;
            return bounds;
        }

        private Point GetCurrentFrameChainVisualOffset_()
        {
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)Driver.SwitchTo();
            FrameChain fc = GetCurrentFrameChain().Clone();
            Point offset = new Point();
            for (int i = fc.Count - 1; i >= 0; i--)
            {
                switchTo.ParentFrame();
                IFrame frame = fc[i];
                Point frameLocation = EyesSeleniumUtilsCommon.GetClientVisualOffset(frame.ReferenceObj, jsExecutor_, Logger);
                offset.Offset(frameLocation);
            }
            switchTo.Frames(fc);
            return offset;
        }

        private IWebElement GetTargetElementFromSettings_(ISeleniumCheckTarget seleniumCheckTarget)
        {
            By targetSelector = seleniumCheckTarget.GetTargetSelector();
            IWebElement targetElement = seleniumCheckTarget.GetTargetElement();
            if (targetElement == null && targetSelector != null)
            {
                targetElement = Driver.FindElement(targetSelector);
            }
            else if (targetElement != null && !(targetElement is EyesRemoteWebElement))
            {
                targetElement = new EyesRemoteWebElement(Logger, Driver, targetElement);
            }
            return targetElement;
        }

        public void Check(params ICheckSettings[] checkSettings)
        {
            ArgumentGuard.IsValidState(IsOpen, "Eyes not open");
            bool originalForceFPS = ForceFullPageScreenshot;
            lastCheckSettings_ = new List<object>();
            lastCheckSettingsInternal_ = new List<ICheckSettingsInternal>();
            if (checkSettings.Length > 1)
            {
                ForceFullPageScreenshot = true;
            }

            Dictionary<int, IGetRegions> getRegions = new Dictionary<int, IGetRegions>();
            Dictionary<int, ICheckSettingsInternal> checkSettingsInternalDictionary = new Dictionary<int, ICheckSettingsInternal>();

            for (int i = 0; i < checkSettings.Length; ++i)
            {
                ICheckSettings settings = checkSettings[i];
                ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)settings;
                ((IList<object>)lastCheckSettings_).Add(checkSettingsInternal.ToSerializableDictionary());
                ((IList<ICheckSettingsInternal>)lastCheckSettingsInternal_).Add(checkSettingsInternal);
                checkSettingsInternalDictionary.Add(i, checkSettingsInternal);

                Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();

                if (targetRegion != null)
                {
                    getRegions.Add(i, new SimpleRegionByRectangle(targetRegion.Value));
                }
                else if (settings is ISeleniumCheckTarget seleniumCheckTarget)
                {
                    IWebElement targetElement = GetTargetElement(seleniumCheckTarget);
                    if (targetElement == null && GetFrameChainLength() == 1)
                    {
                        targetElement = EyesSeleniumUtils.FindFrameByFrameCheckTarget(seleniumCheckTarget.GetFrameChain()[0], Driver);
                    }

                    if (targetElement != null)
                    {
                        getRegions.Add(i, CreateSimpleRegionByElement(targetElement));
                    }
                }
            }

            Logger.Verbose("setting scrollRootElement_...");
            userDefinedSRE_ = GetScrollRootElementFromSREContainer((ISeleniumCheckTarget)checkSettings[0], Driver);

            Logger.Verbose("scrollRootElement_ set to {0}", userDefinedSRE_);

            CurrentFramePositionProvider = null;
            PositionProvider = GetPositionProvider(Logger, StitchMode,
                jsExecutor_, userDefinedSRE_, userAgent_);

            MatchRegions(getRegions, checkSettingsInternalDictionary, checkSettings);
            ForceFullPageScreenshot = originalForceFPS;
        }

        public abstract int GetFrameChainLength();
        protected abstract IGetRegions CreateSimpleRegionByElement(IWebElement targetElement);

        private void MatchRegions(Dictionary<int, IGetRegions> getRegions,
                                  Dictionary<int, ICheckSettingsInternal> checkSettingsInternalDictionary,
                                  ICheckSettings[] checkSettings)
        {
            if (getRegions.Count == 0) return;

            this.OriginalFC = GetCurrentFrameChain().Clone();

            Region bbox = FindBoundingBox(getRegions, checkSettings);

            MatchWindowTask mwt = new MatchWindowTask(Logger, ServerConnector, runningSession_, Configuration.MatchTimeout, this);

            ScaleProviderFactory scaleProviderFactory = UpdateScalingParams();
            FullPageCaptureAlgorithm algo = CreateFullPageCaptureAlgorithm(scaleProviderFactory, new RenderingInfo());

            object activeElement = null;
            if (Config_.HideCaret)
            {
                activeElement = jsExecutor_.ExecuteScript(HIDE_CARET);
            }

            Region region = Region.Empty;
            bool hasFrames = GetFrameChainLength() > 0;
            if (hasFrames)
            {
                region = new Region(bbox.Location, EyesRemoteWebElement.GetClientSize(userDefinedSRE_, Driver));
            }
            else
            {
                IWebElement defaultSRE = GetDefaultRootElement();
                if (!defaultSRE.Equals(userDefinedSRE_))
                {
                    region = GetElementInnerRegion_(userDefinedSRE_);
                }
            }
            region.Intersect(effectiveViewport_);
            jsExecutor_.ExecuteScript(SET_DATA_APPLITOOLS_SCROLL_ATTR, (PositionProvider as ISeleniumPositionProvider)?.ScrolledElement);
            Bitmap screenshotImage = algo.GetStitchedRegion(region, bbox, PositionProvider, PositionProvider);

            DebugScreenshotProvider.Save(screenshotImage, "original");
            EyesScreenshot screenshot = new EyesWebDriverScreenshot(Logger, Driver, screenshotImage, EyesWebDriverScreenshot.ScreenshotTypeEnum.VIEWPORT, new Point(0, 0));

            for (int i = 0; i < checkSettings.Length; ++i)
            {
                if (!getRegions.ContainsKey(i)) continue;
                IGetRegions getRegion = getRegions[i];
                ICheckSettingsInternal checkSettingsInternal = checkSettingsInternalDictionary[i];
                List<EyesScreenshot> subScreenshots = GetSubScreenshots(hasFrames ? Region.Empty : bbox, screenshot, getRegion);
                MatchRegion(checkSettingsInternal, mwt, subScreenshots);
            }

            screenshot.DisposeImage();

            if (Config_.HideCaret && activeElement != null)
            {
                jsExecutor_.ExecuteScript("arguments[0].focus();", activeElement);
            }

            ((EyesWebDriverTargetLocator)Driver.SwitchTo()).Frames(OriginalFC);
        }

        private Region GetElementInnerRegion_(IWebElement element)
        {
            if (!(element is EyesRemoteWebElement eyesElement))
            {
                eyesElement = new EyesRemoteWebElement(Logger, Driver, element);
            }

            Point location = eyesElement.Location;
            SizeAndBorders sizeAndBorders = eyesElement.SizeAndBorders;

            Region region = new Region(
                location.X + sizeAndBorders.Borders.Left,
                location.Y + sizeAndBorders.Borders.Top,
                sizeAndBorders.Size.Width,
                sizeAndBorders.Size.Height);
            return region;
        }

        private List<EyesScreenshot> GetSubScreenshots(Region bbox, EyesScreenshot screenshot, IGetRegions getRegion)
        {
            List<EyesScreenshot> subScreenshots = new List<EyesScreenshot>();
            foreach (IMutableRegion r in getRegion.GetRegions(this, screenshot))
            {
                r.Offset(-bbox.Left, -bbox.Top);
                EyesScreenshot subScreenshot = screenshot.GetSubScreenshot(new Region(r), false);
                subScreenshots.Add(subScreenshot);
            }
            return subScreenshots;
        }

        private void MatchRegion(ICheckSettingsInternal checkSettingsInternal, MatchWindowTask mwt, List<EyesScreenshot> subScreenshots)
        {
            string name = checkSettingsInternal.GetName();
            string source = GetCurrentUrl();
            Logger.Log(TraceLevel.Info, TestId, Stage.Check, StageType.MatchStart, new { subScreenshots = subScreenshots.Count });
            foreach (EyesScreenshot subScreenshot in subScreenshots)
            {
                DebugScreenshotProvider.Save(subScreenshot.Image, $"subscreenshot_{name}");

                ImageMatchSettings ims = MatchWindowTask.CreateImageMatchSettings(checkSettingsInternal, this, subScreenshot);
                MatchWindowTask.CollectRegions(this, subScreenshot, checkSettingsInternal, ims);
                Location location = subScreenshot.GetLocationInScreenshot(Point.Empty, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
                AppOutput appOutput = new AppOutput(name, location, subScreenshot);

                MatchWindowData data = PrepareForMatch(checkSettingsInternal,
                    new Trigger[0], appOutput, name, false, ims, null, source);

                MatchResult matchResult = PerformMatch(data);

                Logger.Log(TraceLevel.Info, TestId, Stage.Check, StageType.MatchComplete, new { matchResult.AsExpected });
            }
        }

        private Region FindBoundingBox(Dictionary<int, IGetRegions> getRegions, ICheckSettings[] checkSettings)
        {
            Size rectSize = GetViewportSize();

            using (Bitmap dummyImage = new Bitmap(rectSize.Width, rectSize.Height))
            {
                EyesScreenshot screenshot = CreateScreenshot(dummyImage);
                return FindBoundingBox(getRegions, checkSettings, screenshot);
            }
        }

        protected abstract EyesScreenshot CreateScreenshot(Bitmap image);

        private Region FindBoundingBox(Dictionary<int, IGetRegions> getRegions, ICheckSettings[] checkSettings, EyesScreenshot screenshot)
        {
            Region bbox = Region.Empty;
            for (int i = 0; i < checkSettings.Length; ++i)
            {
                if (!getRegions.TryGetValue(i, out IGetRegions getRegion)) continue;
                IList<IMutableRegion> regions = getRegion.GetRegions(this, screenshot);
                foreach (IMutableRegion region in regions)
                {
                    if (bbox.IsEmpty)
                    {
                        bbox = new Region(region.Left, region.Top, region.Width, region.Height);
                    }
                    else
                    {
                        bbox = bbox.ExpandToContain(region);
                    }
                }
            }
            Point offset = screenshot.GetLocationInScreenshot(Point.Empty, CoordinatesTypeEnum.CONTEXT_AS_IS);
            return bbox.Offset(offset.X, offset.Y);
        }

        protected abstract IWebElement GetTargetElement(ISeleniumCheckTarget seleniumCheckTarget);

        protected override object GetAgentSetup()
        {
            Assembly eyesSeleniumAsm = this.GetType().Assembly;
            Assembly eyesCoreAsm = typeof(EyesBase).Assembly;
            string eyesSeleniumTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesSeleniumAsm);
            string eyesCoreTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesCoreAsm);
            string fluentCommandString = null;
            if (lastCheckSettingsInternal_ is ICheckSettingsInternal csInternal)
            {
                fluentCommandString = csInternal.GetFluentCommandString();
            }
            else if (lastCheckSettingsInternal_ is IList<ICheckSettingsInternal> lstCsInternal)
            {
                StringBuilder fluentCommandSB = new StringBuilder();
                foreach (ICheckSettingsInternal csi in lstCsInternal)
                {
                    fluentCommandSB.Append(csi.GetFluentCommandString());
                    fluentCommandSB.Append(", ");
                }
                if (fluentCommandSB.Length > 2) fluentCommandSB.Length -= 2;
                fluentCommandString = fluentCommandSB.ToString();
            }
            GetWebDriverInfo(
                out object remoteWebDriver,
                out HtmlElementInfo html,
                out HtmlElementInfo body,
                out string scrollRootElement,
                out string sessionId,
                out object capabilities);

            return new
            {
                SeleniumSessionId = sessionId,
                WebDriver = new
                {
                    remoteWebDriver.GetType().Name,
                    remoteWebDriver.GetType().Assembly.GetName().Version,
                    Capabilities = capabilities,
                },
                SeleniumSDKAssemblyData = new
                {
                    eyesSeleniumAsm.ImageRuntimeVersion,
                    eyesSeleniumTargetFramework
                },
                CoreSDKAssemblyData = new
                {
                    eyesCoreAsm.ImageRuntimeVersion,
                    eyesCoreTargetFramework
                },
                FullAgentId,
                DevicePixelRatio,
                CutProvider,
                ScaleProvider,
                Config_.StitchMode,
                Config_.HideScrollbars,
                Config_.HideCaret,
                ForceFullPageScreenshot,
                HtmlSizes = new
                {
                    DocumentElement = new
                    {
                        html.Id,
                        Client = html.ClientSize,
                        Scroll = html.ScrollSize,
                        Reported = html.Size
                    },
                    Body = new
                    {
                        body.Id,
                        Client = body.ClientSize,
                        Scroll = body.ScrollSize,
                        Reported = body.Size
                    },
                },
                ScrollRootElement = scrollRootElement,
                CheckSettings = lastCheckSettings_,
                FluentCommandString = fluentCommandString,
            };
        }

        protected abstract void GetWebDriverInfo(out object remoteWebDriver,
            out HtmlElementInfo html, out HtmlElementInfo body,
            out string scrollRootElement, out string sessionId, out object capabilities);

        protected abstract string GetSessionId();

        public IList<TestResultContainer> GetAllTestResults()
        {
            if (!((IEyes)this).IsCompleted())
            {
                return null;
            }

            return new TestResultContainer[] { testResultContainer_ };
        }

        public override IDictionary<string, IRunningTest> GetAllTests()
        {
            return new Dictionary<string, IRunningTest>() { { TestId, this } };
        }

        bool IEyes.IsCompleted()
        {
            return testResultContainer_ != null;
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            ScaleProviderFactory scaleProviderFactory = UpdateScalingParams();
            EyesScreenshot result;
            CheckState<IWebElement> state = ((ISeleniumCheckTargetCommon<IWebDriver, IWebElement>)checkSettingsInternal).State;
            IWebElement targetElement = state.TargetElementInternal;
            bool stitchContent = state.StitchContent;
            if (state.EffectiveViewport.Contains(state.FullRegion) && !state.FullRegion.IsEmpty)
            {
                Logger.Verbose($"{nameof(state.EffectiveViewport)}: {{0}} ; {nameof(state.FullRegion)}: {{1}}",
                    state.EffectiveViewport, state.FullRegion);
                result = GetViewportScreenshot(scaleProviderFactory, state);
                EyesScreenshot fullRegionResult = result.GetSubScreenshot(state.FullRegion, true);
                result.DisposeImage();
                result = fullRegionResult;
            }
            else if (targetElement != null || stitchContent)
            {
                result = GetFrameOrElementScreenshot(scaleProviderFactory, state);
            }
            else
            {
                result = GetViewportScreenshot(scaleProviderFactory, state);
            }

            if (targetRegion.HasValue)
            {
                EyesScreenshot targetRegionResult = result.GetSubScreenshot(targetRegion.Value, false);
                result.DisposeImage();
                result = targetRegionResult;
            }

            result.DomUrl = TryCaptureAndPostDom(checkSettingsInternal);
            EnsureCorrectFrame(state);
            result.OriginLocation = state.OriginLocation;
            return result;
        }

        protected abstract void EnsureCorrectFrame(CheckState<IWebElement> state);
        protected abstract EyesScreenshot GetFrameOrElementScreenshot(ScaleProviderFactory scaleProviderFactory, CheckState<IWebElement> state);
        protected abstract EyesScreenshot GetViewportScreenshot(ScaleProviderFactory scaleProviderFactory, CheckState<IWebElement> state);

        public void CloseAsync()
        {
            Close(false);
        }

        public override TestResults Close(bool throwEx)
        {
            TestResults results;
            try
            {
                results = StopSession(false);
                CloseCompleted(results);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Close, e, TestId);
                CloseFailed(e);
                throw;
            }

            if (exception_ != null && throwEx)
            {
                throw new Exception("Error", exception_);
            }

            if (runner_ != null)
            {
                runner_.AggregateResult(testResultContainer_);
            }
            return results;
        }

        protected internal override MatchWindowData PrepareForMatch(ICheckTask checkTask)
        {
            throw new NotImplementedException();
        }

        protected internal override ICheckTask IssueCheck(ICheckSettings checkSettings,
            IList<VisualGridSelector[]> regionSelectors, string source, IList<IUserAction> userInputs)
        {
            throw new NotImplementedException();
        }

        protected internal override void CheckCompleted(ICheckTask checkTask, MatchResult matchResult)
        {
            throw new NotImplementedException();
        }

        protected override void SetConfigImpl(Applitools.IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public abstract int GetFrameChainLength(ISeleniumCheckTarget seleniumCheckTarget);

        public bool IsCompleted()
        {
            throw new System.NotImplementedException();
        }

        public abstract void AddKeyboardTrigger(IWebElement element, string text);
        public abstract void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor);
    }
}

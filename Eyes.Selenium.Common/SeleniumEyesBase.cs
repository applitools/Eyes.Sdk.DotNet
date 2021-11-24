using Applitools.Fluent;
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
//using CssTranslatePositionProvider = Applitools.Selenium.Scrolling.CssTranslatePositionProvider;
//using ScrollPositionProvider = Applitools.Selenium.Scrolling.ScrollPositionProvider;

namespace Applitools.Selenium.Common
{
    internal abstract class SeleniumEyesBase<IWebElement, IWebDriver> : RunningTest, IEyes, IUserActionsEyes<IWebElement>, ISeleniumEyesCommon
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

        protected bool? isDisabled_;
        protected object lastCheckSettings_;
        protected object lastCheckSettingsInternal_;
        protected IEyesJsExecutor jsExecutor_;
        internal UserAgent userAgent_;
        private IWebDriver driver_;

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

            jsExecutor_ = new SeleniumJavaScriptExecutor(driver_);

            ImageProvider = ImageProviderFactory.GetImageProvider(userAgent_, this, Logger, driver_);
            regionPositionCompensation_ = RegionPositionCompensationFactory.GetRegionPositionCompensation(userAgent_, this, Logger);

            OpenBase();
            DevicePixelRatio = UnknownDevicePixelRatio_;
            //PositionProvider = SeleniumScrollPositionProviderFactory.GetPositionProvider(StitchMode, jsExecutor_, scrollRootElement_);
            string batchId = Batch.Id;
            runner_.AddBatch(batchId, this);

            return driver_;
        }

        protected abstract string GetUserAgent();

        protected void InitDriver_(IWebDriver driver)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));

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

            Type driverType = driver_.RemoteWebDriver.GetType();
            Logger.Log(TraceLevel.Info, Stage.Open, new { driverType });
            MethodInfo getPlatformNameMI = driverType.GetProperty("PlatformName")?.GetGetMethod();
            MethodInfo getSessionDetailsMI = driverType.GetProperty("SessionDetails")?.GetGetMethod();

            if (getPlatformNameMI != null)
            {
                PlatformName = (string)getPlatformNameMI.Invoke(driver_.RemoteWebDriver, null);
            }
            else
            {
                PlatformName = (driver_.GetSessionDetail("platformName") as string) ?? (driver_.GetSessionDetail("platform") as string);
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
                new { message = "Initialized Web Driver.", seleniumSessionId = driver_.RemoteWebDriver.SessionId, PlatformName });
        }

        private void InitSessionDetails_(MethodInfo getSessionDetailsMI)
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = (IDictionary<string, object>)getSessionDetailsMI.Invoke(GetRemoteWebDriver(), null);
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = getSessionDetailsMI.Invoke(GetRemoteWebDriver(), null)
                    });
            }
        }

        protected abstract object GetRemoteWebDriver();

        private void InitSessionDetailsUsingInternalImplementation_()
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = driver_.SessionDetails;
                if (CachedSessionDetails == null) break;
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = driver_.SessionDetails
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

                Logger.Verbose("URL: {0}", driver_.Url);
                ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
                ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;
                lastCheckSettings_ = checkSettingsInternal.ToSerializableDictionary();
                lastCheckSettingsInternal_ = checkSettingsInternal;

                CheckState<IWebElement> state = InitCheckState_(checkSettings, checkSettingsInternal, seleniumCheckTarget);

                Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();

                Logger.Verbose("setting " + nameof(userDefinedSRE_) + " ...");
                userDefinedSRE_ = TryGetUserDefinedSREFromSREContainer(seleniumCheckTarget, driver_);
                IWebElement scrollRootElement = userDefinedSRE_;
                if (scrollRootElement == null)
                {
                    scrollRootElement = EyesSeleniumUtils.GetDefaultRootElement(driver_);
                }
                Logger.Verbose(nameof(userDefinedSRE_) + " set to {0}", userDefinedSRE_?.ToString() ?? "null");
                Logger.Verbose(nameof(scrollRootElement) + " set to {0}", scrollRootElement);

                CurrentFramePositionProvider = null;
                PositionProvider = GetPositionProvider(Logger, StitchMode, jsExecutor_, scrollRootElement, userAgent_);
                CaretVisibilityProvider caretVisibilityProvider = new CaretVisibilityProvider(Logger, driver_, Config_);

                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();

                PageState pageState = new PageState(Logger, driver_, StitchMode, userAgent_);
                pageState.PreparePage(seleniumCheckTarget, Config_, scrollRootElement);

                FrameChain frameChainAfterSwitchToTarget = driver_.GetFrameChain().Clone();

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
                else if (seleniumCheckTarget.GetFrameChain().Count > 0)
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

        internal CheckState<IWebElement> InitCheckState_(ICheckSettings checkSettings,
            ICheckSettingsInternal checkSettingsInternal, ISeleniumCheckTarget seleniumCheckTarget)
        {
            CheckState<IWebElement> state = new CheckState<IWebElement>();
            seleniumCheckTarget.State = state;
            state.StitchContent = checkSettingsInternal.GetStitchContent() ?? Config_.IsForceFullPageScreenshot ?? false;

            ICheckSettingsInternal clonedCheckSettings = (ICheckSettingsInternal)checkSettings.Clone();

            // Ensure frame is not used as a region
            ((SeleniumCheckSettings)checkSettings).SanitizeSettings(Logger, driver_, state);
            return state;
        }

        private void CheckWindow_(ICheckSettingsInternal checkSettingsInternal)
        {
            Logger.Verbose("Target.Window()");

            CheckWindowBase(null, checkSettingsInternal, Source);
        }

        protected abstract string Source { get; }

        private void CheckFullWindow_(ICheckSettingsInternal checkSettingsInternal, CheckState<IWebElement> state,
            IWebElement scrollRootElement)
        {
            Logger.Verbose("Target.Window().Fully(true)");

            InitPositionProvidersForCheckWindow_(state, scrollRootElement);

            CheckWindowBase(null, checkSettingsInternal, source: driver_.Url);
        }

        private void InitPositionProvidersForCheckWindow_(CheckState<IWebElement> state, IWebElement scrollRootElement)
        {
            if (StitchMode == StitchModes.Scroll)
            {
                state.StitchPositionProvider = new ScrollPositionProvider(Logger, driver_, scrollRootElement);
            }
            else // Stitch mode == CSS
            {
                if (userDefinedSRE_ != null)
                {
                    state.StitchPositionProvider = new ElementPositionProvider(Logger, driver_, userDefinedSRE_);
                }
                else
                {
                    state.StitchPositionProvider = new CssTranslatePositionProvider(Logger, driver_, scrollRootElement);
                    state.OriginPositionProvider = new ScrollPositionProvider(Logger, driver_, scrollRootElement);
                }
            }
        }

        private Rectangle ComputeEffectiveViewport(FrameChain frameChain, RectangleSize initialSize)
        {
            Rectangle viewport = new Rectangle(Point.Empty, initialSize);
            if (userDefinedSRE_ != null)
            {
                Rectangle sreInnerBounds = EyesRemoteWebElement.GetClientBoundsWithoutBorders(userDefinedSRE_, driver_, Logger);
                viewport.Intersect(sreInnerBounds);
            }
            Point offset = Point.Empty;
            foreach (Frame frame in frameChain)
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
            if (((ISeleniumCheckTarget)checkSettingsInternal).GetFrameChain().Count > 0)
            {
                Logger.Verbose("Target.Frame(frame)...Region(element).Fully(true)");
            }
            else
            {
                Logger.Verbose("Target.Region(element).Fully(true)");
            }

            // Hide scrollbars
            string orignalOverflow = EyesSeleniumUtils.SetOverflow("hidden", driver_, targetElement);

            // Get element's scroll size and bounds
            Size scrollSize = EyesRemoteWebElement.GetScrollSize(targetElement, driver_, Logger);
            Rectangle elementBounds = EyesRemoteWebElement.GetClientBounds(targetElement, driver_, Logger);
            Rectangle elementInnerBounds = EyesRemoteWebElement.GetClientBoundsWithoutBorders(targetElement, driver_, Logger);
            string positionStyle = EyesRemoteWebElement.GetComputedStyle("position", targetElement, driver_);

            Point el = EyesRemoteWebElement.GetOffsetPosition(targetElement, driver_, Logger);
            el.Offset(elementInnerBounds.X - elementBounds.X, elementInnerBounds.Y - elementBounds.Y);
            IWebElement currentFrame = driver_.FindElement(By.TagName("html"));
            if (userDefinedSRE_ != null && !userDefinedSRE_.Equals(currentFrame))
            {
                el.Offset(EyesRemoteWebElement.GetScrollPosition(currentFrame, driver_, Logger));
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
                        elementBounds = EyesRemoteWebElement.GetClientBoundsWithoutBorders(targetElement, driver_, Logger);
                    }
                    else
                    {
                        elementBounds = EyesRemoteWebElement.GetClientBounds(targetElement, driver_, Logger);
                    }
                }
            }

            Rectangle fullElementBounds = elementBounds;
            fullElementBounds.Width = Math.Max(fullElementBounds.Width, scrollSize.Width);
            fullElementBounds.Height = Math.Max(fullElementBounds.Height, scrollSize.Height);
            FrameChain currentFrameChain = driver_.GetFrameChain().Clone();
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
                state.StitchOffset = (Size)EyesRemoteWebElement.GetScrollPosition(sre, driver_, Logger);
            }

            // 2. Intersect the element and the effective viewport
            Rectangle elementBoundsInScreenshotCoordinates = elementBounds;
            elementBoundsInScreenshotCoordinates.Offset(screenshotOffset);
            state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, elementBoundsInScreenshotCoordinates);


            Rectangle? crop = ComputeCropRectangle(fullElementBounds, targetRegion);
            CheckWindowBase(crop, checkSettingsInternal, source: driver_.Url);

            EyesSeleniumUtils.SetOverflow(orignalOverflow, driver_, targetElement);
        }

        private Point GetCurrentFrameChainVisualOffset_()
        {
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();
            FrameChain fc = driver_.GetFrameChain().Clone();
            Point offset = new Point();
            for (int i = fc.Count - 1; i >= 0; i--)
            {
                switchTo.ParentFrame();
                Frame frame = fc[i];
                Point frameLocation = EyesRemoteWebElement.GetClientVisualOffset(frame.Reference, driver_, Logger);
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
                targetElement = driver_.FindElement(targetSelector);
            }
            else if (targetElement != null && !(targetElement is EyesRemoteWebElement))
            {
                targetElement = new EyesRemoteWebElement(Logger, driver_, targetElement);
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
                    if (targetElement == null && seleniumCheckTarget.GetFrameChain().Count == 1)
                    {
                        targetElement = EyesSeleniumUtils.FindFrameByFrameCheckTarget(seleniumCheckTarget.GetFrameChain()[0], driver_);
                    }

                    if (targetElement != null)
                    {
                        getRegions.Add(i, new SimpleRegionByElement(targetElement));
                    }
                }
            }

            Logger.Verbose("setting scrollRootElement_...");
            userDefinedSRE_ = GetScrollRootElementFromSREContainer((IScrollRootElementContainer)checkSettings[0], driver_);

            Logger.Verbose("scrollRootElement_ set to {0}", userDefinedSRE_);

            CurrentFramePositionProvider = null;
            PositionProvider = GetPositionProvider(Logger, StitchMode,
                jsExecutor_, userDefinedSRE_, userAgent_);

            MatchRegions(getRegions, checkSettingsInternalDictionary, checkSettings);
            ForceFullPageScreenshot = originalForceFPS;
        }


        private void MatchRegions(Dictionary<int, IGetRegions> getRegions,
                                  Dictionary<int, ICheckSettingsInternal> checkSettingsInternalDictionary,
                                  ICheckSettings[] checkSettings)
        {
            if (getRegions.Count == 0) return;

            this.OriginalFC = driver_.GetFrameChain().Clone();

            Region bbox = FindBoundingBox(getRegions, checkSettings);

            MatchWindowTask mwt = new MatchWindowTask(Logger, ServerConnector, runningSession_, Configuration.MatchTimeout, this);

            ScaleProviderFactory scaleProviderFactory = UpdateScalingParams();
            FullPageCaptureAlgorithm algo = CreateFullPageCaptureAlgorithm(scaleProviderFactory, new RenderingInfo());

            object activeElement = null;
            if (Config_.HideCaret)
            {
                activeElement = driver_.ExecuteScript(HIDE_CARET);
            }

            Region region = Region.Empty;
            bool hasFrames = driver_.GetFrameChain().Count > 0;
            if (hasFrames)
            {
                region = new Region(bbox.Location, EyesRemoteWebElement.GetClientSize(userDefinedSRE_, driver_));
            }
            else
            {
                IWebElement defaultSRE = EyesSeleniumUtils.GetDefaultRootElement(driver_);
                if (!defaultSRE.Equals(userDefinedSRE_))
                {
                    region = GetElementInnerRegion_(userDefinedSRE_);
                }
            }
            region.Intersect(effectiveViewport_);
            jsExecutor_.ExecuteScript(SET_DATA_APPLITOOLS_SCROLL_ATTR, (PositionProvider as ISeleniumPositionProvider)?.ScrolledElement);
            Bitmap screenshotImage = algo.GetStitchedRegion(region, bbox, PositionProvider, PositionProvider);

            DebugScreenshotProvider.Save(screenshotImage, "original");
            EyesScreenshot screenshot = new EyesWebDriverScreenshot(Logger, driver_, screenshotImage, EyesWebDriverScreenshot.ScreenshotTypeEnum.VIEWPORT, new Point(0, 0));

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
                driver_.ExecuteScript("arguments[0].focus();", activeElement);
            }

            ((EyesWebDriverTargetLocator)driver_.SwitchTo()).Frames(OriginalFC);
        }

        private Region GetElementInnerRegion_(IWebElement element)
        {
            if (!(element is EyesRemoteWebElement eyesElement))
            {
                eyesElement = new EyesRemoteWebElement(Logger, driver_, element);
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
            string source = driver_.Url;
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
                EyesScreenshot screenshot = new EyesWebDriverScreenshot(Logger, driver_, dummyImage);
                return FindBoundingBox(getRegions, checkSettings, screenshot);
            }
        }

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

        private IWebElement GetTargetElement(ISeleniumCheckTarget seleniumCheckTarget)
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
            object remoteWebDriver, capabilities;
            HtmlElementInfo html, body;
            string scrollRootElement, sessionId;
            GetWebDriverInfo(out remoteWebDriver, out html, out body, out scrollRootElement, out sessionId, out capabilities);
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
            EyesWebDriverScreenshot result;
            CheckState state = ((ISeleniumCheckTarget)checkSettingsInternal).State;
            IWebElement targetElement = state.TargetElementInternal;
            bool stitchContent = state.StitchContent;
            if (state.EffectiveViewport.Contains(state.FullRegion) && !state.FullRegion.IsEmpty)
            {
                Logger.Verbose($"{nameof(state.EffectiveViewport)}: {{0}} ; {nameof(state.FullRegion)}: {{1}}",
                    state.EffectiveViewport, state.FullRegion);
                result = GetViewportScreenshot_(scaleProviderFactory, state);
                EyesWebDriverScreenshot fullRegionResult = (EyesWebDriverScreenshot)result.GetSubScreenshot(
                    state.FullRegion, true);
                result.DisposeImage();
                result = fullRegionResult;
            }
            else if (targetElement != null || stitchContent)
            {
                result = GetFrameOrElementScreenshot_(scaleProviderFactory, state);
            }
            else
            {
                result = GetViewportScreenshot_(scaleProviderFactory, state);
            }

            if (targetRegion.HasValue)
            {
                EyesWebDriverScreenshot targetRegionResult = (EyesWebDriverScreenshot)result.GetSubScreenshot(
                    targetRegion.Value, false);
                result.DisposeImage();
                result = targetRegionResult;
            }

            result.DomUrl = TryCaptureAndPostDom(checkSettingsInternal);
            FrameChain frameChain = driver_.GetFrameChain();
            if (state.FrameToSwitchTo != null &&
                (frameChain.Count == 0 ||
                state.FrameToSwitchTo != frameChain.Last().Reference))
            {
                driver_.SwitchTo().Frame(state.FrameToSwitchTo);
            }
            result.OriginLocation = state.OriginLocation;
            return result;
        }

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
        public abstract int GetFrameChainLength();


        public bool IsCompleted()
        {
            throw new System.NotImplementedException();
        }

        public abstract void AddKeyboardTrigger(IWebElement element, string text);
        public abstract void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor);
    }
}

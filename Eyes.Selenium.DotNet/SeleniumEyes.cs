using Applitools.Capture;
using Applitools.Cropping;
using Applitools.Fluent;
using Applitools.Positioning;
using Applitools.Selenium.Capture;
using Applitools.Selenium.Fluent;
using Applitools.Selenium.Positioning;
using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using Applitools.Utils.Cropping;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using Applitools.VisualGrid;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Region = Applitools.Utils.Geometry.Region;
using ScrollPositionProvider = Applitools.Selenium.Scrolling.ScrollPositionProvider;
using CssTranslatePositionProvider = Applitools.Selenium.Scrolling.CssTranslatePositionProvider;
using System.Net;

namespace Applitools.Selenium
{
    /// <summary>
    /// Applitools Eyes Selenium DotNet API.
    /// </summary>
    internal class SeleniumEyes : RunningTest, IEyes, ISeleniumEyes, IUserActionsEyes
    {
        private const string SET_DATA_APPLITOOLS_SCROLL_ATTR = "var e = arguments[0]; if (e != null) e.setAttribute('data-applitools-scroll','true');";

        private const string HIDE_CARET = "var activeElement = document.activeElement; activeElement && activeElement.blur(); return activeElement;";
        #region Fields

        private static readonly double UnknownDevicePixelRatio_ = 0;
        private static readonly double DefaultDevicePixelRatio_ = 1;

        private EyesWebDriver driver_;
        private bool dontGetTitle_;
        private SeleniumJavaScriptExecutor jsExecutor_;

        internal UserAgent userAgent_;
        private IRegionPositionCompensation regionPositionCompensation_;

        private Region effectiveViewport_;

        private IWebElement userDefinedSRE_;
        private ISeleniumConfigurationProvider configProvider_;

        private readonly SeleniumPositionProviderFactory positionProviderFactory_ = new SeleniumPositionProviderFactory();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public SeleniumEyes(ISeleniumConfigurationProvider configurationProvider, Uri serverUrl, ClassicRunner runner)
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
        public SeleniumEyes(ISeleniumConfigurationProvider configurationProvider, ClassicRunner runner)
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

        private Configuration Config_ => configProvider_.GetConfiguration();

        #endregion Constructors

        #region Properties

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
        public override string ServerUrl { get => base.ServerUrl ?? runner_.ServerUrl; set => base.ServerUrl = value; }
        public override WebProxy Proxy { get => base.Proxy ?? runner_.Proxy; set => base.Proxy = value; }

        private bool? isDisabled_;
        private object lastCheckSettings_;
        private object lastCheckSettingsInternal_;

        public IDictionary<string, object> CachedSessionDetails { get; private set; }


        public override bool IsDisabled { get => isDisabled_ ?? runner_.IsDisabled; set => isDisabled_ = value; }

        public IPositionProvider CurrentFramePositionProvider { get; protected set; }

        public void SetScrollToRegion(bool shouldScroll) { }

        public double DevicePixelRatio { get; set; }

        public IImageProvider ImageProvider { get; private set; }

        internal FrameChain OriginalFC { get; set; }

        public string PlatformName { get; private set; }

        #endregion Properties

        #region Methods

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
            //if (!viewportSize.IsEmpty)
            //{
            Config_.SetViewportSize(new RectangleSize(viewportSize));
            //}
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

            string uaString = driver_.GetUserAgent();
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

        private void InitDriver_(IWebDriver driver)
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
                CachedSessionDetails = (IDictionary<string, object>)getSessionDetailsMI.Invoke(driver_.RemoteWebDriver, null);
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = getSessionDetailsMI.Invoke(driver_.RemoteWebDriver, null)
                    });
            }
        }

        private void InitSessionDetailsUsingInternalImplementation_()
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = driver_.SessionDetails;
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

        private FullPageCaptureAlgorithm CreateFullPageCaptureAlgorithm(ScaleProviderFactory scaleProviderFactory, RenderingInfo renderingInfo)
        {
            ISizeAdjuster sizeAdjuster = ImageProviderFactory.GetImageSizeAdjuster(userAgent_, jsExecutor_);
            return new FullPageCaptureAlgorithm(
                Logger, TestId, regionPositionCompensation_, WaitBeforeScreenshots,
                DebugScreenshotProvider,
                (image) => new EyesWebDriverScreenshot(Logger, driver_, image,
                    EyesWebDriverScreenshot.ScreenshotTypeEnum.VIEWPORT, Point.Empty),
                scaleProviderFactory, CutProvider, Config_.StitchOverlap, ImageProvider,
                renderingInfo.MaxImageHeight, renderingInfo.MaxImageArea, sizeAdjuster);
        }

        /// <summary>
        /// Takes a snapshot of the application under test, where the capture area and settings
        /// are given by <paramref name="checkSettings"/>.
        /// </summary>
        /// <param name="checkSettings">A settings object defining the capture area and parameters.
        /// Created fluently using the <see cref="Target"/> static class.</param>
        public void Check(ICheckSettings checkSettings)
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

                CheckState state = new CheckState();
                seleniumCheckTarget.State = state;
                state.StitchContent = (checkSettingsInternal.GetStitchContent() ?? false) || ForceFullPageScreenshot;

                ICheckSettingsInternal clonedCheckSettings = (ICheckSettingsInternal)checkSettings.Clone();

                // Ensure frame is not used as a region
                ((SeleniumCheckSettings)checkSettings).SanitizeSettings(Logger, driver_, state);

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

        private void CheckWindow_(ICheckSettingsInternal checkSettingsInternal)
        {
            Logger.Verbose("Target.Window()");

            CheckWindowBase(null, checkSettingsInternal, source: driver_.Url);
        }

        private void CheckFullWindow_(ICheckSettingsInternal checkSettingsInternal, CheckState state,
            IWebElement scrollRootElement)
        {
            Logger.Verbose("Target.Window().Fully(true)");

            InitPositionProvidersForCheckWindow_(state, scrollRootElement);

            CheckWindowBase(null, checkSettingsInternal, source: driver_.Url);
        }

        private void InitPositionProvidersForCheckWindow_(CheckState state, IWebElement scrollRootElement)
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
            Rectangle? targetRegion, CheckState state)
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

        private void CheckElement_(ICheckSettingsInternal checkSettingsInternal, IWebElement targetElement,
            Rectangle? targetRegion, CheckState state)
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

        private static Rectangle? ComputeCropRectangle(Rectangle fullRect, Rectangle? cropRect)
        {
            if (cropRect == null) return null;
            Rectangle crop = fullRect;
            Rectangle cropRectValue = cropRect.Value;
            cropRectValue.Offset(crop.Location);
            crop.Intersect(cropRectValue);
            return crop;
        }

        private Rectangle BringRegionToView(Rectangle bounds, Point viewportLocation, bool compensateSRE = true)
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

        private void CheckFullRegion_(ICheckSettingsInternal checkSettingsInternal, Rectangle targetRegion, CheckState state)
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
            Frame currentFrame = currentFrameChain.Peek();
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

        private void CheckFullFrame_(ICheckSettingsInternal checkSettingsInternal, CheckState state)
        {
            Logger.Verbose("Target.Frame(frame).Fully(true)");
            FrameChain currentFrameChain = driver_.GetFrameChain().Clone();
            Point visualOffset = GetFrameChainOffset_(currentFrameChain);
            Frame currentFrame = currentFrameChain.Peek();
            state.EffectiveViewport = Rectangle.Intersect(state.EffectiveViewport, new Rectangle(visualOffset, currentFrame.InnerSize));

            IWebElement currentFrameSRE = GetCurrentFrameScrollRootElement();
            Size currentSREScrollSize = EyesRemoteWebElement.GetScrollSize(currentFrameSRE, driver_, Logger);
            state.FullRegion = new Rectangle(state.EffectiveViewport.Location, currentSREScrollSize);

            //string originalOverflow = EyesSeleniumUtils.SetOverflow("hidden", driver_, currentFrameSRE);

            CheckWindowBase(null, checkSettingsInternal, source: driver_.Url);

            //EyesSeleniumUtils.SetOverflow(originalOverflow, driver_, currentFrameSRE);
        }

        private static Point GetFrameChainOffset_(FrameChain frameChain)
        {
            Point offset = Point.Empty;
            foreach (Frame frame in frameChain)
            {
                offset.Offset(frame.Location);
            }
            return offset;
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

        internal static IWebElement GetScrollRootElementFromSREContainer(IScrollRootElementContainer scrollRootElementContainer, EyesWebDriver driver)
        {
            IWebElement scrollRootElement = TryGetUserDefinedSREFromSREContainer(scrollRootElementContainer, driver);
            if (scrollRootElement == null)
            {
                scrollRootElement = EyesSeleniumUtils.GetDefaultRootElement(driver);
            }
            return scrollRootElement;
        }

        internal static IWebElement TryGetUserDefinedSREFromSREContainer(IScrollRootElementContainer scrollRootElementContainer, EyesWebDriver driver)
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

        internal IWebElement GetCurrentFrameScrollRootElement()
        {
            return EyesSeleniumUtils.GetCurrentFrameScrollRootElement(driver_, userDefinedSRE_);
        }

        void IUserActionsEyes.AddMouseTrigger(MouseAction action, IWebElement element, Point cursor)
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

        void IUserActionsEyes.AddKeyboardTrigger(IWebElement element, string text)
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

        private ScaleProviderFactory UpdateScalingParams()
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

        private ScaleProviderFactory GetScaleProviderFactory_()
        {
            IWebElement element = EyesSeleniumUtils.GetDefaultRootElement(driver_);
            Size entireSize = EyesRemoteWebElement.GetEntireSize(element, driver_, Logger);
            return new ContextBasedScaleProviderFactory(entireSize,
                    GetViewportSize(), DevicePixelRatio, ScaleProvider, SetScaleProvider);
        }

        private void InitDevicePixelRatio_()
        {
            Logger.Verbose("Trying to extract device pixel ratio...");
            try
            {
                object dpr = JSBrowserCommands.WithReturn.GetDevicePixelRatio(driver_);
                DevicePixelRatio = double.Parse(dpr.ToString());
            }
            catch (Exception)
            {
                Logger.Verbose("Failed to extract device pixel ratio! Using default.");
                DevicePixelRatio = DefaultDevicePixelRatio_;
            }
            Logger.Verbose("Device pixel ratio: {0}", DevicePixelRatio);
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

        private EyesWebDriverScreenshot GetFrameOrElementScreenshot_(ScaleProviderFactory scaleProviderFactory, CheckState state)
        {
            EyesWebDriverScreenshot result;
            bool switchedFrame = false;
            FrameChain originalFrameChain = driver_.GetFrameChain().Clone();
            Point originalViewportLocation = effectiveViewport_.Location;
            if (state.EffectiveViewport.IsEmpty)
            {
                switchedFrame = true;
                Frame frame;
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
                Frame frame;
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

        private EyesWebDriverScreenshot GetViewportScreenshot_(ScaleProviderFactory scaleProviderFactory, CheckState state)
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
                Frame frame;
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

        internal IPositionProvider GetPositionProvider(Logger logger, StitchModes stitchMode, IEyesJsExecutor executor, IWebElement scrollRootElement, UserAgent userAgent = null)
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

        protected override object GetAgentSetup()
        {
            RemoteWebDriver remoteWebDriver = driver_.RemoteWebDriver;
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
            EyesRemoteWebElement html = (EyesRemoteWebElement)driver_.FindElement(By.TagName("html"));
            EyesRemoteWebElement body = (EyesRemoteWebElement)driver_.FindElement(By.TagName("body"));
            return new
            {
                SeleniumSessionId = remoteWebDriver.SessionId.ToString(),
                WebDriver = new
                {
                    remoteWebDriver.GetType().Name,
                    remoteWebDriver.GetType().Assembly.GetName().Version,
                    remoteWebDriver.Capabilities,
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
                        Id = html.ToString(),
                        Client = html.ClientSize,
                        Scroll = html.ScrollSize,
                        Reported = html.Size
                    },
                    Body = new
                    {
                        Id = body.ToString(),
                        Client = body.ClientSize,
                        Scroll = body.ScrollSize,
                        Reported = body.Size
                    },
                },
                ScrollRootElement = userDefinedSRE_?.ToString(),
                CheckSettings = lastCheckSettings_,
                FluentCommandString = fluentCommandString,
            };
        }

        public override IDictionary<string, IRunningTest> GetAllTests()
        {
            return new Dictionary<string, IRunningTest>() { { TestId, this } };
        }

        bool IEyes.IsCompleted()
        {
            return testResultContainer_ != null;
        }

        public IList<TestResultContainer> GetAllTestResults()
        {
            if (!((IEyes)this).IsCompleted())
            {
                return null;
            }

            return new TestResultContainer[] { testResultContainer_ };
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

        #endregion Methods
    }
}

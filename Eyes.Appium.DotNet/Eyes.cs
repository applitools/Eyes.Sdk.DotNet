using Applitools.Appium.Capture;
using Applitools.Appium.Exceptions;
using Applitools.Appium.Fluent;
using Applitools.Capture;
using Applitools.Cropping;
using Applitools.Fluent;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium
{
    public sealed class Eyes : EyesBase
    {
        private const string NATIVE_APP = "NATIVE_APP";

        private RemoteWebDriver driver_;
        private MethodInfo getPlatformName_;
        private MethodInfo getSessionDetails_;

        public IDictionary<string, object> CachedSessionDetails { get; private set; }

        internal double GetScaleRatioForRegions()
        {
            return getPlatformName_.Invoke(driver_, null).Equals("iOS") ? 1 : ScaleRatio;
        }

        public static Func<IWebDriver, bool> IsLandscapeOrientation = IsLandscapeOrientationImpl_;

        private IImageProvider imageProvider_;

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl)
            : base(serverUrl)
        {
            Init_();
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
            : base()
        {
            Init_();
        }

        private void Init_()
        {
        }

        #endregion

        #region Configuration
        private Configuration configuration_ = new Configuration();

        protected override Configuration Configuration { get => configuration_; }

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        protected override void SetConfigImpl(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }
        #endregion

        #region Properties

        protected override bool ViewportSizeRequired => false;

        public Rectangle CachedViewport { get; private set; }

        #endregion

        internal IWebDriver GetDriver() { return driver_; }

        public RemoteWebDriver Open(RemoteWebDriver driver, string appName, string testName)
        {
            Logger.GetILogHandler().Open();

            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return driver;
            }

            driver_ = driver;
            Type driverType = driver.GetType();
            if (driverType.GenericTypeArguments.Length != 1)
            {
                throw new EyesException($"driver is not an AppiumDriver<IWebElement>");
            }
            Type argType = driverType.GenericTypeArguments[0];
            Type appiumDriverType = typeof(AppiumDriver<>).MakeGenericType(argType);
            if (!appiumDriverType.IsAssignableFrom(driverType))
            {
                throw new EyesException($"driver is not an AppiumDriver<{argType.Name}>");
            }
            getPlatformName_ = appiumDriverType.GetProperty("PlatformName").GetGetMethod();
            getSessionDetails_ = appiumDriverType.GetProperty("SessionDetails").GetGetMethod();

            InitSessionDetails_();
            InitViewportFromSessionDetails_();

            imageProvider_ = new TakesScreenshotImageProvider(Logger, driver_, this);
            Region viewport = CachedViewport;
            Size viewportSize = viewport.Scale(ScaleRatio).Size;
            OpenBase(appName, testName, viewportSize);

            return driver_;
        }

        private void InitViewportFromSessionDetails_()
        {
            object viewportRectObj = null;
            object pixelRatioObj = 1;

            if (CachedSessionDetails?.TryGetValue("pixelRatio", out pixelRatioObj) == true)
            {
                Logger.Verbose("setting pixel ratio to {0}", pixelRatioObj);
                ScaleRatio = 1 / Convert.ToDouble(pixelRatioObj);
            }
            if (CachedSessionDetails?.TryGetValue("viewportRect", out viewportRectObj) == true)
            {
                Dictionary<string, object> viewportRectDict = (Dictionary<string, object>)viewportRectObj;
                CachedViewport = new Rectangle(
                    Convert.ToInt32(viewportRectDict["left"]), Convert.ToInt32(viewportRectDict["top"]),
                    Convert.ToInt32(viewportRectDict["width"]), Convert.ToInt32(viewportRectDict["height"]));

            }
        }

        private void InitSessionDetails_()
        {
            int attempts = 2;
            do
            {
                Thread.Sleep(attempts == 2 ? 0 : 1000);
                CachedSessionDetails = (IDictionary<string, object>)getSessionDetails_.Invoke(driver_, null);
            } while (attempts-- > 0 && !CachedSessionDetails.ContainsKey("viewportRect"));

            if (CachedSessionDetails == null)
            {
                Logger.Log(TraceLevel.Warn, Stage.Open,
                    new
                    {
                        message = "could not get viewportRect in session details from appium server! " +
                                  "Using default device size instead, this might create incorrect images.",
                        sessionDetails = getSessionDetails_.Invoke(driver_, null)
                    });
            }
        }

        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            ArgumentGuard.IsValidState(IsOpen, "Eyes not open");
            try
            {
                ICheckSettingsInternal checkSettingsInternal = checkSettings as ICheckSettingsInternal;
                Rectangle? targetRegion = GetTargetRegion_(checkSettingsInternal);
                string app;
                try { app = driver_.Capabilities.GetCapability(MobileCapabilityType.App)?.ToString() ?? driver_.Url; }
                catch { app = null; }
                CheckWindowBase(targetRegion, checkSettingsInternal, source: app);
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, ex, TestId);
                throw;
            }
        }

        private Rectangle GetTargetRegion_(ICheckSettingsInternal checkSettingsInternal)
        {
            Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();
            if (targetRegion.HasValue) return targetRegion.Value;

            IAppiumCheckTarget appiumCheckTarget = checkSettingsInternal as IAppiumCheckTarget;
            if (appiumCheckTarget != null)
            {
                By targetSelector = appiumCheckTarget.GetTargetSelector();
                IWebElement targetElement = appiumCheckTarget.GetTargetElement();
                if (targetElement == null && targetSelector != null)
                {
                    targetElement = driver_.FindElement(targetSelector);
                }

                if (targetElement != null)
                {
                    Point location = targetElement.Location;
                    Size size = targetElement.Size;
                    Region targetRect = new Rectangle(location, size);
                    double scaleRatio = GetScaleRatioForRegions();
                    Logger.Verbose("Scaling {0} by {1}", targetRect, scaleRatio);
                    targetRect = targetRect.Scale(scaleRatio);
                    Region physicalViewport = CachedViewport;
                    Region logicalViewport = physicalViewport.Scale(ScaleRatio);
                    if (!targetRect.Contains(logicalViewport))
                    {
                        targetRect = targetRect.Offset(-logicalViewport.Location.X, -logicalViewport.Location.Y);
                    }
                    targetRect.Intersect(new Region(Point.Empty, logicalViewport.Size));
                    return targetRect.Rectangle;
                }
            }
            return Rectangle.Empty;
        }

        public void Check(string name, ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            ArgumentGuard.IsValidState(IsOpen, "Eyes not open");

            Check(checkSettings.WithName(name));
        }

        public void CheckWindow(string name)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }

            Check(name, Target.Window().Fully());
        }

        /// <summary>
        /// Returns whether the device represented by <paramref name="driver"/> is in landscape mode or not.
        /// </summary>
        /// <param name="driver">The driver for which to check the orientation.</param>
        /// <returns>True if this is a mobile device and is in landscape orientation, or False otherwise.</returns>
        private static bool IsLandscapeOrientationImpl_(IWebDriver driver)
        {
            // We can only find orientation for mobile devices.
            if (driver is IContextAware contextAware && driver is IRotatable rotatable)
            {
                string originalContext = null;
                try
                {
                    // We must be in native context in order to ask for orientation, because of an Appium bug.
                    originalContext = contextAware.Context;
                    if (contextAware.Contexts.Count > 1 && !originalContext.Equals(NATIVE_APP, StringComparison.OrdinalIgnoreCase))
                    {
                        contextAware.Context = NATIVE_APP;
                    }
                    else
                    {
                        originalContext = null;
                    }
                    ScreenOrientation orientation = rotatable.Orientation;
                    return orientation == ScreenOrientation.Landscape;
                }
                catch (Exception e)
                {
                    throw new EyesDriverOperationException("Failed to get orientation!", e);
                }
                finally
                {
                    if (originalContext != null)
                    {
                        contextAware.Context = originalContext;
                    }
                }
            }
            return false;
        }

        protected override Size GetViewportSize()
        {
            RectangleSize viewportSize = Configuration.ViewportSize;
            if (viewportSize == null || viewportSize.IsEmpty())
            {
                viewportSize = GetScreenshotSize();
                Configuration.SetViewportSize(viewportSize);
            }

            return viewportSize;
        }

        private Size GetScreenshotSize()
        {
            using (Bitmap screenshotImage = imageProvider_.GetImage())
            {
                return screenshotImage.Size;
            }
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            Bitmap screenshotImage = imageProvider_.GetImage();
            DebugScreenshotProvider.Save(screenshotImage, "original");
            Region updatedRegion = CachedViewport;
            updatedRegion = updatedRegion.Scale(ScaleRatio);
            if (!(CutProvider is NullCutProvider))
            {
                updatedRegion = CutProvider.ToRectangle(screenshotImage.Size);
                Bitmap croppedScreenshotImage = CutProvider.Cut(screenshotImage);
                DebugScreenshotProvider.Save(croppedScreenshotImage, "cropped");
                screenshotImage.Dispose();
                screenshotImage = croppedScreenshotImage;
            }
            EyesAppiumScreenshot result = new EyesAppiumScreenshot(Logger, screenshotImage, updatedRegion, this);
            if (targetRegion != null && !targetRegion.Value.IsEmpty)
            {
                result = (EyesAppiumScreenshot)result.GetSubScreenshot(targetRegion.Value, false);
            }
            return result;
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            Configuration.SetViewportSize(size);
        }

        protected override string GetInferredEnvironment()
        {
            return null;
        }

        protected override object GetEnvironment_()
        {
            AppEnvironment appEnv = (AppEnvironment)base.GetEnvironment_();
            string deviceName = (string)driver_.Capabilities.GetCapability(MobileCapabilityType.DeviceName);
            appEnv.DeviceInfo = deviceName;
            appEnv.DisplaySize = viewportSize_ ?? driver_.Manage().Window.Size;
            return appEnv;
        }

        protected override string GetTitle()
        {
            return string.Empty;
        }
    }
}

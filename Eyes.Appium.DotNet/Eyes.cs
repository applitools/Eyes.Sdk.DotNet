using System;
using System.Globalization;
using Applitools.Fluent;
using Applitools.Utils;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;

namespace Applitools.Appium
{
    public sealed class Eyes : EyesBase
    {
        private readonly Selenium.Eyes seleniumEyes_;
        private RemoteWebDriver driver_;

        //public IDictionary<string, object> CachedSessionDetails { get; private set; }

        //public static Func<IWebDriver, bool> IsLandscapeOrientation = IsLandscapeOrientationImpl_;

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes(ILogHandler logHandler = null)
            : this(new ClassicRunner(), serverUri: (string)null, logHandler: logHandler)
        {
        }

        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
            : this(new ClassicRunner(), serverUrl.AbsoluteUri, logHandler)
        {
        }

        public Eyes(string serverUrl, ILogHandler logHandler = null)
            : this(new ClassicRunner(), serverUrl, logHandler)
        {
        }

        public Eyes(AppiumEyesRunner runner, Uri serverUri, ILogHandler logHandler = null)
            : this(runner, serverUri.AbsoluteUri, logHandler: logHandler)
        {
        }

        public Eyes(AppiumEyesRunner runner, string serverUri, ILogHandler logHandler = null)
            : base(runner, logHandler: logHandler)
        {
            var selectorTransformer = new AppiumSelectorTransformer();
            seleniumEyes_ = new Selenium.Eyes((AppiumEyesRunner)Runner, "URL", logHandler, selectorTransformer);
            ServerUrl = serverUri;
        }

        #endregion

        #region Configuration

        public Configuration GetConfiguration()
        {
            return Config;
        }

        #endregion

        #region Properties

        //public Rectangle CachedViewport { get; private set; }

        #endregion

        [Obsolete("Use SetNmgCapabilities")]
        public static void SetNMGCapabilities(AppiumOptions caps,
            string apiKey = null,
            string eyesServerUrl = null,
            ProxySettings proxySettings = null)
        {
            SetNmgCapabilities(caps, apiKey, eyesServerUrl, proxySettings);
        }

        public static void SetNmgCapabilities(AppiumOptions caps,
            string apiKey = null,
            string eyesServerUrl = null,
            ProxySettings proxySettings = null)
        {
            var iosCapsKey = "processArguments";
            var iosCapValue = "{\"args\": [], \"env\":"
                              // ReSharper disable once StringLiteralTypo
                              + "{\"DYLD_INSERT_LIBRARIES\":\"@executable_path/Frameworks/UFG_lib.xcframework/ios-arm64/UFG_lib.framework/UFG_lib"
                              + ":"
                              + "@executable_path/Frameworks/UFG_lib.xcframework/ios-arm64_x86_64-simulator/UFG_lib.framework/UFG_lib\",";

            var iosCapValueSuffix = "}}";

            var androidCapKey = "optionalIntentArguments";
            var androidCapValue = "--es APPLITOOLS \'{";
            var androidCapValueSuffix = "}\'";

            // Take the API key from the environment if it's not explicitly given.
            if (apiKey == null)
            {
                apiKey = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new EyesException("No API key was given, or is an empty string.");
                }
            }

            androidCapValue += "\"NML_API_KEY\":\"" + apiKey + "\",";
            iosCapValue += "\"NML_API_KEY\":\"" + apiKey + "\",";

            // Check for the server URL in the env variable. (might still be null and this is fine.
            if (eyesServerUrl == null)
            {
                eyesServerUrl = Environment.GetEnvironmentVariable("APPLITOOLS_SERVER_URL");
            }

            if (!string.IsNullOrEmpty(eyesServerUrl))
            {
                androidCapValue += "\"NML_SERVER_URL\":\"" + eyesServerUrl + "\",";
                iosCapValue += "\"NML_SERVER_URL\":\"" + eyesServerUrl + "\",";
            }

            if (proxySettings == null)
            {
                var proxyFromEnv = Environment.GetEnvironmentVariable("APPLITOOLS_HTTP_PROXY");
                if (!string.IsNullOrEmpty(proxyFromEnv))
                {
                    proxySettings = new ProxySettings(proxyFromEnv);
                }
            }

            if (proxySettings != null)
            {
                androidCapValue += "\"NML_PROXY_URL\":\"" + proxySettings + "\",";
                iosCapValue += "\"NML_PROXY_URL\":\"" + proxySettings + "\",";
            }

            androidCapValue += androidCapValueSuffix;
            iosCapValue += iosCapValueSuffix;

            caps.AddAdditionalCapability(androidCapKey, androidCapValue);
            caps.AddAdditionalCapability(iosCapsKey, iosCapValue);
        }

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

            seleniumEyes_.SetConfiguration(Config);
            seleniumEyes_.Open(driver, appName, testName);

            ApiKey = seleniumEyes_.ApiKey;
            Runner.ApiKey = seleniumEyes_.Runner.ApiKey;
            Runner.ServerUrl = seleniumEyes_.Runner.ServerUrl;
            Runner.Proxy = seleniumEyes_.Runner.Proxy;
            AppName = seleniumEyes_.AppName;
            TestName = seleniumEyes_.TestName;
            ViewportSize = seleniumEyes_.ViewportSize;
            IsOpen = true;

            return driver_;
        }

        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }
            IAppiumCheckTarget appiumCheckTarget = (IAppiumCheckTarget)checkSettings;
            var request = seleniumEyes_.CreateCheckRequest(checkSettings);
            request.Payload.Settings.Webview = (object) appiumCheckTarget.GetWebview() ??
                                              appiumCheckTarget.IsDefaultWebview();
            seleniumEyes_.SendCheckRequest(request);
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

        public override TestResults GetResults(bool throwEx = true)
        {
            return seleniumEyes_.GetResults(throwEx);
        }

        public override TestResults Abort()
        {
            IsOpen = false;
            return seleniumEyes_.Abort();
        }

        public override TestResults Close(bool throwEx = true)
        {
            return seleniumEyes_.Close(throwEx);
        }
    }
}
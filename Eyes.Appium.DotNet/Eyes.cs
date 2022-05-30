using Applitools.Utils;
using OpenQA.Selenium.Remote;
using System;
using OpenQA.Selenium.Appium;

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
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl)
            : this()
        {
            Init_();
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
            : this(new ClassicRunner()) 
        {
            Init_();
        }

        public Eyes(EyesRunner runner, ILogHandler logHandler = null)
            : base(runner, logHandler: logHandler)
        {
            var selectorTransformer = new AppiumSelectorTransformer();
            seleniumEyes_ = new Selenium.Eyes(Runner, "URL", logHandler, selectorTransformer);

            Init_();
        }

        private void Init_()
        {
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

            seleniumEyes_.Check(checkSettings);
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

using Applitools.Utils;
using OpenQA.Selenium.Remote;
using System;
using OpenQA.Selenium.Appium;
using Target = Applitools.Common.Target;

namespace Applitools.Appium
{
    public sealed class Eyes : EyesBase
    {
        private Selenium.Eyes _seleniumEyes;
        private RemoteWebDriver driver_;
        private ISelectorTransformer selectorTransformer;

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

        public Eyes(EyesRunner runner)
            : base(runner)
        {
            selectorTransformer = new AppiumSelectorTransformer();
            _seleniumEyes = new Selenium.Eyes(Runner, "URL", selectorTransformer: selectorTransformer);

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

            _seleniumEyes.Open(driver, appName, testName);
            ApiKey = _seleniumEyes.ApiKey;
            Runner.ApiKey = _seleniumEyes.Runner.ApiKey;
            AppName = _seleniumEyes.AppName;
            TestName = _seleniumEyes.TestName;
            ViewportSize = _seleniumEyes.ViewportSize;
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

            _seleniumEyes.Check(checkSettings);
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
            return _seleniumEyes.Abort();
        }
        
        public override TestResults Close(bool throwEx = true)
        {
            return _seleniumEyes.Close(throwEx);
        }
    }
}

using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using Applitools.Common;

namespace Applitools.Appium
{
    public sealed class Eyes : EyesBase
    {
        private RemoteWebDriver driver_;

        public IDictionary<string, object> CachedSessionDetails { get; private set; }
        
        //public static Func<IWebDriver, bool> IsLandscapeOrientation = IsLandscapeOrientationImpl_;

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl)
            //: base(serverUrl)
        {
            Init_();
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
        {
            Init_();
        }

        private void Init_()
        {
        }

        #endregion

        #region Configuration
        private Configuration configuration_ = new Configuration();

        protected Configuration Configuration { get => configuration_; }

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        protected void SetConfigImpl(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }
        #endregion

        #region Properties
        
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



            return driver_;
        }
        
        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }


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
            throw new EyesException();
        }
    }
}

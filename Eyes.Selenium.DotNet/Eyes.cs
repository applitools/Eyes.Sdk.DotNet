using Applitools.Selenium.VisualGrid;
using Applitools.Utils;
using Applitools.Utils.Cropping;
using Applitools.VisualGrid;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace Applitools.Selenium
{
    public class Eyes : EyesBaseConfig, IEyesBase, ISeleniumConfigurationProvider
    {
        private readonly bool isVisualGridEyes_ = false;
        internal VisualGridEyes visualGridEyes_ = null;
        internal SeleniumEyes seleniumEyes_ = null;
        internal EyesRunner runner_ = null;
        private Configuration configuration_ = new Configuration();
        internal static bool moveWindow_ = true;
        internal readonly ISeleniumEyes activeEyes_;

        #region ctors

        public Eyes(ILogHandler logHandler = null)
        {
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            runner_ = new ClassicRunner(logHandler);
            configuration_.SetForceFullPageScreenshot(false);
            seleniumEyes_ = new SeleniumEyes(this, (ClassicRunner)runner_);
            activeEyes_ = seleniumEyes_;
        }

        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
        {
            ArgumentGuard.NotNull(serverUrl, nameof(serverUrl));
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            runner_ = new ClassicRunner(logHandler);
            configuration_.SetForceFullPageScreenshot(false);
            seleniumEyes_ = new SeleniumEyes(this, serverUrl, (ClassicRunner)runner_);
            activeEyes_ = seleniumEyes_;
        }

        public Eyes(EyesRunner runner)
        {
            if (runner == null)
            {
                runner = new ClassicRunner();
            }
            runner_ = runner;
            if (runner is VisualGridRunner visualGridRunner)
            {
                visualGridEyes_ = new VisualGridEyes(this, visualGridRunner);
                isVisualGridEyes_ = true;
                activeEyes_ = visualGridEyes_;
            }
            else
            {
                seleniumEyes_ = new SeleniumEyes(this, (ClassicRunner)runner);
                activeEyes_ = seleniumEyes_;
            }
        }

        internal Eyes(IServerConnectorFactory serverConnectorFactory)
        {
            runner_ = new ClassicRunner();
            configuration_.SetForceFullPageScreenshot(false);
            seleniumEyes_ = new SeleniumEyes(this, (ClassicRunner)runner_, serverConnectorFactory);
            activeEyes_ = seleniumEyes_;
        }


        #endregion

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            ArgumentGuard.NotNull(configuration, nameof(configuration));
            configuration_ = new Configuration(configuration);

            string serverUrl = configuration_.ServerUrl;
            if (serverUrl != null)
            {
                ServerUrl = serverUrl;
            }

            string apiKey = configuration_.ApiKey;
            if (apiKey != null)
            {
                ApiKey = apiKey;
            }

            WebProxy proxy = configuration_.Proxy;
            if (proxy != null)
            {
                Proxy = proxy;
            }
        }

        protected internal override Applitools.Configuration Config { get => configuration_; }
        Configuration ISeleniumConfigurationProvider.GetConfiguration() { return configuration_; }


        #region configuration properties

        public StitchModes StitchMode
        {
            get => configuration_.StitchMode;
            set => configuration_.SetStitchMode(value);
        }

        public int WaitBeforeScreenshots
        {
            get => configuration_.WaitBeforeScreenshots;
            set => configuration_.SetWaitBeforeScreenshots(value);
        }

        public bool ForceFullPageScreenshot
        {
            get => configuration_.IsForceFullPageScreenshot ?? isVisualGridEyes_;
            set => configuration_.SetForceFullPageScreenshot(value);
        }

        public bool HideScrollbars
        {
            get => configuration_.HideScrollbars;
            set => configuration_.SetHideScrollbars(value);
        }

        public bool HideCaret
        {
            get => configuration_.HideCaret;
            set => configuration_.SetHideCaret(value);
        }

        #endregion

        public bool IsDisabled
        {
            get => activeEyes_.IsDisabled;
            set => activeEyes_.IsDisabled = value;
        }

        public bool IsOpen { get => activeEyes_.IsOpen; }

        public Logger Logger
        {
            get => activeEyes_.Logger;
        }

        public TestResults Abort()
        {
            Logger.Verbose("enter. visual grid? {0}", isVisualGridEyes_);
            return activeEyes_.Abort();
        }

        public TestResults AbortIfNotClosed()
        {
            return Abort();
        }

        public void AbortAsync()
        {
            Logger.Verbose("enter. visual grid? {0}", isVisualGridEyes_);
            activeEyes_.AbortAsync();
        }

        #region selenium specific properties
        public IDebugScreenshotProvider DebugScreenshotProvider
        {
            get => seleniumEyes_?.DebugScreenshotProvider;
            set
            {
                if (!isVisualGridEyes_)
                {
                    seleniumEyes_.DebugScreenshotProvider = value;
                }
            }
        }

        public ICutProvider CutProvider
        {
            get => seleniumEyes_.CutProvider;
            set
            {
                if (!isVisualGridEyes_)
                {
                    seleniumEyes_.CutProvider = value;
                }
            }
        }

        #endregion

        public double DevicePixelRatio
        {
            get
            {
                if (isVisualGridEyes_) return 0;
                return seleniumEyes_.DevicePixelRatio;
            }
            set
            {
                if (isVisualGridEyes_) return;
                seleniumEyes_.DevicePixelRatio = value;
            }
        }

        public double ScaleRatio
        {
            get
            {
                if (isVisualGridEyes_) return 0;
                return seleniumEyes_.ScaleRatio;
            }
            set
            {
                if (isVisualGridEyes_) return;
                seleniumEyes_.ScaleRatio = value;
            }
        }

        public void SetScrollToRegion(bool shouldScroll)
        {
            if (isVisualGridEyes_) return;
            seleniumEyes_.SetScrollToRegion(shouldScroll);
        }

        /// <summary>
        /// Sets a handler of log messages generated by this API.
        /// </summary>
        /// <param name="logHandler">Handles log messages generated by this API.</param>
        public void SetLogHandler(ILogHandler logHandler)
        {
            activeEyes_.SetLogHandler(logHandler);
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
        public IWebDriver Open(IWebDriver driver)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Open, StageType.Disabled);
                return driver;
            }
            return activeEyes_.Open(driver);
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
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, testName, Stage.Open, StageType.Disabled);
                return driver;
            }
            return activeEyes_.Open(driver, appName, testName, viewportSize);
        }

        /// <summary>
        /// Starts a new test that does not dictate the viewport size of the application under
        /// test.
        /// </summary>
        public IWebDriver Open(IWebDriver driver, string appName, string testName)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, testName, Stage.Open, StageType.Disabled);
                return driver;
            }
            return activeEyes_.Open(driver, appName, testName, Size.Empty);
        }

        /// <summary>
        /// Takes a snapshot of the application under test, where the capture area and settings
        /// are given by <paramref name="checkSettings"/>.
        /// </summary>
        /// <param name="checkSettings">A settings object defining the capture area and parameters.
        /// Created fluently using the <see cref="Target"/> static class.</param>
        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Check, StageType.Disabled);
                return;
            }
            activeEyes_.Check(checkSettings);
        }

        /// <summary>
        /// Takes a snapshot of the application under test, where the capture area and settings
        /// are given by <paramref name="checkSettings"/>.
        /// </summary>
        /// <param name="name">A tag to be associated with the match.</param>
        /// <param name="checkSettings">A settings object defining the capture area and parameters.
        /// Created fluently using the <see cref="Target"/> static class.</param>
        public void Check(string name, ICheckSettings checkSettings)
        {
            Check(checkSettings.WithName(name));
        }

        public void Check(params ICheckSettings[] checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Check, StageType.Disabled);
                return;
            }
            activeEyes_.Check(checkSettings);
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="fully">An optional bool indicating if a full page screenshot should be captured.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(string tag = null, bool? fully = null)
        {
            if (isVisualGridEyes_)
            {
                Check(tag, Target.Window().Fully(fully ?? true));
            }
            else
            {
                Check(tag, Target.Window().Fully(fully ?? false));
            }
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="fully">An optional bool indicating if a full page screenshot should be captured.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(TimeSpan matchTimeout, string tag = null, bool? fully = null)
        {
            if (isVisualGridEyes_)
            {
                Check(tag, Target.Window().Timeout(matchTimeout).Fully(fully ?? true));
            }
            else
            {
                Check(tag, Target.Window().Timeout(matchTimeout).Fully(fully ?? false));
            }
        }

        /// <summary>
        /// If <paramref name="stitchContent"/> is <code>false</code> then behaves the same as <see cref="CheckRegion(By, string, int)"/>.
        /// Otherwise, behaves the same as <see cref="CheckElement(By, string, int)"/>.
        /// </summary>
        /// <param name="selector">Selects the region to check</param>
        /// <param name="tag">A tag to be associated with the snapshot.</param>
        /// <param name="stitchContent">
        /// If <paramref name="stitchContent"/> is <code>false</code> then behaves the same as <see cref="CheckRegion(By, string, int)"/>.
        /// Otherwise, behaves the same as <see cref="CheckElement(By, string, int)"/>
        /// <param name="matchTimeout">The amount of milliseconds to retry matching</param>
        /// </param>
        public void CheckRegion(By selector, string tag, bool stitchContent, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Fully(stitchContent).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="selector">Selects the region to check.</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of milliseconds to retry matching</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(By selector, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="selector">Selects the region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(By selector, TimeSpan matchTimeout, string tag = null)
        {
            ArgumentGuard.NotNull(selector, nameof(selector));
            Check(tag, Target.Region(selector).Timeout(matchTimeout));
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(region).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Region(region).Timeout(matchTimeout));
        }

        public void CheckRegionInFrame(string frameNameOrId, By by, string tag, bool stitchContent, int matchTimeout = -1)
        {
            Check(tag, Target.Frame(frameNameOrId).Region(by).Fully(stitchContent).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches a specific element with the expected region output.
        /// </summary>
        /// <param name="selector">The selector by which to get the element.</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        /// <param name="matchTimeout">The amount of time to retry matching in milliseconds.</param>
        public void CheckElement(By selector, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        /// <summary>
        /// Matches the frame given as parameter, by switching into the frame and using stitching to get an image of the frame.
        /// </summary>
        /// <param name="frameNameOrId">The name or id of the frame to check. (The same name/id as would be used in a call to driver.SwitchTo().Frame())</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        public void CheckFrame(string frameNameOrId, string tag = null)
        {
            Check(tag, Target.Frame(frameNameOrId).Fully());
        }

        /// <summary>
        /// Matches the frame given as parameter, by switching into the frame and using stitching to get an image of the frame.
        /// </summary>
        /// <param name="frameNameOrId">The name or id of the frame to check. (The same name/id as would be used in a call to driver.SwitchTo().Frame())</param>
        /// <param name="matchTimeout">The amount of time to retry matching.</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        public void CheckFrame(string frameNameOrId, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameNameOrId).Timeout(matchTimeout).Fully());
        }

        /// <summary>
        /// Matches the frame given as parameter, by switching into the frame and using stitching to get an image of the frame.
        /// </summary>
        /// <param name="frameIndex">The index of the frame to check. (The same index as would be used in a call to driver.SwitchTo().Frame())</param>
        /// <param name="matchTimeout">The amount of time to retry matching.</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        public void CheckFrame(int frameIndex, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameIndex).Timeout(matchTimeout).Fully());
        }

        /// <summary>
        /// Matches the frame given as parameter, by switching into the frame and using stitching to get an image of the frame.
        /// </summary>
        /// <param name="frameReference">The element which is the frame to check. (The same element as would be used in a call to driver.SwitchTo().Frame())</param>
        /// <param name="matchTimeout">The amount of time to retry matching.</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        public void CheckFrame(IWebElement frameReference, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameReference).Timeout(matchTimeout).Fully());
        }

        /// <summary>
        /// Matches the frame given as parameter, by switching into the frame and using stitching to get an image of the frame.
        /// </summary>
        /// <param name="framePath">The path to the frame to check. This is a list of frame names/IDs (where each frame is nested in the previous frame)</param>
        /// <param name="matchTimeout">The amount of time to retry matching.</param>
        /// <param name="tag">Optional. A tag to be associated with the match.</param>
        public void CheckFrame(string[] framePath, TimeSpan matchTimeout, string tag = null)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, tag, Stage.Check, StageType.Disabled);
                return;
            }
            ArgumentGuard.NotNull(framePath, nameof(framePath));
            ArgumentGuard.GreaterThan(framePath.Length, 0, nameof(framePath.Length));

            Fluent.SeleniumCheckSettings settings = Target.Frame(framePath[0]);
            for (int i = 1; i < framePath.Length; i++)
            {
                settings.Frame(framePath[i]);
            }

            Check(tag, settings.Timeout(matchTimeout).Fully());
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegionBase InRegion(Rectangle region)
        {
            return seleniumEyes_?.InRegion(region);
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegion InRegion(By selector)
        {
            return seleniumEyes_?.InRegion(selector);
        }

        public EyesWebDriver GetDriver()
        {
            return activeEyes_.GetDriver();
        }

        /// <summary>
        /// Ends the test and returns its results.
        /// </summary>
        public virtual TestResults Close(bool throwEx = true)
        {
            return activeEyes_.Close(throwEx);
        }

        /// <summary>
        /// Ends the test asynchronously.
        /// </summary>
        public virtual void CloseAsync()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return;
            }
            Logger.Verbose("enter. visual grid? {0}", isVisualGridEyes_);
            activeEyes_.CloseAsync();
        }

        public void Log(string msg, params object[] args)
        {
            activeEyes_.Logger.Log(TraceLevel.Notice, TestName, Stage.General, new { message = string.Format(msg, args) });
        }

        public void AddProperty(string name, string value)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }
            activeEyes_.AddProperty(name, value);
        }

        public void ClearProperties()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }
            activeEyes_.ClearProperties();
        }

        IDictionary<string, IRunningTest> IEyesBase.GetAllTests()
        {
            return activeEyes_.GetAllTests();
        }
    }
}

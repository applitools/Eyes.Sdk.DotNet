using Applitools.Selenium.VisualGrid;
using Applitools.Utils;
using Applitools.Utils.Cropping;
using Applitools.VisualGrid;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Eyes.Sdk.Core.DotNet.Utils;
using Applitools.Selenium.Commands;
using Applitools.Selenium.Commands.Requests;
using Applitools.Selenium.Commands.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium.Remote;

namespace Applitools.Selenium
{
    public class Eyes : EyesBaseConfig, IEyesBase, ISeleniumConfigurationProvider
    {
        private Configuration configuration_ = new Configuration();
        private string apiKey;
        internal static bool moveWindow_ = true;
        private string _apiKey;
        private string _serverUrl;
        private ProxySettings _proxy;
        private bool _isOpen;
        private Logger _logger;


        private WebSocketClient socketClient_;
        private CoreMakeManagerResponse coreMakeManagerResponse_;
        private string managerApplitoolsRefId_;
        private EyesRef eyes_;
        private readonly string serverUrl_;
        private IWebDriver driver_;
        private UniversalDriverConfig driverRef_;
        private EyesConfig eysConfig_;
        private IDebugScreenshotProvider _debugScreenshotProvider;

        // TODO: remove
        internal EyesRunner runner_ = null;
        internal readonly ISeleniumEyes activeEyes_;
        internal SeleniumEyes seleniumEyes_ = null;
        internal VisualGridEyes visualGridEyes_ = null;
        protected readonly string API_KEY = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");

        #region ctors

        public Eyes(ILogHandler logHandler = null)
        {
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            _logger = new Logger();
            configuration_.SetForceFullPageScreenshot(false);

            serverUrl_ = "https://eyesapi.applitools.com/";
            socketClient_ = new WebSocketClient();
        }

        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
        : this(logHandler)
        {
            ArgumentGuard.NotNull(serverUrl, nameof(serverUrl));
        }

        public Eyes(EyesRunner runner)
            : this(NullLogHandler.Instance)
        {
        }

        internal Eyes(IServerConnectorFactory serverConnectorFactory)
            : this(NullLogHandler.Instance)
        {
        }

        internal virtual SeleniumEyes CreateSeleniumEyes_(EyesRunner runner)
        {
            throw new NotImplementedException();
        }

        internal virtual VisualGridEyes CreateVisualGridEyes_(VisualGridRunner visualGridRunner)
        {
            throw new NotImplementedException();
        }


        #endregion

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        protected override void SetConfigImpl(Applitools.IConfiguration configuration)
        {
            configuration_ = new Configuration((IConfiguration)configuration);
        }

        protected internal override Applitools.Configuration Config => configuration_;
        Configuration ISeleniumConfigurationProvider.GetConfiguration() { return configuration_; }

        //public override string FullAgentId => {}

        public override string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }

        public override string ServerUrl { get => _serverUrl; set { _serverUrl = value; configuration_.ServerUrl = value; } }

        public override ProxySettings Proxy { get => _proxy; set { _proxy = value; configuration_.Proxy = value; } }

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
            get => configuration_.IsForceFullPageScreenshot ?? false;
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

        public bool IsDisabled { get; set; }

        public bool IsOpen { get; private set; }

        public Logger Logger => _logger;

        public TestResults Abort()
        {
            Logger.Verbose("enter. Abort");

            var request = new EyesAbortRequest
            {
                Payload = new EyesAbortRequestPayload
                {
                    Eyes = eyes_
                }
            };

            var testResults = SendRequest<EyesAbortRequest, TestResults>(request).Result;

            //socketClient_.Close();
            IsOpen = false;

            return testResults;
        }

        public TestResults AbortIfNotClosed()
        {
            return Abort();
        }

        public void AbortAsync()
        {
            Logger.Verbose("enter. AbortAsync");
            Task.Factory.StartNew(Abort);
        }

        #region selenium specific properties

        public IDebugScreenshotProvider DebugScreenshotProvider { get; set; }

        public ICutProvider CutProvider { get; set; }

        #endregion

        public double DevicePixelRatio { get; set; }

        public double ScaleRatio { get; set; }

        public void SetScrollToRegion(bool shouldScroll)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a handler of log messages generated by this API.
        /// </summary>
        /// <param name="logHandler">Handles log messages generated by this API.</param>
        public void SetLogHandler(ILogHandler logHandler)
        {
            throw new NotImplementedException();
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
            return OpenEyes(driver);
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
            return OpenEyes(driver, appName, testName, viewportSize);
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
            return OpenEyes(driver, appName, testName, Size.Empty);
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

        internal virtual void CheckImpl_(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Check, StageType.Disabled);
                return;
            }

            var seleniumCheckSettings = (SeleniumCheckSettings) checkSettings;
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesCheckRequestPayload
                {
                    Eyes = eyes_,
                    Config = new EyesConfig
                    {
                        DisplayName = TestName,
                        TestName = TestName,
                        AppName = AppName,
                        ServerUrl = ServerUrl
                    },
                    Settings = CreateUniversalCheckSettings(seleniumCheckSettings)
                }
            };

            var result = SendRequest<EyesCheckRequest, EyesCheckResponse>(request).Result;
            if (result?.Payload?.Result?.AsExpected == false)
            {
                throw new TestFailedException();
            }
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
            Check(checkSettings);
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
            Check(tag, Target.Window().Fully(fully ?? configuration_.IsForceFullPageScreenshot ?? false));
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
            Check(tag, Target.Window().Timeout(matchTimeout).Fully(fully ?? false));
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegion InRegion(By selector)
        {
            throw new NotImplementedException();
        }

        public IWebDriver GetDriver()
        {
            return driver_;
        }

        /// <summary>
        /// Ends the test and returns its results.
        /// </summary>
        public virtual TestResults Close(bool throwEx = true)
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Payload = new EyesCloseRequestPayload
                {
                    Eyes = eyes_
                }
            };

            var testResults = SendRequest<EyesCloseRequest, TestResults>(eyesCloseRequest).Result;

            //socketClient_.Close();
            IsOpen = false;

            return testResults;
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
            Logger.Verbose("enter. CloseAsync");

            Task.Factory.StartNew(() => Close());
        }

        public void Log(string msg, params object[] args)
        {
            _logger.Log(TraceLevel.Notice, TestName, Stage.General, new { message = string.Format(msg, args) });
        }

        public void AddProperty(string name, string value)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }

            throw new NotImplementedException();
        }

        public void ClearProperties()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }
            throw new NotImplementedException();
        }

        IDictionary<string, IRunningTest> IEyesBase.GetAllTests()
        {
            throw new NotImplementedException();
        }


        private IWebDriver OpenEyes(IWebDriver driver, string appName = null, string testName = null, Size? viewportSize = null)
        {
            TestName = testName;
            AppName = appName;
            driver_ = driver;
            var url = GetSocketUrl(21077);
            socketClient_.ConnectAsync(url).GetAwaiter().GetResult();

            var makeSdk = CreateCoreMakeSdk();
            var makeSdkJson = ToJson(makeSdk);

            socketClient_.SendData(makeSdkJson);

            var coreMakeManager = CreateCoreMakeManager();

            coreMakeManagerResponse_ = SendRequest(coreMakeManager).Result;
            managerApplitoolsRefId_ = coreMakeManagerResponse_.Payload.Result.ApplitoolsRefId;

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(managerApplitoolsRefId_, driver, appName, testName, viewportSize);
            driverRef_ = eyesManagerOpenEyes.Payload.Driver;
            eysConfig_ = eyesManagerOpenEyes.Payload.Config;

            var openEyesResponsePayload = SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Result.Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException($"{error.Message}{Environment.NewLine}{error.Stack}");
            }

            IsOpen = true;
            return driver;
        }

        private UniversalCheckSettings CreateUniversalCheckSettings(SeleniumCheckSettings checkSettings)
        {
            var checkSettingsInternal = (ICheckSettingsInternal) checkSettings;
            var settings = new UniversalCheckSettings
            {
                Region = GetUniversalRegion(checkSettings),
                Fully = checkSettingsInternal.GetStitchContent(),
                Frames = GetFrames(checkSettings),
                ScrollRootElement = GetScrollRootElement(checkSettings),
                IgnoreRegions = checkSettingsInternal.GetIgnoreRegions().Select(i => i.ToRegion()).ToArray(),
                StrictRegions = checkSettingsInternal.GetStrictRegions().Select(i => i.ToRegion()).ToArray(),
                LayoutRegions = checkSettingsInternal.GetLayoutRegions().Select(i => i.ToRegion()).ToArray(),
                ContentRegions = checkSettingsInternal.GetContentRegions().Select(i => i.ToRegion()).ToArray(),
                FloatingRegions = checkSettingsInternal.GetFloatingRegions().Select(i => i.ToRegion()).ToArray(),
                AccessibilityRegions = checkSettingsInternal.GetAccessibilityRegions().Select(i => i.ToRegion()).ToArray()
            };

            return settings;
        }

        private SelectorRegion GetScrollRootElement(SeleniumCheckSettings checkSettings)
        {
            var root = ((IScrollRootElementContainer) checkSettings).GetScrollRootSelector();

            if (root != null)
            {
                return new SelectorRegion
                {
                    Type = root.Criteria,
                    Selector = root.Mechanism
                };
            }

            return null;
        }

        private IList<TRegion> GetIgnoreRegions(ICheckSettingsInternal checkSettings)
        {
            return checkSettings.GetIgnoreRegions().Select(i => i.ToRegion()).ToList();
        }

        private IList<FrameReference> GetFrames(SeleniumCheckSettings checkSettings)
        {
            var frameChain = ((ISeleniumCheckTarget) checkSettings).GetFrameChain();
            return frameChain.Select(i => new FrameReference
            {
                Frame = i.GetFrameNameOrId()
            }).ToList();
        }

        private TRegion GetUniversalRegion(ICheckSettings checkSettings)
        {
            TRegion region = null;
            if (checkSettings is ICheckSettingsInternal checkSettingsInternal)
            {
                region = CreateCheckSettingsTargetRegion(checkSettingsInternal);
            }

            if (region == null)
            {
                if (checkSettings is ISeleniumCheckTarget seleniumCheckTarget)
                {
                    region = CreateCheckSettingsRegionSelector(seleniumCheckTarget);
                    if (region == null)
                    {
                        var element = GetTargetElement(seleniumCheckTarget);
                        if (element != null)
                        {
                            region = new ElementRegion
                            {
                                ElementId = element.GetPrivateFieldValue<string>("elementId")
                            };
                        }
                    }
                }
            }

            return region;
        }

        private IWebElement GetTargetElement(ISeleniumCheckTarget checkSettings)
        {
            return checkSettings.GetTargetElement();
        }

        private SelectorRegion CreateCheckSettingsRegionSelector(ISeleniumCheckTarget settings)
        {
            SelectorRegion selector = null;

            var targetSelector = settings.GetTargetSelector();
            if (targetSelector != null)
            {
                selector = new SelectorRegion
                {
                    Selector = targetSelector.Criteria,
                    Type = targetSelector.Mechanism
                };
            }

            return selector;
        }

        private TRegion CreateCheckSettingsTargetRegion(ICheckSettingsInternal settings)
        {
            TRegion region = null;

            var rectangle = settings.GetTargetRegion();
            if (rectangle.HasValue)
            {
                region = new RectangleRegion
                {
                    X = rectangle.Value.X,
                    Y = rectangle.Value.Y,
                    Width = rectangle.Value.Width,
                    Height = rectangle.Value.Height
                };
            }

            return region;
        }

        private string ToJson<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }

        private EyesManageCloseAllEyes CreateEyesManageCloseAllEyes()
        {
            var closeAllEyes = new EyesManageCloseAllEyes
            {
                Payload = new EyesManageCloseAllEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = managerApplitoolsRefId_
                    }
                }
            };

            return closeAllEyes;
        }

        private CoreMakeSdk CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdk();
            result.AddOrUpdatePayload("name", "netSDK");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }

        private CoreMakeManager CreateCoreMakeManager()
        {
            var result = new CoreMakeManager
            {
                Key = Guid.NewGuid().ToString()
            };
            result.AddOrUpdatePayload("type", "classic");

            return result;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(
            string applitoolsRefId,
            IWebDriver webDriver,
            string appName,
            string testName,
            Size? viewportSize)
        {
            var remoteWebDriver = (WebDriver)webDriver;
            var serverUrl = GetServerUrl(remoteWebDriver.CommandExecutor);
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesManagerOpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Driver = new UniversalDriverConfig
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString()
                    },
                    Config = new EyesConfig
                    {
                        AppName = appName,
                        TestName = testName,
                        ServerUrl = serverUrl_,
                        ViewportSize = viewportSize, 
                        ApiKey = API_KEY
                    }
                }
            };

            return result;
        }

        private async Task<CoreMakeManagerResponse> SendRequest(CoreMakeManager coreMakeManager)
        {
            var coreMakeManagerJson = ToJson(coreMakeManager);

            await socketClient_.SendData(coreMakeManagerJson);

            CoreMakeManagerResponse coreMakeManagerResponse;

            do
            {
                var response = await socketClient_.ReceiveMessage();

                coreMakeManagerResponse = JsonConvert.DeserializeObject<CoreMakeManagerResponse>(response);
            } while (coreMakeManagerResponse?.Key != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

        private async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest data)
        {
            var json = ToJson(data);

            await socketClient_.SendData(json);

            TResponse result;

            do
            {
                var response = await socketClient_.ReceiveMessage();
                result = JsonConvert.DeserializeObject<TResponse>(response);
            } while (result == null);

            return result;
        }

        private async Task SendRequest<TRequest>(TRequest data)
        {
            var json = ToJson(data);

            await socketClient_.SendData(json);
        }

        private string GetServerUrl(ICommandExecutor commandExecutor)
        {
            var driverServiceCommandExecutor = commandExecutor as DriverServiceCommandExecutor;
            var instance = driverServiceCommandExecutor != null ? driverServiceCommandExecutor.HttpExecutor : commandExecutor;

            return instance.GetPrivateFieldValue<Uri>("remoteServerUri").AbsoluteUri;
        }

        private IDictionary<string, object> GetCapabilities(WebDriver webDriver)
        {
            return webDriver.Capabilities.GetPrivateFieldValue<IDictionary<string, object>>("capabilities");
        }
    }
}

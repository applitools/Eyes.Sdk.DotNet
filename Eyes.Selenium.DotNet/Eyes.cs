using Applitools.Utils;
using Applitools.Utils.Cropping;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using OpenQA.Selenium.Remote;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium
{
    public class Eyes : EyesBase
    {
        private IWebDriver driver_;
        
        private readonly ISelectorTransformer selectorTransformer_;
        private readonly IOcrRegionTransformer ocrRegionTransformer_;

        /// <summary>
        /// Specifies how detected mismatches are reported.
        /// </summary>
        [Obsolete]
        public FailureReports FailureReports { get; set; } = FailureReports.OnClose;

        #region ctors

        public Eyes()
            : this(new ClassicRunner(), logHandler: null)
        {
        }

        public Eyes(ILogHandler logHandler)
            : this(new ClassicRunner(), logHandler: logHandler)
        {
        }

        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
        : this(new ClassicRunner(), serverUrl?.AbsolutePath, logHandler)
        {
        }

        public Eyes(
            SeleniumEyesRunner runner, 
            string serverUrlFieldName = "remoteServerUri", 
            ILogHandler logHandler = null, 
            ISelectorTransformer selectorTransformer = null) 
            : base(runner, serverUrlFieldName: serverUrlFieldName, logHandler: logHandler)
        {
            selectorTransformer_ = selectorTransformer ?? new SeleniumSelectorTransformer();
            ocrRegionTransformer_ = new OcrRegionTransformer(selectorTransformer_);
        }

        #endregion

        public Configuration GetConfiguration()
        {
            return Config;
        }

        public void SetConfiguration(Configuration config)
        {
            Config = config;
        }

        [Obsolete("Use GetExecutionCloudUrl")]
        public static string GetExecutionCloudURL(string apiKey = null, Uri serverUrl = null,
            ProxySettings proxySettings = null)
        {
            return GetExecutionCloudUrl(apiKey, serverUrl, proxySettings);
        }
        
        public static string GetExecutionCloudUrl(string apiKey = null, Uri serverUrl = null, ProxySettings proxySettings = null)
        {
            var runner = new ClassicRunner();
            var request = new CoreMakeECClientRequest
            {
                Payload = new CoreMakeECClientRequestPayload
                {
                    Settings = new EGClientSettings
                    {
                        Capabilities = new ECClientCapabilities
                        {
                            ApiKey = apiKey,
                            ServerUrl = serverUrl
                        },
                        Proxy = proxySettings
                    }
                }
            };

            var response = runner.SendRequest<CoreMakeECClientRequest, CoreMakeECClientResponse>(request);
            var error = response.Payload.Error;
            if (error != null)
            {
                throw new TestFailedException(error.ToString());
            }

            return response.Payload.Result.Url;
        }

        #region selenium specific properties

        public ICutProvider CutProvider { get; set; }

        public double DevicePixelRatio { get; set; }

        public double ScaleRatio { get; set; }

        #endregion

        public void SetScrollToRegion(bool shouldScroll)
        {
        }


        /// <summary>
        /// Starts a new test.
        /// </summary>
        /// <param name="driver">The web driver that controls the browser 
        /// hosting the application under test.</param>
        /// <returns>A wrapper around webdriver</returns>
        public IWebDriver Open(IWebDriver driver)
        {
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
            return OpenEyes(driver, appName, testName, viewportSize);
        }

        /// <summary>
        /// Starts a new test that does not dictate the viewport size of the application under
        /// test.
        /// </summary>
        public IWebDriver Open(IWebDriver driver, string appName, string testName)
        {
            return OpenEyes(driver, appName, testName);
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

            foreach (var checkSetting in checkSettings)
            {
                Check(checkSetting);
            }
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
            var settings = Target.Window();
            if (fully.HasValue)
            {
                settings = settings.Fully(fully.Value);
            }
            Check(tag, settings);
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
            var settings = Target.Window().Timeout(matchTimeout);
            if (fully.HasValue)
            {
                settings = settings.Fully(fully.Value);
            }
            Check(tag, settings);
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
      
        public override ICollection<string> ExtractText(ICollection<OcrRegionBase> regions)
        {
            Logger.Log("ExtractText");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new List<string>();
            }

            var eyesConfig = ToConfig(Config);
            var remoteWebDriver = (RemoteWebDriver) driver_;
            var serverUrl = GetServerUrl(remoteWebDriver);
            var extractTextRequest = new EyesExtractTextRequest
            {
                Payload = new ExtractTextRequestPayload
                {
                    Config = ToConfig(Config),
                    Target = new DriverTarget
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString(),
                        Proxy = Config.Proxy
                    },
                    Settings = ToExtractTextSettings(regions)
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRequest, EyesExtractTextResponse>(extractTextRequest);

            return response.Payload.Result;
        }

        public override IDictionary<string, IList<TextRegion>> ExtractTextRegions(TextRegionSettings textRegionSettings)
        {
            Logger.Log(nameof(ExtractTextRegions));

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, IList<TextRegion>>();
            }

            var eyesConfig = ToConfig(Config);
            var remoteWebDriver = (RemoteWebDriver) driver_;
            var serverUrl = GetServerUrl(remoteWebDriver);
            var extractTextRegionsRequest = new EyesExtractTextRegionsRequest
            {
                Payload = new ExtractTextRegionsRequestPayload
                {
                    Config = eyesConfig,
                    Target = new DriverTarget
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString(),
                        Proxy = Config.Proxy
                    },
                    Settings = textRegionSettings
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRegionsRequest, EyesExtractTextRegionsResponse>(extractTextRegionsRequest);

            return response.Payload.Result;
        }

        public IDictionary<string, IList<Region>> Locate(params string[] locatorNames)
        {
            return Locate(locatorNames.ToList());
        }

        public override IDictionary<string, IList<Region>> Locate(VisualLocatorSettings settings)
        {
            var remoteWebDriver = (RemoteWebDriver)driver_;
            var serverUrl = GetServerUrl(remoteWebDriver);
            return LocateImpl(new DriverTarget
            {
                ServerUrl = serverUrl,
                Capabilities = GetCapabilities(remoteWebDriver),
                SessionId = remoteWebDriver.SessionId.ToString(),
                Proxy = Config.Proxy
            }, settings);
        }
        
        public IDictionary<string, IList<Region>> Locate(ICollection<string> locatorNames)
        {
            Logger.Log("Locate");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, IList<Region>>();
            }

            var eyesConfig = ToConfig(Config);
            var remoteWebDriver = (RemoteWebDriver)driver_;
            var serverUrl = GetServerUrl(remoteWebDriver);
            var locateRequest = new EyesLocateRequest
            {
                Payload = new EyesLocateRequestPayload
                {
                    Config = ToConfig(Config),
                    Target = new DriverTarget
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString(),
                        Proxy = Config.Proxy
                    },
                    Settings = ToLocateSettings(Config, locatorNames)
                }
            };

            var response = Runner.SendRequest<EyesLocateRequest, EyesLocateResponse>(locateRequest);

            return response.Payload.Result;
        }

        /// <summary>
        /// Sets the OS (e.g., Windows) and application (e.g., Chrome) 
        /// that host the application under test.
        /// </summary>
        /// <param name="hostOS">The name of the OS hosting the application under test or 
        /// <c>null</c> to auto-detect.</param>
        /// <param name="hostApp">The name of the application hosting the application under
        /// test or <c>null</c> to auto-detect.</param>
        public void SetAppEnvironment(string hostOS, string hostApp)
        {
            if (IsDisabled)
            {
                Logger.Verbose("Ignored");
                return;
            }

            Logger.Verbose("SetAppEnvironment({0}, {1})", hostOS, hostApp);

            if (string.IsNullOrWhiteSpace(hostOS))
            {
                hostOS = null;
            }
            else
            {
                hostOS = hostOS.Trim();
            }

            if (string.IsNullOrWhiteSpace(hostApp))
            {
                hostApp = null;
            }
            else
            {
                hostApp = hostApp.Trim();
            }

            Config.SetHostOS(hostOS);
            Config.SetHostApp(hostApp);
        }
        
        internal virtual void CheckImpl_(ICheckSettings checkSettings)
        {
            Logger.Log("CheckImpl");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Check, StageType.Disabled);
                return;
            }

            var request = CreateCheckRequest(checkSettings);

            SendCheckRequest(request);
        }

        internal EyesCheckRequest CreateCheckRequest(ICheckSettings checkSettings)
        {
            var seleniumCheckSettings = (ICheckSettingsInternal)checkSettings;
            var eyesConfig = ToConfig(Config);
            var remoteWebDriver = (RemoteWebDriver)driver_;
            var serverUrl = GetServerUrl(remoteWebDriver);
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new CheckRequestPayload
                {
                    Eyes = eyes_,
                    Config = eyesConfig,
                    Target = new DriverTarget
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString(),
                        Proxy = Config.Proxy
                    },
                    Settings = CreateUniversalCheckSettings(seleniumCheckSettings)
                }
            };
            return request;
        }

        internal void SendCheckRequest(EyesCheckRequest request)
        {
            var result = Runner.SendRequest<EyesCheckRequest, EyesCheckResponse>(request);
            var error = result?.Payload?.Error;
            if (error != null)
            {
                throw new EyesException(error.ToString());
            }

            if (result?.Payload?.Result == null)
            {
                return;
            }

            foreach (var matchResult in result.Payload.Result)
            {
                if (matchResult.AsExpected)
                {
                    continue;
                }

#pragma warning disable CS0612
                if (FailureReports == FailureReports.Immediate)
                {
                    throw new EyesException($"Unexpected match result for {matchResult}");
                }
#pragma warning restore CS0612
            }
        }

        private IWebDriver OpenEyes(IWebDriver driver, string appName = null, string testName = null, Size? viewportSize = null)
        {
            Logger.Log("OpenEyes");

            if (string.IsNullOrEmpty(ApiKey))
            {
                ApiKey = API_KEY;
            }
            if (string.IsNullOrEmpty(appName) == false)
            {
                AppName = appName;
            }
            if (string.IsNullOrEmpty(testName) == false)
            {
                TestName = testName;
            }
            if (viewportSize != null)
            {
                ViewportSize = viewportSize;
            }
            driver_ = driver;
            Runner.ApiKey = ApiKey;
            Runner.ServerUrl = ServerUrl;
            Runner.Proxy = Proxy;

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, testName, Stage.Open, StageType.Disabled);
                return driver;
            }
            
            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(Runner.ManagerApplitoolsRefId, driver);

            var openEyesResponsePayload = Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException($"{error.Message}{Environment.NewLine}{error.Stack}");
            }

            IsOpen = true;
            return driver;
        }

        private EyesGetResultsRequest CreateEyesGetResultsRequest(bool throwEx = false, bool? removeDuplicateTests = null)
        {
            var request = new EyesGetResultsRequest
            {
                Payload = new EyesGetResultsRequestPayload
                {
                    Eyes = eyes_,
                    Settings = ToResultsSettings(Config, throwEx, removeDuplicateTests)
                }
            };
            return request;
        }

        private EyesCloseRequest CreateEyesCloseRequest(bool throwEx = false)
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Payload = new CloseResponsePayload
                {
                    Eyes = eyes_,
                    Settings = ToCloseSettings(Config, throwEx, Runner.RemoveDuplicateTests)
                }
            };
            return eyesCloseRequest;
        }

        private EyesAbortRequest CreateEyesAbortRequest()
        {
            var request = new EyesAbortRequest
            {
                Payload = new AbortPayload
                {
                    Eyes = eyes_
                }
            };
            return request;
        }
        
        protected override UniversalCheckSettings CreateUniversalCheckSettings(ICheckSettingsInternal checkSettingsInternal)
        {
            var settings = base.CreateUniversalCheckSettings(checkSettingsInternal);
            settings.Frames = GetFrames(checkSettingsInternal);
            settings.ScrollRootElement = GetScrollRootElement(checkSettingsInternal);
            return settings;
        }

        private TRegion GetScrollRootElement(ICheckSettingsInternal checkSettings)
        {
            if (checkSettings is IScrollRootElementContainer container)
            {
                var selector = container.GetScrollRootSelector();
                if (selector != null)
                {
                    return selectorTransformer_.GetRegionSelector(selector);
                }

                var element = container.GetScrollRootElement();
                if (element != null)
                {
                    return new RegionElement
                    {
                        ElementId = element.GetElementId()
                    };
                }
            }

            return null;
        }

        private IList<IFrame> GetFrames(ICheckSettingsInternal checkSettings)
        {
            var frames = new List<IFrame>();
            if (checkSettings is ISeleniumCheckTarget seleniumCheckTarget)
            {
                foreach (var item in seleniumCheckTarget.GetFrameChain())
                {
                    if (string.IsNullOrEmpty(item.FrameNameOrId) == false)
                    {
                        frames.Add(new FrameNameOrId
                        {
                            Frame = item.FrameNameOrId
                        });
                    }

                    if (item.FrameSelector != null)
                    {
                        frames.Add(new FrameSelector
                        {
                            Frame = selectorTransformer_.GetRegionSelector(item.FrameSelector)
                        });
                    }

                    if (item.FrameReference != null)
                    {
                        frames.Add(new FrameElement
                        {
                            ElementId = item.FrameReference.GetElementId()
                        });
                    }
                }
            }

            return frames;
        }

        protected override TRegion GetUniversalRegion(ICheckSettingsInternal checkSettings)
        {
            TRegion region = null;
            if (checkSettings is ICheckSettingsInternal checkSettingsInternal)
            {
                region = CreateCheckSettingsTargetRegion(checkSettingsInternal);
            }

            if (region == null)
            {
                if (checkSettings is ITargetContainer seleniumCheckTarget)
                {
                    region = CreateCheckSettingsRegionSelector(seleniumCheckTarget);
                    if (region == null)
                    {
                        var element = GetTargetElement(seleniumCheckTarget);
                        if (element != null)
                        {
                            region = new RegionElement
                            {
                                ElementId = element.GetElementId()
                            };
                        }
                    }
                }
            }

            return region;
        }

        private IWebElement GetTargetElement(ITargetContainer checkSettings)
        {
            return checkSettings.GetTargetElement();
        }
        
        private RegionSelector CreateCheckSettingsRegionSelector(ITargetContainer settings)
        {
            RegionSelector regionSelector = null;
            
            var targetSelector = settings.GetTargetSelector();
            if (targetSelector != null)
            {
                regionSelector = selectorTransformer_.GetRegionSelector(targetSelector);
            }

            return regionSelector;
        }
        
        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(
            string applitoolsRefId,
            IWebDriver webDriver)
        {
            var remoteWebDriver = (RemoteWebDriver)webDriver;
            var serverUrl = GetServerUrl(remoteWebDriver);
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new OpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Target = new DriverTarget
                    {
                        ServerUrl = serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString(),
                        Proxy = Config.Proxy
                    },
                    Config = ToConfig(Config),
                    Settings = ToOpenSettings(Config)
                }
            };
            return result;
        }
        
        private string GetServerUrl(RemoteWebDriver webDriver)
        {
            var commandExecutor = webDriver.GetBaseTypePrivateFieldValue<RemoteWebDriver, ICommandExecutor>("executor");
            return GetServerUrl(commandExecutor);
        }

        private string GetServerUrl(ICommandExecutor commandExecutor)
        {
            var instance = commandExecutor is DriverServiceCommandExecutor driverServiceCommandExecutor 
                ? driverServiceCommandExecutor.HttpExecutor 
                : commandExecutor;

            return instance.GetPrivateFieldValue<Uri>(serverUrlFieldName_).AbsoluteUri;
        }

        private IDictionary<string, object> GetCapabilities(RemoteWebDriver webDriver)
        {
            return webDriver.Capabilities.GetPrivateFieldValue<IDictionary<string, object>>("capabilities");
        }

        protected override ICollection<ExtractTextSettings> ToExtractTextSettings(ICollection<OcrRegionBase> regions)
        {
	        var extractTextSettings = new List<ExtractTextSettings>();
	        foreach (var region in regions)
	        {
		        var ocrRegion = ocrRegionTransformer_.GetRegion((OcrRegion)region);
		        var extractTextSetting = new ExtractTextSettings
		        {
			        Region = ocrRegion,
			        Hint = region.GetHint(),
			        Language = region.GetLanguage(),
			        MinMatch = region.GetMinMatch()
		        };
		        extractTextSettings.Add(extractTextSetting);
	        }

	        return extractTextSettings;
        }
    }
}

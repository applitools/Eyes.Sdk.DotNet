using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Applitools.Fluent;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Utils;

namespace Applitools.Images
{
    /// <summary>
    /// Applitools Eyes Image validation API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "Disposed on Close or AbortIfNotClosed")]
    [ComVisible(true)]
    public sealed class Eyes : EyesBase
    {
        #region Fields

        private readonly string API_KEY = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");

        private string title_;
        private string screenshotUrl_;
        private readonly Uri serverUrl_;
        private string managerApplitoolsRefId_;
        private EyesRef eyes_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
            : this(new ClassicRunner(), serverUrl, logHandler)
        {
        }

        ///// <summary>
        ///// Creates a new Eyes instance that interacts with the Eyes cloud service.
        ///// </summary>
        ///// <param name="logger">The logger to use.</param>
        //public Eyes(Logger logger) : this(new ClassicRunner(), null, new )
        //{
        //    // No need to retry since we are providing the image to match.
        //    MatchTimeout = TimeSpan.Zero;
        //}

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
        {
            // No need to retry since we are providing the image to match.
            MatchTimeout = TimeSpan.Zero;
        }

        public Eyes(EyesRunner runner, Uri serverUrl = null, ILogHandler logHandler = null)
            : base(runner, logHandler: logHandler)
        {
            serverUrl_ = serverUrl;
        }

        #endregion

        /// <summary>
        /// Specifies how detected mismatches are reported.
        /// </summary>
        [Obsolete]
        public FailureReports FailureReports { get; set; } = FailureReports.OnClose;

        #region Methods

        #region Public

        public Configuration GetConfiguration()
        {
            return Config;
        }

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The application's viewport size or <c>Size.Empty</c> to 
        /// infer the viewport size from the first checked image</param>
        public void Open(
            string appName,
            string testName,
            Size viewportSize)
        {
            OpenEyes(appName, testName, viewportSize);
        }

        /// <summary>
        /// Starts a new test that does not dictate a viewport size.
        /// </summary>
        public void Open(string appName, string testName)
        {
            OpenEyes(appName, testName);
        }

        /// <summary>
        /// Matches the input bitmap with the next expected image.
        /// </summary>
        public AppImage CheckImage(Bitmap image, string tag = null, bool replaceLast = false)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            //return Check(tag, Target.Image(image).ReplaceLast(replaceLast));
            return Check(tag, Target.Image(image));
        }

        public AppImage Check(string name, ICheckSettings checkSettings)
        {
            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            return Check(checkSettings.WithName(name));
        }

        /// <summary>
        /// Matches the input base64 encoded image with the next expected image.
        /// </summary>
        public AppImage CheckImage(string image64, string tag = null, bool replaceLast = false)
        {
            using (Stream s = new MemoryStream(Convert.FromBase64String(image64)))
            {
                Bitmap bmp = new Bitmap(s);
                return CheckImage(bmp, tag, replaceLast);
            }
        }

        /// <summary>
        /// Matches the image stored in the input file with the next expected image.
        /// </summary>
        public AppImage CheckImageFile(string path, string tag = null, bool replaceLast = false)
        {
            using (Stream s = FileUtils.GetSequentialReader(path))
            {
                Bitmap bmp = new Bitmap(s);
                return CheckImage(bmp, tag, replaceLast);
            }
        }

        /// <summary>
        /// Matches the input image with the next expected image.
        /// </summary>
        public AppImage CheckImage(byte[] image, string tag = null, bool replaceLast = false)
        {
            using (Stream s = new MemoryStream(image))
            {
                Bitmap bmp = new Bitmap(s);

                return CheckImage(bmp, tag, replaceLast);
            }
        }

        /// <summary>
        /// Matches the image stored in the input file with the next expected image.
        /// </summary>
        public AppImage CheckImageAtUrl(string url, string tag = null, bool replaceLast = false)
        {
            ArgumentGuard.NotNull(url, nameof(url));
            return Check(Target.Url(url).WithName(tag).ReplaceLast(replaceLast));
        }

        /// <summary>
        /// Perform visual validation for the current image.
        /// </summary>
        /// <param name="image">The image to perform visual validation for.</param>
        /// <param name="region">The region to validate within the image.</param>
        /// <param name="tag">An optional tag to be associated with the validation checkpoint.</param>
        public AppImage CheckRegion(Bitmap image, Rectangle region, string tag)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            return Check(tag, Target.Image(image).Region(region));
        }

        /// <summary>
        /// Perform visual validation for the current image.
        /// </summary>
        /// <param name="image">The image to perform visual validation for.</param>
        /// <param name="region">The region to validate within the image.</param>
        public AppImage CheckRegion(Bitmap image, Rectangle region)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            return Check(Target.Image(image).Region(region));
        }

        public AppImage Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                return new AppImage(false);
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            checkSettings = checkSettings.Timeout(TimeSpan.Zero); // no need for automatic retry when dealing with images.
            IImagesCheckTarget imagesCheckTarget = checkSettings as IImagesCheckTarget;
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            var eyesConfig = ToConfig(Config);
            //eyesConfig.Open.Properties = _properties;
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new CheckRequestPayload
                {
                    Eyes = eyes_,
                    Target = new ImageTarget
                    {
                        Image = imagesCheckTarget.ToImage()
                    },
                    Config = eyesConfig,
                    Settings = CreateUniversalCheckSettings(checkSettingsInternal)
                }
            };

            var result = Runner.SendRequest<EyesCheckRequest, EyesCheckResponse>(request);
            var error = result.Payload.Error;
            if (error != null)
            {
                throw new EyesException(error.ToString());
            }

            var asExpected = true;
            foreach (var matchResult in result.Payload.Result)
            {
                if (matchResult.AsExpected == false)
                {
                    asExpected = false;
                    if (FailureReports == FailureReports.Immediate)
                    {
                        throw new EyesException($"Unexpected match result for {matchResult}");
                    }
                }
            }
            
            return new AppImage(asExpected);
        }

        public ICollection<string> ExtractText(params OcrRegion[] regions)
        {
            return ExtractText(regions.ToList());
        }

        public ICollection<string> ExtractText(ICollection<OcrRegion> regions)
        {
            if (IsDisabled)
            {
                return new List<string>();
            }

            var firstRegion = regions.FirstOrDefault();
            if (firstRegion == null)
            {
                throw new Exception("At least one region is required");
            }

            var eyesConfig = ToConfig(Config);
            var eyesExtractTextRequest = new EyesExtractTextRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new ExtractTextRequestPayload
                {
                    Target = new ImageTarget
                    {
                        Image = firstRegion.GetImage()
                    },
                    Config = eyesConfig,
                    Settings = regions.Select(r => new ExtractTextSettings
                    {
                        Hint = r.GetHint(),
                        MinMatch = r.GetMinMatch(),
                        Language = r.GetLanguage(),
                        Region = GetRegion(r)
                    }).ToList()
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRequest, EyesExtractTextResponse>(eyesExtractTextRequest);

            return response.Payload.Result;
        }

        public IDictionary<string, ICollection<TextRegion>> ExtractTextRegions(TextRegionSettings textRegionSettings)
        {
            Logger.Log("ExtractTextRegions");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, ICollection<TextRegion>>();
            }

            var eyesConfig = ToConfig(Config);
            var extractTextRegionsRequest = new EyesExtractTextRegionsRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new ExtractTextRegionsRequestPayload
                {
                    Target = new ImageTarget
                    {
                        Image = textRegionSettings.Image
                    },
                    Config = eyesConfig,
                    Settings = textRegionSettings
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRegionsRequest, EyesExtractTextRegionsResponse>(extractTextRegionsRequest);

            return response.Payload.Result;
        }

        public void SetTitle(string title)
        {
            title_ = title;
        }

        #endregion

        #endregion

        public override TestResults GetResults(bool throwEx = true)
        {
            Logger.Log("GetResults");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            var request = CreateEyesGetResultsRequest(throwEx);

            var response = Runner.SendRequest<EyesGetResultsRequest, EyesGetResultsResponse>(request);

            var error = response.Payload.Error;
            if (error != null)
            {
                IsOpen = false;
                throw new TestFailedException(error.ToString());
            }

            var testResults = response.Payload.Result;

            foreach (var testResult in testResults)
            {
                testResult.Runner = Runner;
                if (testResult.IsPassed == false && throwEx)
                {
                    IsOpen = false;
                    throw new TestFailedException(testResult, $"{testResult.Url}{Environment.NewLine}");
                }
            }

            IsOpen = false;
            return testResults.First();
        }

        public override TestResults Abort()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            Logger.Verbose("enter. Abort");

            if (IsOpen == false)
            {
                return new TestResults
                {
                    Runner = Runner
                };
            }

            var request = CreateEyesAbortRequest();

            var payload = Runner.SendRequest<EyesAbortRequest, EyesCloseAbortResponse>(request).Payload;
            
            if (payload.Error != null)
            {
                IsOpen = false;
                throw new EyesException(payload.Error.ToString());
            }

            return GetResults();
        }

        public override TestResults Close(bool throwEx = true)
        {
            Logger.Log("Close");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            var eyesCloseRequest = CreateEyesCloseRequest(throwEx);

            var response = Runner.SendRequest<EyesCloseRequest, EyesCloseAbortResponse>(eyesCloseRequest);

            var error = response.Payload.Error;
            if (error != null && throwEx)
            {
                IsOpen = false;
                throw new TestFailedException(error.ToString());
            }

            return GetResults(throwEx);
        }

        private void OpenEyes(string appName = null, string testName = null, Size? viewportSize = null)
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

            Runner.ApiKey = ApiKey;
            Runner.ServerUrl = ServerUrl;
            Runner.Proxy = Proxy;

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, testName, Stage.Open, StageType.Disabled);
                return;
            }

            managerApplitoolsRefId_ = Runner.GetCoreMakeManager();

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(managerApplitoolsRefId_);

            var openEyesResponsePayload = Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException(error.ToString());
            }

            IsOpen = true;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(string applitoolsRefId)
        {
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new OpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Config = ToConfig(Config)
                }
            };
            //result.Payload.Config.Open.Properties = properties_;
            return result;
        }

        private EyesCloseRequest CreateEyesCloseRequest(bool throwEx = false)
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Payload = new CloseResponsePayload
                {
                    Eyes = eyes_,
                    Settings = ToCloseSettings(Config, throwEx)
                }
            };
            return eyesCloseRequest;
        }

        private EyesGetResultsRequest CreateEyesGetResultsRequest(bool throwEx = false)
        {
            var eyesCloseRequest = new EyesGetResultsRequest
            {
                Payload = new EyesGetResultsRequestPayload
                {
                    Eyes = eyes_,
                    Settings = ToResultsSettings(Config, throwEx)
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

        private RegionRectangle GetRegion(OcrRegion ocrRegion)
        {
            var region = ocrRegion.GetRegion();
            if (region == null)
            {
                return null;
            }

            return new RegionRectangle
            {
                Width = region.Value.Width,
                Height = region.Value.Height,
                X = region.Value.Left,
                Y = region.Value.Top,
            };
        }

        private UniversalCheckSettings CreateUniversalCheckSettings(ICheckSettingsInternal checkSettingsInternal)
        {
            var settings = new UniversalCheckSettings
            {
                Hooks = new Hooks
                {
                    BeforeCaptureScreenshot = checkSettingsInternal.GetScriptHooks().FirstOrDefault(i => i.Key == CheckSettings.BEFORE_CAPTURE_SCREENSHOT).Value
                },
                LayoutBreakpoints = GetLayoutBreakpoints(checkSettingsInternal),
                MatchLevel = checkSettingsInternal.GetMatchLevel(),
                Name = checkSettingsInternal.GetName(),
                Region = GetUniversalRegion(checkSettingsInternal),
                Fully = checkSettingsInternal.GetStitchContent(),
                UserCommandId = checkSettingsInternal.GetVariationGroupId(),
                IgnoreDisplacements = checkSettingsInternal.GetIgnoreDisplacements(),
                SendDom = checkSettingsInternal.GetSendDom(),
                UseDom = checkSettingsInternal.GetUseDom(),
                EnablePatterns = checkSettingsInternal.GetEnablePatterns(),
                //Frames = GetFrames(checkSettingsInternal),
                IgnoreRegions = checkSettingsInternal.GetIgnoreRegions().Select(i => i.ToRegion()).ToArray(),
                StrictRegions = checkSettingsInternal.GetStrictRegions().Select(i => i.ToRegion()).ToArray(),
                LayoutRegions = checkSettingsInternal.GetLayoutRegions().Select(i => i.ToRegion()).ToArray(),
                ContentRegions = checkSettingsInternal.GetContentRegions().Select(i => i.ToRegion()).ToArray(),
                FloatingRegions = checkSettingsInternal.GetFloatingRegions().Select(i => i.ToRegion()).ToArray(),
                AccessibilityRegions = checkSettingsInternal.GetAccessibilityRegions().Select(i => i.ToRegion()).ToArray(),
                LazyLoad = checkSettingsInternal.GetLazyLoad()
            };

            return settings;
        }
        private object GetLayoutBreakpoints(ICheckSettingsInternal settings)
        {
            if (settings.GetLayoutBreakpoints() != null && settings.GetLayoutBreakpoints().Any())
            {
                return settings.GetLayoutBreakpoints();
            }

            return settings.GetLayoutBreakpointsEnabled();
        }

        private TRegion GetUniversalRegion(ICheckSettingsInternal checkSettings)
        {
            TRegion region = null;
            if (checkSettings is ICheckSettingsInternal checkSettingsInternal)
            {
                region = CreateCheckSettingsTargetRegion(checkSettingsInternal);
            }
            
            return region;
        }

        private TRegion CreateCheckSettingsTargetRegion(ICheckSettingsInternal settings)
        {
            TRegion region = null;

            var rectangle = settings.GetTargetRegion();
            if (rectangle.HasValue)
            {
                region = new RegionRectangle
                {
                    X = rectangle.Value.X,
                    Y = rectangle.Value.Y,
                    Width = rectangle.Value.Width,
                    Height = rectangle.Value.Height
                };
            }

            return region;
        }
    }
}
using System;
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

        public AppImage Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                return new AppImage(false);
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            checkSettings = checkSettings.Timeout(TimeSpan.Zero); // no need for automatic retry when dealing with images.
            IImagesCheckTarget imagesCheckTarget = checkSettings as IImagesCheckTarget;
            ICheckSettingsInternal checkSettingsInternal = checkSettings as ICheckSettingsInternal;
            var eyesConfig = ToConfig(Config);
            //eyesConfig.Open.Properties = _properties;
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new CheckRequestPayload
                {
                    Eyes = eyes_,
                    Config = eyesConfig,
                    Settings = CreateUniversalCheckSettings(checkSettingsInternal)
                }
            };

            var result = Runner.SendRequest<EyesCheckRequest, EyesCheckResponse>(request).Result;
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
        
        public void SetTitle(string title)
        {
            title_ = title;
        }

        #endregion
        
        #endregion

        public override TestResults Abort()
        {
            throw new NotImplementedException();
        }

        public override TestResults Close(bool throwEx = true)
        {
            throw new NotImplementedException();
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

            var openEyesResponsePayload = Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Result.Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException(error.ToString());
            }

            IsOpen = true;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(
            string applitoolsRefId)
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
                Fully = checkSettingsInternal.GetStitchContent(),
                UserCommandId = checkSettingsInternal.GetVariationGroupId(),
                IgnoreDisplacements = checkSettingsInternal.GetIgnoreDisplacements(),
                SendDom = checkSettingsInternal.GetSendDom(),
                UseDom = checkSettingsInternal.GetUseDom(),
                //Frames = GetFrames(checkSettingsInternal),
                IgnoreRegions = checkSettingsInternal.GetIgnoreRegions().Select(i => i.ToRegion()).ToArray(),
                StrictRegions = checkSettingsInternal.GetStrictRegions().Select(i => i.ToRegion()).ToArray(),
                LayoutRegions = checkSettingsInternal.GetLayoutRegions().Select(i => i.ToRegion()).ToArray(),
                ContentRegions = checkSettingsInternal.GetContentRegions().Select(i => i.ToRegion()).ToArray(),
                FloatingRegions = checkSettingsInternal.GetFloatingRegions().Select(i => i.ToRegion()).ToArray(),
                AccessibilityRegions = checkSettingsInternal.GetAccessibilityRegions().Select(i => i.ToRegion()).ToArray()
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

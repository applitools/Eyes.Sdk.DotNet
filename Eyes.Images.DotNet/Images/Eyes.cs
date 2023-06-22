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
using Region = Applitools.Utils.Geometry.Region;

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

        private string title_;
        private readonly Uri serverUrl_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        /// <param name="logHandler">Optional log handler</param>
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
            IImagesCheckTarget imagesCheckTarget = (IImagesCheckTarget)checkSettings;
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            var eyesConfig = ToConfig(Config);
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
#pragma warning disable CS0612
                    if (FailureReports == FailureReports.Immediate)
                    {
                        throw new EyesException($"Unexpected match result for {matchResult}");
                    }
#pragma warning restore CS0612
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
            
            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(Runner.ManagerApplitoolsRefId);

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
    }
}
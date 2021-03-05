using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Applitools.Fluent;
using Applitools.Utils;
using Applitools.Utils.Geometry;

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
        private EyesScreenshot screenshot_;
        private string screenshotUrl_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl) : base(serverUrl)
        {
            // No need to retry since we are providing the image to match.
            MatchTimeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public Eyes(Logger logger) : base(logger)
        {
            // No need to retry since we are providing the image to match.
            MatchTimeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes() : base()
        {
            // No need to retry since we are providing the image to match.
            MatchTimeout = TimeSpan.Zero;
        }

        #endregion

        #region Configuration

        private Configuration configuration_ = new Configuration();

        protected override Configuration Configuration { get => configuration_; }

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }

        #endregion

        #region Properties

        protected override bool ViewportSizeRequired => false;
        #endregion

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
            Configuration.SetAppName(appName);
            Configuration.SetTestName(testName);
            Configuration.SetViewportSize(viewportSize);
            OpenBase();
        }

        /// <summary>
        /// Starts a new test that does not dictate a viewport size.
        /// </summary>
        public void Open(string appName, string testName)
        {
            Open(appName, testName, Size.Empty);
        }

        public AppImage Check(string name, ICheckSettings checkSettings)
        {
            return this.Check(checkSettings.WithName(name));
        }

        public AppImage Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                return new AppImage(false);
            }

            checkSettings = checkSettings.Timeout(TimeSpan.Zero); // no need for automatic retry when dealing with images.
            IImagesCheckTarget imagesCheckTarget = checkSettings as IImagesCheckTarget;
            ICheckSettingsInternal checkSettingsInternal = checkSettings as ICheckSettingsInternal;
            Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();
            Bitmap image = imagesCheckTarget.Image;
            Uri imageUri = imagesCheckTarget.ImageUri;
            if (image != null)
            {
                if (Configuration.ViewportSize.IsEmpty())
                {
                    Configuration.ViewportSize = image.Size;
                }

                EyesImagesScreenshot screenshot = new EyesImagesScreenshot(image);
                Logger.Verbose("checking image. Image Size: {0}", image.Size);
                screenshot_ = screenshot;
                screenshotUrl_ = null;
            }
            else if (imageUri != null)
            {
                screenshotUrl_ = imageUri.AbsoluteUri;
                Logger.Verbose("checking image by URI: {0}", screenshotUrl_);
                screenshot_ = null;
            }

            MatchResult mr = CheckWindowBase(targetRegion, checkSettings);
            return new AppImage(mr.AsExpected);
        }

        /// <summary>
        /// Matches the input bitmap with the next expected image.
        /// </summary>
        public AppImage CheckImage(Bitmap image, string tag = null, bool replaceLast = false)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            return Check(tag, Target.Image(image).ReplaceLast(replaceLast));
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
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegionBase InRegion(Rectangle region)
        {
            return InRegionBase(() => region);
        }

        public void SetTitle(string title)
        {
            title_ = title;
        }

        #endregion

        #region Protected

        protected override Size GetViewportSize()
        {
            return Configuration.ViewportSize.ToSize();
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            Configuration.SetViewportSize(size);
        }

        protected override string GetInferredEnvironment()
        {
            return string.Empty;
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            if (targetRegion.HasValue && !targetRegion.Value.IsEmpty)
            {
                return screenshot_.GetSubScreenshot(targetRegion.Value, true);
            }
            return screenshot_;
        }

        protected override string GetScreenshotUrl()
        {
            return screenshotUrl_;
        }

        protected override string GetTitle()
        {
            return title_;
        }

        #endregion

        #endregion
    }
}

namespace Applitools.LeanFT
{
    using Applitools;
    using Applitools.Capture;
    using Applitools.Common;
    using Applitools.Fluent;
    using Applitools.Utils;
    using Applitools.Utils.Geometry;
    using HP.LFT.SDK;
    using HP.LFT.SDK.Web;
    using System;
    using System.Drawing;
    using System.Threading;
    using Utils.Images;

    /// <summary>
    /// Applitools Eyes LeanFT DotNet API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "EyesWebDriver should only be disposed by the test")]
    public sealed class Eyes : EyesBase
    {
        #region Fields

        private ITestObject topLevelObject_;
        private bool isHideScrollbarsSetByUser_ = false;
        private bool hideScrollbars_ = false;
        private LeanFTJavaScriptExecutor jsExecutor_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl)
            : base(serverUrl)
        {
            jsExecutor_ = new LeanFTJavaScriptExecutor(ExecuteScript_);
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
            : base()
        {
            jsExecutor_ = new LeanFTJavaScriptExecutor(ExecuteScript_);
        }

        #endregion

        #region Configuration

        private Configuration configuration_ = new Configuration();

        protected override Configuration Configuration { get => configuration_; }

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        protected override void SetConfigImpl(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Forces a full page screenshot (by scrolling and stitching) if the browser only 
        /// supports viewport screenshots).
        /// </summary>
        public bool ForceFullPageScreenshot { get; set; }
        public StitchModes StitchMode { get; set; }
        public int WaitBeforeScreenshots { get; set; }
        public bool HideScrollbars
        {
            get
            {
                return hideScrollbars_;
            }
            set
            {
                hideScrollbars_ = value;
                isHideScrollbarsSetByUser_ = true;
            }
        }

        protected override bool ViewportSizeRequired => false;
        #endregion

        #region Methods

        /// <summary>
        /// Starts a new test.
        /// </summary>
        /// <param name="topLevelObject">The instance of the top level object that controls the window 
        /// hosting the application under test. Either <c>ITopLevelObject</c> or <c>IBrowser</c></param>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The required browser's viewport size 
        /// (i.e., the visible part of the document's body) or <c>Size.Empty</c>
        /// to allow any viewport size.</param>
        public void Open(
            ITestObject topLevelObject,
            string appName,
            string testName,
            Size viewportSize)
        {
            ArgumentGuard.NotNull(topLevelObject, nameof(topLevelObject));

            if (topLevelObject is ITopLevelObject || topLevelObject is IBrowser)
            {
                topLevelObject_ = topLevelObject;
            }
            else
            {
                string errMsg = "Expecting either a top level object for desktop AUT or IBrowser for web AUT ({0})".Fmt(topLevelObject.GetType().Name);
                Logger.Log(TraceLevel.Error, Stage.Open, StageType.Failed, new { errMsg });
                throw new EyesException(errMsg);
            }

            OpenBase(appName, testName, viewportSize);
        }

        /// <summary>
        /// Starts a new test that does not dictate the viewport size of the application under
        /// test.
        /// </summary>
        public void Open(ITestObject driver, string appName, string testName)
        {
            Open(driver, appName, testName, Size.Empty);
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(string tag = null)
        {
            CheckWindowBase(null, tag);
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(TimeSpan matchTimeout, string tag = null)
        {
            CheckWindowBase(null, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="selector">Selects the region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckObject(ITestObject testObject, string tag = null)
        {
            ArgumentGuard.NotNull(testObject, nameof(testObject));
            var obj = testObject as ILocationInfoProvider;
            if (obj == null)
            {
                throw new ArgumentException("Expecting a test object that implements ILocationInfoProvider",
                    nameof(testObject));
            }
            if (tag == null)
            {
                tag = testObject.DisplayName;
            }
            Rectangle? targetRegion = new Rectangle(obj.Location, obj.Size);
            CheckWindowBase(targetRegion, tag);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="selector">Selects the region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckObject(ILocationInfoProvider testObject, TimeSpan matchTimeout, string tag = null)
        {
            ArgumentGuard.NotNull(testObject, nameof(testObject));
            Rectangle targetRegion = new Rectangle(testObject.Location, testObject.Size);
            CheckWindowBase(targetRegion, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, string tag = null)
        {
            CheckWindowBase(region, tag);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <exception cref="Applitools.Eyes.TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, TimeSpan matchTimeout, string tag = null)
        {
            CheckWindowBase(region, tag, (int)matchTimeout.TotalMilliseconds);
        }

        protected override Size GetViewportSize()
        {
            if (topLevelObject_ is ITopLevelObject topObj)
            {
                return topObj.GetSnapshot().Size;
            }

            if (topLevelObject_ is IBrowser browser)
            {
                try
                {
                    var size = JSBrowserCommands.WithoutReturn.GetViewportSize((s) => ExecuteScript_(s));
                    if (size.Width > 0 && size.Height > 0)
                    {
                        return size;
                    }
                    else
                    {
                        //Logger.Log("GetViewportSize(): Got null width/height.");
                    }
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(Logger, Stage.Open, ex);
                }

                //Logger.Log("GetViewportSize(): Using page size.");
                return browser.Page.Size;
            }

            throw new ArgumentException("Top level object is of unsupported type");
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            ArgumentGuard.NotNull(size, nameof(size));

            Logger.Verbose("SetViewportSize({0})", size);

            const int SLEEP = 1000;
            const int RETRIES = 3;

            var requiredSize = new Size(size.Width, size.Height);

            if (topLevelObject_ is ITopLevelObject)
            {
                throw new NotSupportedException("Cannot resize a Desktop application to establish viewport size");
            }

            var browser = topLevelObject_ as IBrowser;
            if (browser == null)
            {
                throw new ArgumentException("Top level object is of unsupported type");
            }
            browser.ResizeTo(requiredSize);
            var retriesLeft = RETRIES;
            Size browserSize;
            do
            {
                Thread.Sleep(SLEEP);
                browserSize = browser.Size;
            }
            while (--retriesLeft > 0 && browserSize != requiredSize);

            if (browserSize != requiredSize)
            {
                string errMsg = $"Failed to set browser size! Browser size: {browserSize}";
                Logger.Log(TraceLevel.Error, Stage.Open, StageType.Failed, new { errMsg });
                throw new TestFailedException(errMsg);
            }

            var viewportSize = GetViewportSize();
            Logger.Verbose("initial viewport size: {0}", viewportSize);

            browser.ResizeTo(new Size(
                    (2 * browserSize.Width) - viewportSize.Width,
                    (2 * browserSize.Height) - viewportSize.Height));

            retriesLeft = RETRIES;
            do
            {
                Thread.Sleep(SLEEP);
                viewportSize = GetViewportSize();
                Logger.Verbose("viewport size: {0}", viewportSize);
            }
            while (--retriesLeft > 0 && viewportSize != requiredSize);

            if (viewportSize != requiredSize)
            {
                // One last attempt. Solves the "maximized browser" bug (border size for maximized 
                // browser sometimes different than non-maximized, so the original browser size 
                // calculation is wrong).
                Logger.Verbose("attempting one more time...");
                browserSize = browser.Size;
                var updatedBrowserSize = new Size(
                    browserSize.Width + (requiredSize.Width - viewportSize.Width),
                    browserSize.Height + (requiredSize.Height - viewportSize.Height));

                Logger.Verbose("browser size: {0}", browserSize);
                Logger.Verbose("required browser size: {0}", updatedBrowserSize);
                browser.ResizeTo(updatedBrowserSize);

                retriesLeft = RETRIES;
                do
                {
                    Thread.Sleep(SLEEP);
                    viewportSize = GetViewportSize();
                    Logger.Verbose("browser size: {0}", browser.Size);
                    Logger.Verbose("viewport size: {0}", viewportSize);
                }
                while (--retriesLeft > 0 && viewportSize != requiredSize);
            }

            if (viewportSize != requiredSize)
            {
                string errMsg = "Failed to set the viewport size.";
                Logger.Log(TraceLevel.Error, Stage.Open, StageType.Failed, new { errMsg });
                throw new TestFailedException(errMsg);
            }
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            IHideScrollbarsProvider scrollbarsProvider;
            if (isHideScrollbarsSetByUser_)
            {
                scrollbarsProvider = HideScrollbars ?
                    (IHideScrollbarsProvider)new LeanFTJSHideScrollbarsProvider(jsExecutor_) :
                    (IHideScrollbarsProvider)new NullHideScrollbarsProvider();
            }
            else // Use automatic inference for whether or not to remove scrollabrs.
            {
                scrollbarsProvider = ForceFullPageScreenshot ?
                    (IHideScrollbarsProvider)new LeanFTJSHideScrollbarsProvider(jsExecutor_) :
                    (IHideScrollbarsProvider)new NullHideScrollbarsProvider();
            }

            scrollbarsProvider.HideScrollbars();
            EyesScreenshot screenshot;

            if (ForceFullPageScreenshot)
            {
                if (topLevelObject_ is IBrowser)
                {
                    screenshot = GetFullPageScreenshot_();
                }
                else
                {
                    throw new ArgumentException("Full page screenshot is supported only for browser test object");
                }
            }
            else
            {
                screenshot = new EyesLeanFTScreenshot(CaptureScreen_());
            }

            if (targetRegion.HasValue)
            {
                screenshot = screenshot.GetSubScreenshot(targetRegion.Value, true);
            }
            scrollbarsProvider.RestoreScrollbarsState();

            return screenshot;
        }

        protected override string GetTitle()
        {
            if (topLevelObject_ is IBrowser browser)
            {
                return browser.Page.Title;
            }

            if (topLevelObject_ is ITopLevelObject)
            {
                return string.Empty;
            }

            throw new ArgumentException("Top level object is of unsupported type");
        }

        protected override string GetInferredEnvironment()
        {
            if (topLevelObject_ is IBrowser)
            {
                var userAgent = ExecuteScript_("navigator.userAgent;");
                if (userAgent != null)
                {
                    return "useragent:" + userAgent.ToString();
                }

                return null;
            }

            if (topLevelObject_ is ITopLevelObject)
            {
                // return pos
                //throw new NotImplementedException("Desktop tests not supported yet");
                return null;
            }

            throw new ArgumentException("Top level object is of unsupported type");
        }

        #endregion

        #region Private Memebers

        private object ExecuteScript_(string script)
        {
            if (topLevelObject_ is ITopLevelObject)
            {
                throw new NotSupportedException("Cannot run javascript on a Desktop application.");
            }

            if (topLevelObject_ is IBrowser browser)
            {
                //Log(script);
                return browser.Page.RunJavaScript(script);
            }

            throw new ArgumentException("Top level object is of unsupported type");
        }

        /// <summary>
        /// Get the scroll position of the current frame.
        /// </summary>
        /// <returns>The scroll position of the current frame.</returns>
        private Point GetCurrentScrollPosition_()
        {
            return JSBrowserCommands.WithoutReturn.GetCurrentScrollPosition(jsExecutor_);
        }

        /// <summary>
        /// Scrolls to the given position.
        /// </summary>
        /// <param name="scrollPosition">The position to scroll to.</param>
        private void ScrollTo_(Point scrollPosition)
        {
            JSBrowserCommands.WithoutReturn.ScrollTo(scrollPosition, jsExecutor_);
        }

        /// <summary>
        /// Get the size of the entire page based on the scroll width/height.
        /// </summary>
        /// <returns>The size of the entire page.</returns>
        private Size GetEntirePageSize_()
        {
            return JSBrowserCommands.WithoutReturn.GetEntirePageSize(jsExecutor_);
        }

        /// <summary>
        /// Creates a full page image by scrolling the viewport and "stitching" the screenshots to 
        /// each other.  
        /// </summary>
        /// <returns>The image of the entire page.</returns>
        private EyesScreenshot GetFullPageScreenshot_()
        {
            IPositionProvider stitchProvider = null;
            switch (StitchMode)
            {
                case StitchModes.CSS:
                    stitchProvider = new LeanFTCssTranslatePositionProvider(jsExecutor_);
                    break;
                case StitchModes.Scroll:
                default:
                    stitchProvider = new LeanFTScrollPositionProvider(jsExecutor_);
                    break;
            }

            ScaleProviderFactory scaleProviderFactory = new FixedScaleProviderFactory(1.0, SetScaleProvider);
            IImageProvider imageProvider = new LeanFTImageProvider(this.topLevelObject_);
            
            RenderingInfo renderingInfo = ServerConnector.GetRenderingInfo();
            FullPageCaptureAlgorithm algo = new FullPageCaptureAlgorithm(Logger, TestId, null, WaitBeforeScreenshots, DebugScreenshotProvider,
            (image) => new EyesLeanFTScreenshot(image), scaleProviderFactory, CutProvider, 0, imageProvider,
            renderingInfo.MaxImageHeight, renderingInfo.MaxImageArea);

            byte[] screenshotBytes = algo.GetFullPageScreenshot(stitchProvider);

            EyesLeanFTScreenshot screenshot = new EyesLeanFTScreenshot(screenshotBytes);
            return screenshot;
        }

        private byte[] CaptureScreen_()
        {
            Bitmap screenshot;
            if (topLevelObject_ is IBrowser browserTopObj)
            {
                screenshot = (Bitmap)browserTopObj.Page.GetSnapshot();
            }
            else
            {
                if (topLevelObject_ is ITopLevelObject topObj)
                {
                    screenshot = (Bitmap)topObj.GetSnapshot();
                }
                else
                {
                    throw new ArgumentException("Top level object is of unsupported type");
                }
            }

            return BasicImageUtils.EncodeAsPng(screenshot);
        }

        #endregion
    }
}

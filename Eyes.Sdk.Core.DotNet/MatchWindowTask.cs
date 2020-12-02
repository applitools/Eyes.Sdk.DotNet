using Applitools.Ufg;
using Applitools.VisualGrid;
using Applitools.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    public sealed class MatchWindowTask : IDisposable, ILastScreenshotBounds
    {

        #region Fields

        private const int MatchInterval_ = 500;

        private readonly IServerConnector serverConnector_;
        private readonly RunningSession runningSession_;
        private readonly int defaultRetryTimeout_;
        private readonly AppOutputProviderDelegate getAppOutput_;
        private readonly EyesBase eyes_;

        private MatchResult matchResult_;
        private EyesScreenshot lastScreenshot_;
        private string lastScreenshotHash_;

        #endregion

        #region Constructors

        /// <summary>
        /// Captures a screenshot and sends it to be matched by the server.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="serverConnector">The gateway to the Eyes server.</param>
        /// <param name="runningSession">The running session in which we should match the window.</param>
        /// <param name="retryTimeout">The time allowed to retry matching in case of a mismatch.</param>
        /// <param name="eyes">The EyesBase that created this instance.</param>
        /// <param name="appOutputProvider">Provides application output, given the input last application screenshot.</param>
        public MatchWindowTask(
            Logger logger,
            IServerConnector serverConnector,
            RunningSession runningSession,
            TimeSpan retryTimeout,
            EyesBase eyes,
            AppOutputProviderDelegate appOutputProvider)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(serverConnector, nameof(serverConnector));
            ArgumentGuard.NotNull(runningSession, nameof(runningSession));
            ArgumentGuard.NotNull(eyes, nameof(eyes));
            //ArgumentGuard.NotNull(appOutputProvider, nameof(appOutputProvider));

            Logger_ = logger;
            serverConnector_ = serverConnector;
            runningSession_ = runningSession;
            defaultRetryTimeout_ = (int)retryTimeout.TotalMilliseconds;
            getAppOutput_ = appOutputProvider;
            matchResult_ = null;
            eyes_ = eyes;
        }

        public MatchWindowTask(Logger logger, IServerConnector serverConnector,
                               RunningSession runningSession, TimeSpan retryTimeout,
                               EyesBase eyes)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(serverConnector, nameof(serverConnector));
            ArgumentGuard.NotNull(runningSession, nameof(runningSession));

            Logger_ = logger;
            serverConnector_ = serverConnector;
            runningSession_ = runningSession;
            defaultRetryTimeout_ = (int)retryTimeout.TotalMilliseconds;
            getAppOutput_ = null;
            eyes_ = eyes;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The bounds of the last screenshot relative to the window.
        /// </summary>
        public Rectangle LastScreenshotBounds { get; private set; }

        /// <summary>
        /// Message logger.
        /// </summary>
        private Logger Logger_ { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Repeatedly obtains an application snapshot and matches it with the next expected 
        /// output, until a match is found or the timeout expires.
        /// </summary>
        /// <param name="region">The rectangle to be captured in the screenshot. Pass <c>Rectangle.Empty</c> or <c>null</c> for the entire screenshot.</param>
        /// <param name="userInputs">User input preceding this match.</param>
        /// <param name="tag">Optional tag to associated with the match (can be <c>null</c>.</param>
        /// <param name="shouldRunOnceOnRetryTimeout">Force a single match attempt at the end of the match timeout.</param>
        /// <param name="replaceLast">Whether to replace the last step, e.g. in case of a mismatch.</param>
        /// <param name="checkSettingsInternal">The check settings to use.</param>
        /// <param name="source">The source of image to match.</param>
        /// <param name="domUrl">The URL where the DOM is.</param>
        /// <returns>Returns the results of the match.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Disposed on rewrite. Last image is disposed by Dispose()")]
        public MatchResult MatchWindow(Rectangle? region, IList<Trigger> userInputs, string tag, bool shouldRunOnceOnRetryTimeout, bool replaceLast,
            ICheckSettingsInternal checkSettingsInternal, string source, string domUrl = null)
        {
            ImageMatchSettings imageMatchSettings = CreateImageMatchSettings(checkSettingsInternal, eyes_);
            int retryTimeout = checkSettingsInternal.GetTimeout();
            if (retryTimeout < 0)
            {
                retryTimeout = defaultRetryTimeout_;
            }

            Logger_.Verbose("retryTimeout: {0} ; replaceLast: {1}", retryTimeout, replaceLast);
            EyesScreenshot screenshot = TakeScreenshot_(
                region, userInputs, tag, shouldRunOnceOnRetryTimeout,
                replaceLast, checkSettingsInternal, imageMatchSettings, retryTimeout, source);

            if (replaceLast)
            {
                return matchResult_;
            }

            UpdateLastScreenshot_(screenshot);

            return matchResult_;
        }

        public static void CollectRegions(EyesBase eyes, EyesScreenshot screenshot,
            ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings)
        {
            eyes.Logger.Verbose("enter");
            CollectSimpleRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
            CollectFloatingRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
            CollectAccessibilityRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
            LogRegions_(eyes.Logger, imageMatchSettings);
            eyes.Logger.Verbose("exit");
        }

        public MatchResult PerformMatch(IList<Trigger> userInputs,
                                       AppOutputWithScreenshot appOutput,
                                       string tag, bool replaceLast,
                                       ImageMatchSettings imageMatchSettings,
                                       EyesBase eyes, string source)
        {
            // called from regular flow and from check many flow.
            string agentSetupStr = eyes.GetAgentSetupString();
            return PerformMatch_(userInputs, appOutput, tag, replaceLast, imageMatchSettings, agentSetupStr, source, null);
        }

        public MatchResult PerformMatch(AppOutputWithScreenshot appOutput,
                                       string tag, bool ignoreMismatch,
                                       ICheckSettingsInternal checkSettingsInternal,
                                       ImageMatchSettings imageMatchSettings,
                                       IList<IRegion> regions,
                                       IList<VisualGridSelector[]> regionSelectors,
                                       IList<VGUserAction> userActions,
                                       EyesBase eyes, string source, string renderId = null)
        {
            EyesScreenshot screenshot = appOutput.Screenshot;
            string agentSetupStr = eyes.GetAgentSetupString();

            List<Trigger> userInputs = new List<Trigger>();
            CollectRegions_(imageMatchSettings, userInputs, regions, regionSelectors, userActions);
            CollectRegions_(imageMatchSettings, checkSettingsInternal);
            return PerformMatch_(userInputs, appOutput, tag, ignoreMismatch, imageMatchSettings, agentSetupStr, source, renderId);
        }

        private void CollectRegions_(ImageMatchSettings imageMatchSettings, ICheckSettingsInternal checkSettingsInternal)
        {
            imageMatchSettings.Ignore = ConvertSimpleRegions(checkSettingsInternal.GetIgnoreRegions(), imageMatchSettings.Ignore);
            imageMatchSettings.Content = ConvertSimpleRegions(checkSettingsInternal.GetContentRegions(), imageMatchSettings.Content);
            imageMatchSettings.Layout = ConvertSimpleRegions(checkSettingsInternal.GetLayoutRegions(), imageMatchSettings.Layout);
            imageMatchSettings.Strict = ConvertSimpleRegions(checkSettingsInternal.GetStrictRegions(), imageMatchSettings.Strict);
            imageMatchSettings.Floating = ConvertFloatingRegions(checkSettingsInternal.GetFloatingRegions(), imageMatchSettings.Floating);
            imageMatchSettings.Accessibility = ConvertAccessibilityRegions(checkSettingsInternal.GetAccessibilityRegions(), imageMatchSettings.Accessibility);
        }

        private AccessibilityRegionByRectangle[] ConvertAccessibilityRegions(IGetAccessibilityRegion[] accessibilityRegions, AccessibilityRegionByRectangle[] currentRegions)
        {
            List<AccessibilityRegionByRectangle> mutableRegions = new List<AccessibilityRegionByRectangle>();
            if (currentRegions != null)
            {
                mutableRegions.AddRange(currentRegions);
            }

            foreach (IGetAccessibilityRegion getRegions in accessibilityRegions)
            {
                if (getRegions is AccessibilityRegionByRectangle)
                {
                    mutableRegions.AddRange(getRegions.GetRegions(null, null));
                }
            }

            return mutableRegions.ToArray();
        }

        private FloatingMatchSettings[] ConvertFloatingRegions(IGetFloatingRegion[] floatingRegions, FloatingMatchSettings[] currentRegions)
        {
            List<FloatingMatchSettings> mutableRegions = new List<FloatingMatchSettings>();
            if (currentRegions != null)
            {
                mutableRegions.AddRange(currentRegions);
            }

            foreach (IGetFloatingRegion getRegions in floatingRegions)
            {
                if (getRegions is FloatingRegionByRectangle)
                {
                    mutableRegions.AddRange(getRegions.GetRegions(null, null));
                }
            }

            return mutableRegions.ToArray();
        }

        private static IMutableRegion[] ConvertSimpleRegions(IGetRegions[] simpleRegions, IMutableRegion[] currentRegions)
        {
            List<IMutableRegion> mutableRegions = new List<IMutableRegion>();
            if (currentRegions != null)
            {
                mutableRegions.AddRange(currentRegions);
            }

            foreach (IGetRegions getRegions in simpleRegions)
            {
                if (getRegions is SimpleRegionByRectangle)
                {
                    mutableRegions.AddRange(getRegions.GetRegions(null, null));
                }
            }

            return mutableRegions.ToArray();
        }

        private static void CollectRegions_(ImageMatchSettings imageMatchSettings, IList<Trigger> userInputs,
                                            IList<IRegion> regions, IList<VisualGridSelector[]> regionSelectors,
                                            IList<VGUserAction> userActions)
        {
            if (regions == null) return;

            int currentCounter = 0;
            int currentTypeIndex = 0;
            int currentTypeRegionCount = regionSelectors[0].Length;

            List<IMutableRegion>[] mutableRegions = new List<IMutableRegion>[]{
                new List<IMutableRegion>(), // Ignore Regions
                new List<IMutableRegion>(), // Layout Regions
                new List<IMutableRegion>(), // Strict Regions
                new List<IMutableRegion>(), // Content Regions
                new List<IMutableRegion>(), // Floating Regions
                new List<IMutableRegion>(), // Accessibility Regions
                new List<IMutableRegion>(), // User Action Regions
                new List<IMutableRegion>(), // Target Element Location
            };

            foreach (IRegion r in regions)
            {
                bool canAddRegion = false;
                while (!canAddRegion)
                {
                    currentCounter++;
                    if (currentCounter > currentTypeRegionCount)
                    {
                        currentTypeIndex++;
                        currentTypeRegionCount = regionSelectors[currentTypeIndex].Length;
                        currentCounter = 0;
                    }
                    else
                    {
                        canAddRegion = true;
                    }
                }
                MutableRegion mr = new MutableRegion(r);
                mutableRegions[currentTypeIndex].Add(mr);
            }

            Point location = Point.Empty;

            // If target element location available
            if (mutableRegions[7].Count > 0)
            {
                location = mutableRegions[7][0].Location;
            }

            imageMatchSettings.Ignore = FilterEmptyEntries_(mutableRegions[0], location);
            imageMatchSettings.Layout = FilterEmptyEntries_(mutableRegions[1], location);
            imageMatchSettings.Strict = FilterEmptyEntries_(mutableRegions[2], location);
            imageMatchSettings.Content = FilterEmptyEntries_(mutableRegions[3], location);

            List<FloatingMatchSettings> floatingMatchSettings = new List<FloatingMatchSettings>();
            for (int i = 0; i < regionSelectors[4].Length; i++)
            {
                IMutableRegion mr = mutableRegions[4][i];
                if (mr.Area == 0) continue;
                VisualGridSelector vgs = regionSelectors[4][i];

                if (vgs.Category is IGetFloatingRegionOffsets gfr)
                {
                    FloatingMatchSettings fms = new FloatingMatchSettings()
                    {
                        MaxLeftOffset = gfr.MaxLeftOffset,
                        MaxUpOffset = gfr.MaxUpOffset,
                        MaxRightOffset = gfr.MaxRightOffset,
                        MaxDownOffset = gfr.MaxDownOffset,
                        Top = mr.Top - location.Y,
                        Left = mr.Left - location.X,
                        Width = mr.Width,
                        Height = mr.Height
                    };
                    floatingMatchSettings.Add(fms);
                }
            }
            imageMatchSettings.Floating = floatingMatchSettings.ToArray();

            List<AccessibilityRegionByRectangle> accessibilityRegions = new List<AccessibilityRegionByRectangle>();
            for (int i = 0; i < regionSelectors[5].Length; i++)
            {
                IMutableRegion mr = mutableRegions[5][i];
                if (mr.Area == 0) continue;
                VisualGridSelector vgs = regionSelectors[5][i];

                if (vgs.Category is IGetAccessibilityRegionType gar)
                {
                    AccessibilityRegionByRectangle accessibilityRegion = new AccessibilityRegionByRectangle(
                        mr.Left - location.X,
                        mr.Top - location.Y,
                        mr.Width,
                        mr.Height,
                        gar.AccessibilityRegionType);
                    accessibilityRegions.Add(accessibilityRegion);
                }
            }
            imageMatchSettings.Accessibility = accessibilityRegions.ToArray();

            for (int i = 0; i < regionSelectors[6].Length; i++)
            {
                IMutableRegion mr = mutableRegions[6][i];
                if (mr.Area == 0) continue;
                VGUserAction userAction = userActions[i];
                Trigger trigger = UserActionToTrigger(mr, userAction);
                userInputs.Add(trigger);
            }
        }

        public static Trigger UserActionToTrigger(IMutableRegion mr, VGUserAction userAction)
        {
            Trigger trigger = null;
            Region r = mr.Rectangle;
            if (userAction is VGTextTrigger vgTextTrigger)
            {
                trigger = new TextTrigger(r, vgTextTrigger.Text);
            }
            else if (userAction is VGMouseTrigger vgMouseTrigger)
            {
                trigger = new MouseTrigger(vgMouseTrigger.Action, r, vgMouseTrigger.Cursor);
            }
            return trigger;
        }

        private static IMutableRegion[] FilterEmptyEntries_(List<IMutableRegion> list, Point location)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Area == 0)
                {
                    list.RemoveAt(i);
                }
                else
                {
                    list[i].Offset(-location.X, -location.Y);
                }
            }
            return list.ToArray();
        }

        private MatchResult PerformMatch_(IList<Trigger> userInputs,
                                        AppOutputWithScreenshot appOutput,
                                        string tag, bool replaceLast,
                                        ImageMatchSettings imageMatchSettings,
                                        string agentSetupStr,
                                        string source,
                                        string renderId)
        {
            // Prepare match data.
            MatchWindowData data = new MatchWindowData(appOutput.AppOutput, tag, agentSetupStr);

            data.IgnoreMismatch = false;
            data.Options = new ImageMatchOptions(imageMatchSettings);
            data.Options.Name = tag;
            data.Options.UserInputs = userInputs;
            data.Options.IgnoreMismatch = false;
            data.Options.IgnoreMatch = false;
            data.Options.ForceMismatch = false;
            data.Options.ForceMatch = false;
            data.Options.Source = source;
            data.Options.RenderId = renderId;
            data.Options.ReplaceLast = replaceLast;
            data.RenderId = renderId;

            return serverConnector_.MatchWindow(runningSession_, data);
        }

        private static void CollectSimpleRegions_(ICheckSettingsInternal checkSettingsInternal,
                                             ImageMatchSettings imageMatchSettings, EyesBase eyes,
                                             EyesScreenshot screenshot)
        {
            eyes.Logger.Verbose("enter");
            imageMatchSettings.Ignore = CollectSimpleRegions_(eyes, screenshot, checkSettingsInternal.GetIgnoreRegions(), "Ignore");
            imageMatchSettings.Strict = CollectSimpleRegions_(eyes, screenshot, checkSettingsInternal.GetStrictRegions(), "Strict");
            imageMatchSettings.Layout = CollectSimpleRegions_(eyes, screenshot, checkSettingsInternal.GetLayoutRegions(), "Layout");
            imageMatchSettings.Content = CollectSimpleRegions_(eyes, screenshot, checkSettingsInternal.GetContentRegions(), "Content");
            eyes.Logger.Verbose("exit");
        }

        private static IMutableRegion[] CollectSimpleRegions_(EyesBase eyes,
                                          EyesScreenshot screenshot, IGetRegions[] regionProviders, string type)
        {
            eyes.Logger.Verbose("enter - region type: {0}", type);
            List<IMutableRegion> mutableRegions = new List<IMutableRegion>();
            foreach (IGetRegions regionProvider in regionProviders)
            {
                mutableRegions.AddRange(regionProvider.GetRegions(eyes, screenshot));
            }
            eyes.Logger.Verbose("exit");
            return mutableRegions.ToArray();
        }

        private static void CollectFloatingRegions_(ICheckSettingsInternal checkSettingsInternal,
                                             ImageMatchSettings imageMatchSettings, EyesBase eyes,
                                             EyesScreenshot screenshot)
        {
            eyes.Logger.Verbose("enter");
            List<FloatingMatchSettings> floatingRegions = new List<FloatingMatchSettings>();
            foreach (IGetFloatingRegion regionProvider in checkSettingsInternal.GetFloatingRegions())
            {
                floatingRegions.AddRange(regionProvider.GetRegions(eyes, screenshot));
            }
            imageMatchSettings.Floating = floatingRegions.ToArray();
            eyes.Logger.Verbose("exit");
        }

        private static void CollectAccessibilityRegions_(ICheckSettingsInternal checkSettingsInternal,
                                       ImageMatchSettings imageMatchSettings, IEyesBase eyes,
                                       EyesScreenshot screenshot)
        {
            eyes.Logger.Verbose("enter");
            List<AccessibilityRegionByRectangle> accessibilityRegions = new List<AccessibilityRegionByRectangle>();
            foreach (IGetAccessibilityRegion regionProvider in checkSettingsInternal.GetAccessibilityRegions())
            {
                accessibilityRegions.AddRange(regionProvider.GetRegions(eyes, screenshot));
            }
            imageMatchSettings.Accessibility = accessibilityRegions.ToArray();
            eyes.Logger.Verbose("exit");
        }

        private EyesScreenshot TakeScreenshot_(Rectangle? region, IList<Trigger> userInputs, string tag,
            bool shouldRunOnceOnRetryTimeout, bool replacLast, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings,
            int retryTimeout, string source)
        {
            EyesScreenshot screenshot;
            Stopwatch sw = Stopwatch.StartNew();
            lastScreenshotHash_ = null;
            if (shouldRunOnceOnRetryTimeout || retryTimeout == 0)
            {
                if (retryTimeout > 0)
                {
                    Thread.Sleep(retryTimeout);
                }

                screenshot = TryTakingScreenshot_(region, userInputs, tag, replacLast, checkSettingsInternal, imageMatchSettings, source);
            }
            else
            {
                screenshot = RetryTakingScreenshot_(region, userInputs, tag, replacLast, checkSettingsInternal, imageMatchSettings, retryTimeout, source);
            }

            Logger_.Verbose("Completed in {0}", sw.Elapsed);
            return screenshot;
        }

        private EyesScreenshot RetryTakingScreenshot_(Rectangle? region, IList<Trigger> userInputs, string tag, bool replaceLast, ICheckSettingsInternal checkSettingsInternal,
            ImageMatchSettings imageMatchSettings, int retryTimeout, string source)
        {
            Logger_.Verbose("enter");
            // Retry matching and ignore mismatches while the retry timeout does not expires.
            var sw2 = Stopwatch.StartNew();
            EyesScreenshot screenshot = null;
            while (sw2.ElapsedMilliseconds < retryTimeout)
            {
                Thread.Sleep(MatchInterval_);
                screenshot = TryTakingScreenshot_(region, userInputs, tag, replaceLast, checkSettingsInternal, imageMatchSettings, source);

                if (matchResult_.AsExpected)
                {
                    break;
                }
            }

            if (!matchResult_.AsExpected)
            {
                // Try one last time...
                screenshot = TryTakingScreenshot_(region, userInputs, tag, replaceLast, checkSettingsInternal, imageMatchSettings, source);
            }
            Logger_.Verbose("exit");
            return screenshot;
        }

        private void UpdateLastScreenshot_(EyesScreenshot screenshot)
        {
            // Server will register the failed image so we keep it as the last screenshot
            // to compress against.
            DisposeLastScreenshot_();

            if (screenshot != null)
            {
                lastScreenshot_ = screenshot;
                LastScreenshotBounds = new Rectangle(Point.Empty, screenshot.Image.Size);
            }
        }

        private EyesScreenshot TryTakingScreenshot_(Rectangle? region, IList<Trigger> userInputs, string tag, bool replaceLast, ICheckSettingsInternal checkSettingsInternal,
            ImageMatchSettings imageMatchSettings, string source)
        {
            Logger_.Verbose("enter");
            AppOutputWithScreenshot appOutputWithScreenshot = GetAppOutput_(region, checkSettingsInternal, imageMatchSettings);
            EyesScreenshot screenshot = appOutputWithScreenshot.Screenshot;
            AppOutput appOutput = appOutputWithScreenshot.AppOutput;
            string currentScreenshotHash = screenshot == null ? appOutput.ScreenshotUrl : CommonUtils.GetSha256Hash(appOutput.ScreenshotBytes);
            Logger_.Verbose("current screenshot hash: {0}", currentScreenshotHash);
            if (lastScreenshotHash_ == currentScreenshotHash)
            {
                Logger_.Log("second screenshot is the same as the first, no point in uploading to server.");
            }
            else
            {
                matchResult_ = PerformMatch(userInputs, appOutputWithScreenshot, tag,
                    replaceLast || (lastScreenshotHash_ != null), imageMatchSettings, eyes_, source);
                lastScreenshotHash_ = currentScreenshotHash;
            }
            Logger_.Verbose("exit");
            return screenshot;
        }

        private void DisposeLastScreenshot_()
        {
            if (lastScreenshot_ != null)
            {
                lastScreenshot_.Image.DisposeIfNotNull();
                lastScreenshot_ = null;
            }
        }

        public void Dispose()
        {
            DisposeLastScreenshot_();
        }

        private AppOutputWithScreenshot GetAppOutput_(Rectangle? region, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var appOutput = getAppOutput_(region, checkSettingsInternal, imageMatchSettings);
                Logger_.Verbose("completed in {0}ms", sw.ElapsedMilliseconds);

                return appOutput;
            }
            catch (Exception ex)
            {
                Logger_.Log("failed in {0}ms - {1}", sw.ElapsedMilliseconds, ex);
                throw;
            }
        }

        public static ImageMatchSettings CreateImageMatchSettings(ICheckSettingsInternal checkSettingsInternal, EyesBase eyes)
        {
            ImageMatchSettings imageMatchSettings = null;
            if (checkSettingsInternal != null)
            {
                imageMatchSettings = eyes.DefaultMatchSettings.Clone();
                imageMatchSettings.MatchLevel = checkSettingsInternal.GetMatchLevel() ?? eyes.MatchLevel;
                imageMatchSettings.IgnoreCaret = checkSettingsInternal.GetIgnoreCaret() ?? eyes.IgnoreCaret;
                imageMatchSettings.UseDom = checkSettingsInternal.GetUseDom() ?? eyes.UseDom;
                imageMatchSettings.EnablePatterns = checkSettingsInternal.GetEnablePatterns() ?? eyes.EnablePatterns;
                imageMatchSettings.IgnoreDisplacements = checkSettingsInternal.GetIgnoreDisplacements() ?? eyes.Config.IgnoreDisplacements;
                imageMatchSettings.AccessibilitySettings = eyes.Config.AccessibilityValidation;
            }
            return imageMatchSettings;
        }

        public static ImageMatchSettings CreateImageMatchSettings(ICheckSettingsInternal checkSettingsInternal, EyesBase eyes, EyesScreenshot screenshot)
        {
            eyes.Logger.Verbose("enter");
            ImageMatchSettings imageMatchSettings = CreateImageMatchSettings(checkSettingsInternal, eyes);
            if (imageMatchSettings != null)
            {
                CollectSimpleRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
                CollectFloatingRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
                CollectAccessibilityRegions_(checkSettingsInternal, imageMatchSettings, eyes, screenshot);
                LogRegions_(eyes.Logger, imageMatchSettings);
            }
            eyes.Logger.Verbose("exit");
            return imageMatchSettings;
        }

        private static void LogRegions_(Logger logger, ImageMatchSettings ims)
        {
            LogTypedRegions_(logger, "Ignore", ims.Ignore);
            LogTypedRegions_(logger, "Strict", ims.Strict);
            LogTypedRegions_(logger, "Content", ims.Content);
            LogTypedRegions_(logger, "Layout", ims.Layout);
            LogTypedRegions_(logger, "Floating", ims.Floating);
            LogTypedRegions_(logger, "Accessibility", ims.Accessibility);
        }

        private static void LogTypedRegions_(Logger logger, string regionType, object[] regions)
        {
            if (regions == null || regions.Length == 0)
            {
                logger.Verbose("{0} Regions list is null or empty", regionType);
                return;
            }
            logger.Verbose("{0} Regions:", regionType);
            foreach (object region in regions)
            {
                logger.Verbose("    {0}", region);
            }
        }

        #endregion
    }
}

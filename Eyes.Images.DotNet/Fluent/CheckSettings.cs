using Applitools.VisualGrid;
using Applitools.Fluent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    /// <summary>
    /// The Match settings object to use in the various Eyes.Check methods.
    /// </summary>
    public class CheckSettings : ICheckSettings, ICheckSettingsInternal
    {
        protected List<IGetRegions> ignoreRegions_ = new List<IGetRegions>();
        protected List<IGetRegions> contentRegions_ = new List<IGetRegions>();
        protected List<IGetRegions> layoutRegions_ = new List<IGetRegions>();
        protected List<IGetRegions> strictRegions_ = new List<IGetRegions>();
        protected List<IGetFloatingRegion> floatingRegions_ = new List<IGetFloatingRegion>();
        protected List<IGetAccessibilityRegion> accessibilityRegions_ = new List<IGetAccessibilityRegion>();
        private int timeout_ = -1;
        private int? waitBeforeCapture_;
        private bool? stitchContent_;
        private LazyLoadOptions lazyLoad_;
        private Rectangle? targetRegion_;
        private bool? ignoreCaret_ = null;
        private MatchLevel? matchLevel_ = null;
        private string name_;
        private bool? sendDom_ = null;
        private bool? useDom_ = null;
        private bool? enablePatterns_ = null;
        private bool? ignoreDisplacements_;
        private bool replaceLast_;
        private Dictionary<string, string> scriptHooks_ = new Dictionary<string, string>();
        private VisualGridOption[] visualGridOptions_;
        private string variationGroupId_;
        private string pageId_;

        protected LayoutBreakpointsOptions LayoutBreakpointsOptions { get; set; }

        public static readonly string BEFORE_CAPTURE_SCREENSHOT = "beforeCaptureScreenshot";

        internal protected CheckSettings()
        {
            fluentCode_.Append("Target.Window()");
        }

        internal protected CheckSettings(Rectangle region)
        {
            targetRegion_ = region;
            fluentCode_.Append($"Target.Region(new Rectangle({region.X},{region.Y},{region.Width},{region.Height}))");
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <param name="timeout"></param>
        public CheckSettings(int timeout)
        {
            timeout_ = timeout;
        }

        #region ICheckSettingsInternal

        Rectangle? Fluent.ICheckSettingsInternal.GetTargetRegion()
        {
            return targetRegion_;
        }

        int Fluent.ICheckSettingsInternal.GetTimeout()
        {
            return timeout_;
        }

        int? Fluent.ICheckSettingsInternal.GetWaitBeforeCapture()
        {
            return waitBeforeCapture_;
        }

        bool? Fluent.ICheckSettingsInternal.GetStitchContent()
        {
            return stitchContent_;
        }

        MatchLevel? Fluent.ICheckSettingsInternal.GetMatchLevel()
        {
            return matchLevel_;
        }

        IGetRegions[] Fluent.ICheckSettingsInternal.GetIgnoreRegions()
        {
            return ignoreRegions_.ToArray();
        }

        IGetRegions[] Fluent.ICheckSettingsInternal.GetStrictRegions()
        {
            return strictRegions_.ToArray();
        }

        IGetRegions[] Fluent.ICheckSettingsInternal.GetContentRegions()
        {
            return contentRegions_.ToArray();
        }

        IGetRegions[] Fluent.ICheckSettingsInternal.GetLayoutRegions()
        {
            return layoutRegions_.ToArray();
        }

        IGetFloatingRegion[] Fluent.ICheckSettingsInternal.GetFloatingRegions()
        {
            return floatingRegions_.ToArray();
        }

        IGetAccessibilityRegion[] ICheckSettingsInternal.GetAccessibilityRegions()
        {
            return accessibilityRegions_.ToArray();
        }

        LazyLoadOptions ICheckSettingsInternal.GetLazyLoad()
        {
            return lazyLoad_;
        }

        bool? Fluent.ICheckSettingsInternal.GetIgnoreCaret()
        {
            return ignoreCaret_;
        }

        string ICheckSettingsInternal.GetVariationGroupId()
        {
            return variationGroupId_;
        }

        LayoutBreakpointsOptions ICheckSettingsInternal.GetLayoutBreakpointsOptions()
        {
            return LayoutBreakpointsOptions;
        }

        #endregion

        protected void Floating_(IGetFloatingRegion floatingRegionProvider)
        {
            floatingRegions_.Add(floatingRegionProvider);
        }

        protected void Floating_(Rectangle rect, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            Floating_(new FloatingRegionByRectangle(rect, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
        }

        protected void Accessibility_(IGetAccessibilityRegion accessibilityRegionProvider)
        {
            accessibilityRegions_.Add(accessibilityRegionProvider);
        }

        protected void Accessibility_(Rectangle rect, AccessibilityRegionType regionType)
        {
            Accessibility_(new AccessibilityRegionByRectangle(rect, regionType));
        }

        protected void Ignore_(IGetRegions regionProvider)
        {
            ignoreRegions_.Add(regionProvider);
        }

        protected void Content_(IGetRegions regionProvider)
        {
            contentRegions_.Add(regionProvider);
        }

        protected void Layout_(IGetRegions regionProvider)
        {
            layoutRegions_.Add(regionProvider);
        }

        protected void Strict_(IGetRegions regionProvider)
        {
            strictRegions_.Add(regionProvider);
        }

        protected StringBuilder fluentCode_ = new StringBuilder();
        private DensityMetrics densityMetrics_;

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Exact"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Exact()
        {
            var clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Exact;
            clone.fluentCode_.Append($".{nameof(Exact)}()");
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Layout"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout()
        {
            var clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Layout;
            clone.fluentCode_.Append($".{nameof(Layout)}()");
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Strict"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict()
        {
            var clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Strict;
            clone.fluentCode_.Append($".{nameof(Strict)}()");
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Content"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        [Obsolete("Use IgnoreColors()")]
        public ICheckSettings Content()
        {
            var clone = Clone();
#pragma warning disable CS0618
            clone.matchLevel_ = Applitools.MatchLevel.Content;
#pragma warning restore CS0618
            clone.fluentCode_.Append($".{nameof(Content)}()");
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.IgnoreColors"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings IgnoreColors()
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.IgnoreColors;
            clone.fluentCode_.Append($".{nameof(IgnoreColors)}()");
            return clone;
        }

        /// <summary>
        /// Set the match level by which to compare the screenshot.
        /// </summary>
        /// <param name="matchLevel">The match level to use.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings MatchLevel(MatchLevel matchLevel)
        {
            var clone = Clone();
            clone.matchLevel_ = matchLevel;
            clone.fluentCode_.Append($".{nameof(MatchLevel)}({nameof(MatchLevel)}.{matchLevel})");
            return clone;
        }


        #region Floating

        /// <summary>
        /// Adds a floating region.
        /// </summary>
        /// <param name="maxOffset">How much each of the content rectangles can move in any direction.</param>
        /// <param name="regions">One or more content rectangles.</param>
        /// <remarks>A floating region is a a region that can be placed within the boundaries of a bigger region.</remarks>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Floating(int maxOffset, params Rectangle[] regions)
        {
            var clone = Clone();
            clone.fluentCode_.Append($"{nameof(Floating)}({maxOffset}");
            foreach (Rectangle region in regions)
            {
                clone.Floating_(region, maxOffset, maxOffset, maxOffset, maxOffset);
                clone.fluentCode_.Append(
                    $", new {nameof(Rectangle)}({region.X},{region.Y},{region.Width},{region.Height})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds a floating region.
        /// </summary>
        /// <param name="region">The content rectangle.</param>
        /// <param name="maxUpOffset">How much the content can move up.</param>
        /// <param name="maxDownOffset">How much the content can move down.</param>
        /// <param name="maxLeftOffset">How much the content can move to the left.</param>
        /// <param name="maxRightOffset">How much the content can move to the right.</param>
        /// <remarks>A floating region is a a region that can be placed within the boundaries of a bigger region.</remarks>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Floating(Rectangle region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            var clone = Clone();
            clone.Floating_(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
            clone.fluentCode_.Append(
                $"{nameof(Floating)}(new {nameof(Rectangle)}({region.X},{region.Y},{region.Width},{region.Height}),{maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset})");
            return clone;
        }

        #endregion

        #region Accessibility

        public ICheckSettings Accessibility(AccessibilityRegionByRectangle region)
        {
            var clone = Clone();
            clone.Accessibility_(region);
            clone.fluentCode_.Append(
                $"{nameof(Accessibility)}(new {nameof(AccessibilityRegionByRectangle)}({region.Left},{region.Top},{region.Width},{region.Height},{nameof(AccessibilityRegionType)}.{region.Type}))");
            return clone;
        }

        public ICheckSettings Accessibility(Rectangle region, AccessibilityRegionType regionType)
        {
            var clone = Clone();
            clone.Accessibility_(region, regionType);
            clone.fluentCode_.Append(
                $"{nameof(Accessibility)}(new {nameof(Rectangle)}({region.X},{region.Y},{region.Width},{region.Height},{nameof(AccessibilityRegionType)}.{regionType}))");
            return clone;
        }

        #endregion

        /// <summary>
        /// Defines that the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Fully()
        {
            var clone = Clone();
            clone.stitchContent_ = true;
            clone.fluentCode_.Append($".{nameof(Fully)}()");
            return clone;
        }

        /// <summary>
        /// Defines whether the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <param name="fully">Set the value of whether to take a full screenshot or not.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Fully(bool fully)
        {
            var clone = Clone();
            clone.stitchContent_ = fully;
            clone.fluentCode_.Append($".{nameof(Fully)}({fully})");
            return clone;
        }

        public ICheckSettings LazyLoad(LazyLoadOptions lazyLoad)
        {
            var clone = Clone();
            clone.lazyLoad_ = lazyLoad;
            clone.fluentCode_.Append(
                $"Target.LazyLoad(new LazyLoad({lazyLoad.ScrollLength},{lazyLoad.WaitingTime},{lazyLoad.MaxAmountToScroll}))");
            return clone;
        }

        public ICheckSettings LazyLoad()
        {
            var clone = Clone();
            var lazyLoad = new LazyLoadOptions(300, 2000, 15000);
            clone.lazyLoad_ = lazyLoad;
            clone.fluentCode_.Append(
                $"Target.LazyLoad(new LazyLoad({lazyLoad.ScrollLength},{lazyLoad.WaitingTime},{lazyLoad.MaxAmountToScroll}))");
            return clone;
        }

        public ICheckSettings PageId(string pageId)
        {
            var clone = Clone();
            clone.pageId_ = pageId;
            return clone;
        }

        string ICheckSettingsInternal.GetPageId()
        {
            return pageId_;
        }

        /// <summary>
        /// Adds one ignore region.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(Rectangle region, Padding padding = null, string regionId = null)
        {
            var list = new List<Rectangle> { region };
            return Ignore(list, padding, regionId);
        }

        public ICheckSettings Ignore(Region region, params Region[] regions)
        {
            var list = new List<Rectangle> { region.ToRectangle() };
            list.AddRange(regions.Select(r => r.ToRectangle()));
            return Ignore(list);
        }

        /// <summary>
        /// Adds one ignore region.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(Region region, Padding padding = null, string regionId = null)
        {
            var list = new List<Rectangle> { region.ToRectangle() };
            return Ignore(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="regions">One or more regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(Rectangle region, params Rectangle[] regions)
        {
            var list = new List<Rectangle> { region };
            list.AddRange(regions);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="regions">An enumerable of regions to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(IEnumerable<Rectangle> regions, Padding padding = null, string regionId = null)
        {
            var clone = Clone();
            clone.fluentCode_.Append($".{nameof(Ignore)}(");
            foreach (Rectangle r in regions)
            {
                clone.Ignore_(new SimpleRegionByRectangle(r, padding, regionId));
                clone.fluentCode_.Append($", new Rectangle({r.X},{r.Y},{r.Width},{r.Height})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one content region.
        /// </summary>
        /// <param name="region">A region to match using the Content method.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content(Rectangle region, Padding padding = null, string regionId = null)
        {
            var list = new List<Rectangle> { region };
            return Content(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="region">A region to match using the Content method.</param>
        /// <param name="regions">One or more regions to match using the Content method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content(Rectangle region, params Rectangle[] regions)
        {
            var list = new List<Rectangle> { region };
            list.AddRange(regions);
            return Content(list);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="regions">An enumerable of regions to match using the Content method.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content(IEnumerable<Rectangle> regions, Padding padding = null, string regionId = null)
        {
            var clone = Clone();
            clone.fluentCode_.Append($".{nameof(Content)}(");
            foreach (Rectangle r in regions)
            {
                clone.Content_(new SimpleRegionByRectangle(r, padding, regionId));
                clone.fluentCode_.Append($", new Rectangle({r.X},{r.Y},{r.Width},{r.Height})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one layout region.
        /// </summary>
        /// <param name="region">A region to match using the Layout method.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout(Rectangle region, Padding padding = null, string regionId = null)
        {
            var list = new List<Rectangle> { region };
            return Layout(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="region">A region to match using the Layout method.</param>
        /// <param name="regions">One or more regions to match using the Layout method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout(Rectangle region, params Rectangle[] regions)
        {
            var list = new List<Rectangle> { region };
            list.AddRange(regions);
            return Layout(list);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="regions">An enumerable of regions to match using the Layout method.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout(IEnumerable<Rectangle> regions, Padding padding = null, string regionId = null)
        {
            var clone = Clone();
            clone.fluentCode_.Append($".{nameof(Layout)}(");
            foreach (Rectangle r in regions)
            {
                clone.Layout_(new SimpleRegionByRectangle(r, padding, regionId));
                clone.fluentCode_.Append($", new Rectangle({r.X},{r.Y},{r.Width},{r.Height})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one strict region.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict(Rectangle region, Padding padding = null, string regionId = null)
        {
            var list = new List<Rectangle> { region };
            return Strict(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="region">A region to match using the Strict method.</param>
        /// <param name="regions">One or more regions to match using the Strict method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict(Rectangle region, params Rectangle[] regions)
        {
            var list = new List<Rectangle> { region };
            list.AddRange(regions);
            return Strict(list);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="regions">An enumerable of regions to match using the Strict method.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict(IEnumerable<Rectangle> regions, Padding padding = null, string regionId = null)
        {
            var clone = Clone();
            clone.fluentCode_.Append($".{nameof(Strict)}(");
            foreach (Rectangle r in regions)
            {
                clone.Strict_(new SimpleRegionByRectangle(r, padding, regionId));
                clone.fluentCode_.Append($", new Rectangle({r.X},{r.Y},{r.Width},{r.Height})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <inheritdoc />
        public ICheckSettings Timeout(TimeSpan timeout)
        {
            var clone = Clone();
            clone.timeout_ = (int)timeout.TotalMilliseconds;
            clone.fluentCode_.Append($".{nameof(Timeout)}({timeout.TotalMilliseconds})");
            return clone;
        }

        /// <inheritdoc />
        public ICheckSettings WaitBeforeCapture(TimeSpan waitTime)
        {
            var clone = Clone();
            clone.waitBeforeCapture_ = (int)waitTime.TotalMilliseconds;
            clone.fluentCode_.Append($".{nameof(WaitBeforeCapture)}({waitTime.TotalMilliseconds})");
            return clone;
        }

        /// <inheritdoc />
        public ICheckSettings IgnoreCaret(bool ignoreCaret = true)
        {
            var clone = Clone();
            clone.ignoreCaret_ = ignoreCaret;
            clone.fluentCode_.Append($".{nameof(IgnoreCaret)}({ignoreCaret})");
            return clone;
        }

        /// <summary>
        /// Defines whether to send the document DOM or not.
        /// </summary>
        /// <param name="sendDom">When <c>true</c> sends the DOM to the server (the default).</param>
        /// <returns>An updated clone of this settings object.</returns> 
        public ICheckSettings SendDom(bool sendDom = true)
        {
            var clone = Clone();
            clone.sendDom_ = sendDom;
            clone.fluentCode_.Append($".{nameof(SendDom)}({sendDom})");
            return clone;
        }

        /// <summary>
        /// A setter for the checkpoint name.
        /// </summary>
        /// <param name="name">A name by which to identify the checkpoint.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings WithName(string name)
        {
            var clone = Clone();
            clone.name_ = name;
            clone.fluentCode_.Append($".{nameof(WithName)}(\"{name}\")");
            return clone;
        }

        public ICheckSettings UseDom(bool useDom = true)
        {
            var clone = Clone();
            clone.useDom_ = useDom;
            clone.fluentCode_.Append($".{nameof(UseDom)}({useDom})");
            return clone;
        }

        public ICheckSettings ReplaceLast(bool replaceLast = true)
        {
            var clone = Clone();
            clone.replaceLast_ = replaceLast;
            clone.fluentCode_.Append($".{nameof(ReplaceLast)}({replaceLast})");
            return clone;
        }

        public ICheckSettings EnablePatterns(bool enablePatterns = true)
        {
            var clone = Clone();
            clone.enablePatterns_ = enablePatterns;
            clone.fluentCode_.Append($".{nameof(EnablePatterns)}({enablePatterns})");
            return clone;
        }

        public ICheckSettings IgnoreDisplacements(bool ignoreDisplacements = true)
        {
            var clone = Clone();
            clone.ignoreDisplacements_ = ignoreDisplacements;
            clone.fluentCode_.Append($".{nameof(IgnoreDisplacements)}({ignoreDisplacements})");
            return clone;
        }

        [Obsolete("Use " + nameof(BeforeRenderScreenshotHook) + " instead.")]
        public ICheckSettings ScriptHook(string hook)
        {
            var clone = Clone();
            clone.scriptHooks_.Add(BEFORE_CAPTURE_SCREENSHOT, hook);
            clone.fluentCode_.Append($".{nameof(ScriptHook)}({hook})");
            return clone;
        }

        public ICheckSettings BeforeRenderScreenshotHook(string hook)
        {
            var clone = Clone();
            clone.scriptHooks_.Add(BEFORE_CAPTURE_SCREENSHOT, hook);
            clone.fluentCode_.Append($".{nameof(BeforeRenderScreenshotHook)}({hook})");
            return clone;
        }

        public ICheckSettings VisualGridOptions(params VisualGridOption[] options)
        {
            var clone = Clone();
            if (options == null) return clone;
            clone.visualGridOptions_ = (VisualGridOption[])options.Clone();
            clone.fluentCode_.Append($".{nameof(VisualGridOptions)}(");
            foreach (VisualGridOption option in options)
            {
                clone.fluentCode_.Append($"new {nameof(VisualGridOption)}(\"{option.Key}\", {option.Value})");
            }

            clone.fluentCode_.Append(")");
            return clone;
        }

        public ICheckSettings DensityMetrics(int xDpi, int yDpi, double? scaleRatio = null)
        {
            var clone = Clone();
            clone.densityMetrics_ = new DensityMetrics { XDpi = xDpi, YDpi = yDpi, ScaleRatio = scaleRatio };
            return clone;
        }

        protected void UpdateTargetRegion(Rectangle region)
        {
            targetRegion_ = region;
        }

        protected void PopulateClone_(CheckSettings clone)
        {
            clone.targetRegion_ = targetRegion_;
            clone.matchLevel_ = matchLevel_;
            clone.stitchContent_ = stitchContent_;
            clone.timeout_ = timeout_;
            clone.ignoreCaret_ = ignoreCaret_;
            clone.sendDom_ = sendDom_;
            clone.name_ = name_;
            clone.useDom_ = useDom_;
            clone.replaceLast_ = replaceLast_;
            clone.enablePatterns_ = enablePatterns_;
            clone.ignoreDisplacements_ = ignoreDisplacements_;
            clone.waitBeforeCapture_ = waitBeforeCapture_;

            clone.ignoreRegions_.AddRange(ignoreRegions_);
            clone.contentRegions_.AddRange(contentRegions_);
            clone.layoutRegions_.AddRange(layoutRegions_);
            clone.strictRegions_.AddRange(strictRegions_);
            clone.floatingRegions_.AddRange(floatingRegions_);

            clone.accessibilityRegions_.AddRange(accessibilityRegions_);

            clone.visualGridOptions_ = (VisualGridOption[])visualGridOptions_?.Clone();
            if (LayoutBreakpointsOptions != null)
            {
                clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions(LayoutBreakpointsOptions);
            }

            foreach (var kvp in scriptHooks_)
            {
                clone.scriptHooks_.Add(kvp.Key, kvp.Value);
            }

            clone.fluentCode_ = fluentCode_;
            clone.variationGroupId_ = variationGroupId_;
            clone.pageId_ = pageId_;
        }

        protected virtual CheckSettings Clone()
        {
            var clone = new CheckSettings();
            PopulateClone_(clone);
            return clone;
        }

        ICheckSettings ICheckSettings.Clone()
        {
            return Clone();
        }

        string ICheckSettingsInternal.GetName()
        {
            return name_;
        }

        bool? ICheckSettingsInternal.GetSendDom()
        {
            return sendDom_;
        }

        protected void SetStitchContent(bool stitchContent)
        {
            stitchContent_ = stitchContent;
        }

        bool? ICheckSettingsInternal.GetUseDom()
        {
            return useDom_;
        }

        bool ICheckSettingsInternal.GetReplaceLast()
        {
            return replaceLast_;
        }

        bool ICheckSettingsInternal.IsCheckWindow()
        {
            return targetRegion_ == null && GetTargetSelector() == null;
        }

        bool? ICheckSettingsInternal.GetEnablePatterns()
        {
            return enablePatterns_;
        }

        bool? ICheckSettingsInternal.GetIgnoreDisplacements()
        {
            return ignoreDisplacements_;
        }

        IDictionary<string, string> ICheckSettingsInternal.GetScriptHooks()
        {
            return scriptHooks_;
        }

        VisualGridOption[] ICheckSettingsInternal.GetVisualGridOptions()
        {
            return visualGridOptions_;
        }

        SizeMode ICheckSettingsInternal.GetSizeMode()
        {
            ICheckSettingsInternal checkSettingsInternal = this;
            bool stitchContent = checkSettingsInternal.GetStitchContent() ?? false;
            Rectangle? region = checkSettingsInternal.GetTargetRegion();
            if (region == null && GetTargetSelector() == null)
            {
                return stitchContent ? SizeMode.FullPage : SizeMode.Viewport;
            }

            if (region != null)
            {
                return SizeMode.Region;
            }

            /* if (selector != null) */
            {
                return stitchContent ? SizeMode.FullSelector : SizeMode.Selector;
            }
        }

        public ICheckSettings VariationGroupId(string variationGroupId)
        {
            var clone = Clone();
            clone.variationGroupId_ = variationGroupId;
            return clone;
        }

        public virtual VisualGridSelector GetTargetSelector()
        {
            return null;
        }

        string ICheckSettingsInternal.GetFluentCommandString()
        {
            return fluentCode_.ToString();
        }

        DensityMetrics ICheckSettingsInternal.GetDensityMetrics()
        {
            return densityMetrics_;
        }
    }
}
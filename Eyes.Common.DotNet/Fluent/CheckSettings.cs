using Applitools.VisualGrid;
using Applitools.Fluent;
using System;
using System.Collections.Generic;
using System.Drawing;

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
        private List<IGetFloatingRegion> floatingRegions_ = new List<IGetFloatingRegion>();
        private List<IGetAccessibilityRegion> accessibilityRegions_ = new List<IGetAccessibilityRegion>();
        private int timeout_ = -1;
        private bool? stitchContent_;
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

        private static readonly string BEFORE_CAPTURE_SCREENSHOT = "beforeCaptureScreenshot";

        /// <summary>
        /// 
        /// </summary>
        internal protected CheckSettings() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        internal protected CheckSettings(Rectangle region)
        {
            targetRegion_ = region;
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

        Rectangle? Fluent.ICheckSettingsInternal.GetTargetRegion() { return targetRegion_; }

        int Fluent.ICheckSettingsInternal.GetTimeout() { return timeout_; }

        bool? Fluent.ICheckSettingsInternal.GetStitchContent() { return stitchContent_; }

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

        bool? Fluent.ICheckSettingsInternal.GetIgnoreCaret() { return ignoreCaret_; }

        #endregion

        protected void Floating_(IGetFloatingRegion floatingRegionProvider)
        {
            floatingRegions_.Add(floatingRegionProvider);
        }

        protected void Floating_(Rectangle rect, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
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

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Exact"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Exact()
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Exact;
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Layout"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout()
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Layout;
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Strict"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict()
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Strict;
            return clone;
        }

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Content"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content()
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = Applitools.MatchLevel.Content;
            return clone;
        }

        /// <summary>
        /// Set the match level by which to compare the screenshot.
        /// </summary>
        /// <param name="matchLevel">The match level to use.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings MatchLevel(MatchLevel matchLevel)
        {
            CheckSettings clone = Clone();
            clone.matchLevel_ = matchLevel;
            return clone;
        }


        #region Floating

        /// <summary>
        /// Adds a floating region.
        /// </summary>
        /// <param name="maxOffset">How much each of the content rectangles can move in any direction.</param>
        /// <param name="regions">One or more content rectangles.</param>
        /// <remarks>A floating region is a a region that can be placed within the boundries of a bigger region.</remarks>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Floating(int maxOffset, params Rectangle[] regions)
        {
            CheckSettings clone = Clone();
            foreach (Rectangle region in regions)
            {
                clone.Floating_(region, maxOffset, maxOffset, maxOffset, maxOffset);
            }
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
        /// <remarks>A floating region is a a region that can be placed within the boundries of a bigger region.</remarks>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Floating(Rectangle region, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            CheckSettings clone = Clone();
            clone.Floating_(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
            return clone;
        }

        #endregion

        #region Accessibility

        public ICheckSettings Accessibility(AccessibilityRegionByRectangle region)
        {
            CheckSettings clone = Clone();
            clone.Accessibility_(region);
            return clone;
        }

        public ICheckSettings Accessibility(Rectangle region, AccessibilityRegionType regionType)
        {
            CheckSettings clone = Clone();
            clone.Accessibility_(region, regionType);
            return clone;
        }

        #endregion

        /// <summary>
        /// Defines that the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Fully()
        {
            CheckSettings clone = Clone();
            clone.stitchContent_ = true;
            return clone;
        }

        /// <summary>
        /// Defines whether the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <param name="fully">Set the value of whether to take a full screenshot or not.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Fully(bool fully)
        {
            CheckSettings clone = Clone();
            clone.stitchContent_ = fully;
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="regions">One or more regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(Rectangle region, params Rectangle[] regions)
        {
            CheckSettings clone = Clone();
            clone.Ignore_(new SimpleRegionByRectangle(region));
            foreach (Rectangle r in regions)
            {
                clone.Ignore_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Ignore(IEnumerable<Rectangle> regions)
        {
            CheckSettings clone = Clone();
            foreach (Rectangle r in regions)
            {
                clone.Ignore_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="region">A region to match using the Content method.</param>
        /// <param name="regions">One or more regions to match using the Content method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content(Rectangle region, params Rectangle[] regions)
        {
            CheckSettings clone = Clone();
            clone.Content_(new SimpleRegionByRectangle(region));
            foreach (Rectangle r in regions)
            {
                clone.Content_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Content method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Content(IEnumerable<Rectangle> regions)
        {
            CheckSettings clone = Clone();
            foreach (Rectangle r in regions)
            {
                clone.Content_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="region">A region to match using the Layout method.</param>
        /// <param name="regions">One or more regions to match using the Layout method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout(Rectangle region, params Rectangle[] regions)
        {
            CheckSettings clone = Clone();
            clone.Layout_(new SimpleRegionByRectangle(region));
            foreach (Rectangle r in regions)
            {
                clone.Layout_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Layout method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Layout(IEnumerable<Rectangle> regions)
        {
            CheckSettings clone = Clone();
            foreach (Rectangle r in regions)
            {
                clone.Layout_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="region">A region to match using the Strict method.</param>
        /// <param name="regions">One or more regions to match using the Strict method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict(Rectangle region, params Rectangle[] regions)
        {
            CheckSettings clone = Clone();
            clone.Strict_(new SimpleRegionByRectangle(region));
            foreach (Rectangle r in regions)
            {
                clone.Strict_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Strict method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Strict(IEnumerable<Rectangle> regions)
        {
            CheckSettings clone = Clone();
            foreach (Rectangle r in regions)
            {
                clone.Strict_(new SimpleRegionByRectangle(r));
            }
            return clone;
        }

        /// <summary>
        /// Defines the timeout to use when aquiring and comparing screenshots.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings Timeout(TimeSpan timeout)
        {
            CheckSettings clone = Clone();
            clone.timeout_ = (int)timeout.TotalMilliseconds;
            return clone;
        }

        /// <summary>
        /// Defines if to detect and ignore a blinking caret in the screenshot. Defaults to <c>true</c>.
        /// </summary>
        /// <param name="ignoreCaret">Whether or not to detect and ignore a blinking caret in the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings IgnoreCaret(bool ignoreCaret = true)
        {
            CheckSettings clone = Clone();
            clone.ignoreCaret_ = ignoreCaret;
            return clone;
        }

        /// <summary>
        /// Defines whether to send the document DOM or not.
        /// </summary>
        /// <param name="sendDom">When <c>true</c> sends the DOM to the server (the default).</param>
        /// <returns>An updated clone of this settings object.</returns> 
        public ICheckSettings SendDom(bool sendDom = true)
        {
            CheckSettings clone = Clone();
            clone.sendDom_ = sendDom;
            return clone;
        }

        /// <summary>
        /// A setter for the checkpoint name.
        /// </summary>
        /// <param name="name">A name by which to identify the checkpoint.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public ICheckSettings WithName(string name)
        {
            CheckSettings clone = Clone();
            clone.name_ = name;
            return clone;
        }

        public ICheckSettings UseDom(bool useDom = true)
        {
            CheckSettings clone = Clone();
            clone.useDom_ = useDom;
            return clone;
        }
        public ICheckSettings ReplaceLast(bool replaceLast = true)
        {
            CheckSettings clone = Clone();
            clone.replaceLast_ = replaceLast;
            return clone;
        }

        public ICheckSettings EnablePatterns(bool enablePatterns = true)
        {
            CheckSettings clone = Clone();
            clone.enablePatterns_ = enablePatterns;
            return clone;
        }

        public ICheckSettings IgnoreDisplacements(bool ignoreDisplacements = true)
        {
            CheckSettings clone = Clone();
            clone.ignoreDisplacements_ = ignoreDisplacements;
            return clone;
        }

        [Obsolete("Use " + nameof(BeforeRenderScreenshotHook) + " instead.")]
        public ICheckSettings ScriptHook(string hook)
        {
            CheckSettings clone = Clone();
            clone.scriptHooks_.Add(BEFORE_CAPTURE_SCREENSHOT, hook);
            return clone;
        }

        public ICheckSettings BeforeRenderScreenshotHook(string hook)
        {
            CheckSettings clone = Clone();
            clone.scriptHooks_.Add(BEFORE_CAPTURE_SCREENSHOT, hook);
            return clone;
        }

        public ICheckSettings VisualGridOptions(params VisualGridOption[] options)
        {
            CheckSettings clone = Clone();
            clone.visualGridOptions_ = (VisualGridOption[])options?.Clone();
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

            clone.ignoreRegions_.AddRange(ignoreRegions_);
            clone.contentRegions_.AddRange(contentRegions_);
            clone.layoutRegions_.AddRange(layoutRegions_);
            clone.strictRegions_.AddRange(strictRegions_);
            clone.floatingRegions_.AddRange(floatingRegions_);

            clone.accessibilityRegions_.AddRange(accessibilityRegions_);

            clone.visualGridOptions_ = (VisualGridOption[])visualGridOptions_?.Clone();

            foreach (KeyValuePair<string, string> kvp in scriptHooks_)
            {
                clone.scriptHooks_.Add(kvp.Key, kvp.Value);
            }
        }

        protected virtual CheckSettings Clone()
        {
            CheckSettings clone = new CheckSettings();
            PopulateClone_(clone);
            return clone;
        }

        ICheckSettings ICheckSettings.Clone() { return Clone(); }

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
            else if (region != null)
            {
                return SizeMode.Region;
            }
            else /* if (selector != null) */
            {
                return stitchContent ? SizeMode.FullSelector : SizeMode.Selector;
            }
        }


        public virtual VisualGridSelector GetTargetSelector()
        {
            return null;
        }
    }
}

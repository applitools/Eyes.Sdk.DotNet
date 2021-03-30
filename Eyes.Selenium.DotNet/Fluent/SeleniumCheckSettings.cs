using Applitools.Utils;
using Applitools.VisualGrid;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Selenium.Fluent
{
    public class SeleniumCheckSettings : CheckSettings, ISeleniumCheckTarget
    {
        private By targetSelector_;
        private IWebElement targetElement_;
        private readonly List<FrameLocator> frameChain_ = new List<FrameLocator>();
        private By scrollRootSelector_;
        private IWebElement scrollRootElement_;
        private VisualGridSelector vgTargetSelector_ = null;
        private bool layoutBreakpointsEnabled_;
        private bool? useCookies_;
        private readonly List<int> layoutBreakpoints_ = new List<int>();

        internal SeleniumCheckSettings()
        {
        }

        internal SeleniumCheckSettings(By targetSelector)
        {
            targetSelector_ = targetSelector;
            fluentCode_.Append($"Target.Region({targetSelector})");
        }

        internal SeleniumCheckSettings(IWebElement targetElement)
        {
            targetElement_ = targetElement;
            fluentCode_.Append($"Target.Region({targetElement})");
        }

        internal SeleniumCheckSettings(Rectangle region)
            : base(region)
        {
        }

        By ISeleniumCheckTarget.GetTargetSelector()
        {
            return targetSelector_;
        }

        IWebElement ISeleniumCheckTarget.GetTargetElement()
        {
            return targetElement_;
        }

        IList<FrameLocator> ISeleniumCheckTarget.GetFrameChain()
        {
            return frameChain_;
        }

        CheckState ISeleniumCheckTarget.State { get; set; }

        By IScrollRootElementContainer.GetScrollRootSelector()
        {
            return scrollRootSelector_;
        }

        IWebElement IScrollRootElementContainer.GetScrollRootElement()
        {
            return scrollRootElement_;
        }

        public SeleniumCheckSettings Frame(By by)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameSelector = by });
            clone.fluentCode_.Append($".Frame({by})");
            return clone;
        }

        public SeleniumCheckSettings Frame(string frameNameOrId)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameNameOrId = frameNameOrId });
            clone.fluentCode_.Append($".Frame(\"{frameNameOrId}\")");
            return clone;
        }

        public SeleniumCheckSettings Frame(int index)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameIndex = index });
            clone.fluentCode_.Append($".Frame({index})");
            return clone;
        }

        public SeleniumCheckSettings Frame(IWebElement frameReference)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameReference = frameReference });
            clone.fluentCode_.Append($".Frame({frameReference})");
            return clone;
        }

        internal void SanitizeSettings(Logger logger, IWebDriver driver, CheckState state)
        {
            bool isFully = state.StitchContent;
            if (frameChain_.Count > 0 && targetElement_ == null && targetSelector_ == null && !isFully/* &&
                ((Applitools.Fluent.ICheckSettingsInternal)this).GetTargetRegion() == null*/)
            {
                FrameLocator lastFrame = frameChain_[frameChain_.Count - 1];
                frameChain_.RemoveAt(frameChain_.Count - 1);
                targetElement_ = EyesSeleniumUtils.FindFrameByFrameCheckTarget(lastFrame, driver);
                state.FrameToSwitchTo = targetElement_;
                logger.Log(TraceLevel.Notice, Stage.Check,
                    new { message = "Using Target.Frame() for the purpose of Target.Region()" });
            }
        }

        public SeleniumCheckSettings Region(Rectangle rect)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(rect);
            clone.fluentCode_.Append($".Region(new Rectangle({rect.X},{rect.Y},{rect.Width},{rect.Height}))");
            return clone;
        }

        public SeleniumCheckSettings Region(By by)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.targetSelector_ = by;
            clone.fluentCode_.Append($".Region({by})");
            return clone;
        }

        public SeleniumCheckSettings Region(IWebElement targetElement)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.targetElement_ = targetElement;
            clone.fluentCode_.Append($".Region({targetElement})");
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selector">A selector representing a region to ignore when validating the screenshot.</param>
        /// <param name="selectors">One or more selectors representing regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Ignore(By selector, params By[] selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Ignore_(new SimpleRegionBySelector(selector));
            clone.fluentCode_.Append($".{nameof(Ignore)}({selector}");
            foreach (By sel in selectors)
            {
                clone.Ignore_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Ignore(IEnumerable<By> selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Ignore)}(");
            foreach (By sel in selectors)
            {
                clone.Ignore_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="element">An element to ignore when validating the screenshot.</param>
        /// <param name="elements">One or more elements to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Ignore(IWebElement element, params IWebElement[] elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Ignore_(new SimpleRegionByElement(element));
            clone.fluentCode_.Append($".{nameof(Ignore)}({element}");
            foreach (IWebElement elem in elements)
            {
                clone.Ignore_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Ignore(IEnumerable<IWebElement> elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Ignore)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Ignore_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selector">A selector representing a layout region.</param>
        /// <param name="selectors">One or more selectors representing layout regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Layout(By selector, params By[] selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Layout_(new SimpleRegionBySelector(selector));
            clone.fluentCode_.Append($".{nameof(Layout)}({selector}");
            foreach (By sel in selectors)
            {
                clone.Layout_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing layout regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Layout(IEnumerable<By> selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Layout)}(");
            foreach (By sel in selectors)
            {
                clone.Layout_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="element">An element representing a layout region.</param>
        /// <param name="elements">One or more elements, each representing a layout region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Layout(IWebElement element, params IWebElement[] elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Layout_(new SimpleRegionByElement(element));
            clone.fluentCode_.Append($".{nameof(Layout)}({element}");
            foreach (IWebElement elem in elements)
            {
                clone.Layout_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a layout region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Layout(IEnumerable<IWebElement> elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Layout)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Layout_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selector">A selector representing a strict region.</param>
        /// <param name="selectors">One or more selectors representing strict regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Strict(By selector, params By[] selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Strict_(new SimpleRegionBySelector(selector));
            clone.fluentCode_.Append($".{nameof(Strict)}({selector}");
            foreach (By sel in selectors)
            {
                clone.Strict_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }
        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing strict regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Strict(IEnumerable<By> selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Strict)}(");
            foreach (By sel in selectors)
            {
                clone.Strict_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="element">An element representing a strict region.</param>
        /// <param name="elements">One or more elements, each representing a strict region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Strict(IWebElement element, params IWebElement[] elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Strict_(new SimpleRegionByElement(element));
            clone.fluentCode_.Append($".{nameof(Strict)}({element}");
            foreach (IWebElement elem in elements)
            {
                clone.Strict_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a strict region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Strict(IEnumerable<IWebElement> elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Strict)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Strict_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selector">A selector representing a content region.</param>
        /// <param name="selectors">One or more selectors representing content regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Content(By selector, params By[] selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Content_(new SimpleRegionBySelector(selector));
            clone.fluentCode_.Append($".{nameof(Content)}({selector}");
            foreach (By sel in selectors)
            {
                clone.Content_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing content regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Content(IEnumerable<By> selectors)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Content)}(");
            foreach (By sel in selectors)
            {
                clone.Content_(new SimpleRegionBySelector(sel));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="element">An element representing a content region.</param>
        /// <param name="elements">One or more elements, each representing a content region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Content(IWebElement element, params IWebElement[] elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Content_(new SimpleRegionByElement(element));
            clone.fluentCode_.Append($".{nameof(Content)}({element}");
            foreach (IWebElement elem in elements)
            {
                clone.Content_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a content region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public SeleniumCheckSettings Content(IEnumerable<IWebElement> elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Content)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Content_(new SimpleRegionByElement(elem));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        public SeleniumCheckSettings Floating(By regionSelector, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({regionSelector},{maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset})");
            return clone;
        }

        public SeleniumCheckSettings Floating(By regionSelector, int maxOffset = 0)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxOffset, maxOffset, maxOffset, maxOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({regionSelector},{maxOffset})");
            return clone;
        }

        public SeleniumCheckSettings Floating(IWebElement element, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({element},{maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset})");
            return clone;
        }

        public SeleniumCheckSettings Floating(IWebElement element, int maxOffset = 0)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxOffset, maxOffset, maxOffset, maxOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({element},{maxOffset})");
            return clone;
        }

        public SeleniumCheckSettings Floating(int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset, params IWebElement[] elementsToIgnore)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Floating)}({maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset}");
            foreach (IWebElement element in elementsToIgnore)
            {
                clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
                clone.fluentCode_.Append($", {element}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }



        public SeleniumCheckSettings Accessibility(By regionSelector, AccessibilityRegionType regionType)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionBySelector(regionSelector, regionType));
            clone.fluentCode_.Append($".{nameof(Accessibility)}({regionSelector},{nameof(AccessibilityRegionType)}.{regionType})");
            return clone;
        }


        public SeleniumCheckSettings Accessibility(IWebElement element, AccessibilityRegionType regionType)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
            clone.fluentCode_.Append($".{nameof(Accessibility)}({element},{nameof(AccessibilityRegionType)}.{regionType})");
            return clone;
        }

        public SeleniumCheckSettings Accessibility(AccessibilityRegionType regionType, params IWebElement[] elements)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Accessibility)}({nameof(AccessibilityRegionType)}.{regionType}");
            foreach (IWebElement element in elements)
            {
                clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
                clone.fluentCode_.Append($", {element}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }



        public SeleniumCheckSettings ScrollRootElement(By selector)
        {
            SeleniumCheckSettings clone = Clone_();
            if (frameChain_.Count == 0)
            {
                clone.scrollRootSelector_ = selector;
            }
            else
            {
                frameChain_[frameChain_.Count - 1].ScrollRootSelector = selector;
            }
            clone.fluentCode_.Append($".{nameof(ScrollRootElement)}({selector})");
            return clone;
        }
        public SeleniumCheckSettings ScrollRootElement(IWebElement element)
        {
            SeleniumCheckSettings clone = Clone_();
            if (frameChain_.Count == 0)
            {
                clone.scrollRootElement_ = element;
            }
            else
            {
                frameChain_[frameChain_.Count - 1].ScrollRootElement = element;
            }
            clone.fluentCode_.Append($".{nameof(ScrollRootElement)}({element})");
            return clone;
        }

        public SeleniumCheckSettings LayoutBreakpointsEnabled(bool shouldSet)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.layoutBreakpointsEnabled_ = shouldSet;
            clone.layoutBreakpoints_.Clear();
            return clone;
        }

        public SeleniumCheckSettings LayoutBreakpoints(params int[] breakpoints)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.layoutBreakpointsEnabled_ = false;
            clone.layoutBreakpoints_.Clear();
            if (breakpoints == null || breakpoints.Length == 0)
            {
                return clone;
            }

            foreach (int breakpoint in breakpoints)
            {
                ArgumentGuard.GreaterThan(breakpoint, 0, nameof(breakpoint));
                clone.layoutBreakpoints_.Add(breakpoint);
            }

            clone.layoutBreakpoints_.Sort();
            return clone;
        }

        List<int> ISeleniumCheckTarget.GetLayoutBreakpoints()
        {
            return layoutBreakpoints_;
        }

        bool ISeleniumCheckTarget.GetLayoutBreakpointsEnabled()
        {
            return layoutBreakpointsEnabled_;
        }


        public SeleniumCheckSettings UseCookies(bool useCookies)
        {
            SeleniumCheckSettings clone = Clone_();
            clone.useCookies_ = useCookies;
            return clone;
        }

        bool? ISeleniumCheckTarget.GetUseCookies()
        {
            return useCookies_;
        }
        
        #region overrides
        public new SeleniumCheckSettings Accessibility(AccessibilityRegionByRectangle region)
        {
            return (SeleniumCheckSettings)base.Accessibility(region);
        }

        public new SeleniumCheckSettings Accessibility(Rectangle region, AccessibilityRegionType regionType)
        {
            return (SeleniumCheckSettings)base.Accessibility(region, regionType);
        }
        public new SeleniumCheckSettings Ignore(Rectangle region, params Rectangle[] regions)
        {
            return (SeleniumCheckSettings)base.Ignore(region, regions);
        }

        public new SeleniumCheckSettings Ignore(IEnumerable<Rectangle> regions)
        {
            return (SeleniumCheckSettings)base.Ignore(regions);
        }

        public new SeleniumCheckSettings Content(Rectangle region, params Rectangle[] regions)
        {
            return (SeleniumCheckSettings)base.Content(region, regions);
        }

        public new SeleniumCheckSettings Content(IEnumerable<Rectangle> regions)
        {
            return (SeleniumCheckSettings)base.Content(regions);
        }

        public new SeleniumCheckSettings Layout(Rectangle region, params Rectangle[] regions)
        {
            return (SeleniumCheckSettings)base.Layout(region, regions);
        }

        public new SeleniumCheckSettings Layout(IEnumerable<Rectangle> regions)
        {
            return (SeleniumCheckSettings)base.Layout(regions);
        }

        public new SeleniumCheckSettings Strict(Rectangle region, params Rectangle[] regions)
        {
            return (SeleniumCheckSettings)base.Strict(region, regions);
        }

        public new SeleniumCheckSettings Strict(IEnumerable<Rectangle> regions)
        {
            return (SeleniumCheckSettings)base.Strict(regions);
        }

        public new SeleniumCheckSettings Floating(int maxOffset, params Rectangle[] regions)
        {
            return (SeleniumCheckSettings)base.Floating(maxOffset, regions);
        }

        public new SeleniumCheckSettings Floating(Rectangle region, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            return (SeleniumCheckSettings)base.Floating(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
        }

        public new SeleniumCheckSettings Exact()
        {
            return (SeleniumCheckSettings)base.Exact();
        }

        public new SeleniumCheckSettings Layout()
        {
            return (SeleniumCheckSettings)base.Layout();
        }

        public new SeleniumCheckSettings Strict()
        {
            return (SeleniumCheckSettings)base.Strict();
        }

        public new SeleniumCheckSettings Content()
        {
            return (SeleniumCheckSettings)base.Content();
        }

        public new SeleniumCheckSettings MatchLevel(MatchLevel matchLevel)
        {
            return (SeleniumCheckSettings)base.MatchLevel(matchLevel);
        }

        public new SeleniumCheckSettings Fully()
        {
            return (SeleniumCheckSettings)base.Fully();
        }

        public new SeleniumCheckSettings Fully(bool fully)
        {
            return (SeleniumCheckSettings)base.Fully(fully);
        }

        public new SeleniumCheckSettings Timeout(TimeSpan timeout)
        {
            return (SeleniumCheckSettings)base.Timeout(timeout);
        }

        public new SeleniumCheckSettings IgnoreCaret(bool ignoreCaret = true)
        {
            return (SeleniumCheckSettings)base.IgnoreCaret(ignoreCaret);
        }

        public new SeleniumCheckSettings SendDom(bool sendDom = true)
        {
            return (SeleniumCheckSettings)base.SendDom(sendDom);
        }

        public new SeleniumCheckSettings WithName(string name)
        {
            return (SeleniumCheckSettings)base.WithName(name);
        }

        public new SeleniumCheckSettings ReplaceLast(bool replaceLast = true)
        {
            return (SeleniumCheckSettings)base.ReplaceLast(replaceLast);
        }

        public new SeleniumCheckSettings UseDom(bool useDom = true)
        {
            return (SeleniumCheckSettings)base.UseDom(useDom);
        }

        public new SeleniumCheckSettings EnablePatterns(bool enablePatterns = true)
        {
            return (SeleniumCheckSettings)base.EnablePatterns(enablePatterns);
        }

        public new SeleniumCheckSettings IgnoreDisplacements(bool ignoreDisplacements = true)
        {
            return (SeleniumCheckSettings)base.IgnoreDisplacements(ignoreDisplacements);
        }

        public new SeleniumCheckSettings BeforeRenderScreenshotHook(string hook)
        {
            return (SeleniumCheckSettings)base.BeforeRenderScreenshotHook(hook);
        }

        [Obsolete("Use " + nameof(BeforeRenderScreenshotHook) + " instead.")]
        public new SeleniumCheckSettings ScriptHook(string hook)
        {
            return (SeleniumCheckSettings)base.ScriptHook(hook);
        }

        public new SeleniumCheckSettings VisualGridOptions(params VisualGridOption[] options)
        {
            return (SeleniumCheckSettings)base.VisualGridOptions(options);
        }
        #endregion

        private SeleniumCheckSettings Clone_()
        {
            return (SeleniumCheckSettings)Clone();
        }

        internal void SetTargetSelector(VisualGridSelector targetSelector)
        {
            vgTargetSelector_ = targetSelector;
        }

        public override VisualGridSelector GetTargetSelector()
        {
            return vgTargetSelector_;
        }

        protected override CheckSettings Clone()
        {
            SeleniumCheckSettings clone = new SeleniumCheckSettings();
            base.PopulateClone_(clone);
            clone.targetElement_ = targetElement_;
            clone.targetSelector_ = targetSelector_;
            clone.frameChain_.AddRange(frameChain_);
            clone.scrollRootElement_ = scrollRootElement_;
            clone.scrollRootSelector_ = scrollRootSelector_;
            clone.vgTargetSelector_ = vgTargetSelector_;
            ((ISeleniumCheckTarget)clone).State = ((ISeleniumCheckTarget)this).State;
            clone.layoutBreakpointsEnabled_ = layoutBreakpointsEnabled_;
            clone.layoutBreakpoints_.AddRange(layoutBreakpoints_);
            clone.useCookies_ = useCookies_;
            return clone;
        }

        public override Dictionary<string, object> ToSerializableDictionary()
        {
            Dictionary<string, object> dict = base.ToSerializableDictionary();
            dict.Add("TargetElement", targetElement_?.ToString());
            dict.Add("TargetSelector", targetSelector_?.ToString());
            dict.Add("FrameChain", StringUtils.Concat(frameChain_, " --> "));
            dict.Add("ScrollRootElement", scrollRootElement_?.ToString());
            dict.Add("ScrollRootSelector", scrollRootSelector_?.ToString());
            dict.Add("VGTargetSelector", vgTargetSelector_);
            dict.Add("LayoutBreakpointsEnabled", layoutBreakpointsEnabled_);
            dict.Add("LayoutBreakpoints", layoutBreakpoints_);
            dict.Add("UseCookies", useCookies_);

            return dict;
        }
    }
}

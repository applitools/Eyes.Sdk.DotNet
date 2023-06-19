using System;
using System.Collections.Generic;
using System.Drawing;
using Applitools.VisualGrid;
using OpenQA.Selenium;

namespace Applitools.Appium
{
    public class AppiumCheckSettings : CheckSettings, IAppiumCheckTarget
    {
        private By targetSelector_;
        private IWebElement targetElement_;
        private readonly List<FrameLocator> frameChain_ = new List<FrameLocator>();
        private By scrollRootSelector_;
        private IWebElement scrollRootElement_;
        private VisualGridSelector vgTargetSelector_;
        private bool? useCookies_;
        private bool? isDefaultWebview_;
        private string webview_;

        internal AppiumCheckSettings()
        {
        }

        internal AppiumCheckSettings(By targetSelector)
        {
            targetSelector_ = targetSelector;
            fluentCode_.Clear();
            fluentCode_.Append($"Target.Region({targetSelector})");
        }

        internal AppiumCheckSettings(IWebElement targetElement)
        {
            targetElement_ = targetElement;
            fluentCode_.Clear();
            fluentCode_.Append($"Target.Region({targetElement})");
        }

        internal AppiumCheckSettings(Rectangle region)
            : base(region)
        {
        }

        By ITargetContainer.GetTargetSelector()
        {
            return targetSelector_;
        }

        IWebElement ITargetContainer.GetTargetElement()
        {
            return targetElement_;
        }

        public AppiumCheckSettings Webview(bool? isDefaultWebview = true)
        {
            AppiumCheckSettings clone = Clone_();
            clone.isDefaultWebview_ = isDefaultWebview;
            return clone;
        }
        
        public AppiumCheckSettings Webview(string webview)
        {
            AppiumCheckSettings clone = Clone_();
            clone.webview_ = webview;
            return clone;
        }
        
        public AppiumCheckSettings Frame(By by)
        {
            AppiumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameSelector = by });
            clone.fluentCode_.Append($".Frame({by})");
            return clone;
        }

        public AppiumCheckSettings Frame(string frameNameOrId)
        {
            AppiumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameNameOrId = frameNameOrId });
            clone.fluentCode_.Append($".Frame(\"{frameNameOrId}\")");
            return clone;
        }

        public AppiumCheckSettings Frame(int index)
        {
            AppiumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameIndex = index });
            clone.fluentCode_.Append($".Frame({index})");
            return clone;
        }

        public AppiumCheckSettings Frame(IWebElement frameReference)
        {
            AppiumCheckSettings clone = Clone_();
            clone.frameChain_.Add(new FrameLocator() { FrameReference = frameReference });
            clone.fluentCode_.Append($".Frame({frameReference})");
            return clone;
        }
        
        public AppiumCheckSettings Region(Rectangle rect)
        {
            AppiumCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(rect);
            clone.fluentCode_.Append($".Region(new Rectangle({rect.X},{rect.Y},{rect.Width},{rect.Height}))");
            return clone;
        }

        public AppiumCheckSettings Region(By by)
        {
            AppiumCheckSettings clone = Clone_();
            clone.targetSelector_ = by;
            clone.fluentCode_.Append($".Region({by})");
            return clone;
        }

        public AppiumCheckSettings Region(IWebElement targetElement)
        {
            AppiumCheckSettings clone = Clone_();
            clone.targetElement_ = targetElement;
            clone.fluentCode_.Append($".Region({targetElement})");
            return clone;
        }

        /// <summary>
        /// Adds one ignore region.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(By region, Padding padding = null, string regionId = null)
        {
            var list = new List<By> { region };
            return Ignore(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selector">A selector representing a region to ignore when validating the screenshot.</param>
        /// <param name="selectors">One or more selectors representing regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(By selector, params By[] selectors)
        {
            var list = new List<By> { selector };
            list.AddRange(selectors);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selectors">An enumerable of selectors representing regions to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IEnumerable<By> selectors, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Ignore)}(");
            foreach (By sel in selectors)
            {
                clone.Ignore_(new SimpleRegionBySelector(sel, padding, regionId));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one ignore region.
        /// </summary>
        /// <param name="element">An element to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IWebElement element, Padding padding = null, string regionId = null)
        {
            var list = new List<IWebElement> { element };
            return Ignore(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="element">An element to ignore when validating the screenshot.</param>
        /// <param name="elements">One or more elements to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IWebElement element, params IWebElement[] elements)
        {
            var list = new List<IWebElement> { element };
            list.AddRange(elements);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="elements">An enumerable of elements to ignore when validating the screenshot.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IEnumerable<IWebElement> elements, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Ignore)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Ignore_(new SimpleRegionByElement(elem, padding, regionId));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one layout region.
        /// </summary>
        /// <param name="selector">A selector representing a layout region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(By selector, Padding padding = null, string regionId = null)
        {
            var list = new List<By> { selector };
            return Layout(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selector">A selector representing a layout region.</param>
        /// <param name="selectors">One or more selectors representing layout regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(By selector, params By[] selectors)
        {
            var list = new List<By> { selector };
            list.AddRange(selectors);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selectors">An enumerable of selectors representing layout regions.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IEnumerable<By> selectors, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Layout)}(");
            foreach (By sel in selectors)
            {
                clone.Layout_(new SimpleRegionBySelector(sel, padding, regionId));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one layout region.
        /// </summary>
        /// <param name="element">An element representing a layout region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IWebElement element, Padding padding = null, string regionId = null)
        {
            var list = new List<IWebElement> { element };
            return Layout(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="element">An element representing a layout region.</param>
        /// <param name="elements">One or more elements, each representing a layout region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IWebElement element, params IWebElement[] elements)
        {
            var list = new List<IWebElement> { element };
            list.AddRange(elements);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="elements">An enumerable of elements, each representing a layout region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IEnumerable<IWebElement> elements, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Layout)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Layout_(new SimpleRegionByElement(elem, padding, regionId));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one strict region.
        /// </summary>
        /// <param name="selector">A selector representing a strict region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(By selector, Padding padding = null, string regionId = null)
        {
            var list = new List<By> { selector };
            return Strict(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selector">A selector representing a strict region.</param>
        /// <param name="selectors">One or more selectors representing strict regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(By selector, params By[] selectors)
        {
            var list = new List<By> { selector };
            list.AddRange(selectors);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selectors">An enumerable of selectors representing strict regions.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IEnumerable<By> selectors, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Strict)}(");
            foreach (By sel in selectors)
            {
                clone.Strict_(new SimpleRegionBySelector(sel, padding, regionId));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one strict region.
        /// </summary>
        /// <param name="element">An element representing a strict region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IWebElement element, Padding padding = null, string regionId = null)
        {
            var list = new List<IWebElement> { element };
            return Strict(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="element">An element representing a strict region.</param>
        /// <param name="elements">One or more elements, each representing a strict region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IWebElement element, params IWebElement[] elements)
        {
            var list = new List<IWebElement> { element };
            list.AddRange(elements);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="elements">An enumerable of elements, each representing a strict region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IEnumerable<IWebElement> elements, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Strict)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Strict_(new SimpleRegionByElement(elem, padding, regionId));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one content region.
        /// </summary>
        /// <param name="selector">A selector representing a content region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(By selector, Padding padding = null, string regionId = null)
        {
            var list = new List<By> { selector };
            return Content(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selector">A selector representing a content region.</param>
        /// <param name="selectors">One or more selectors representing content regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(By selector, params By[] selectors)
        {
            var list = new List<By> { selector };
            list.AddRange(selectors);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selectors">An enumerable of selectors representing content regions.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IEnumerable<By> selectors, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Content)}(");
            foreach (By sel in selectors)
            {
                clone.Content_(new SimpleRegionBySelector(sel, padding, regionId));
                clone.fluentCode_.Append($", {sel}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        /// <summary>
        /// Adds one content region.
        /// </summary>
        /// <param name="element">An element representing a content region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IWebElement element, Padding padding = null, string regionId = null)
        {
            var list = new List<IWebElement> { element };
            return Content(list, padding, regionId);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="element">An element representing a content region.</param>
        /// <param name="elements">One or more elements, each representing a content region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IWebElement element, params IWebElement[] elements)
        {
            var list = new List<IWebElement> { element };
            list.AddRange(elements);
            return Ignore(list);
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="elements">An enumerable of elements, each representing a content region.</param>
        /// <param name="padding">The padding</param>
        /// <param name="regionId">The regionId</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IEnumerable<IWebElement> elements, Padding padding = null, string regionId = null)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Content)}(");
            foreach (IWebElement elem in elements)
            {
                clone.Content_(new SimpleRegionByElement(elem, padding, regionId));
                clone.fluentCode_.Append($", {elem}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }

        public AppiumCheckSettings Floating(By regionSelector, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({regionSelector},{maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset})");
            return clone;
        }

        public AppiumCheckSettings Floating(By regionSelector, int maxOffset = 0)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxOffset, maxOffset, maxOffset, maxOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({regionSelector},{maxOffset})");
            return clone;
        }

        public AppiumCheckSettings Floating(IWebElement element, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({element},{maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset})");
            return clone;
        }

        public AppiumCheckSettings Floating(IWebElement element, int maxOffset = 0)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxOffset, maxOffset, maxOffset, maxOffset));
            clone.fluentCode_.Append($".{nameof(Floating)}({element},{maxOffset})");
            return clone;
        }

        public AppiumCheckSettings Floating(int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset, params IWebElement[] elementsToIgnore)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Floating)}({maxUpOffset},{maxDownOffset},{maxLeftOffset},{maxRightOffset}");
            foreach (IWebElement element in elementsToIgnore)
            {
                clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
                clone.fluentCode_.Append($", {element}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }



        public AppiumCheckSettings Accessibility(By regionSelector, AccessibilityRegionType regionType)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionBySelector(regionSelector, regionType));
            clone.fluentCode_.Append($".{nameof(Accessibility)}({regionSelector},{nameof(AccessibilityRegionType)}.{regionType})");
            return clone;
        }


        public AppiumCheckSettings Accessibility(IWebElement element, AccessibilityRegionType regionType)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
            clone.fluentCode_.Append($".{nameof(Accessibility)}({element},{nameof(AccessibilityRegionType)}.{regionType})");
            return clone;
        }

        public AppiumCheckSettings Accessibility(AccessibilityRegionType regionType, params IWebElement[] elements)
        {
            AppiumCheckSettings clone = Clone_();
            clone.fluentCode_.Append($".{nameof(Accessibility)}({nameof(AccessibilityRegionType)}.{regionType}");
            foreach (IWebElement element in elements)
            {
                clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
                clone.fluentCode_.Append($", {element}");
            }
            clone.fluentCode_.Append(")");
            return clone;
        }



        public AppiumCheckSettings ScrollRootElement(By selector)
        {
            AppiumCheckSettings clone = Clone_();
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
        public AppiumCheckSettings ScrollRootElement(IWebElement element)
        {
            AppiumCheckSettings clone = Clone_();
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
        
        public AppiumCheckSettings LayoutBreakpointsEnabled(bool shouldSet)
        {
            AppiumCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions().Breakpoints(shouldSet);
            return clone;
        }

        public AppiumCheckSettings LayoutBreakpoints(params int[] breakpoints)
        {
            AppiumCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions().Breakpoints(breakpoints);
            return clone;
        }

        public AppiumCheckSettings LayoutBreakpoints(bool shouldSet)
        {
            AppiumCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions().Breakpoints(shouldSet);
            return clone;
        }
        
        public AppiumCheckSettings LayoutBreakpoints(LayoutBreakpointsOptions options)
        {
            AppiumCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions(options);
            return clone;
        }

        public AppiumCheckSettings UseCookies(bool useCookies)
        {
            AppiumCheckSettings clone = Clone_();
            clone.useCookies_ = useCookies;
            return clone;
        }
        
        #region overrides
        public new AppiumCheckSettings Accessibility(AccessibilityRegionByRectangle region)
        {
            return (AppiumCheckSettings)base.Accessibility(region);
        }

        public new AppiumCheckSettings Accessibility(Rectangle region, AccessibilityRegionType regionType)
        {
            return (AppiumCheckSettings)base.Accessibility(region, regionType);
        }
        public new AppiumCheckSettings Ignore(Rectangle region, params Rectangle[] regions)
        {
            return (AppiumCheckSettings)base.Ignore(region, regions);
        }

        public AppiumCheckSettings Ignore(IEnumerable<Rectangle> regions)
        {
            return (AppiumCheckSettings)base.Ignore(regions);
        }

        public new AppiumCheckSettings Content(Rectangle region, params Rectangle[] regions)
        {
            return (AppiumCheckSettings)base.Content(region, regions);
        }

        public AppiumCheckSettings Content(IEnumerable<Rectangle> regions)
        {
            return (AppiumCheckSettings)base.Content(regions);
        }

        public new AppiumCheckSettings Layout(Rectangle region, params Rectangle[] regions)
        {
            return (AppiumCheckSettings)base.Layout(region, regions);
        }

        public AppiumCheckSettings Layout(IEnumerable<Rectangle> regions)
        {
            return (AppiumCheckSettings)base.Layout(regions);
        }

        public new AppiumCheckSettings Strict(Rectangle region, params Rectangle[] regions)
        {
            return (AppiumCheckSettings)base.Strict(region, regions);
        }

        public AppiumCheckSettings Strict(IEnumerable<Rectangle> regions)
        {
            return (AppiumCheckSettings)base.Strict(regions);
        }

        public new AppiumCheckSettings Floating(int maxOffset, params Rectangle[] regions)
        {
            return (AppiumCheckSettings)base.Floating(maxOffset, regions);
        }

        public new AppiumCheckSettings Floating(Rectangle region, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            return (AppiumCheckSettings)base.Floating(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
        }

        public new AppiumCheckSettings Exact()
        {
            return (AppiumCheckSettings)base.Exact();
        }

        public new AppiumCheckSettings Layout()
        {
            return (AppiumCheckSettings)base.Layout();
        }

        public new AppiumCheckSettings Strict()
        {
            return (AppiumCheckSettings)base.Strict();
        }

        public new AppiumCheckSettings Content()
        {
#pragma warning disable CS0618
            return (AppiumCheckSettings)base.Content();
#pragma warning restore CS0618
        }

        public new AppiumCheckSettings MatchLevel(MatchLevel matchLevel)
        {
            return (AppiumCheckSettings)base.MatchLevel(matchLevel);
        }

        public new AppiumCheckSettings Fully()
        {
            return (AppiumCheckSettings)base.Fully();
        }

        public new AppiumCheckSettings Fully(bool fully)
        {
            return (AppiumCheckSettings)base.Fully(fully);
        }

        public new AppiumCheckSettings Timeout(TimeSpan timeout)
        {
            return (AppiumCheckSettings)base.Timeout(timeout);
        }

        public new AppiumCheckSettings IgnoreCaret(bool ignoreCaret = true)
        {
            return (AppiumCheckSettings)base.IgnoreCaret(ignoreCaret);
        }

        public new AppiumCheckSettings SendDom(bool sendDom = true)
        {
            return (AppiumCheckSettings)base.SendDom(sendDom);
        }

        public new AppiumCheckSettings WithName(string name)
        {
            return (AppiumCheckSettings)base.WithName(name);
        }

        public new AppiumCheckSettings ReplaceLast(bool replaceLast = true)
        {
            return (AppiumCheckSettings)base.ReplaceLast(replaceLast);
        }

        public new AppiumCheckSettings UseDom(bool useDom = true)
        {
            return (AppiumCheckSettings)base.UseDom(useDom);
        }

        public new AppiumCheckSettings EnablePatterns(bool enablePatterns = true)
        {
            return (AppiumCheckSettings)base.EnablePatterns(enablePatterns);
        }

        public new AppiumCheckSettings IgnoreDisplacements(bool ignoreDisplacements = true)
        {
            return (AppiumCheckSettings)base.IgnoreDisplacements(ignoreDisplacements);
        }

        public new AppiumCheckSettings BeforeRenderScreenshotHook(string hook)
        {
            return (AppiumCheckSettings)base.BeforeRenderScreenshotHook(hook);
        }

        [Obsolete("Use " + nameof(BeforeRenderScreenshotHook) + " instead.")]
        public new AppiumCheckSettings ScriptHook(string hook)
        {
            return (AppiumCheckSettings)base.ScriptHook(hook);
        }

        public new AppiumCheckSettings VisualGridOptions(params VisualGridOption[] options)
        {
            return (AppiumCheckSettings)base.VisualGridOptions(options);
        }

        public new AppiumCheckSettings VariationGroupId(string variationGroupId)
        {
            return (AppiumCheckSettings)base.VariationGroupId(variationGroupId);
        }
        #endregion

        private AppiumCheckSettings Clone_()
        {
            return (AppiumCheckSettings)Clone();
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
            AppiumCheckSettings clone = new AppiumCheckSettings();
            PopulateClone_(clone);
            clone.targetElement_ = targetElement_;
            clone.targetSelector_ = targetSelector_;
            clone.frameChain_.AddRange(frameChain_);
            clone.scrollRootElement_ = scrollRootElement_;
            clone.scrollRootSelector_ = scrollRootSelector_;
            clone.vgTargetSelector_ = vgTargetSelector_;
            clone.useCookies_ = useCookies_;
            clone.isDefaultWebview_ = isDefaultWebview_;
            clone.webview_ = webview_;
            return clone;
        }

        bool? IAppiumCheckTarget.IsDefaultWebview()
        {
            return isDefaultWebview_;
        }

        string IAppiumCheckTarget.GetWebview()
        {
            return webview_;
        }
    }
}

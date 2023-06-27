using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public class PlaywrightCheckSettings : CheckSettings, IPlaywrightCheckSettings, IPlaywrightCheckTarget
    {
        /// <summary>
        /// The target region.
        /// </summary>
        private Reference targetElement_;

        /// <summary>
        /// The scroll root element.
        /// </summary>
        private Reference scrollRootElement_;

        /// <summary>
        /// The frame chain.
        /// </summary>
        private readonly List<FrameLocator> frameChain_ = new List<FrameLocator>();

        /// <summary>Specify the target as a region.</summary>
        /// <param name="region">the region</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Region(Region region)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(region.Rectangle);
            return clone;
        }

        /// <summary>Specify the target as a region.</summary>
        /// <param name="selector">the selector</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Region(string selector)
        {
            return Region(new Selector(selector));
        }

        /// <summary>Specify the target as a region.</summary>
        /// <param name="locator">the locator</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Region(ILocator locator)
        {
            return Region(new Selector(locator));
        }

        /// <summary>Specify the target as a region.</summary>
        /// <param name="element">the element</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Region(IElementHandle element)
        {
            return Region(new Element(element));
        }

        private PlaywrightCheckSettings Region(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.targetElement_ = selector;
            return clone;
        }

        private PlaywrightCheckSettings Region(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.targetElement_ = element;
            return clone;
        }

        /// <summary>Specify the target as a frame.</summary>
        /// <param name="frameNameOrId">the frame's name or id.</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Frame(string frameNameOrId)
        {
            FrameLocator frameLocator = new FrameLocator
            {
                FrameNameOrId = frameNameOrId
            };
            return Frame(frameLocator);
        }

        /// <summary>Specify the target as a frame.</summary>
        /// <param name="frameIndex">the frame's index.</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Frame(int frameIndex)
        {
            FrameLocator frameLocator = new FrameLocator
            {
                FrameIndex = frameIndex
            };
            return Frame(frameLocator);
        }

        /// <summary>Specify the target as a frame.</summary>
        /// <param name="frameLocator">the locator to the frame.</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Frame(ILocator frameLocator)
        {
            return Frame(new Selector(frameLocator));
        }

        /// <summary>Specify the target as a frame.</summary>
        /// <param name="frameElement">the frame's element.</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Frame(IElementHandle frameElement)
        {
            return Frame(new Element(frameElement));
        }

        private PlaywrightCheckSettings Frame(Selector selector)
        {
            FrameLocator frameLocator = new FrameLocator
            {
                FrameSelector = selector
            };
            return Frame(frameLocator);
        }

        private PlaywrightCheckSettings Frame(Element element)
        {
            FrameLocator frameLocator = new FrameLocator
            {
                FrameElement = element
            };
            return Frame(frameLocator);
        }

        /** internal */
        private PlaywrightCheckSettings Frame(FrameLocator frameLocator)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.frameChain_.Add(frameLocator);
            return clone;
        }

        /// <summary>Ignore a region by selector.</summary>
        /// <param name="selector">A CSS or XPath element selector</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(string selector)
        {
            return Ignore(new Selector(selector));
        }


        /// <summary>Ignore a region by a locator.</summary>
        /// <param name="locator">An element locator to ignore</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(ILocator locator)
        {
            return Ignore(new Selector(locator));
        }

        /// <summary>Ignore a region by element handle.</summary>
        /// <param name="element">An element handle to ignore</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(IElementHandle element)
        {
            return Ignore(new Element(element));
        }

        private PlaywrightCheckSettings Ignore(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.ignoreRegions_.Add(element);
            return clone;
        }

        /// <summary>Adds an ignore region with a region ID.</summary>
        /// <param name="selector">the css or xpath selector to ignore match</param>
        /// <param name="regionId">the region ID</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(string selector, string regionId)
        {
            return Ignore(new Selector(selector), regionId);
        }

        /// <summary>Adds an ignore region with a region ID.</summary>
        /// <param name="locator">the locator to ignore match</param>
        /// <param name="regionId">the region ID</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(ILocator locator, string regionId)
        {
            return Ignore(new Selector(locator), regionId);
        }

        /// <summary>Adds an ignore region with a region ID.</summary>
        /// <param name="element">the element to ignore match</param>
        /// <param name="regionId">the region ID</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Ignore(IElementHandle element, string regionId)
        {
            return Ignore(new Element(element), regionId);
        }

        private PlaywrightCheckSettings Ignore(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.RegionId = regionId;
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.RegionId = regionId;
            clone.ignoreRegions_.Add(element);
            return clone;
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(string selector, Padding padding)
        {
            return Ignore(new Selector(selector), padding);
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param locator  the locator to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(ILocator locator, Padding padding)
        {
            return Ignore(new Selector(locator), padding);
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param element  the element to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(IElementHandle element, Padding padding)
        {
            return Ignore(new Element(element), padding);
        }

        private PlaywrightCheckSettings Ignore(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.Padding = padding;
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.Padding = padding;
            clone.ignoreRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Ignore(Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            foreach (Region r in regions)
            {
                clone.Ignore_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings Ignore(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.Ignore_(region);
            foreach (Region r in regions)
            {
                clone.Ignore_(r);
            }

            return clone;
        }

        private void Ignore_(Region region)
        {
            ignoreRegions_.Add(new Applitools.Playwright.Universal.Dto.SimpleRegionByRectangle(region.Rectangle));
        }

        /// <summary>Adds a layout region.</summary>
        /// <param name="selector">The css or xpath selector to match using the layout algorithm.</param>
        /// <returns>An updated instance of this settings object</returns>
        public PlaywrightCheckSettings Layout(string selector)
        {
            return Layout(new Selector(selector));
        }

        /// <summary>Adds a layout region.</summary>
        /// <param name="locator">The locator to match using the layout algorithm.</param>
        /// <returns>An updated instance of this settings object</returns>
        public PlaywrightCheckSettings Layout(ILocator locator)
        {
            return Layout(new Selector(locator));
        }

        /// <summary>Adds a layout region.</summary>
        /// <param name="element">The element to match using the layout algorithm.</param>
        /// <returns>An updated instance of this settings object</returns>
        public PlaywrightCheckSettings Layout(IElementHandle element)
        {
            return Layout(new Element(element));
        }

        private PlaywrightCheckSettings Layout(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Layout(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.layoutRegions_.Add(element);
            return clone;
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(string selector, string regionId)
        {
            return Layout(new Selector(selector), regionId);
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param locator   the locator to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(ILocator locator, string regionId)
        {
            return Layout(new Selector(locator), regionId);
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param element   the element to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(IElementHandle element, string regionId)
        {
            return Layout(new Element(element), regionId);
        }

        private PlaywrightCheckSettings Layout(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.RegionId = regionId;
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Layout(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.RegionId = regionId;
            clone.layoutRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Layout(string selector, Padding padding)
        {
            return Layout(new Selector(selector), padding);
        }

        public PlaywrightCheckSettings Layout(ILocator locator, Padding padding)
        {
            return Layout(new Selector(locator), padding);
        }

        public PlaywrightCheckSettings Layout(IElementHandle element, Padding padding)
        {
            return Layout(new Element(element), padding);
        }

        private PlaywrightCheckSettings Layout(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.Padding = padding;
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Layout(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.Padding = padding;
            clone.layoutRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Layout(Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            foreach (Region r in regions)
            {
                clone.Layout_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings Layout(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.Layout_(region);
            foreach (Region r in regions)
            {
                clone.Layout_(r);
            }

            return clone;
        }

        private void Layout_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region.Rectangle);
            this.layoutRegions_.Add(simpleRegionByRectangle);
        }

        public PlaywrightCheckSettings Strict(string selector)
        {
            return Strict(new Selector(selector));
        }

        public PlaywrightCheckSettings Strict(ILocator locator)
        {
            return Strict(new Selector(locator));
        }

        public PlaywrightCheckSettings Strict(IElementHandle element)
        {
            return Strict(new Element(element));
        }

        private PlaywrightCheckSettings Strict(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.strictRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Strict(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.strictRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Strict(string selector, string regionId)
        {
            return Strict(new Selector(selector), regionId);
        }

        public PlaywrightCheckSettings Strict(ILocator locator, string regionId)
        {
            return Strict(new Selector(locator), regionId);
        }

        public PlaywrightCheckSettings Strict(IElementHandle element, string regionId)
        {
            return Strict(new Element(element), regionId);
        }

        private PlaywrightCheckSettings Strict(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.RegionId = regionId;
            clone.strictRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.RegionId = regionId;
            clone.strictRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Strict(string selector, Padding padding)
        {
            return Strict(new Selector(selector), padding);
        }

        public PlaywrightCheckSettings Strict(ILocator locator, Padding padding)
        {
            return Strict(new Selector(locator), padding);
        }

        public PlaywrightCheckSettings Strict(IElementHandle element, Padding padding)
        {
            return Strict(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.Padding = padding;
            clone.strictRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Strict(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.Padding = padding;
            clone.strictRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Strict(Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            foreach (Region r in regions)
            {
                clone.Strict_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings Strict(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.Strict_(region);
            foreach (Region r in regions)
            {
                clone.Strict_(r);
            }

            return clone;
        }

        private void Strict_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region.Rectangle);
            strictRegions_.Add(simpleRegionByRectangle);
        }

        public new PlaywrightCheckSettings IgnoreColors()
        {
            return (PlaywrightCheckSettings)base.IgnoreColors();
        }

        public PlaywrightCheckSettings IgnoreColors(string selector)
        {
            return IgnoreColors(new Selector(selector));
        }

        public PlaywrightCheckSettings IgnoreColors(ILocator locator)
        {
            return IgnoreColors(new Selector(locator));
        }

        public PlaywrightCheckSettings IgnoreColors(IElementHandle element)
        {
            return IgnoreColors(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.contentRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.contentRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings IgnoreColors(string selector, string regionId)
        {
            return IgnoreColors(new Selector(selector), regionId);
        }

        public PlaywrightCheckSettings IgnoreColors(ILocator locator, string regionId)
        {
            return IgnoreColors(new Selector(locator), regionId);
        }

        public PlaywrightCheckSettings IgnoreColors(IElementHandle element, string regionId)
        {
            return IgnoreColors(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.RegionId = regionId;
            clone.contentRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.RegionId = regionId;
            clone.contentRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings IgnoreColors(string selector, Padding padding)
        {
            return IgnoreColors(new Selector(selector), padding);
        }

        public PlaywrightCheckSettings IgnoreColors(ILocator locator, Padding padding)
        {
            return IgnoreColors(new Selector(locator), padding);
        }

        public PlaywrightCheckSettings IgnoreColors(IElementHandle element, Padding padding)
        {
            return IgnoreColors(new Element(element), padding);
        }

        private PlaywrightCheckSettings IgnoreColors(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            selector.Padding = padding;
            clone.contentRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings IgnoreColors(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = Clone_();
            element.Padding = padding;
            clone.contentRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings IgnoreColors(Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            foreach (Region r in regions)
            {
                clone.IgnoreColors_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings IgnoreColors(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.IgnoreColors_(region);
            foreach (Region r in regions)
            {
                clone.IgnoreColors_(r);
            }

            return clone;
        }

        private void IgnoreColors_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region.Rectangle);
            contentRegions_.Add(simpleRegionByRectangle);
        }

        public PlaywrightCheckSettings Floating(string selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionBySelector(selector, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(ILocator locator, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionBySelector(locator, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(IElementHandle element, int maxUpOffset, int maxDownOffset,
            int maxLeftOffset, int maxRightOffset)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(int maxOffset, string selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionBySelector(selector, maxOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(int maxOffset, ILocator locator)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionBySelector(locator, maxOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(int maxOffset, IElementHandle element)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.floatingRegions_.Add(new FloatingRegionByElement(element, maxOffset));
            return clone;
        }

        public PlaywrightCheckSettings Floating(int maxOffset, params Region[] regions)
        {
            PlaywrightCheckSettings clone = Clone_();
            foreach (Region r in regions)
            {
                clone.Floating_(r, maxOffset, maxOffset, maxOffset, maxOffset);
            }

            return clone;
        }


        public PlaywrightCheckSettings Floating(Region region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.Floating_(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
            return clone;
        }

        private void Floating_(Region region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            floatingRegions_.Add(new Applitools.Playwright.Universal.Dto.FloatingRegionByRectangle(region.Rectangle,
                maxUpOffset,
                maxDownOffset,
                maxLeftOffset,
                maxRightOffset));
        }

        public PlaywrightCheckSettings Accessibility(string selector, AccessibilityRegionType type)
        {
            return Accessibility(new AccessibilityRegionBySelector(selector, type));
        }

        public PlaywrightCheckSettings Accessibility(ILocator locator, AccessibilityRegionType type)
        {
            return Accessibility(new AccessibilityRegionBySelector(locator, type));
        }

        public PlaywrightCheckSettings Accessibility(IElementHandle element, AccessibilityRegionType type)
        {
            return Accessibility(new AccessibilityRegionByElement(element, type));
        }

        private PlaywrightCheckSettings Accessibility(AccessibilityRegionBySelector accessibilityRegionBySelector)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.accessibilityRegions_.Add(accessibilityRegionBySelector);
            return clone;
        }

        private PlaywrightCheckSettings Accessibility(AccessibilityRegionByElement accessibilityRegionByElement)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.accessibilityRegions_.Add(accessibilityRegionByElement);
            return clone;
        }

        public PlaywrightCheckSettings Accessibility(Region region, AccessibilityRegionType regionType)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.Accessibility_(region, regionType);
            return clone;
        }

        private void Accessibility_(Region rect, AccessibilityRegionType regionType)
        {
            accessibilityRegions_.Add(new AccessibilityRegionByRectangle(rect.Rectangle, regionType));
        }

        public PlaywrightCheckSettings ScrollRootElement(string selector)
        {
            return ScrollRootElement(new Selector(selector));
        }

        public PlaywrightCheckSettings ScrollRootElement(ILocator locator)
        {
            return ScrollRootElement(new Selector(locator));
        }

        public PlaywrightCheckSettings ScrollRootElement(IElementHandle element)
        {
            return ScrollRootElement(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings ScrollRootElement(Selector selector)
        {
            PlaywrightCheckSettings clone = Clone_();
            if (frameChain_.Count == 0)
            {
                clone.scrollRootElement_ = selector;
            }
            else
            {
                frameChain_.Last().ScrollRootSelector = selector;
            }

            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings ScrollRootElement(Element element)
        {
            PlaywrightCheckSettings clone = Clone_();
            if (frameChain_.Count == 0)
            {
                clone.scrollRootElement_ = element;
            }
            else
            {
                frameChain_.Last().ScrollRootElement = element;
            }

            return clone;
        }

        public PlaywrightCheckSettings LayoutBreakpoints(params int[] breakpoints)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions().Breakpoints(breakpoints);
            return clone;
        }

        public PlaywrightCheckSettings LayoutBreakpoints(bool shouldSet)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions().Breakpoints(shouldSet);
            return clone;
        }
        
        public PlaywrightCheckSettings LayoutBreakpoints(LayoutBreakpointsOptions options)
        {
            PlaywrightCheckSettings clone = Clone_();
            clone.LayoutBreakpointsOptions = new LayoutBreakpointsOptions(options);
            return clone;
        }
        
        public new PlaywrightCheckSettings Fully()
        {
            return (PlaywrightCheckSettings)base.Fully();
        }

        public new PlaywrightCheckSettings Fully(bool fully)
        {
            return (PlaywrightCheckSettings)base.Fully(fully);
        }

        public new PlaywrightCheckSettings WithName(string name)
        {
            return (PlaywrightCheckSettings)base.WithName(name);
        }

        public new PlaywrightCheckSettings IgnoreCaret(bool ignoreCaret)
        {
            return (PlaywrightCheckSettings)base.IgnoreCaret(ignoreCaret);
        }

        public new PlaywrightCheckSettings SendDom(bool sendDom = true)
        {
            return (PlaywrightCheckSettings)base.SendDom(sendDom);
        }

        public new PlaywrightCheckSettings EnablePatterns(bool enablePatterns = true)
        {
            return (PlaywrightCheckSettings)base.EnablePatterns(enablePatterns);
        }

        public new PlaywrightCheckSettings BeforeRenderScreenshotHook(string hook)
        {
            return (PlaywrightCheckSettings)base.BeforeRenderScreenshotHook(hook);
        }

        public new PlaywrightCheckSettings Timeout(TimeSpan timeout)
        {
            return (PlaywrightCheckSettings)base.Timeout(timeout);
        }

        public new PlaywrightCheckSettings WaitBeforeCapture(TimeSpan waitTime)
        {
            return (PlaywrightCheckSettings)base.WaitBeforeCapture(waitTime);
        }

        public new PlaywrightCheckSettings UseDom(bool useDom)
        {
            return (PlaywrightCheckSettings)base.UseDom(useDom);
        }

        public new PlaywrightCheckSettings VariationGroupId(string variationGroupId)
        {
            return (PlaywrightCheckSettings)base.VariationGroupId(variationGroupId);
        }

        public new PlaywrightCheckSettings VisualGridOptions(params VisualGridOption[] options)
        {
            return (PlaywrightCheckSettings)base.VisualGridOptions(options);
        }

        public new PlaywrightCheckSettings PageId(string pageId)
        {
            return (PlaywrightCheckSettings)base.PageId(pageId);
        }

        public new PlaywrightCheckSettings IgnoreDisplacements(bool ignoreDisplacements = true)
        {
            return (PlaywrightCheckSettings)base.IgnoreDisplacements(ignoreDisplacements);
        }

        public new PlaywrightCheckSettings MatchLevel(MatchLevel matchLevel)
        {
            return (PlaywrightCheckSettings)base.MatchLevel(matchLevel);
        }

        public new PlaywrightCheckSettings Strict()
        {
            return (PlaywrightCheckSettings)base.Strict();
        }

        public new PlaywrightCheckSettings Layout()
        {
            return (PlaywrightCheckSettings)base.Layout();
        }

        public new PlaywrightCheckSettings Exact()
        {
            return (PlaywrightCheckSettings)base.Exact();
        }

        public new PlaywrightCheckSettings LazyLoad()
        {
            return (PlaywrightCheckSettings)base.LazyLoad();
        }

        public new PlaywrightCheckSettings LazyLoad(LazyLoadOptions lazyLoadOptions)
        {
            return (PlaywrightCheckSettings)base.LazyLoad(lazyLoadOptions);
        }

        public new PlaywrightCheckSettings DensityMetrics(int xDpi, int yDpi, double? scaleRatio)
        {
            return (PlaywrightCheckSettings)base.DensityMetrics(xDpi, yDpi, scaleRatio);
        }

        private PlaywrightCheckSettings Clone_()
        {
            return (PlaywrightCheckSettings)Clone();
        }

        protected override CheckSettings Clone()
        {
            PlaywrightCheckSettings clone = new PlaywrightCheckSettings();
            PopulateClone_(clone);
            clone.targetElement_ = targetElement_;
            clone.scrollRootElement_ = scrollRootElement_;
            clone.frameChain_.AddRange(frameChain_);
            return clone;
        }

        [Obsolete]
        public new PlaywrightCheckSettings Content()
        {
            return IgnoreColors();
        }

        [Obsolete]
        public PlaywrightCheckSettings Content(string selector)
        {
            return IgnoreColors(selector);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(ILocator locator)
        {
            return IgnoreColors(locator.ElementHandleAsync().GetAwaiter().GetResult());
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(IElementHandle element)
        {
            return IgnoreColors(element);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(string selector, string regionId)
        {
            return IgnoreColors(selector, regionId);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(ILocator locator, string regionId)
        {
            return IgnoreColors(locator, regionId);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(IElementHandle element, string regionId)
        {
            return IgnoreColors(element, regionId);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(string selector, Padding padding)
        {
            return IgnoreColors(selector, padding);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(ILocator locator, Padding padding)
        {
            return IgnoreColors(locator.ElementHandleAsync().GetAwaiter().GetResult(), padding);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(IElementHandle element, Padding padding)
        {
            return IgnoreColors(element, padding);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(Region[] regions)
        {
            return IgnoreColors(regions);
        }

        [Obsolete("Use IgnoreColors")]
        public PlaywrightCheckSettings Content(Region region, params Region[] regions)
        {
            return IgnoreColors(region, regions);
        }

        IList<FrameLocator> IPlaywrightCheckTarget.GetFrameChain()
        {
            return frameChain_;
        }

        Reference IPlaywrightCheckTarget.GetScrollRootElement()
        {
            return scrollRootElement_;
        }

        Reference IPlaywrightCheckTarget.GetTargetElement()
        {
            return targetElement_;
        }

    }
}
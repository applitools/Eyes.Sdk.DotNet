using System;
using System.Collections.Generic;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public class PlaywrightCheckSettings : CheckSettings, IPlaywrightCheckSettings
    {
        /** The target region. */
        private Reference targetElement_;

        /** the scroll root element. */
        private Reference scrollRootElement_;

        /** The frame chain. */
        private readonly List<FrameLocator> frameChain_ = new List<FrameLocator>();

        /** Used to determine if using default layout breakpoints. */
        private bool isDefaultLayoutBreakpointsSet_;

        public PlaywrightCheckSettings()
        {
        }

        /// <summary>Specify the target as a region.</summary>
        /// <param name="region">the region</param>
        /// <returns>an updated instance of this settings object</returns>
        public PlaywrightCheckSettings Region(Region region)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.targetElement_ = selector;
            return clone;
        }

        private PlaywrightCheckSettings Region(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.RegionId = regionId;
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.Padding = padding;
            clone.ignoreRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Ignore(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.Padding = padding;
            clone.ignoreRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Ignore(Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            foreach (Region r in regions)
            {
                clone.Ignore_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings Ignore(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.Ignore_(region);
            foreach (Region r in regions)
            {
                clone.Ignore_(r);
            }

            return clone;
        }

        private void Ignore_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region.Rectangle);
            ignoreRegions_.Add(simpleRegionByRectangle);
        }

        /**
     * Adds a layout region.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(string selector)
        {
            return Layout(new Selector(selector));
        }

        /**
     * Adds a layout region.
     *
     * @param locator  the locator to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(ILocator locator)
        {
            return Layout(new Selector(locator));
        }

        /**
     * Adds a layout region.
     *
     * @param element  the css or xpath selector to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(IElementHandle element)
        {
            return Layout(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Selector selector)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.RegionId = regionId;
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Layout(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.RegionId = regionId;
            clone.layoutRegions_.Add(element);
            return clone;
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(string selector, Padding padding)
        {
            return Layout(new Selector(selector), padding);
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param locator   the locator to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(ILocator locator, Padding padding)
        {
            return Layout(new Selector(locator), padding);
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param element   the element to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(IElementHandle element, Padding padding)
        {
            return Layout(new Element(element), padding);
        }

        private PlaywrightCheckSettings Layout(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.Padding = padding;
            clone.layoutRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Layout(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.Padding = padding;
            clone.layoutRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Layout(Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            foreach (Region r in regions)
            {
                clone.Layout_(r);
            }

            return clone;
        }


        public PlaywrightCheckSettings Layout(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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

        /**
     * Adds a strict region.
     *
     * @param selector  the css or xpath selector to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(string selector)
        {
            return Strict(new Selector(selector));
        }

        /**
     * Adds a strict region.
     *
     * @param locator  the locator to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(ILocator locator)
        {
            return Strict(new Selector(locator));
        }

        /**
     * Adds a strict region.
     *
     * @param element  the element to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(IElementHandle element)
        {
            return Strict(new Element(element));
        }

        private PlaywrightCheckSettings Strict(Selector selector)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.strictRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Strict(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.strictRegions_.Add(element);
            return clone;
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param selector  the css or xpath selector to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(string selector, string regionId)
        {
            return Strict(new Selector(selector), regionId);
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param locator   the locator to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(ILocator locator, string regionId)
        {
            return Strict(new Selector(locator), regionId);
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param element   the element to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(IElementHandle element, string regionId)
        {
            return Strict(new Element(element), regionId);
        }

        private PlaywrightCheckSettings Strict(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.RegionId = regionId;
            clone.strictRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.RegionId = regionId;
            clone.strictRegions_.Add(element);
            return clone;
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param selector   the css or xpath selector to match using the strict algorithm
     * @param padding    the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(string selector, Padding padding)
        {
            return Strict(new Selector(selector), padding);
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param locator   the locator to match using the strict algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(ILocator locator, Padding padding)
        {
            return Strict(new Selector(locator), padding);
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param element   the element to match using the strict algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(IElementHandle element, Padding padding)
        {
            return Strict(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.Padding = padding;
            clone.strictRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings Strict(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.Padding = padding;
            clone.strictRegions_.Add(element);
            return clone;
        }

        public PlaywrightCheckSettings Strict(Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            foreach (Region r in regions)
            {
                clone.Strict_(r);
            }

            return clone;
        }

        public PlaywrightCheckSettings Strict(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
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

        /**
     * Shortcut to set the match level to {@code MatchLevel.CONTENT}.
     * @return An updated clone of this settings object.
     */
        public PlaywrightCheckSettings IgnoreColors()
        {
            return (PlaywrightCheckSettings)base.IgnoreColors();
        }

        /**
     * Adds an ignore colors region.
     *
     * @param selector  the css or xpath selector to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(string selector)
        {
            return IgnoreColors(new Selector(selector));
        }

        /**
     * Adds an ignore colors region.
     *
     * @param locator  the locator to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(ILocator locator)
        {
            return IgnoreColors(new Selector(locator));
        }

        /**
     * Adds an ignore colors region.
     *
     * @param element  the element to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(IElementHandle element)
        {
            return IgnoreColors(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.contentRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.contentRegions_.Add(element);
            return clone;
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param selector  the css or xpath to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(string selector, string regionId)
        {
            return ignoreColors(new Selector(selector), regionId);
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param locator   the locator to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(ILocator locator, string regionId)
        {
            return ignoreColors(new Selector(locator), regionId);
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param element   the element to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(IElementHandle element, string regionId)
        {
            return ignoreColors(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.RegionId = regionId;
            clone.contentRegions_.Add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element, string regionId)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.RegionId = regionId;
            clone.contentRegions_.Add(element);
            return clone;
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(string selector, Padding padding)
        {
            return ignoreColors(new Selector(selector), padding);
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param locator   the locator to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(ILocator locator, Padding padding)
        {
            return ignoreColors(new Selector(locator), padding);
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param element   the element to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(IElementHandle element, Padding padding)
        {
            return ignoreColors(new Element(element), padding);
        }

        private PlaywrightCheckSettings IgnoreColors(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            selector.Padding = padding;
            clone.contentRegions_.Add(selector);
            return clone;
        }

        private PlaywrightCheckSettings IgnoreColors(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            element.Padding = padding;
            clone.contentRegions_.Add(element);
            return clone;
        }

        /**
     * Adds one or more ignore colors regions.
     *
     * @param regions An array of regions to match using the ignore colors method.
     * @return An updated clone of this settings object.
     */
        public PlaywrightCheckSettings IgnoreColors(Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            foreach (Region r in regions)
            {
                clone.ignoreColors_(r);
            }

            return clone;
        }

        /**
     * Adds one or more ignore colors regions.
     *
     * @param region A region to match using the Content method.
     * @param regions Optional extra regions to match using the Content method.
     * @return An updated clone of this settings object.
     */
        public PlaywrightCheckSettings IgnoreColors(Region region, params Region[] regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.ignoreColors_(region);
            foreach (Region r in regions)
            {
                clone.ignoreColors_(r);
            }

            return clone;
        }

        /** internal */
        private void ignoreColors_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region.Rectangle);
            this.contentRegions_.Add(simpleRegionByRectangle);
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param selector          the css or xpath selector
     * @param maxUpOffset       how much the content can move up
     * @param maxDownOffset     how much the content can move down
     * @param maxLeftOffset     how much the content can move to the left
     * @param maxRightOffset    how much the content can move to the right
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(string selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions_.Add(new FloatingRegionSelector(selector, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param locator           the locator
     * @param maxUpOffset       how much the content can move up
     * @param maxDownOffset     how much the content can move down
     * @param maxLeftOffset     how much the content can move to the left
     * @param maxRightOffset    how much the content can move to the right
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(ILocator locator, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions.add(new FloatingRegionSelector(locator, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param element           the element
     * @param maxUpOffset       how much the content can move up
     * @param maxDownOffset     how much the content can move down
     * @param maxLeftOffset     how much the content can move to the left
     * @param maxRightOffset    how much the content can move to the right
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(IElementHandle element, int maxUpOffset, int maxDownOffset,
            int maxLeftOffset, int maxRightOffset)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions.add(new FloatingRegionElement(element, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
            return clone;
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param maxOffset    how much the content can move in all directions
     * @param selector     the css or xpath selector
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(int maxOffset, string selector)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions.add(new FloatingRegionSelector(selector, maxOffset));
            return clone;
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param maxOffset     how much the content can move in all directions
     * @param locator       the locator
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(int maxOffset, Locator locator)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions.add(new FloatingRegionSelector(locator, maxOffset));
            return clone;
        }

        /**
     * Adds a floating region.
     * A floating region is a region that can be placed within the boundaries of a bigger region.
     *
     * @param maxOffset     how much the content can move in all directions
     * @param element       the element
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Floating(int maxOffset, ElementHandle element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floatingRegions.add(new FloatingRegionElement(element, maxOffset));
            return clone;
        }


        public PlaywrightCheckSettings Floating(int maxOffset, Region...regions)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            for (Region r:
            regions) {
                clone.floating_(r, maxOffset, maxOffset, maxOffset, maxOffset);
            }
            return clone;
        }


        public PlaywrightCheckSettings Floating(Region region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.floating_(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
            return clone;
        }

        /** internal */
        protected void floating_(Region region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            this.floatingRegions.add(new FloatingRegionByRectangle(region, maxUpOffset, maxDownOffset, maxLeftOffset,
                maxRightOffset));
        }

        /**
     * Adds an accessibility region with a {@link AccessibilityRegionType}.
     *
     * @param selector      the css or xpath selector
     * @param type          the accessibility region type
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Accessibility(string selector, AccessibilityRegionType type)
        {
            return Accessibility(new AccessibilityRegionBySelector(selector, this.Type));
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
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.accessibilityRegions_.Add(accessibilityRegionBySelector);
            return clone;
        }

        private PlaywrightCheckSettings Accessibility(AccessibilityRegionByElement accessibilityRegionByElement)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.accessibilityRegions_.Add(accessibilityRegionByElement);
            return clone;
        }

        public PlaywrightCheckSettings Accessibility(Region region, AccessibilityRegionType regionType)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.Accessibility_(region, regionType);
            return clone;
        }

        protected void Accessibility_(Region rect, AccessibilityRegionType regionType)
        {
            accessibilityRegions_.Add(new AccessibilityRegionByRectangle(rect.Rectangle, regionType));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param selector  the css or xpath selector
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(string selector)
        {
            return ScrollRootElement(new Selector(selector));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param locator  the locator
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(ILocator locator)
        {
            return ScrollRootElement(new Selector(locator));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param element  the element
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(IElementHandle element)
        {
            return ScrollRootElement(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings ScrollRootElement(Selector selector)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            if (frameChain_.size() == 0)
            {
                clone.scrollRootElement = selector;
            }
            else
            {
                frameChain_.get(frameChain_.size() - 1).setScrollRootSelector(selector);
            }

            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings ScrollRootElement(Element element)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            if (frameChain_.size() == 0)
            {
                clone.scrollRootElement = element;
            }
            else
            {
                frameChain_.get(frameChain_.size() - 1).setScrollRootElement(element);
            }

            return clone;
        }

        /**
     * Enable and set the layout breakpoints for the current target.
     *
     * @param breakpoints  one or more viewport widths explicitly
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings LayoutBreakpoints(int...breakpoints)
        {
            return setLayoutBreakpoints(breakpoints);
        }

        /**
     * Enable layout breakpoints for the current target.
     *
     * @param shouldSet  should enable this feature
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings LayoutBreakpoints(bool shouldSet)
        {
            return SetLayoutBreakpoints(shouldSet);
        }


        public PlaywrightCheckSettings SetLayoutBreakpoints(int...breakpoints)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.IsDefaultLayoutBreakpointsSet = false;
            clone.layoutBreakpoints_.clear();
            if (breakpoints == null || breakpoints.length == 0)
            {
                return this;
            }

            for (int breakpoint :
            breakpoints) {
                ArgumentGuard.greaterThanZero(breakpoint, "breakpoint");
                clone.layoutBreakpoints_.add(breakpoint);
            }

            Collections.sort(clone.layoutBreakpoints_);
            return clone;
        }


        public PlaywrightCheckSettings SetLayoutBreakpoints(bool shouldSet)
        {
            PlaywrightCheckSettings clone = (PlaywrightCheckSettings)Clone();
            clone.IsDefaultLayoutBreakpointsSet = shouldSet;
            clone.layoutBreakpoints_.clear();
            return clone;
        }


        public List<Integer> GetLayoutBreakpoints()
        {
            return layoutBreakpoints_;
        }


        public bool IsDefaultLayoutBreakpointsSet()
        {
            return IsDefaultLayoutBreakpointsSet;
        }


        public PlaywrightCheckSettings Fully()
        {
            return Fully(true);
        }


        public PlaywrightCheckSettings Fully(bool fully)
        {
            return (PlaywrightCheckSettings)super.fully(fully);
        }


        public PlaywrightCheckSettings WithName(string name)
        {
            return (PlaywrightCheckSettings)super.withName(name);
        }


        public PlaywrightCheckSettings IgnoreCaret(bool ignoreCaret)
        {
            return (PlaywrightCheckSettings)super.ignoreCaret(ignoreCaret);
        }


        public PlaywrightCheckSettings SendDom(bool sendDom)
        {
            return (PlaywrightCheckSettings)super.sendDom(sendDom);
        }


        public PlaywrightCheckSettings SendDom()
        {
            return (PlaywrightCheckSettings)super.sendDom();
        }


        public PlaywrightCheckSettings EnablePatterns(bool enablePatterns)
        {
            return (PlaywrightCheckSettings)super.enablePatterns(enablePatterns);
        }


        public PlaywrightCheckSettings EnablePatterns()
        {
            return (PlaywrightCheckSettings)super.enablePatterns();
        }


        public PlaywrightCheckSettings BeforeRenderScreenshotHook(string hook)
        {
            return (PlaywrightCheckSettings)super.beforeRenderScreenshotHook(hook);
        }

        public PlaywrightCheckSettings Timeout(Integer timeoutMilliseconds)
        {
            return (PlaywrightCheckSettings)super.timeout(timeoutMilliseconds);
        }

        public PlaywrightCheckSettings UseDom(bool useDom)
        {
            return (PlaywrightCheckSettings)super.useDom(useDom);
        }

        public PlaywrightCheckSettings VariationGroupId(string variationGroupId)
        {
            return (PlaywrightCheckSettings)super.variationGroupId(variationGroupId);
        }

        public PlaywrightCheckSettings VisualGridOptions(VisualGridOption...options)
        {
            return (PlaywrightCheckSettings)super.visualGridOptions(options);
        }

        public PlaywrightCheckSettings WaitBeforeCapture(Integer milliSec)
        {
            return (PlaywrightCheckSettings)super.waitBeforeCapture(milliSec);
        }

        public PlaywrightCheckSettings SetDisableBrowserFetching(bool disableBrowserFetching)
        {
            return (PlaywrightCheckSettings)super.setDisableBrowserFetching(disableBrowserFetching);
        }

        public PlaywrightCheckSettings AutProxy(AutProxySettings autProxy)
        {
            return (PlaywrightCheckSettings)super.autProxy(autProxy);
        }

        public PlaywrightCheckSettings SetAccessibilityValidation(AccessibilitySettings accessibilitySettings)
        {
            return (PlaywrightCheckSettings)super.setAccessibilityValidation(accessibilitySettings);
        }

        public PlaywrightCheckSettings StitchMode(StitchMode stitchMode)
        {
            return (PlaywrightCheckSettings)super.stitchMode(stitchMode);
        }

        public PlaywrightCheckSettings SetHideScrollBars(bool hideScrollBars)
        {
            return (PlaywrightCheckSettings)super.setHideScrollBars(hideScrollBars);
        }

        public PlaywrightCheckSettings SetHideCaret(bool hideCaret)
        {
            return (PlaywrightCheckSettings)super.setHideCaret(hideCaret);
        }

        public override PlaywrightCheckSettings StitchOverlap(int? overlap)
        {
            return (PlaywrightCheckSettings)base.stitchOverlap(overlap);
        }

        public PlaywrightCheckSettings StitchOverlap(StitchOverlap stitchOverlap)
        {
            return (PlaywrightCheckSettings)super.stitchOverlap(stitchOverlap);
        }

        public PlaywrightCheckSettings PageId(string pageId)
        {
            return (PlaywrightCheckSettings)super.pageId(pageId);
        }

        public PlaywrightCheckSettings IgnoreDisplacements(bool ignoreDisplacements)
        {
            return (PlaywrightCheckSettings)super.ignoreDisplacements(ignoreDisplacements);
        }

        public PlaywrightCheckSettings IgnoreDisplacements()
        {
            return (PlaywrightCheckSettings)super.ignoreDisplacements();
        }

        public PlaywrightCheckSettings IgnoreCaret()
        {
            return (PlaywrightCheckSettings)super.ignoreCaret();
        }

        public PlaywrightCheckSettings MatchLevel(MatchLevel matchLevel)
        {
            return (PlaywrightCheckSettings)super.matchLevel(matchLevel);
        }

        public PlaywrightCheckSettings Strict()
        {
            return (PlaywrightCheckSettings)super.strict();
        }

        public PlaywrightCheckSettings Layout()
        {
            return (PlaywrightCheckSettings)super.layout();
        }

        public PlaywrightCheckSettings Exact()
        {
            return (PlaywrightCheckSettings)super.exact();
        }

        public PlaywrightCheckSettings LazyLoad()
        {
            return (PlaywrightCheckSettings)super.lazyLoad();
        }


        public new PlaywrightCheckSettings LazyLoad(LazyLoadOptions lazyLoadOptions)
        {
            return (PlaywrightCheckSettings)base.LazyLoad(lazyLoadOptions);
        }

        public new PlaywrightCheckSettings DensityMetrics(int xDpi, int yDpi, double? scaleRatio)
        {
            return (PlaywrightCheckSettings)base.DensityMetrics(xDpi, yDpi, scaleRatio);
        }

        protected override CheckSettings Clone()
        {
            PlaywrightCheckSettings clone = new PlaywrightCheckSettings();
            PopulateClone_(clone);
            clone.targetElement_ = targetElement_;
            clone.ScrollRootElement = this.ScrollRootElement;
            clone.frameChain_.addAll(this.frameChain_);
            clone.IsDefaultLayoutBreakpointsSet = this.IsDefaultLayoutBreakpointsSet;
            clone.layoutBreakpoints_.addAll(this.layoutBreakpoints_);
            return clone;
        }

        public LazyLoadOptions GetLazyLoadOptions()
        {
            return super.getLazyLoadOptions();
        }

        /** internal */
        public GetRegion[] GetIgnoreRegions()
        {
            return ignoreRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        public GetRegion[] GetLayoutRegions()
        {
            return layoutRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        public GetRegion[] GetStrictRegions()
        {
            return strictRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        public GetRegion[] GetContentRegions()
        {
            return contentRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        public GetRegion[] GetFloatingRegions()
        {
            return floatingRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        public GetRegion[] GetAccessibilityRegions()
        {
            return accessibilityRegions.toArray(new GetRegion[0]);
        }

        /** see {@link #ignoreColors()} */
        [Obsolete]
        public PlaywrightCheckSettings Content()
        {
            return IgnoreColors();
        }

        /** see {@link #ignoreColors(String)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(string selector)
        {
            return ignoreColors(selector);
        }

        /** see {@link #ignoreColors(Locator)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(ILocator locator)
        {
            return ignoreColors(locator.elementHandle());
        }

        /** see {@link #ignoreColors(ElementHandle)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(IElementHandle element)
        {
            return ignoreColors(element);
        }

        /** see {@link #ignoreColors(String, String)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(string selector, string regionId)
        {
            return ignoreColors(selector, regionId);
        }

        /** see {@link #ignoreColors(Locator, String)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(ILocator locator, string regionId)
        {
            return ignoreColors(locator, regionId);
        }

        /** see {@link #ignoreColors(ElementHandle, String)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(IElementHandle element, string regionId)
        {
            return ignoreColors(element, regionId);
        }

        /** see {@link #ignoreColors(String, Padding)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(string selector, Padding padding)
        {
            return ignoreColors(selector, padding);
        }

        /** see {@link #ignoreColors(Locator, Padding)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(ILocator locator, Padding padding)
        {
            return ignoreColors(locator.elementHandle(), padding);
        }

        /** see {@link #ignoreColors(Locator, Padding)} */
        [Obsolete]
        public PlaywrightCheckSettings Content(IElementHandle element, Padding padding)
        {
            return ignoreColors(element, padding);
        }

        /** see {@link #ignoreColors(Region[])} )} */
        [Obsolete]
        public PlaywrightCheckSettings Content(Region[] regions)
        {
            return ignoreColors(regions);
        }

        /** see {@link #ignoreColors(Region, Region...)} )} )} */
        [Obsolete]
        public PlaywrightCheckSettings Content(Region region, params Region[] regions)
        {
            return ignoreColors(region, regions);
        }
    }
}
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

        /**
     * Specify the target as a region.
     *
     * @param region  the region
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Region(Region region)
        {
            PlaywrightCheckSettings clone = Clone();
            clone.UpdateTargetRegion(region);
            return clone;
        }

        /**
     * Specify the target as a region.
     *
     * @param selector  the css or xpath selector
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Region(string selector)
        {
            return Region(new Selector(selector));
        }

        /**
     * Specify the target as a region.
     *
     * @param locator  the locator
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Region(ILocator locator)
        {
            return Region(new Selector(locator));
        }

        /**
     * Specify the target as a region.
     *
     * @param element  the element
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Region(IElementHandle element)
        {
            return Region(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings Region(Selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.targetElement_ = selector;
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Region(Element element)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.targetElement_ = element;
            return clone;
        }

        /**
     * Specify the target as a frame.
     *
     * @param frameNameOrId  the frame's name or its id
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Frame(@Nonnull @string frameNameOrId)
        {
            FrameLocator frameLocator = new FrameLocator();
            frameLocator.setFrameNameOrId(frameNameOrId);
            return Frame(frameLocator);
        }

        /**
     * Specify the target as a frame.
     *
     * @param frameIndex  the frame's index
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Frame(@Nonnull integer frameIndex)
        {
            FrameLocator frameLocator = new FrameLocator();
            frameLocator.setFrameIndex(frameIndex);
            return Frame(frameLocator);
        }

        /**
     * Specify the target as a frame.
     *
     * @param frameLocator  the frame as a locator
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Frame(@Nonnull locator frameLocator)
        {
            return Frame(new Selector(frameLocator));
        }

        /**
     * Specify the target as a frame.
     *
     * @param frameElement  the frame as an element
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Frame(@Nonnull elementHandle frameElement)
        {
            return Frame(new Element(frameElement));
        }

        /** internal */
        private PlaywrightCheckSettings Frame(Selector selector)
        {
            FrameLocator frameLocator = new FrameLocator();
            frameLocator.setFrameSelector(selector);
            return Frame(frameLocator);
        }

        /** internal */
        private PlaywrightCheckSettings Frame(Element element)
        {
            FrameLocator frameLocator = new FrameLocator();
            frameLocator.setFrameElement(element);
            return Frame(frameLocator);
        }

        /** internal */
        private PlaywrightCheckSettings Frame(FrameLocator frameLocator)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.frameChain_.add(frameLocator);
            return clone;
        }

        /**
     * Adds an ignore region.
     *
     * @param selector  the css or xpath selector to ignore match
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull @string selector)
        {
            return Ignore(new Selector(selector));
        }

        /**
     * Adds an ignore region.
     *
     * @param locator  the locator to ignore match
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull locator locator)
        {
            return Ignore(new Selector(locator));
        }

        /**
     * Adds an ignore region.
     *
     * @param element  the element to ignore match
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull elementHandle element)
        {
            return Ignore(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(Selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.ignoreRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(Element element)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.ignoreRegions.add(element);
            return clone;
        }

        /**
     * Adds an ignore region with a region ID.
     *
     * @param selector  the css or xpath selector to ignore match
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull @string selector, @Nonnull String regionId) {
            return Ignore(new Selector(selector), regionId);
        }

        /**
     * Adds an ignore region with a region ID.
     *
     * @param locator   the locator to ignore match
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull locator locator, @Nonnull String regionId) {
            return Ignore(new Selector(locator), regionId);
        }

        /**
     * Adds an ignore region with a region ID.
     *
     * @param element   the element to ignore match
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull elementHandle element, @Nonnull String regionId) {
            return Ignore(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(Selector selector, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setRegionId(regionId);
            clone.ignoreRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(Element element, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            element.SetRegionId(regionId);
            clone.ignoreRegions.add(element);
            return clone;
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull @string selector, @Nonnull Padding padding) {
            return Ignore(new Selector(selector), padding);
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param locator  the locator to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull locator locator, @Nonnull Padding padding) {
            return Ignore(new Selector(locator), padding);
        }

        /**
     * Adds an ignore region with {@link Padding}.
     *
     * @param element  the element to ignore match
     * @param padding  the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Ignore(@Nonnull elementHandle element, @Nonnull Padding padding) {
            return Ignore(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(@Nonnull selector selector, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setPadding(padding);
            clone.ignoreRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Ignore(@Nonnull element element, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            element.setPadding(padding);
            clone.ignoreRegions.add(element);
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Ignore(@Nonnull region[] regions)
        {
            PlaywrightCheckSettings clone = this.Clone();
            for (region r :
            regions) {
                clone.ignore_(r);
            }
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Ignore(@Nonnull region region, Region... regions) {
            PlaywrightCheckSettings clone = this.Clone();
            clone.ignore_(region);
            for (Region r :
            regions) {
                clone.ignore_(r);
            }
            return clone;
        }

        /** internal */
        @Override

        protected void ignore_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region);
            this.ignoreRegions.add(simpleRegionByRectangle);
        }

        /**
     * Adds a layout region.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull @string selector)
        {
            return Layout(new Selector(selector));
        }

        /**
     * Adds a layout region.
     *
     * @param locator  the locator to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull locator locator)
        {
            return Layout(new Selector(locator));
        }

        /**
     * Adds a layout region.
     *
     * @param element  the css or xpath selector to match using the layout algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull elementHandle element)
        {
            return Layout(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.layoutRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Element element)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.layoutRegions.add(element);
            return clone;
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull @string selector, @Nonnull String regionId) {
            return Layout(new Selector(selector), regionId);
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param locator   the locator to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull locator locator, @Nonnull String regionId) {
            return Layout(new Selector(locator), regionId);
        }

        /**
     * Adds a layout region with a region ID.
     *
     * @param element   the element to match using the layout algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull elementHandle element, @Nonnull String regionId) {
            return Layout(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Selector selector, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setRegionId(regionId);
            clone.layoutRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Layout(Element element, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            element.SetRegionId(regionId);
            clone.layoutRegions.add(element);
            return clone;
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull @string selector, @Nonnull Padding padding) {
            return Layout(new Selector(selector), padding);
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param locator   the locator to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull locator locator, @Nonnull Padding padding) {
            return Layout(new Selector(locator), padding);
        }

        /**
     * Adds a layout region with {@link Padding}.
     *
     * @param element   the element to match using the layout algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Layout(@Nonnull elementHandle element, @Nonnull Padding padding) {
            return Layout(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings Layout(@Nonnull selector selector, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setPadding(padding);
            clone.layoutRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Layout(@Nonnull element element, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            element.setPadding(padding);
            clone.layoutRegions.add(element);
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Layout(@Nonnull region[] regions)
        {
            PlaywrightCheckSettings clone = this.Clone();
            for (region r :
            regions) {
                clone.layout_(r);
            }
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Layout(@Nonnull region region, Region... regions) {
            PlaywrightCheckSettings clone = this.Clone();
            clone.layout_(region);
            for (Region r :
            regions) {
                clone.layout_(r);
            }
            return clone;
        }

        /** internal */
        @Override

        protected void layout_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region);
            this.layoutRegions.add(simpleRegionByRectangle);
        }

        /**
     * Adds a strict region.
     *
     * @param selector  the css or xpath selector to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull @string selector)
        {
            return strict(new Selector(selector));
        }

        /**
     * Adds a strict region.
     *
     * @param locator  the locator to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull locator locator)
        {
            return strict(new Selector(locator));
        }

        /**
     * Adds a strict region.
     *
     * @param element  the element to match using the strict algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull elementHandle element)
        {
            return strict(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings Strict(@Nonnull selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.strictRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Strict(@Nonnull element element)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.strictRegions.add(element);
            return clone;
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param selector  the css or xpath selector to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull @string selector, @Nonnull String regionId) {
            return Strict(new Selector(selector), regionId);
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param locator   the locator to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull locator locator, @Nonnull String regionId) {
            return Strict(new Selector(locator), regionId);
        }

        /**
     * Adds a strict region with a region ID.
     *
     * @param element   the element to match using the strict algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull elementHandle element, @Nonnull String regionId) {
            return Strict(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Selector selector, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setRegionId(regionId);
            clone.strictRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Strict(Element element, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            element.SetRegionId(regionId);
            clone.strictRegions.add(element);
            return clone;
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param selector   the css or xpath selector to match using the strict algorithm
     * @param padding    the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull @string selector, @Nonnull Padding padding) {
            return Strict(new Selector(selector), padding);
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param locator   the locator to match using the strict algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull locator locator, @Nonnull Padding padding) {
            return Strict(new Selector(locator), padding);
        }

        /**
     * Adds a strict region with {@link Padding}.
     *
     * @param element   the element to match using the strict algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings Strict(@Nonnull elementHandle element, @Nonnull Padding padding) {
            return Strict(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings Strict(@Nonnull selector selector, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setPadding(padding);
            clone.strictRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Strict(@Nonnull element element, @Nonnull Padding padding) {
            PlaywrightCheckSettings clone = this.Clone();
            element.setPadding(padding);
            clone.strictRegions.add(element);
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Strict(@Nonnull region[] regions)
        {
            PlaywrightCheckSettings clone = this.Clone();
            for (region r :
            regions) {
                clone.strict_(r);
            }
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Strict(@Nonnull region region, Region... regions) {
            PlaywrightCheckSettings clone = this.Clone();
            clone.strict_(region);
            for (Region r :
            regions) {
                clone.strict_(r);
            }
            return clone;
        }

        /** internal */
        @Override

        protected void strict_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region);
            this.strictRegions.add(simpleRegionByRectangle);
        }

        /**
     * Shortcut to set the match level to {@code MatchLevel.CONTENT}.
     * @return An updated clone of this settings object.
     */
        public PlaywrightCheckSettings IgnoreColors()
        {
            return (PlaywrightCheckSettings)super.ignoreColors();
        }

        /**
     * Adds an ignore colors region.
     *
     * @param selector  the css or xpath selector to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull @string selector)
        {
            return IgnoreColors(new Selector(selector));
        }

        /**
     * Adds an ignore colors region.
     *
     * @param locator  the locator to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull locator locator)
        {
            return IgnoreColors(new Selector(locator));
        }

        /**
     * Adds an ignore colors region.
     *
     * @param element  the element to match using the ignore colors algorithm
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull elementHandle element)
        {
            return IgnoreColors(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.contentRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.contentRegions.add(element);
            return clone;
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param selector  the css or xpath to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull @string selector, @Nonnull String regionId) {
            return ignoreColors(new Selector(selector), regionId);
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param locator   the locator to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull locator locator, @Nonnull String regionId) {
            return ignoreColors(new Selector(locator), regionId);
        }

        /**
     * Adds an ignore colors region with a region ID.
     *
     * @param element   the element to match using the ignore colors algorithm
     * @param regionId  the region ID
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull elementHandle element, @Nonnull String regionId) {
            return ignoreColors(new Element(element), regionId);
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setRegionId(regionId);
            clone.contentRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element, String regionId)
        {
            PlaywrightCheckSettings clone = this.Clone();
            element.SetRegionId(regionId);
            clone.contentRegions.add(element);
            return clone;
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param selector  the css or xpath selector to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull @string selector, @Nonnull Padding padding) {
            return ignoreColors(new Selector(selector), padding);
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param locator   the locator to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull locator locator, @Nonnull Padding padding) {
            return ignoreColors(new Selector(locator), padding);
        }

        /**
     * Adds an ignore colors region with {@link Padding}.
     *
     * @param element   the element to match using the ignore colors algorithm
     * @param padding   the padding
     * @return an updated instance of this settings object
     */
        public PlaywrightCheckSettings IgnoreColors(@Nonnull elementHandle element, @Nonnull Padding padding) {
            return ignoreColors(new Element(element), padding);
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Selector selector, Padding padding)
        {
            PlaywrightCheckSettings clone = this.Clone();
            selector.setPadding(padding);
            clone.contentRegions.add(selector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings IgnoreColors(Element element, Padding padding)
        {
            PlaywrightCheckSettings clone = this.Clone();
            element.SetPadding(padding);
            clone.contentRegions.add(element);
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
            PlaywrightCheckSettings clone = this.Clone();
            for (region r :
            regions) {
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
        public PlaywrightCheckSettings IgnoreColors(@Nonnull region region, Region... regions) {
            PlaywrightCheckSettings clone = this.Clone();
            clone.ignoreColors_(region);
            for (Region r :
            regions) {
                clone.ignoreColors_(r);
            }
            return clone;
        }

        /** internal */
        protected void ignoreColors_(Region region)
        {
            SimpleRegionByRectangle simpleRegionByRectangle = new SimpleRegionByRectangle(region);
            this.contentRegions.add(simpleRegionByRectangle);
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
        public PlaywrightCheckSettings Floating(String selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.floatingRegions.add(new FloatingRegionSelector(selector, maxUpOffset, maxDownOffset, maxLeftOffset,
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
            PlaywrightCheckSettings clone = this.Clone();
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
            PlaywrightCheckSettings clone = this.Clone();
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
        public PlaywrightCheckSettings Floating(int maxOffset, String selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
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
            PlaywrightCheckSettings clone = this.Clone();
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
            PlaywrightCheckSettings clone = this.Clone();
            clone.floatingRegions.add(new FloatingRegionElement(element, maxOffset));
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Floating(int maxOffset, Region...regions)
        {
            PlaywrightCheckSettings clone = this.Clone();
            for (Region r:
            regions) {
                clone.floating_(r, maxOffset, maxOffset, maxOffset, maxOffset);
            }
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Floating(Region region, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.floating_(region, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset);
            return clone;
        }

        /** internal */
        @Override

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
        public PlaywrightCheckSettings Accessibility(@Nonnull @string selector, AccessibilityRegionType Type) {
            return Accessibility(new AccessibilitySelector(selector, type));
        }

        /**
     * Adds an accessibility region with a {@link AccessibilityRegionType}.
     *
     * @param locator       the css or xpath selector
     * @param type          the accessibility region type
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Accessibility(@Nonnull locator locator, AccessibilityRegionType Type) {
            return Accessibility(new AccessibilitySelector(locator, type));
        }

        /**
     * Adds an accessibility region with a {@link AccessibilityRegionType}.
     *
     * @param element       the element
     * @param type          the accessibility region type
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings Accessibility(@Nonnull elementHandle element, AccessibilityRegionType Type) {
            return Accessibility(new AccessibilityElement(element, type));
        }

        /** internal */
        private PlaywrightCheckSettings Accessibility(AccessibilitySelector accessibilitySelector)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.accessibilityRegions.add(accessibilitySelector);
            return clone;
        }

        /** internal */
        private PlaywrightCheckSettings Accessibility(AccessibilityElement accessibilityElement)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.accessibilityRegions.add(accessibilityElement);
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Accessibility(Region region, AccessibilityRegionType regionType)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.accessibility_(region, regionType);
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        protected void accessibility_(Region rect, AccessibilityRegionType regionType)
        {
            this.accessibilityRegions.add(new AccessibilityRegionByRectangle(rect, regionType));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param selector  the css or xpath selector
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(@Nonnull @string selector)
        {
            return ScrollRootElement(new Selector(selector));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param locator  the locator
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(@Nonnull locator locator)
        {
            return ScrollRootElement(new Selector(locator));
        }

        /**
     * Specify the scroll root element for the current target.
     *
     * @param element  the element
     * @return an updated clone of this settings object
     */
        public PlaywrightCheckSettings ScrollRootElement(@Nonnull elementHandle element)
        {
            return ScrollRootElement(new Element(element));
        }

        /** internal */
        private PlaywrightCheckSettings ScrollRootElement(Selector selector)
        {
            PlaywrightCheckSettings clone = this.Clone();
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
            PlaywrightCheckSettings clone = this.Clone();
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
        public PlaywrightCheckSettings LayoutBreakpoints(Boolean shouldSet)
        {
            return SetLayoutBreakpoints(shouldSet);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetLayoutBreakpoints(int...breakpoints)
        {
            PlaywrightCheckSettings clone = this.Clone();
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

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetLayoutBreakpoints(Boolean shouldSet)
        {
            PlaywrightCheckSettings clone = this.Clone();
            clone.IsDefaultLayoutBreakpointsSet = shouldSet;
            clone.layoutBreakpoints_.clear();
            return clone;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public List<Integer> GetLayoutBreakpoints()
        {
            return layoutBreakpoints_;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public Boolean IsDefaultLayoutBreakpointsSet()
        {
            return IsDefaultLayoutBreakpointsSet;
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Fully()
        {
            return Fully(true);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Fully(Boolean fully)
        {
            return (PlaywrightCheckSettings)super.fully(fully);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings WithName(String name)
        {
            return (PlaywrightCheckSettings)super.withName(name);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings IgnoreCaret(Boolean ignoreCaret)
        {
            return (PlaywrightCheckSettings)super.ignoreCaret(ignoreCaret);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SendDom(Boolean sendDom)
        {
            return (PlaywrightCheckSettings)super.sendDom(sendDom);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SendDom()
        {
            return (PlaywrightCheckSettings)super.sendDom();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings EnablePatterns(Boolean enablePatterns)
        {
            return (PlaywrightCheckSettings)super.enablePatterns(enablePatterns);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings EnablePatterns()
        {
            return (PlaywrightCheckSettings)super.enablePatterns();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings BeforeRenderScreenshotHook(String hook)
        {
            return (PlaywrightCheckSettings)super.beforeRenderScreenshotHook(hook);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Timeout(Integer timeoutMilliseconds)
        {
            return (PlaywrightCheckSettings)super.timeout(timeoutMilliseconds);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings UseDom(Boolean useDom)
        {
            return (PlaywrightCheckSettings)super.useDom(useDom);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings VariationGroupId(String variationGroupId)
        {
            return (PlaywrightCheckSettings)super.variationGroupId(variationGroupId);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings VisualGridOptions(VisualGridOption...options)
        {
            return (PlaywrightCheckSettings)super.visualGridOptions(options);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings WaitBeforeCapture(Integer milliSec)
        {
            return (PlaywrightCheckSettings)super.waitBeforeCapture(milliSec);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetDisableBrowserFetching(Boolean disableBrowserFetching)
        {
            return (PlaywrightCheckSettings)super.setDisableBrowserFetching(disableBrowserFetching);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings AutProxy(AutProxySettings autProxy)
        {
            return (PlaywrightCheckSettings)super.autProxy(autProxy);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetAccessibilityValidation(AccessibilitySettings accessibilitySettings)
        {
            return (PlaywrightCheckSettings)super.setAccessibilityValidation(accessibilitySettings);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings StitchMode(StitchMode stitchMode)
        {
            return (PlaywrightCheckSettings)super.stitchMode(stitchMode);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetHideScrollBars(Boolean hideScrollBars)
        {
            return (PlaywrightCheckSettings)super.setHideScrollBars(hideScrollBars);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings SetHideCaret(Boolean hideCaret)
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

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings PageId(String pageId)
        {
            return (PlaywrightCheckSettings)super.pageId(pageId);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings IgnoreDisplacements(Boolean ignoreDisplacements)
        {
            return (PlaywrightCheckSettings)super.ignoreDisplacements(ignoreDisplacements);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings IgnoreDisplacements()
        {
            return (PlaywrightCheckSettings)super.ignoreDisplacements();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings IgnoreCaret()
        {
            return (PlaywrightCheckSettings)super.ignoreCaret();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings MatchLevel(MatchLevel matchLevel)
        {
            return (PlaywrightCheckSettings)super.matchLevel(matchLevel);
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Strict()
        {
            return (PlaywrightCheckSettings)super.strict();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Layout()
        {
            return (PlaywrightCheckSettings)super.layout();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings Exact()
        {
            return (PlaywrightCheckSettings)super.exact();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings LazyLoad()
        {
            return (PlaywrightCheckSettings)super.lazyLoad();
        }

        /**
     * {@inheritDoc}
     */
        @Override

        public PlaywrightCheckSettings LazyLoad(LazyLoadOptions lazyLoadOptions)
        {
            return (PlaywrightCheckSettings)super.lazyLoad(lazyLoadOptions);
        }

        @Override

        public PlaywrightCheckSettings DensityMetrics(int xDpi, int yDpi)
        {
            return (PlaywrightCheckSettings)super.densityMetrics(xDpi, yDpi);
        }

        @Override

        public PlaywrightCheckSettings DensityMetrics(int xDpi, int yDpi, Double scaleRatio)
        {
            return (PlaywrightCheckSettings)super.densityMetrics(xDpi, yDpi, scaleRatio);
        }

        /**
     * Create a new clone of this settings object.
     *
     * @return  the clone of this settings object
     */
        @SuppressWarnings("MethodDoesntCallSuperMethod")

        @Override

        public PlaywrightCheckSettings Clone()
        {
            PlaywrightCheckSettings clone = new PlaywrightCheckSettings();
            super.populateClone(clone);
            clone.targetElement_ = this.targetElement_;
            clone.scrollRootElement = this.scrollRootElement;
            clone.frameChain_.addAll(this.frameChain_);
            clone.IsDefaultLayoutBreakpointsSet = this.IsDefaultLayoutBreakpointsSet;
            clone.layoutBreakpoints_.addAll(this.layoutBreakpoints_);
            return clone;
        }

        /** internal */
        public Reference GetTargetElement()
        {
            return targetElement_;
        }

        /** internal */
        public Reference GetScrollRootElement()
        {
            return scrollRootElement;
        }

        /** internal */
        public List<FrameLocator> GetFrameChain()
        {
            return frameChain_;
        }

        /** internal */
        @Override

        public LazyLoadOptions GetLazyLoadOptions()
        {
            return super.getLazyLoadOptions();
        }

        /** internal */
        @Override

        public GetRegion[] GetIgnoreRegions()
        {
            return ignoreRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        @Override

        public GetRegion[] GetLayoutRegions()
        {
            return layoutRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        @Override

        public GetRegion[] GetStrictRegions()
        {
            return strictRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        @Override

        public GetRegion[] GetContentRegions()
        {
            return contentRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        @Override

        public GetRegion[] GetFloatingRegions()
        {
            return floatingRegions.toArray(new GetRegion[0]);
        }

        /** internal */
        @Override

        public GetRegion[] GetAccessibilityRegions()
        {
            return accessibilityRegions.toArray(new GetRegion[0]);
        }

        /** see {@link #ignoreColors()} */
        @Deprecated
            @Override

        public PlaywrightCheckSettings Content()
        {
            return IgnoreColors();
        }

        /** see {@link #ignoreColors(String)} */
        @Deprecated

        public PlaywrightCheckSettings Content(String selector)
        {
            return ignoreColors(selector);
        }

        /** see {@link #ignoreColors(Locator)} */
        @Deprecated

        public PlaywrightCheckSettings Content(ILocator locator)
        {
            return ignoreColors(locator.elementHandle());
        }

        /** see {@link #ignoreColors(ElementHandle)} */
        @Deprecated

        public PlaywrightCheckSettings Content(IElementHandle element)
        {
            return ignoreColors(element);
        }

        /** see {@link #ignoreColors(String, String)} */
        @Deprecated

        public PlaywrightCheckSettings Content(String selector, String regionId)
        {
            return ignoreColors(selector, regionId);
        }

        /** see {@link #ignoreColors(Locator, String)} */
        @Deprecated

        public PlaywrightCheckSettings Content(ILocator locator, String regionId)
        {
            return ignoreColors(locator, regionId);
        }

        /** see {@link #ignoreColors(ElementHandle, String)} */
        @Deprecated

        public PlaywrightCheckSettings Content(IElementHandle element, String regionId)
        {
            return ignoreColors(element, regionId);
        }

        /** see {@link #ignoreColors(String, Padding)} */
        @Deprecated

        public PlaywrightCheckSettings Content(String selector, Padding padding)
        {
            return ignoreColors(selector, padding);
        }

        /** see {@link #ignoreColors(Locator, Padding)} */
        @Deprecated

        public PlaywrightCheckSettings Content(ILocator locator, Padding padding)
        {
            return ignoreColors(locator.elementHandle(), padding);
        }

        /** see {@link #ignoreColors(Locator, Padding)} */
        @Deprecated

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
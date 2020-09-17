using Applitools.VisualGrid;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Appium.Fluent
{
    public class AppiumCheckSettings : CheckSettings, IAppiumCheckTarget
    {
        private By targetSelector_;
        private IWebElement targetElement_;

        internal AppiumCheckSettings()
        {
        }

        internal AppiumCheckSettings(By targetSelector)
        {
            targetSelector_ = targetSelector;
        }

        internal AppiumCheckSettings(IWebElement targetElement)
        {
            targetElement_ = targetElement;
        }

        internal AppiumCheckSettings(Rectangle region)
            : base(region)
        {
        }

        By IAppiumCheckTarget.GetTargetSelector()
        {
            return targetSelector_;
        }

        IWebElement IAppiumCheckTarget.GetTargetElement()
        {
            return targetElement_;
        }

        public AppiumCheckSettings Region(Rectangle rect)
        {
            AppiumCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(rect);
            return clone;
        }

        public AppiumCheckSettings Region(By by)
        {
            AppiumCheckSettings clone = Clone_();
            clone.targetSelector_ = by;
            return clone;
        }

        public AppiumCheckSettings Region(IWebElement targetElement)
        {
            AppiumCheckSettings clone = Clone_();
            clone.targetElement_ = targetElement;
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selector">A selector representing a region to ignore when validating the screenshot.</param>
        /// <param name="selectors">One or more selectors representing regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(By selector, params By[] selectors)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Ignore_(new SimpleRegionBySelector(selector));
            foreach (By sel in selectors)
            {
                clone.Ignore_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IEnumerable<By> selectors)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (By sel in selectors)
            {
                clone.Ignore_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="element">An element to ignore when validating the screenshot.</param>
        /// <param name="elements">One or more elements to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IWebElement element, params IWebElement[] elements)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Ignore_(new SimpleRegionByElement(element));
            foreach (IWebElement elem in elements)
            {
                clone.Ignore_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Ignore(IEnumerable<IWebElement> elements)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement elem in elements)
            {
                clone.Ignore_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selector">A selector representing a layout region.</param>
        /// <param name="selectors">One or more selectors representing layout regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(By selector, params By[] selectors)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Layout_(new SimpleRegionBySelector(selector));
            foreach (By sel in selectors)
            {
                clone.Layout_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing layout regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IEnumerable<By> selectors)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (By sel in selectors)
            {
                clone.Layout_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="element">An element representing a layout region.</param>
        /// <param name="elements">One or more elements, each representing a layout region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IWebElement element, params IWebElement[] elements)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Layout_(new SimpleRegionByElement(element));
            foreach (IWebElement elem in elements)
            {
                clone.Layout_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a layout region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Layout(IEnumerable<IWebElement> elements)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement elem in elements)
            {
                clone.Layout_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selector">A selector representing a strict region.</param>
        /// <param name="selectors">One or more selectors representing strict regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(By selector, params By[] selectors)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Strict_(new SimpleRegionBySelector(selector));
            foreach (By sel in selectors)
            {
                clone.Strict_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }
        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing strict regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IEnumerable<By> selectors)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (By sel in selectors)
            {
                clone.Strict_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="element">An element representing a strict region.</param>
        /// <param name="elements">One or more elements, each representing a strict region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IWebElement element, params IWebElement[] elements)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Strict_(new SimpleRegionByElement(element));
            foreach (IWebElement elem in elements)
            {
                clone.Strict_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a strict region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Strict(IEnumerable<IWebElement> elements)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement elem in elements)
            {
                clone.Strict_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selector">A selector representing a content region.</param>
        /// <param name="selectors">One or more selectors representing content regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(By selector, params By[] selectors)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Content_(new SimpleRegionBySelector(selector));
            foreach (By sel in selectors)
            {
                clone.Content_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="selectors">An enumerbale of selectors representing content regions.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IEnumerable<By> selectors)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (By sel in selectors)
            {
                clone.Content_(new SimpleRegionBySelector(sel));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="element">An element representing a content region.</param>
        /// <param name="elements">One or more elements, each representing a content region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IWebElement element, params IWebElement[] elements)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Content_(new SimpleRegionByElement(element));
            foreach (IWebElement elem in elements)
            {
                clone.Content_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="elements">An enumerbale of elements, each representing a content region.</param>
        /// <returns>An updated clone of this settings object.</returns>
        public AppiumCheckSettings Content(IEnumerable<IWebElement> elements)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement elem in elements)
            {
                clone.Content_(new SimpleRegionByElement(elem));
            }
            return clone;
        }

        public AppiumCheckSettings Floating(By regionSelector, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            return clone;
        }

        public AppiumCheckSettings Floating(By regionSelector, int maxOffset = 0)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionBySelector(regionSelector, maxOffset, maxOffset, maxOffset, maxOffset));
            return clone;
        }

        public AppiumCheckSettings Floating(IWebElement element, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            return clone;
        }

        public AppiumCheckSettings Floating(IWebElement element, int maxOffset = 0)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Floating_(new FloatingRegionByElement(element, maxOffset, maxOffset, maxOffset, maxOffset));
            return clone;
        }

        public AppiumCheckSettings Floating(int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset, params IWebElement[] elementsToIgnore)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement element in elementsToIgnore)
            {
                clone.Floating_(new FloatingRegionByElement(element, maxUpOffset, maxDownOffset, maxLeftOffset, maxRightOffset));
            }
            return clone;
        }



        public AppiumCheckSettings Accessibility(By regionSelector, AccessibilityRegionType regionType)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionBySelector(regionSelector, regionType));
            return clone;
        }


        public AppiumCheckSettings Accessibility(IWebElement element, AccessibilityRegionType regionType)
        {
            AppiumCheckSettings clone = Clone_();
            clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
            return clone;
        }

        public AppiumCheckSettings Accessibility(AccessibilityRegionType regionType, params IWebElement[] elementsToIgnore)
        {
            AppiumCheckSettings clone = Clone_();
            foreach (IWebElement element in elementsToIgnore)
            {
                clone.Accessibility_(new AccessibilityRegionByElement(element, regionType));
            }
            return clone;
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
            return (AppiumCheckSettings)base.Content();
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

        private AppiumCheckSettings Clone_()
        {
            return (AppiumCheckSettings)Clone();
        }

        protected override CheckSettings Clone()
        {
            AppiumCheckSettings clone = new AppiumCheckSettings();
            base.PopulateClone_(clone);
            clone.targetElement_ = targetElement_;
            clone.targetSelector_ = targetSelector_;
            return clone;
        }
    }
}

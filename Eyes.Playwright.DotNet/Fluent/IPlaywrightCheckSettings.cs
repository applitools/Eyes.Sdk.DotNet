using System;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public interface IPlaywrightCheckSettings : ICheckSettings
    {

        PlaywrightCheckSettings Region(Region region);

        PlaywrightCheckSettings Region(string selector);

        PlaywrightCheckSettings Region(ILocator locator);

        PlaywrightCheckSettings Region(IElementHandle elementHandle);

        PlaywrightCheckSettings Frame(string frameNameOrId);

        PlaywrightCheckSettings Frame(int frameIndex);

        PlaywrightCheckSettings Frame(ILocator locator);

        PlaywrightCheckSettings Frame(IElementHandle elementHandle);

        PlaywrightCheckSettings Ignore(string selector);

        PlaywrightCheckSettings Ignore(ILocator locator);

        PlaywrightCheckSettings Ignore(IElementHandle elementHandle);

        PlaywrightCheckSettings Ignore(string selector, string regionId);

        PlaywrightCheckSettings Ignore(ILocator locator, string regionId);

        PlaywrightCheckSettings Ignore(IElementHandle elementHandle, string regionId);

        PlaywrightCheckSettings Ignore(string selector, Padding padding);

        PlaywrightCheckSettings Ignore(ILocator locator, Padding padding);

        PlaywrightCheckSettings Ignore(IElementHandle elementHandle, Padding padding);

        PlaywrightCheckSettings Layout(string selector);

        PlaywrightCheckSettings Layout(ILocator locator);

        PlaywrightCheckSettings Layout(IElementHandle elementHandle);

        PlaywrightCheckSettings Layout(string selector, string regionId);

        PlaywrightCheckSettings Layout(ILocator locator, string regionId);

        PlaywrightCheckSettings Layout(IElementHandle elementHandle, string regionId);

        PlaywrightCheckSettings Layout(string selector, Padding padding);

        PlaywrightCheckSettings Layout(ILocator locator, Padding padding);

        PlaywrightCheckSettings Layout(IElementHandle elementHandle, Padding padding);

        PlaywrightCheckSettings Strict(string selector);

        PlaywrightCheckSettings Strict(ILocator locator);

        PlaywrightCheckSettings Strict(IElementHandle elementHandle);

        PlaywrightCheckSettings Strict(string selector, string regionId);

        PlaywrightCheckSettings Strict(ILocator locator, string regionId);

        PlaywrightCheckSettings Strict(IElementHandle elementHandle, string regionId);

        PlaywrightCheckSettings Strict(string selector, Padding padding);

        PlaywrightCheckSettings Strict(ILocator locator, Padding padding);

        PlaywrightCheckSettings Strict(IElementHandle elementHandle, Padding padding);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(string selector);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(ILocator locator);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(IElementHandle elementHandle);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(string selector, string regionId);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(ILocator locator, string regionId);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(IElementHandle elementHandle, string regionId);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(Region[] regions);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(Region region, params Region[] regions);
        
        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(string selector, Padding padding);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(ILocator locator, Padding padding);

        [Obsolete("Use IgnoreColors")]
        PlaywrightCheckSettings Content(IElementHandle elementHandle, Padding padding);

        PlaywrightCheckSettings IgnoreColors(string selector);

        PlaywrightCheckSettings IgnoreColors(ILocator locator);

        PlaywrightCheckSettings IgnoreColors(IElementHandle elementHandle);

        PlaywrightCheckSettings IgnoreColors(string selector, string regionId);

        PlaywrightCheckSettings IgnoreColors(ILocator locator, string regionId);

        PlaywrightCheckSettings IgnoreColors(IElementHandle elementHandle, string regionId);

        PlaywrightCheckSettings IgnoreColors(string selector, Padding padding);

        PlaywrightCheckSettings IgnoreColors(ILocator locator, Padding padding);

        PlaywrightCheckSettings IgnoreColors(IElementHandle elementHandle, Padding padding);

        PlaywrightCheckSettings IgnoreColors(Region[] regions);
        
        PlaywrightCheckSettings IgnoreColors(Region region, params Region[] regions);
        
        PlaywrightCheckSettings Floating(string selector, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset);

        PlaywrightCheckSettings Floating(ILocator locator, int maxUpOffset, int maxDownOffset, int maxLeftOffset,
            int maxRightOffset);

        PlaywrightCheckSettings Floating(IElementHandle elementHandle, int maxUpOffset, int maxDownOffset,
            int maxLeftOffset, int maxRightOffset);

        PlaywrightCheckSettings Floating(int maxOffset, string selector);

        PlaywrightCheckSettings Floating(int maxOffset, ILocator locator);

        PlaywrightCheckSettings Floating(int maxOffset, IElementHandle elementHandle);

        PlaywrightCheckSettings Accessibility(string selector, AccessibilityRegionType type);

        PlaywrightCheckSettings Accessibility(ILocator locator, AccessibilityRegionType type);

        PlaywrightCheckSettings Accessibility(IElementHandle elementHandle, AccessibilityRegionType type);

        PlaywrightCheckSettings ScrollRootElement(string selector);

        PlaywrightCheckSettings ScrollRootElement(ILocator locator);

        PlaywrightCheckSettings ScrollRootElement(IElementHandle elementHandle);

        PlaywrightCheckSettings LayoutBreakpoints(params int[] breakpoints);

        PlaywrightCheckSettings LayoutBreakpoints(bool shouldSet);
    }
}
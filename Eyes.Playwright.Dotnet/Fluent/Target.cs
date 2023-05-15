using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public static class Target
    {
        /// <summary>
        /// Specify the target as window.
        /// </summary>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Window()
        {
            return new PlaywrightCheckSettings();
        }

        /// <summary>
        /// Specify the target as a region.
        /// </summary>
        /// <param name="region">The region</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Region(Region region)
        {
            return new PlaywrightCheckSettings().Region(region);
        }

        /// <summary>
        /// Specify the target as a region.
        /// </summary>
        /// <param name="selector">The selector for the region</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Region(string selector)
        {
            return new PlaywrightCheckSettings().Region(selector);
        }

        /// <summary>
        /// Specify the target as a region.
        /// </summary>
        /// <param name="locator">The locator for the region</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Region(ILocator locator)
        {
            return new PlaywrightCheckSettings().Region(locator);
        }

        /// <summary>
        /// Specify the target as a region.
        /// </summary>
        /// <param name="element">The element for the region</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Region(IElementHandle element)
        {
            return new PlaywrightCheckSettings().Region(element);
        }

        /// <summary>
        /// Specify the target as a frame.
        /// </summary>
        /// <param name="frameNameOrId">The frame's name or id</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Frame(string frameNameOrId)
        {
            return new PlaywrightCheckSettings().Frame(frameNameOrId);
        }

        /// <summary>
        /// Specify the target as a frame.
        /// </summary>
        /// <param name="frameIndex">The index of the frame</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Frame(int frameIndex)
        {
            return new PlaywrightCheckSettings().Frame(frameIndex);
        }

        /// <summary>
        /// Specify the target as a frame.
        /// </summary>
        /// <param name="locator">The locator for the frame</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Frame(ILocator locator)
        {
            return new PlaywrightCheckSettings().Frame(locator);
        }

        /// <summary>
        /// Specify the target as a frame.
        /// </summary>
        /// <param name="frameElement">The frame element</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Frame(IElementHandle frameElement)
        {
            return new PlaywrightCheckSettings().Frame(frameElement);
        }
    }
}
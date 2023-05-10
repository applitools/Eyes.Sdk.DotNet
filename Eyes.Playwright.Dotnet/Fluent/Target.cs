using System;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public class Target
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
        /// <param name="frameNameOrId">The locator for the frame</param>
        /// <returns>The check settings</returns>
        public static PlaywrightCheckSettings Frame(string frameNameOrId)
        {
            return new PlaywrightCheckSettings().Frame(frameNameOrId);
        }

        /**
 * Specify the target as a frame.
 *
 * @param frameIndex  the index of the frame.
 * @return the check settings
 */
        public static PlaywrightCheckSettings Frame(Integer frameIndex)
        {
            return new PlaywrightCheckSettings().frame(frameIndex);
        }

        /**
 * Specify the target as a frame.
 *
 * @param locator the frame locator.
 * @return the check settings
 */
        public static PlaywrightCheckSettings Frame(Locator locator)
        {
            return new PlaywrightCheckSettings().frame(locator);
        }

        /**
 * Specify the target as a frame.
 *
 * @param frameElement  the frame element.
 * @return the check settings
 */
        public static PlaywrightCheckSettings Frame(ElementHandle frameElement)
        {
            return new PlaywrightCheckSettings().frame(frameElement);
        }
    }
}
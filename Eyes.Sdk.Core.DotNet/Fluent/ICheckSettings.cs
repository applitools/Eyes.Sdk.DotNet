namespace Applitools
{
    using Applitools.VisualGrid;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The interface of the match settings object.
    /// </summary>
    public interface ICheckSettings
    {
        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="region">A region to ignore when validating the screenshot.</param>
        /// <param name="regions">Optional extra regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Ignore(Rectangle region, params Rectangle[] regions);

        /// <summary>
        /// Adds one or more ignore regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to ignore when validating the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Ignore(IEnumerable<Rectangle> regions);

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="region">A region to match using the Content method.</param>
        /// <param name="regions">Optional extra regions to match using the Content method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Content(Rectangle region, params Rectangle[] regions);

        /// <summary>
        /// Adds one or more content regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Content method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Content(IEnumerable<Rectangle> regions);

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="region">A region to match using the Layout method.</param>
        /// <param name="regions">Optional extra regions to match using the Layout method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Layout(Rectangle region, params Rectangle[] regions);

        /// <summary>
        /// Adds one or more layout regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Layout method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Layout(IEnumerable<Rectangle> regions);

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="region">A region to match using the Strict method.</param>
        /// <param name="regions">Optional extra regions to match using the Strict method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Strict(Rectangle region, params Rectangle[] regions);

        /// <summary>
        /// Adds one or more strict regions.
        /// </summary>
        /// <param name="regions">An enumerbale of regions to match using the Strict method.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Strict(IEnumerable<Rectangle> regions);

        /// <summary>
        /// Defines that the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Fully();

        /// <summary>
        /// Defines that the screenshot will contain the entire element or region, even if it's outside the view.
        /// </summary>
        /// <param name="isFully">Defines whether this feature is enabled.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Fully(bool isFully);

        /// <summary>
        /// Adds a floating region.
        /// </summary>
        /// <param name="maxOffset">How much each of the content rectangles can move in any direction.</param>
        /// <param name="regions">One or more content rectangles.</param>
        /// <remarks>A floating region is a a region that can be placed within the boundaries of a bigger region.</remarks>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Floating(int maxOffset, params Rectangle[] regions);

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
        ICheckSettings Floating(Rectangle region, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset);

        ICheckSettings Accessibility(AccessibilityRegionByRectangle region);
        ICheckSettings Accessibility(Rectangle region, AccessibilityRegionType regionType);

        /// <summary>
        /// Defines the timeout to use when acquiring and comparing screenshots.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Timeout(TimeSpan timeout);

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Layout"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Layout();

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Exact"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Exact();

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Strict"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Strict();

        /// <summary>
        /// Shortcut to set the match level to <see cref="MatchLevel.Content"/>.
        /// </summary>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings Content();

        /// <summary>
        /// Set the match level by which to compare the screenshot.
        /// </summary>
        /// <param name="matchLevel">The match level to use.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings MatchLevel(MatchLevel matchLevel);

        /// <summary>
        /// Defines if to detect and ignore a blinking caret in the screenshot. Defaults to <c>true</c>.
        /// </summary>
        /// <param name="ignoreCaret">Whether or not to detect and ignore a blinking caret in the screenshot.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings IgnoreCaret(bool ignoreCaret = true);

        /// <summary>
        /// A setter for the checkpoint name.
        /// </summary>
        /// <param name="name">A name by which to identify the checkpoint.</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings WithName(string name);

        /// <summary>
        /// Defines whether to send the document DOM or not.
        /// </summary>
        /// <param name="sendDom">When <c>true</c> sends the DOM to the server (the default).</param>
        /// <returns>An updated clone of this settings object.</returns>
        ICheckSettings SendDom(bool sendDom = true);

        /// <summary>
        /// Defines whether to replace the last step (e.g. in case of a mismatch) or not.
        /// </summary>
        /// <param name="replaceLast">When <c>true</c> tells the server to replace the last step.</param>
        /// <returns>An updated clone of this configuration object.</returns>
        ICheckSettings ReplaceLast(bool replaceLast = true);

        ICheckSettings UseDom(bool useDom = true);
        ICheckSettings EnablePatterns(bool useDom = true);
        ICheckSettings IgnoreDisplacements(bool ignoreDisplacements = true);

        [Obsolete("Use " + nameof(BeforeRenderScreenshotHook) + " instead.")]
        ICheckSettings ScriptHook(string jshook);

        ICheckSettings BeforeRenderScreenshotHook(string hook);

        ICheckSettings VisualGridOptions(params VisualGridOption[] options);

        ICheckSettings Clone();
    }
}

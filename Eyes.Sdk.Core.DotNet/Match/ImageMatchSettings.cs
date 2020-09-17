namespace Applitools
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using Utils.Geometry;

    /// <summary>
    /// Encapsulates match settings for the a session.
    /// </summary>
    public class ImageMatchSettings : IEquatable<ImageMatchSettings>
    {
        #region Constructors

        /// <summary>
        /// Encapsulates match settings for a test.
        /// </summary>
        /// <param name="matchLevel">The match level to use. If null, will take the value from <see cref="EyesBaseConfig.DefaultMatchSettings"/>.</param>
        /// <param name="exact">The parameters for the "Exact" match settings.</param>
        public ImageMatchSettings(MatchLevel? matchLevel = null, ExactMatchSettings exact = null)
        {
            MatchLevel = matchLevel;
            Exact = exact;
        }

        private ImageMatchSettings(ImageMatchSettings other)
        {
            if (other != null)
            {
                MatchLevel = other.MatchLevel;
                Exact = other.Exact?.Clone();
                UseDom = other.UseDom;
                IgnoreCaret = other.IgnoreCaret;
                EnablePatterns = other.EnablePatterns;
                AccessibilitySettings = other.AccessibilitySettings;
                IgnoreDisplacements = other.IgnoreDisplacements;
            }
        }

        internal ImageMatchSettings Clone()
        {
            return new ImageMatchSettings(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "strictness" level of the match.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }

        /// <summary>
        /// The parameters for the "Exact" match settings.
        /// </summary>
        public ExactMatchSettings Exact { get; set; }

        /// <summary>
        /// An array of ignore regions.
        /// </summary>
        public IMutableRegion[] Ignore { get; set; } = new IMutableRegion[0];

        /// <summary>
        /// An array of strict regions.
        /// </summary>
        public IMutableRegion[] Strict { get; set; } = new IMutableRegion[0];

        /// <summary>
        /// An array of content regions.
        /// </summary>
        public IMutableRegion[] Content { get; set; } = new IMutableRegion[0];

        /// <summary>
        /// An array of layout regions.
        /// </summary>
        public IMutableRegion[] Layout { get; set; } = new IMutableRegion[0];

        /// <summary>
        /// An array of "floating" regions.
        /// </summary>
        public FloatingMatchSettings[] Floating { get; set; } = new FloatingMatchSettings[0];

        /// <summary>
        /// An array of accessibility regions.
        /// </summary>
        public AccessibilityRegionByRectangle[] Accessibility { get; set; } = new AccessibilityRegionByRectangle[0];

        public AccessibilitySettings AccessibilitySettings { get; set; }

        /// <summary>
        /// Whether or not to ignore the blinking caret if one was captured in the screenshot.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? IgnoreCaret { get; set; }

        /// <summary>
        /// Whether or not to use the page DOM when computing the layout of the page.
        /// </summary>
        public bool UseDom { get; set; }
        public bool EnablePatterns { get; set; }
        public bool IgnoreDisplacements { get; set; }

        public bool Equals(ImageMatchSettings other)
        {
            bool result = other.UseDom == UseDom;
            result &= other.MatchLevel == MatchLevel;
            result &= other.IgnoreCaret == IgnoreCaret;
            result &= other.EnablePatterns == EnablePatterns;
            return result;
        }

        #endregion
    }
}

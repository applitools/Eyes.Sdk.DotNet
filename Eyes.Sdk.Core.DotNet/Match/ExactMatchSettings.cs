using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Encapsulates settings for the "Exact" match level.
    /// </summary>
    public class ExactMatchSettings
    {
        public ExactMatchSettings() { }

        private ExactMatchSettings(ExactMatchSettings other)
        {
            if (other != null)
            {
                MinDiffIntensity = other.MinDiffIntensity;
                MinDiffHeight = other.MinDiffHeight;
                MinDiffWidth = other.MinDiffWidth;
                MatchThreshold = other.MatchThreshold;
            }
        }

        /// <summary>
        /// Minimal non-ignorable pixel intensity difference.
        /// </summary>
        public int MinDiffIntensity { get; set; }

        /// <summary>
        /// Minimal non-ignorable diff region width.
        /// </summary>
        public int MinDiffWidth { get; set; }

        /// <summary>
        /// Minimal non-ignorable diff region height.
        /// </summary>
        public int MinDiffHeight { get; set; }

        /// <summary>
        /// The ratio of differing pixels above which images are considered mismatching.
        /// </summary>
        public double MatchThreshold { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "[min diff intensity: {0}, min diff width {1}, " +
                    "min diff height {2}, match threshold: {3}]".Fmt(MinDiffIntensity,
                    MinDiffWidth, MinDiffHeight, MatchThreshold);
        }

        internal ExactMatchSettings Clone()
        {
            return new ExactMatchSettings(this);
        }
    }
}

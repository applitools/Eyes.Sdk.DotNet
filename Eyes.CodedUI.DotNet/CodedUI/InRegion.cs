[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI
{
    using System.Drawing;
    using Applitools.Utils;
    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// An <see cref="InRegion"/> API.
    /// </summary>
    public class InRegion
    {
        #region Fields

        private readonly Eyes eyes_;
        private readonly InRegionBase inRegion_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegion"/> instance.
        /// </summary>
        public InRegion(Eyes eyes, InRegionBase inRegion)
        {
            ArgumentGuard.NotNull(eyes, nameof(eyes));
            ArgumentGuard.NotNull(inRegion, nameof(inRegion));

            eyes_ = eyes;
            inRegion_ = inRegion;
        }

        #endregion

        /// <summary>
        /// Add another window region.
        /// </summary>
        public InRegions And(Rectangle bounds)
        {
            return new InRegions(eyes_, inRegion_.And(bounds));
        }

        /// <summary>
        /// Add another window region.
        /// </summary>
        public InRegions And(UITestControl control)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            var region = eyes_.ScreenToClient(control.BoundingRectangle);
            return new InRegions(eyes_, inRegion_.And(region));
        }

        /// <summary>
        /// Gets the text found in this region.
        /// </summary>
        public string GetText()
        {
            return inRegion_.GetText();
        }
    }
}

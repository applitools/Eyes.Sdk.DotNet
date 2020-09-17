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
    /// An <see cref="InRegions"/> API.
    /// </summary>
    public class InRegions
    {
        #region Fields

        private readonly Eyes eyes_;
        private readonly InRegionsBase inRegions_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegions"/> instance.
        /// </summary>
        public InRegions(Eyes eyes, InRegionsBase inRegions)
        {
            ArgumentGuard.NotNull(eyes, nameof(eyes));
            ArgumentGuard.NotNull(inRegions, nameof(inRegions));

            eyes_ = eyes;
            inRegions_ = inRegions;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds additional region to this sequence of regions.
        /// </summary>
        public InRegions And(Rectangle bounds)
        {
            inRegions_.And(bounds);
            return this;
        }

        /// <summary>
        /// Adds additional region to this sequence of regions.
        /// </summary>
        public InRegions And(UITestControl control)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            var region = eyes_.ScreenToClient(control.BoundingRectangle);
            inRegions_.And(region);
            return this;
        }

        /// <summary>
        /// Gets the text found in this sequence of regions.
        /// </summary>
        public string[] GetText()
        {
            return inRegions_.GetText();
        }

        #endregion
    }
}

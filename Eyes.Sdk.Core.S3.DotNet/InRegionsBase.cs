namespace Applitools
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using Utils;

    /// <summary>
    /// An <see cref="InRegionsBase"/> API.
    /// </summary>
    public class InRegionsBase
    {
        #region Fields

        private readonly CreateImageHandler createImage_;
        private readonly GetTextHandler getText_;
        private readonly IList<InRegionBase> regions_ = new List<InRegionBase>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegionsBase"/> instance.
        /// </summary>
        public InRegionsBase(CreateImageHandler createImage, GetTextHandler getText)
        {
            ArgumentGuard.NotNull(createImage, nameof(createImage));
            ArgumentGuard.NotNull(getText, nameof(getText));

            createImage_ = createImage;
            getText_ = getText;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds additional region to this sequence of regions.
        /// </summary>
        public InRegionsBase And(Rectangle bounds)
        {
            regions_.Add(new InRegionBase(null, bounds, createImage_, getText_));
            return this;
        }

        /// <summary>
        /// Gets the text found in this sequence of regions.
        /// </summary>
        public string[] GetText()
        {
            string[] texts = new string[regions_.Count];
            for (int i = 0; i < texts.Length; ++i)
            {
                texts[i] = regions_[i].GetText();
            }
            return texts;
        }

        #endregion
    }
}

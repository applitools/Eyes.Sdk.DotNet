namespace Applitools.Selenium
{
    using System.Drawing;
    using Applitools.Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// An <see cref="InRegions"/> API.
    /// </summary>
    public class InRegions
    {
        #region Fields

        private readonly IWebDriver driver_;
        private readonly InRegionsBase inRegions_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegions"/> instance.
        /// </summary>
        public InRegions(IWebDriver driver, InRegionsBase inRegions)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(inRegions, nameof(inRegions));

            driver_ = driver;
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
        public InRegions And(By selector)
        {
            ArgumentGuard.NotNull(selector, nameof(selector));

            var element = driver_.FindElement(selector);
            inRegions_.And(new Rectangle(element.Location, element.Size));
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

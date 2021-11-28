namespace Applitools.Selenium
{
    using System.Drawing;
    using Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// An <see cref="InRegion"/> API.
    /// </summary>
    public class InRegion
    {
        #region Fields

        private readonly IWebDriver driver_;
        private readonly InRegionBase inRegion_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegion"/> instance.
        /// </summary>
        public InRegion(IWebDriver driver, InRegionBase inRegion)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(inRegion, nameof(inRegion));

            driver_ = driver;
            inRegion_ = inRegion;
        }

        #endregion

        /// <summary>
        /// Add another window region.
        /// </summary>
        public InRegions And(Rectangle bounds)
        {
            return new InRegions(driver_, inRegion_.And(bounds));
        }

        /// <summary>
        /// Add another window region.
        /// </summary>
        public InRegions And(By selector)
        {
            ArgumentGuard.NotNull(selector, nameof(selector));
            
            var element = driver_.FindElement(selector);
            return new InRegions(driver_, inRegion_.And(new Rectangle(element.Location, element.Size)));
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

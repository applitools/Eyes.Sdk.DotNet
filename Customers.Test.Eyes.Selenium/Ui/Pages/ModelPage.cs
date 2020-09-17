namespace Applitools.Ui.Pages
{
    using System;
    using System.Threading;
    using Applitools.Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// Base class for model related pages.
    /// </summary>
    public abstract class ModelPage : Page
    {
        protected const string ModelViewButtonSelector = ".zoomButton.zoomFit";

        private const string WorkingAreaClassName_ = "WorkingArea";
        private const string ZoomMenuButtonSelector_ = ".WindowInspectorMenuItem.zoomMenuItem";
        private const string ZoomInButtonSelector_ = ".zoomButton.zoomIn";
        private const string ZoomOutButtonSelector_ = ".zoomButton.zoomOut";
        private const int MousewheelScrollFactor_ = 500;

        private readonly IWebElement workingAreaWebElement_;

        public ModelPage(IWebDriver driver)
            : base(driver)
        {
            workingAreaWebElement_ = 
                SeleniumUtils.WaitForElement(driver, By.ClassName(WorkingAreaClassName_));
        }

        /// <summary>
        /// Zooms in or out by turning the mouse wheel by the specified (positive
        /// or negative) delta.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public virtual ModelPage ZoomByMouseWheel(int delta)
        {
            ArgumentGuard.NotEquals(delta, 0, nameof(delta));

            SeleniumUtils.ScrollElement(
                Driver, workingAreaWebElement_, delta * MousewheelScrollFactor_);
            WaitForZoomTransitionToEnd_();
            return this;
        }

        /// <summary>
        /// Zooms in or out by clicking on zoom buttons.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public virtual ModelPage ZoomByMenuButton(int delta)
        {
            ArgumentGuard.NotEquals(delta, 0, nameof(delta));

            Driver.FindElement(By.CssSelector(ZoomMenuButtonSelector_)).Click();
            IWebElement zoomButton;
            if (delta > 0)
            {
                zoomButton = Driver.FindElement(
                        By.CssSelector(ZoomInButtonSelector_));
            }
            else
            {
                zoomButton = Driver.FindElement(
                        By.CssSelector(ZoomOutButtonSelector_));
            }

            SeleniumUtils.MoveToElement(Driver, zoomButton);

            delta = Math.Abs(delta);
            while (--delta >= 0)
            {
                zoomButton.Click();
                WaitForZoomTransitionToEnd_();
            }

            SeleniumUtils.MoveToTopLeft(Driver);
            return this;
        }

        private void WaitForZoomTransitionToEnd_()
        {
            Thread.Sleep(500);
        }
    }
}

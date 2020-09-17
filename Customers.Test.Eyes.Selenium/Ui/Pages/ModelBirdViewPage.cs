namespace Applitools.Ui.Pages
{
    using System;
    using System.Threading;
    using Applitools.Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// A model bird-view page.
    /// </summary>
    public class ModelBirdViewPage : ModelPage
    {
        public ModelBirdViewPage(IWebDriver driver) :
            base(driver)
        {
        }

        /// <summary>
        /// Wait for the bird view main screen to stabilize and for all animations
        /// to end.
        /// </summary>
        public override void WaitUntilReady()
        {
            SeleniumUtils.WaitForEvent(Driver, "birdViewReady");
            Thread.Sleep(1000);
        }

        public ModelPage ClickWindowImage(string windowId)
        {
            var windowElement = GetWindowImageElement_(windowId);
            windowElement.Click();
            SeleniumUtils.MoveToElement(Driver, windowElement, 0, -40);
            WaitForBirdViewNewRoot_();

            return this;
        }

        /// <summary>
        /// Double clicks the image of the window of the input id.
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        public ModelPage DoubleClickWindowImage(string windowId)
        {
            var windowElement = GetWindowImageElement_(windowId);
            SeleniumUtils.DoubleClick(Driver, windowElement);
            SeleniumUtils.MoveToElement(Driver, windowElement, 0, -40);
            ModelWindowPage windowPage = new ModelWindowPage(Driver);
            windowPage.WaitUntilReady();
            return windowPage;
        }

        /// <summary>
        /// Hovers over the window image of the input id.
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        public ModelPage HoverOverWindow(string windowId)
        {
            SeleniumUtils.MoveToElement(Driver, GetWindowImageElement_(windowId));
            Thread.Sleep(2000);

            return this;
        }

        public override ModelPage ZoomByMouseWheel(int delta)
        {
            return (ModelBirdViewPage)base.ZoomByMouseWheel(delta);
        }

        public override ModelPage ZoomByMenuButton(int delta)
        {
            return (ModelBirdViewPage)base.ZoomByMenuButton(delta);
        }

        /// <summary>
        /// Returns the web-element that holds the image of the model window of
        /// the specified id.
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        private IWebElement GetWindowImageElement_(string windowId)
        {
            ArgumentGuard.NotNull(Driver, nameof(Driver));
            ArgumentGuard.NotNull(windowId, nameof(windowId));

            /// The div element with the ID of the window, is not click-able
            /// so we need to select the inner "img" element.
            return Driver.FindElement(By.Id(windowId))
                    .FindElement(By.TagName("img"));
        }

        /// <summary>
        /// Waits for graph to stabilize following replacement of the central
        /// window image.
        /// </summary>
        private void WaitForBirdViewNewRoot_()
        {
            SeleniumUtils.WaitForEvent(Driver, "birdViewNewRoot");
            Thread.Sleep(1000);
        }
    }
}

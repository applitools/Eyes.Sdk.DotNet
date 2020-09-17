namespace Applitools.Selenium.Scrolling
{
    using Applitools.Utils;
    using OpenQA.Selenium;
    using System;
    using System.Drawing;

    /// <summary>
    /// CSS Translate Transform Position Provider - will move the element graphics without recomputing the page layout.
    /// </summary>
    internal class CssTranslatePositionProvider : IPositionProvider, ISeleniumPositionProvider
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="executor">The JavaScript executor to use.</param>
        public CssTranslatePositionProvider(Logger logger, IEyesJsExecutor executor, IWebElement scrollRootElement)
        {
            ArgumentGuard.NotNull(executor, nameof(executor));
            ArgumentGuard.NotNull(scrollRootElement, nameof(scrollRootElement));

            logger_ = logger;
            executor_ = executor;
            ScrolledElement = scrollRootElement;
        }

        #endregion

        private const string JSSetTransform_ =
            "var originalTransform = arguments[0].style.transform;" +
            "arguments[0].style.transform = '{0}';" +
            "return originalTransform;";

        private const string JSGetEntirePageSize_ =
            "var width = Math.max(arguments[0].clientWidth, arguments[0].scrollWidth);" +
            "var height = Math.max(arguments[0].clientHeight, arguments[0].scrollHeight);" +
            "return (width + ';' + height);";

        private const string JSGetCurrentTransform_ =
            "return arguments[0].style.transform;";

        #region Fields and Properties

        private Logger logger_;
        private IEyesJsExecutor executor_;

        public IWebElement ScrolledElement { get; }

        private Point LastSetPosition_ { get; set; }

        #endregion

        #region Methods

        /// <returns>The current position</returns>
        public Point GetCurrentPosition()
        {
            return LastSetPosition_;
        }

        /// <summary>
        /// Sets the current position.
        /// </summary>
        /// <param name="pos">The position to set.</param>
        /// <returns>The actual position the element had scrolled to.</returns>
        public Point SetPosition(Point pos)
        {
            if (pos.IsEmpty)
            {
                logger_.Verbose("setting position of {0} to \"none\"", ScrolledElement);
                executor_.ExecuteScript($"arguments[0].style.transform = 'none';", ScrolledElement);
            }
            else
            {
                logger_.Verbose("setting position of {0} to {1}", ScrolledElement, pos);
                Point negatedPos = new Point(-pos.X, -pos.Y);
                Point negatedPos2 = new Point(10, -pos.Y);
                executor_.ExecuteScript(
                    $"arguments[0].style.transform = 'translate({negatedPos2.X}px, {negatedPos2.Y}px)';", ScrolledElement);

                executor_.ExecuteScript(
                    $"arguments[0].style.transform = 'translate({negatedPos.X}px, {negatedPos.Y}px)';", ScrolledElement);
            }
            LastSetPosition_ = pos;
            return LastSetPosition_;
        }

        /// <returns>The entire size of the current context.</returns>
        public Size GetEntireSize()
        {
            logger_.Verbose("enter (scrollRootElement_: {0})", ScrolledElement);
            string entireSizeStr = (string)executor_.ExecuteScript(JSGetEntirePageSize_, ScrolledElement);
            string[] wh = entireSizeStr.Split(';');
            Size size = new Size(Convert.ToInt32(wh[0]), Convert.ToInt32(wh[1]));
            logger_.Verbose(size.ToString());
            return size;
        }

        /// <summary>
        /// Get the current state of the position provider. This is different from 
        /// <c>GetCurrentPosition</c> in that the state of the position provider includes
        /// also the CSS "transform" style in addition to the coordinates.
        /// </summary>
        /// <returns>The current state of the position provider, which can later be restored by 
        /// passing it as a parameter to <c>RestoreState</c></returns>
        public PositionMemento GetState()
        {
            logger_.Verbose("enter (scrollRootElement_: {0})", ScrolledElement);
            return new CssTranslatePositionMemento(
                (string)executor_.ExecuteScript(JSGetCurrentTransform_, ScrolledElement), LastSetPosition_);
        }

        /// <summary>
        /// Restores the state of the position provider to the state provided as a parameter.
        /// </summary>
        /// <param name="state">The state to restore to.</param>
        public void RestoreState(PositionMemento state)
        {
            logger_.Verbose("enter (scrollRootElement_: {0}), state: {1}", ScrolledElement, state);
            executor_.ExecuteScript(JSSetTransform_.Fmt(((CssTranslatePositionMemento)state).Transform), ScrolledElement);
            LastSetPosition_ = ((CssTranslatePositionMemento)state).Position;
        }

        public override string ToString()
        {
            return $"{nameof(CssTranslatePositionProvider)} (Last set position = {LastSetPosition_} ; scrollRootElement_ = {ScrolledElement})";
        }

        #endregion
    }
}

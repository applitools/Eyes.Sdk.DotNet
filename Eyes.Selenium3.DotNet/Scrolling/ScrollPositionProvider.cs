namespace Applitools.Selenium.Scrolling
{
    using Applitools.Utils;
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    internal class ScrollPositionProvider : IPositionProvider, ISeleniumPositionProvider, IEquatable<ScrollPositionProvider>
    {
        #region Constructors

        public ScrollPositionProvider(Logger logger, IEyesJsExecutor executor, IWebElement scrollRootElement)
        {
            ArgumentGuard.NotNull(executor, nameof(executor));
            ArgumentGuard.NotNull(scrollRootElement, nameof(scrollRootElement));

            logger_ = logger;
            executor_ = executor;
            ScrolledElement = scrollRootElement;
        }

        #endregion

        protected Logger logger_;
        protected IEyesJsExecutor executor_;

        public IWebElement ScrolledElement { get; }

        #region Methods

        public Point GetCurrentPosition()
        {
            return GetCurrentPosition(executor_, ScrolledElement);
        }

        internal static Point GetCurrentPosition(IEyesJsExecutor executor, IWebElement scrollRootElement)
        {
            var position = executor.ExecuteScript("return arguments[0].scrollLeft+';'+arguments[0].scrollTop;", scrollRootElement);
            return EyesSeleniumUtils.ParseLocationString(position);
        }

        protected virtual string SetPositionCommandFormat => "arguments[0].scrollLeft={0};arguments[0].scrollTop={1}; return (arguments[0].scrollLeft+';'+arguments[0].scrollTop);";

        public virtual Point SetPosition(Point pos)
        {
            logger_.Verbose("setting position of {0} to {1}", ScrolledElement, pos);
            var position = executor_.ExecuteScript(string.Format(SetPositionCommandFormat, pos.X, pos.Y), ScrolledElement);
            return EyesSeleniumUtils.ParseLocationString(position);
        }

        /// <returns>The entire size of the current context.</returns>
        public Size GetEntireSize()
        {
            Size size = EyesRemoteWebElement.GetEntireSize(ScrolledElement, executor_, logger_);
            return size;
        }

        public PositionMemento GetState()
        {
            logger_.Verbose("enter (scrollRootElement_: {0})", ScrolledElement);
            return new ScrollPositionMemento(GetCurrentPosition());
        }

        public void RestoreState(PositionMemento state)
        {
            SetPosition(((ScrollPositionMemento)state).Position);
        }

        public void ScrollToBottomRight()
        {
            JSBrowserCommands.WithReturn.ScrollToBottomRight(executor_);
        }

        public bool Equals(ScrollPositionProvider other)
        {
            return this.ScrolledElement.Equals(other.ScrolledElement);
        }

        public override bool Equals(object other)
        {
            if (other is ScrollPositionProvider scrollPositionProvider)
            {
                return Equals(scrollPositionProvider);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return -1500623415 + EqualityComparer<IWebElement>.Default.GetHashCode(ScrolledElement);
        }

        #endregion
    }
}

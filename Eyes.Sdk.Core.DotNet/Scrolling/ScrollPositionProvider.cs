namespace Applitools.Utils
{
    using System.Drawing;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    internal class ScrollPositionProvider : IPositionProvider
    {
        #region Constructors

        public ScrollPositionProvider(Logger logger, IEyesJsExecutor executor)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(executor, nameof(executor));

            logger_ = logger;
            Executor_ = executor;
        }

        #endregion

        #region Properties

        private Logger logger_;
        private IEyesJsExecutor Executor_ { get; set; }

        #endregion 

        #region Methods

        public Point GetCurrentPosition()
        {
            return JSBrowserCommands.WithReturn.GetCurrentScrollPosition(Executor_);
        }

        public Point SetPosition(Point pos)
        {
            return JSBrowserCommands.WithReturn.ScrollTo(pos, Executor_);
        }

        public Size GetEntireSize()
        {
            logger_.Verbose("enter");
            Size size = JSBrowserCommands.WithReturn.GetCurrentFrameContentEntireSize(Executor_);
            logger_.Verbose(size.ToString());
            return size;
        }

        public PositionMemento GetState()
        {
            return new ScrollPositionMemento(GetCurrentPosition());
        }

        public void RestoreState(PositionMemento state)
        {
            SetPosition(((ScrollPositionMemento)state).Position);
        }

        public void ScrollToBottomRight()
        {
            JSBrowserCommands.WithReturn.ScrollToBottomRight(Executor_);
        }

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

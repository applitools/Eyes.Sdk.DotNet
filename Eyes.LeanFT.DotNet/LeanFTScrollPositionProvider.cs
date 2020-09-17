namespace Applitools
{
    using System.Drawing;
    using Utils;

    public class LeanFTScrollPositionProvider : IPositionProvider
    {
        #region Constructors

        public LeanFTScrollPositionProvider(IEyesJsExecutor executor)
        {
            ArgumentGuard.NotNull(executor, nameof(executor));

            Executor_ = executor;
        }

        #endregion

        #region Properties

        private IEyesJsExecutor Executor_ { get; set; }

        #endregion 

        #region Methods

        public Point GetCurrentPosition() 
        {
            return JSBrowserCommands.WithoutReturn.GetCurrentScrollPosition(Executor_);
        }

        public Point SetPosition(Point pos)
        {
            return JSBrowserCommands.WithoutReturn.ScrollTo(pos, Executor_);
        }

        public Size GetEntireSize() 
        {
            return JSBrowserCommands.WithoutReturn.GetCurrentFrameContentEntireSize(Executor_);
        }

        public PositionMemento GetState()
        {
            return new ScrollPositionMemento(GetCurrentPosition());
        }

        public void RestoreState(PositionMemento state)
        {
            Point pos = new Point(((ScrollPositionMemento)state).X, 
                ((ScrollPositionMemento)state).Y);

            SetPosition(pos);
        }

        #endregion
    }
}

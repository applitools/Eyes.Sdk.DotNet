namespace Applitools
{
    using System.Drawing;
    using Utils;

    public class LeanFTCssTranslatePositionProvider : IPositionProvider
    {
        #region Constructors

        public LeanFTCssTranslatePositionProvider(IEyesJsExecutor executor)
        {
            ArgumentGuard.NotNull(executor, nameof(executor));

            Executor_ = executor;
        }

        #endregion

        #region Properties

        private IEyesJsExecutor Executor_ { get; set; }
        private Point LastSetPosition_ { get; set; }

        #endregion

        #region Methods

        public Point GetCurrentPosition()
        {
            return LastSetPosition_;
        }

        public Point SetPosition(Point pos)
        {
            JSBrowserCommands.WithoutReturn.TranslateTo(pos, Executor_);
            LastSetPosition_ = pos;
            return LastSetPosition_;
        }

        public Size GetEntireSize()
        {
            return JSBrowserCommands.WithoutReturn.GetEntirePageSize(Executor_);
        }

        public PositionMemento GetState()
        {
            return new CssTranslatePositionMemento(
                (string) JSBrowserCommands.WithoutReturn.GetCurrentTransform(Executor_), LastSetPosition_);
        }

        public void RestoreState(PositionMemento state)
        {
            JSBrowserCommands.WithoutReturn.SetTranform(((CssTranslatePositionMemento)state).Transform, Executor_);
            LastSetPosition_ = ((CssTranslatePositionMemento)state).Position;
        }

        #endregion
    }
}

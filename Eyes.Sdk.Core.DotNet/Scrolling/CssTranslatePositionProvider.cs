namespace Applitools.Utils
{
    using System.Drawing;

    /// <summary>
    /// CSS Translate Transform Position Provider - will move the element graphics without recomputing the page layout.
    /// </summary>
    internal class CssTranslatePositionProvider : IPositionProvider
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="executor">The JavaScript executor to use.</param>
        public CssTranslatePositionProvider(Logger logger, IEyesJsExecutor executor)
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
            JSBrowserCommands.WithReturn.TranslateTo(pos, Executor_);
            LastSetPosition_ = pos;
            return LastSetPosition_;
        }

        /// <returns>The entire size of the current context.</returns>
        public Size GetEntireSize()
        {
            Size size = JSBrowserCommands.WithReturn.GetEntirePageSize(Executor_);
            logger_.Log(TraceLevel.Info, Stage.General, new { size });
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
            return new CssTranslatePositionMemento(
                (string)JSBrowserCommands.WithReturn.GetCurrentTransform(Executor_), LastSetPosition_);
        }

        /// <summary>
        /// Restores the state of the position provider to the state provided as a parameter.
        /// </summary>
        /// <param name="state">The state to restore to.</param>
        public void RestoreState(PositionMemento state)
        {
            JSBrowserCommands.WithReturn.SetTranform(((CssTranslatePositionMemento)state).Transform, Executor_);
            LastSetPosition_ = ((CssTranslatePositionMemento)state).Position;
        }

        #endregion
    }
}

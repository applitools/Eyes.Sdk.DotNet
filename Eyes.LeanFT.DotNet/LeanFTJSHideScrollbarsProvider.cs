namespace Applitools
{
    using Applitools.Utils;

    public class LeanFTJSHideScrollbarsProvider : IHideScrollbarsProvider
    {

        #region Constructors

        public LeanFTJSHideScrollbarsProvider(IEyesJsExecutor executor)
        {
            ArgumentGuard.NotNull(executor, nameof(executor));

            Executor_ = executor;
        }

        #endregion


        #region Properties

        private IEyesJsExecutor Executor_ { get; set; }
        private string OriginalScrollbarsState_ { get; set; }
        private bool HideScrollbarsCalled_ { get; set; }

        #endregion


        #region Methods

        public void HideScrollbars()
        {
            OriginalScrollbarsState_ = (string)JSBrowserCommands.WithoutReturn.HideScrollbars(Executor_);
            HideScrollbarsCalled_ = true;
        }

        public void RestoreScrollbarsState()
        {
            ArgumentGuard.NotEquals(HideScrollbarsCalled_, false, "HideScrollbars must be called first!");
            JSBrowserCommands.WithoutReturn.SetOverflow(OriginalScrollbarsState_, Executor_);
        }

        #endregion
    }
}

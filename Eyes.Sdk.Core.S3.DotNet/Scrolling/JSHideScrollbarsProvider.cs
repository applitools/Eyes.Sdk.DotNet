namespace Applitools.Utils
{
    public class JSHideScrollbarsProvider : IHideScrollbarsProvider
    {

        #region Constructors

        public JSHideScrollbarsProvider(IEyesJsExecutor executor)
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
            OriginalScrollbarsState_ = (string) JSBrowserCommands.WithReturn.HideScrollbars(Executor_);
            HideScrollbarsCalled_ = true;
        }

        public void RestoreScrollbarsState()
        {
            ArgumentGuard.NotEquals(HideScrollbarsCalled_, false, "HideScrollbars must be called first!");
            JSBrowserCommands.WithReturn.SetOverflow(OriginalScrollbarsState_,Executor_);
        }

        #endregion
    }
}

namespace Applitools
{
    public class NullHideScrollbarsProvider : Utils.IHideScrollbarsProvider
    {
        public static readonly NullHideScrollbarsProvider Instance = new NullHideScrollbarsProvider();

        #region Methods

        public void HideScrollbars()
        {
            // Do nothing.
        }
        public void RestoreScrollbarsState()
        {
            // Do nothing.
        }

        #endregion
    }
}

namespace Applitools.Correlate.Capture.Win
{
    using System.Security;
    using System.Windows.Forms;
    using Applitools.Utils.Gui.Win;

    /// <summary>
    /// A <see cref="Form"/> that cannot become active.
    /// </summary>
    public class InactiveForm : Form
    {
        #region Methods

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security", 
            "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase",
            Justification = "Workaround Code Analysis bug")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "Workaround a Code Analysis bug")]
        protected override CreateParams CreateParams
        {
            [SecuritySafeCritical]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |=
                    unchecked((int)SafeNativeMethods.ExtendedWindowStyles.WS_EX_NOACTIVATE);
                return createParams;
            }
        }

        #endregion
    }
}

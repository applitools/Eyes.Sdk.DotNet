namespace Applitools
{
    /// <summary>
    /// Determines how detected failures are reported.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming", 
        "CA1717:OnlyFlagsEnumsShouldHavePluralNames",
        Justification = "Consistent with other SDKs")]
    public enum FailureReports
    {
        /// <summary>
        /// Failures are reported immediately when they are detected.
        /// </summary>
        Immediate,

        /// <summary>
        /// Failures are reported when tests are completed (i.e., when
        /// <c>Eyes.close()</c> is called).
        /// </summary>
        OnClose,
    }
}

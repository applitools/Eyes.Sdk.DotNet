namespace Applitools
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Encapsulates scaling logic.
    /// </summary>
    [ComVisible(true)]
    public interface IScaleProvider
    {
        ///<summary>
        /// The ratio by which an image will be scaled.
        /// </summary>
        double ScaleRatio { get; }
    }
}

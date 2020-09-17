namespace Applitools.Utils
{
    using System.Runtime.InteropServices;
    using System.Drawing;

    /// <summary>
    /// Position provider API.
    /// </summary>
    [ComVisible(true)]
    public interface IPositionProvider
    {
        #region Methods

        /// <returns>The current position</returns>
        Point GetCurrentPosition();

        /// <summary>
        /// Sets the current position.
        /// </summary>
        /// <param name="pos">The position to set.</param>
        /// <returns>The actual position the element had scrolled to.</returns>
        Point SetPosition(Point pos);

        /// <returns>The entire size of the current context.</returns>
        Size GetEntireSize();

        /// <summary>
        /// Get the current state of the position provider. This is different from 
        /// <c>GetCurrentPosition</c> in that the state of the position provider might include 
        /// other data than just the coordinates. For example a CSS translation based position 
        /// provider (in WebDriver based SDKs), might save the entire "transform" style value as 
        /// its state.
        /// </summary>
        /// <returns>The current state of the position provider, which can later be restored by 
        /// passing it as a parameter to <c>RestoreState</c></returns>
        PositionMemento GetState();

        /// <summary>
        /// Restores the state of the position provider to the state provided as a parameter.
        /// </summary>
        /// <param name="state">The state to restore to.</param>
        void RestoreState(PositionMemento state);

        #endregion
    }
}

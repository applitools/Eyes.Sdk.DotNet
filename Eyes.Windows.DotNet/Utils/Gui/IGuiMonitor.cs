namespace Applitools.Utils.Gui
{
    using System;
    using System.Security;

    /// <summary>
    /// A GUI monitor interface
    /// </summary>
    public interface IGuiMonitor : IDisposable
    {
        #region Events

        /// <summary>
        /// Fired serially when GUI events occurs.
        /// </summary>
        event EventHandler<GuiEventArgs> GuiEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the set of events to be reported by this monitor.
        /// </summary>
        GuiEventTypes EventMask 
        { 
            get;
            set; 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts monitoring.
        /// </summary>
        [SecurityCritical]
        void StartMonitoring();

        /// <summary>
        /// Stops monitoring.
        /// </summary>
        void StopMonitoring();

        #endregion
    }
}

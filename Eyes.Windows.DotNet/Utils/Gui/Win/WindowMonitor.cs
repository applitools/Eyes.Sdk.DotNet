namespace Applitools.Utils.Gui.Win
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using Applitools.Utils.Async;

    /// <summary>
    /// Monitors window activity.
    /// </summary>
    public sealed class WindowMonitor : IGuiMonitor
    {
        #region Fields

        private PeriodicAction periodic_;
        private TimeSpan interval_;
        
        private SafeNativeMethods.GUITHREADINFO guiThreadInfo_;
        private IntPtr active_;
        private IntPtr focus_;
        private SafeNativeMethods.RECT prev_;
        private GuiEventTypes eventMask_;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new <see cref="WindowMonitor"/> instance.
        /// </summary>
        public WindowMonitor(int interval)
        {
            guiThreadInfo_ = new SafeNativeMethods.GUITHREADINFO();
            guiThreadInfo_.cbSize = Marshal.SizeOf(guiThreadInfo_);

            interval_ = TimeSpan.FromMilliseconds(interval);
            eventMask_ = GuiEventTypes.All;
            ResetState_();
        }
        
        /// <summary>
        /// Creates a new <see cref="WindowMonitor"/> instance.
        /// </summary>
        public WindowMonitor() : this(200) 
        { 
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler<GuiEventArgs> GuiEvent;

        #endregion

        #region Properties

        /// <inheritdoc />
        public GuiEventTypes EventMask
        {
            get { return eventMask_; }
            set { eventMask_ = value; }
        }

        #endregion

        #region Methods

        #region Public

        /// <inheritdoc />
        [SecurityCritical]
        public void StartMonitoring()
        {
            if (periodic_ != null)
            {
                ResetState_();
                return;
            }

            periodic_ = new PeriodicAction(interval_, PeriodicActionHandler_, true);
        }

        /// <inheritdoc />
        public void StopMonitoring()
        {
            if (periodic_ != null)
            {
                periodic_.Dispose();
                periodic_ = null;
            }

            ResetState_();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopMonitoring();
        }

        #endregion

        #region Private

        private void ResetState_()
        {
            active_ = IntPtr.Zero;
            focus_ = IntPtr.Zero;
            prev_ = new SafeNativeMethods.RECT();
        }

        private void PeriodicActionHandler_()
        {
            if (!SafeNativeMethods.GetGUIThreadInfo(0, ref guiThreadInfo_))
            {
                return;
            }

            // report negative events
            bool reportFocus = ReportExpired_(
                ref focus_, 
                guiThreadInfo_.hwndFocus, 
                GuiEventTypes.WindowLostFocus);
            bool reportActive = ReportExpired_(
                ref active_, 
                guiThreadInfo_.hwndActive, 
                GuiEventTypes.WindowDeactivated);

            // report positive events
            FireWindowEvent_(reportActive, GuiEventTypes.WindowActivated, active_);
            FireWindowEvent_(reportFocus, GuiEventTypes.WindowGotFocus, focus_);

            // handle move & size
            if (reportActive)
            {
                SafeNativeMethods.GetWindowRect(active_, out prev_);
            }
            else if (IntPtr.Zero != active_)
            {
                // the active window did not change, unless it is still moving / resizing 
                // lets check if its changed location or position
                const uint GUI_INMOVESIZE = 0x2;
                if (0 == (guiThreadInfo_.flags & GUI_INMOVESIZE))
                {
                    SafeNativeMethods.RECT current = new SafeNativeMethods.RECT();
                    SafeNativeMethods.GetWindowRect(active_, out current);
                    FireWindowEvent_(
                        prev_.Left != current.Left || prev_.Top != current.Top,
                        GuiEventTypes.WindowMoved, 
                        active_);
                    FireWindowEvent_(
                        prev_.Width != current.Width || prev_.Height != current.Height,
                        GuiEventTypes.WindowResized, 
                        active_);
                    prev_ = current;
                }
            }
        }

        private void FireWindowEvent_(bool pred, GuiEventTypes eventType, IntPtr wh)
        {
            if (pred && null != GuiEvent && (0 != (eventMask_ & eventType)))
            {
                try
                {
                    GuiEvent(this, new WindowEventArgs(DateTimeOffset.Now, eventType, wh));
                }
                catch
                {
                }
            }
        }

        private bool ReportExpired_(ref IntPtr curwh, IntPtr newwh, GuiEventTypes guiEvent)
        {
            if (curwh == newwh)
            {
                return false;
            }

            FireWindowEvent_(IntPtr.Zero != curwh, guiEvent, curwh);
            curwh = newwh;

            return IntPtr.Zero != curwh;
        }

        #endregion

        #endregion
    }
}

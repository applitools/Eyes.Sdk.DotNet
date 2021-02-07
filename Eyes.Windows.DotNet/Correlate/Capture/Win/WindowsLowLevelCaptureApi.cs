namespace Applitools.Correlate.Capture.Win
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Utils;
    using Utils.Geometry;
    using Utils.Gui;
    using Utils.Gui.Win;

    /// <summary>
    /// A Window OS specific implementation of <see cref="ILowLevelCaptureApi"/>.
    /// </summary>
    public sealed class WindowsLowLevelCaptureApi : ILowLevelCaptureApi
    {
        #region Fields

        private readonly object initLock_ = new object();
        private readonly object guiEventLock_ = new object();
        private readonly Logger logger_;
        private readonly IGuiMonitor hidMonitor_;
        private readonly IGuiMonitor windowMonitor_;
        private ScreenCaptureForm popupWindow_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="WindowsLowLevelCaptureApi"/> instance.
        /// </summary>
        public WindowsLowLevelCaptureApi(Logger logger)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));

            logger_ = logger;

            hidMonitor_ = new MouseKeyboardMonitor();
            hidMonitor_.GuiEvent += (source, args) =>
            {
                FireGuiEvent_(args);
            };

            windowMonitor_ = new WindowMonitor();
            windowMonitor_.GuiEvent += (source, args) =>
            {
                FireGuiEvent_(args);
            };

            lock (initLock_)
            {
                Thread t = new Thread(new ThreadStart(ScreenCaptureMessageLoop_));
                t.IsBackground = true;
                t.Name = "ScreenCaptureMessageLoop_";
                t.Start();
                Monitor.Wait(initLock_);
                CommonUtils.Retry(
                    TimeSpan.FromMinutes(1), 
                    TimeSpan.FromMilliseconds(100), 
                    () => popupWindow_.IsHandleCreated);
            }
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler<GuiEventArgs> GuiEvent;

        #endregion

        #region Methods

        #region Public

        /// <inheritdoc />
        public GuiEventTypes EventMask
        {
            get 
            { 
                return windowMonitor_.EventMask; 
            }

            set 
            {
                windowMonitor_.EventMask = value;
                hidMonitor_.EventMask = value;
            }
        }

        /// <summary>
        /// Start monitoring active applications.
        /// </summary>
        /// <param name="monitorHid">Whether or not to monitor HID actions</param>
        [SecurityCritical]
        public void StartMonitoring(bool monitorHid)
        {
            windowMonitor_.StartMonitoring();
            if (monitorHid)
            {
                hidMonitor_.StartMonitoring();
            }
        }

        /// <inheritdoc />
        [SecurityCritical]
        public void StartMonitoring()
        {
            StartMonitoring(true);
        }

        /// <inheritdoc />
        public void StopMonitoring()
        {
            windowMonitor_.StopMonitoring();
            hidMonitor_.StopMonitoring();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            hidMonitor_.Dispose();
            windowMonitor_.Dispose();
            popupWindow_.Dispose();
        }

        /// <inheritdoc />
        public bool PlaceWindowAbove(IntPtr window1, IntPtr window2)
        {
            IntPtr window3 = SafeNativeMethods.GetWindow(
                window2, SafeNativeMethods.GetWindowCmd.GW_HWNDPREV);
            if (window3 == IntPtr.Zero)
            {
                window3 = IsTopMost_(window2) ?
                    SafeNativeMethods.HWND_TOPMOST : SafeNativeMethods.HWND_TOP;
            }

            return WindowsGuiUtils.ChangeWindowZOrder(logger_, window1, window3, false);
        }

        /// <inheritdoc />
        public bool ActivateWindow(IntPtr window)
        {
            logger_.Verbose("activating window {0}", window);
            if (!SafeNativeMethods.SetForegroundWindow(window))
            {
                return false;
            }

            var start = DateTimeOffset.Now;
            const int MaxSecondsToWait = 5;
            while (DateTimeOffset.Now.Subtract(start).TotalSeconds < MaxSecondsToWait)
            {
                if (SafeNativeMethods.GetForegroundWindow() == window)
                {
                    return true;
                }

                Thread.Sleep(500);
            }

            return SafeNativeMethods.GetForegroundWindow() == window;
        }

        /// <inheritdoc />
        public bool IsWindowVisible(IntPtr window)
        {
            return SafeNativeMethods.IsWindowVisible(window);
        }

        /// <inheritdoc />
        [SecurityCritical]
        public Bitmap CaptureWindow(
            IntPtr window, 
            bool clientArea, 
            RectangularMargins padding,
            bool fromScreen = true, 
            int redrawWait = 0)
        {
            if (fromScreen)
            {
                return popupWindow_.TakeSnapshot(window, clientArea, padding, redrawWait);
            }

            return CaptureWindowFromWindow_(window, clientArea);
        }

        /// <inheritdoc />
        public IntPtr GetWindowFromPoint(Point point)
        {
            return SafeNativeMethods.WindowFromPoint(point);
        }

        /// <inheritdoc />
        public IntPtr GetAncestorWindow(IntPtr window, bool owner)
        {
            var flags = owner ? SafeNativeMethods.GetAncestorFlags.GA_ROOTOWNER :
                SafeNativeMethods.GetAncestorFlags.GA_ROOT;
            
            return SafeNativeMethods.GetAncestor(window, flags);
        }

        /// <inheritdoc />  
        public WindowCaptureInfo GetWindowInfo(IntPtr window)
        {
            return GetWindowInfo(logger_, window);
        }

        public static WindowCaptureInfo GetWindowInfo(Logger logger, IntPtr window)
        {
            string traceMsg = Tracer.FormatCall("GetWindowInfo", window);

            try
            {
                logger.Log(traceMsg);

                if (IntPtr.Zero == window)
                {
                    logger.Log(TraceLevel.Error, Stage.General, new { message = "Invalid window handle" });
                    return null;
                }

                if (!WindowsGuiUtils.GetWindowRectangles(logger, window, out SafeNativeMethods.RECT wr, out SafeNativeMethods.RECT cr))
                {
                    logger.Log(TraceLevel.Error, Stage.General, new { message = "GetWindowRectangles failed" });
                    return null;
                }

                const int MaxCaptionLength = 500;

                StringBuilder caption = new StringBuilder(MaxCaptionLength);

                if (0 == SafeNativeMethods.GetWindowText(window, caption, caption.Capacity))
                {
                    // I delibrately don't trace this as it fails for most windows...
                }

                StringBuilder className = new StringBuilder(MaxCaptionLength);

                if (0 == SafeNativeMethods.GetClassName(window, className, className.Capacity))
                {
                    // I delibrately don't trace this as it fails for most windows...
                }

                uint threadId = SafeNativeMethods.GetWindowThreadProcessId(
                    window, out uint processId);

                WindowCaptureInfo windowInfo = new WindowCaptureInfo(
                    window,
                    (int)processId,
                    (int)threadId,
                    caption.ToString(),
                    className.ToString(),
                    new Rectangle(wr.Left, wr.Top, wr.Width, wr.Height),
                    new Rectangle(cr.Left, cr.Top, cr.Width, cr.Height));

                logger.Verbose("{0} succeeded", traceMsg);

                return windowInfo;
            }
            catch (Exception ex)
            {
                logger.Verbose("{0} failed: {1}", traceMsg, ex);
                throw;
            }
        }

        /// <inheritdoc />
        public IntPtr GetActiveWindow()
        {
            return SafeNativeMethods.GetForegroundWindow();
        }

        /// <inheritdoc />
        public Form CreateNonActiveForm()
        {
            return new InactiveForm();
        }

        #endregion

        #region Private

        private bool IsTopMost_(IntPtr window)
        {
            string traceMsg = Tracer.FormatCall("IsTopMost_", window);

            try
            {
                logger_.Log(traceMsg);

                var wi = new SafeNativeMethods.WINDOWINFO();
                wi.cbSize = (uint)Marshal.SizeOf(typeof(SafeNativeMethods.WINDOWINFO));

                if (!SafeNativeMethods.GetWindowInfo(window, ref wi))
                {
                    logger_.Log(
                        "{0}: GetWindowInfo failed ({1})",
                        traceMsg,
                        Marshal.GetLastWin32Error());
                    return false;
                }

                logger_.Verbose("{0} succeeded", traceMsg);

                return 0 != (wi.dwExStyle & 
                    SafeNativeMethods.ExtendedWindowStyles.WS_EX_TOPMOST);
            }
            catch (Exception ex)
            {
                logger_.Verbose("{0} failed: {1}", traceMsg, ex);
                throw;
            }
        }

        private void FireGuiEvent_(GuiEventArgs args)
        {
            if (null != GuiEvent)
            {
                try
                {
                    lock (guiEventLock_)
                    {
                        GuiEvent(this, args);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(logger_, Stage.General, ex);
                }
            }
        }

        /// <summary>
        /// Captures the content of the input window or its client area.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "Workaround a Code Analysis bug")]
        [SecurityCritical]
        private Bitmap CaptureWindowFromWindow_(IntPtr window, bool clientArea)
        {
            string traceMsg = Tracer.FormatCall(
                "CaptureWindowFromWindow_", window, clientArea);

            try
            {
                logger_.Log(traceMsg);

                DateTime start = DateTime.Now;

                // we actually capture the root window and then copy the requested region
                // since several windows may paint on the same area of the control.
                IntPtr rwh = SafeNativeMethods.GetAncestor(
                    window,
                    SafeNativeMethods.GetAncestorFlags.GA_ROOT);

                if (!SafeNativeMethods.GetWindowRect(rwh, out SafeNativeMethods.RECT rwr))
                {
                    logger_.Log(
                        "{0}: GetWindowRect failed ({1})",
                        traceMsg,
                        Marshal.GetLastWin32Error());
                    return null;
                }

                if (rwr.Width == 0 || rwr.Height == 0)
                {
                    return null;
                }

                using (Bitmap rbmp = new Bitmap(rwr.Width, rwr.Height))
                {
                    using (Graphics g = Graphics.FromImage(rbmp))
                    {
                        IntPtr dc = g.GetHdc();
                        try
                        {
                            if (!SafeNativeMethods.PrintWindow(rwh, dc, 0))
                            {
                                logger_.Log(
                                    "{0}: PrintWindow failed ({1})",
                                    traceMsg,
                                    Marshal.GetLastWin32Error());
                                return null;
                            }
                        }
                        finally
                        {
                            g.ReleaseHdc();
                        }
                    }

                    if (!WindowsGuiUtils.GetWindowRectangles(logger_, window, out SafeNativeMethods.RECT wr, out SafeNativeMethods.RECT cr))
                    {
                        logger_.Log("{0}: GetWindowRectangles failed", traceMsg);
                        return null;
                    }

                    SafeNativeMethods.RECT r = clientArea ? cr : wr;

                    // make r's coordinates relative to rbmp; that is, (0,0)
                    r.Move(-rwr.Left, -rwr.Top);

                    Bitmap bmp = rbmp.Clone(
                        new Rectangle(r.Left, r.Top, r.Width, r.Height),
                        rbmp.PixelFormat);

                    logger_.Verbose(
                        "{0} succeeded in {1}ms", 
                        traceMsg,
                        DateTime.Now.Subtract(start).TotalMilliseconds);

                    return bmp;
                }
            }
            catch (Exception ex)
            {
                logger_.Verbose("{0} failed: {1}", traceMsg, ex);
                throw;
            }
        }
        
        private void ScreenCaptureMessageLoop_()
        {
            lock (initLock_)
            {
                popupWindow_ = new ScreenCaptureForm(logger_);
                Monitor.Pulse(initLock_);
            }

            logger_.Log("ScreenCaptureForm is {0}", popupWindow_.Handle);
            
            Application.Run(popupWindow_);
        }

        #endregion

        #endregion
    }
}

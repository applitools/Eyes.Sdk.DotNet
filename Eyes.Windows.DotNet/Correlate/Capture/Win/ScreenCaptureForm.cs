namespace Applitools.Correlate.Capture.Win
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;
    using Applitools.Utils;
    using Applitools.Utils.Geometry;
    using Applitools.Utils.Gui.Win;

    /// <summary>
    /// A <see cref="Form"/> that captures the screen pixels of other windows while
    /// hiding the cursor and placing a black background behind them.
    /// </summary>
    public sealed class ScreenCaptureForm : InactiveForm
    {
        #region Fields

        private readonly object lock_ = new object();
        private readonly Logger logger_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ScreenCaptureForm"/> instance.
        /// </summary>
        public ScreenCaptureForm(Logger logger)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));

            logger_ = logger;

            this.ShowIcon = false;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.SetBounds(0, 0, 0, 0);
            this.Opacity = 0;
        }

        #endregion

        #region Delegates

        private delegate Bitmap CaptureWindowFromScreenInvoker_(
            IntPtr window, bool clientArea, RectangularMargins padding, int redrawWait);

        #endregion

        #region Methods

        /// <summary>
        /// Takes a screen snapshot of the input window.
        /// </summary>
        /// <param name="window">Window to capture</param>
        /// <param name="clientArea">
        /// Whether to capture the client area or the full window
        /// </param>
        /// <param name="padding">
        /// Inner padding defining the window region to capture or <c>null</c> to capture the
        /// entire window
        /// </param>
        /// <param name="redrawWait">
        /// Delay in milliseconds to allow the background window to draw
        /// </param>
        public Bitmap TakeSnapshot(
            IntPtr window, 
            bool clientArea, 
            RectangularMargins padding,
            int redrawWait)
        {
            lock (lock_)
            {
                if (IsDisposed)
                {
                    return null;
                }

                return (Bitmap)this.Invoke(
                    new CaptureWindowFromScreenInvoker_(CaptureWindowFromScreen_),
                    window,
                    clientArea,
                    padding,
                    redrawWait);
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage", 
            "CA2215:Dispose methods should call base class dispose",
            Justification = "Base method is indeed invoked - the rule is unable to verify it")]
        protected override void Dispose(bool disposing)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(Dispose), disposing);
                return;
            }

            lock (lock_)
            {
                base.Dispose(disposing);
            }
        }

        #region Private

        /// <summary>
        /// Captures the screen area overlapping the input window or its client area
        /// while placing this form as the window background which normalizes
        /// transparent areas.
        /// </summary>
        /// <returns>The captured window image or <c>null</c> on failure</returns>
        private Bitmap CaptureWindowFromScreen_(
            IntPtr window, bool clientArea, RectangularMargins padding, int redrawWait)
        {
            string traceMsg = Tracer.FormatCall(
                "CaptureWindowFromScreen_", window, clientArea, redrawWait);

            Bitmap result = null;

            try
            {
                logger_.Log(traceMsg);

                if (window == IntPtr.Zero)
                {
                    this.Opacity = 0;
                    return null;
                }

                DateTime start = DateTime.Now;

                if (!WindowsGuiUtils.GetWindowRectangles(logger_, window, out SafeNativeMethods.RECT wr, out SafeNativeMethods.RECT cr))
                {
                    logger_.Log("{0}: GetWindowRectangles failed", traceMsg);
                    this.Opacity = 0;
                    return null;
                }

                var r = clientArea ? cr : wr;

                r.Left += padding.Left;
                r.Top  += padding.Top;
                r.Right -= padding.Right;
                r.Bottom -= padding.Bottom;

                if (r.Width <= 0 || r.Height <= 0)
                {
                    logger_.Log("{0}: Target rectangle is Zero", traceMsg);
                    this.Opacity = 0;
                    return null;
                }

                result = new Bitmap(r.Width, r.Height);

                bool hideCursor = r.Contains(Cursor.Position);
                if (hideCursor)
                {
                    Cursor.Hide();
                }

                if (!WindowsGuiUtils.ChangeWindowZOrder(logger_, this.Handle, window, true))
                {
                    logger_.Log("{0}: Failed to position background window", traceMsg);
                }

                this.SetBounds(r.Left, r.Top, r.Width, r.Height);
                this.Opacity = 100;

                this.Refresh();

                if (redrawWait > 0)
                {
                    Thread.Sleep(redrawWait);
                }

                using (Graphics g = Graphics.FromImage(result))
                {
                    try
                    {
                        g.CopyFromScreen(
                            new Point(r.Left, r.Top),
                            new Point(0, 0),
                            new Size(r.Width, r.Height));
                    }
                    catch (Exception ex)
                    {
                        logger_.Log("{0}: CopyFromScreen failed: {1}", traceMsg, ex);
                        throw;
                    }
                }

                if (hideCursor)
                {
                    Cursor.Show();
                }

                // Fail capture if window was moved while it was taken.
                if (WindowsGuiUtils.GetWindowRectangles(logger_, window, out SafeNativeMethods.RECT wr2, out SafeNativeMethods.RECT cr2))
                {
                    if (!wr2.Equals(wr))
                    {
                        logger_.Verbose("{0}: Aborting due to window movement!", traceMsg);
                        result.Dispose();
                        return null;
                    }
                }

                logger_.Verbose(
                    "{0} succeeded in {1}ms", 
                    traceMsg, 
                    DateTime.Now.Subtract(start).TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (result != null)
                {
                    result.Dispose();
                }

                CommonUtils.LogExceptionStackTrace(logger_, Stage.General, ex);
                return null;
            }
        }

        #endregion

        #endregion
    }
}

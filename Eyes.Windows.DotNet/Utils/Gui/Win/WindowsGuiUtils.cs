namespace Applitools.Utils.Gui.Win
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;
    using Applitools.Utils;

    /// <summary>
    /// Windows GUI utils.
    /// </summary>
    [CLSCompliant(false)]
    public static class WindowsGuiUtils
    {
        /// <summary>
        /// Returns the coordinates of the window and client area (including the Main Menu)
        /// relative to the screen.
        /// </summary>
        /// <remarks>
        /// This method exists since some windows (e.g., Calculator main window) report
        /// a window size that is smaller than it really is.
        /// </remarks>
        public static bool GetWindowRectangles(
            Logger logger,
            IntPtr window,
            out SafeNativeMethods.RECT windowRectangle,
            out SafeNativeMethods.RECT clientRectangle)
        {
            string traceMsg = Tracer.FormatCall("GetWindowRectangles", window);

            ArgumentGuard.NotNull(logger, nameof(logger));

            try
            {
                logger.Log(traceMsg);
                windowRectangle = clientRectangle = new SafeNativeMethods.RECT();

                SafeNativeMethods.WINDOWINFO wi = new SafeNativeMethods.WINDOWINFO();
                wi.cbSize = (uint)Marshal.SizeOf(typeof(SafeNativeMethods.WINDOWINFO));

                if (!SafeNativeMethods.GetWindowInfo(window, ref wi))
                {
                    logger.Log(
                        "{0}: GetWindowInfo failed ({1})",
                        traceMsg,
                        Marshal.GetLastWin32Error());
                    return false;
                }

                windowRectangle = wi.rcWindow;
                clientRectangle = wi.rcClient;

                // We assume that border width and height is symmetric...
                const SafeNativeMethods.WindowStyles CaptionFlag = 
                    SafeNativeMethods.WindowStyles.WS_CAPTION;
                if (CaptionFlag == (wi.dwStyle & CaptionFlag))
                {
                    var border = (windowRectangle.Width - clientRectangle.Width) / 2;

                    clientRectangle.Top = windowRectangle.Top + border +
                        SystemInformation.CaptionHeight;
                }

                logger.Verbose("{0} succeeded", traceMsg);

                return true;
            }
            catch (Exception ex)
            {
                logger.Log("{0} failed: {1}", traceMsg, ex);
                throw;
            }
        }

        /// <summary>
        /// Places <c>window</c> directly below <c>belowWindow</c>.
        /// </summary>
        /// <param name="logger">The program execution logger.</param>
        /// <param name="window">Window which position is to be changed.</param>
        /// <param name="belowWindow">Window under which <c>window</c> is to be positioned.</param>
        /// <param name="block">Whether to wait until window is repositioned.</param>
        /// <param name="activate">Weather to activate the window when swapping to it, or not. Defaults to <c>false</c>.</param>
        /// <returns>Whether the operation succeeded</returns>
        public static bool ChangeWindowZOrder(
            Logger logger, 
            IntPtr window, 
            IntPtr belowWindow, 
            bool block, 
            bool activate = false)
        {
            string traceMsg = Tracer.FormatCall(
                "ChangeWindowZOrder", window, belowWindow, block);

            ArgumentGuard.NotNull(logger, nameof(logger));

            try
            {
                logger.Log(traceMsg);

                var flags = SafeNativeMethods.SetWindowPosFlags.SWP_NOSIZE |
                    SafeNativeMethods.SetWindowPosFlags.SWP_NOMOVE;
                if (!activate)
                {
                    flags |= SafeNativeMethods.SetWindowPosFlags.SWP_NOACTIVATE;
                }

                if (!SafeNativeMethods.SetWindowPos(window, belowWindow, 0, 0, 0, 0, flags))
                {
                    logger.Log(
                        "{0}: SetWindowPos failed ({1})",
                        traceMsg,
                        Marshal.GetLastWin32Error());
                    return false;
                }

                if (block)
                {
                    // The call to SetWindowPos may return before it is fully processed by
                    // the window (e.g., by calling ReplyMessage).
                    // By sending another null message we guarantee that SetWindowPos is fully
                    // processed since the same UI thread in window handles all messages.
                    SendNullMessage_(window);
                }

                logger.Verbose("{0} succeeded", traceMsg);

                return true;
            }
            catch (Exception ex)
            {
                logger.Log("{0} failed {1}", traceMsg, ex);
                throw;
            }
        }

        private static bool SendNullMessage_(IntPtr window)
        {
            const int SendMessageTimeout = 2000;

            return IntPtr.Zero != SafeNativeMethods.SendMessageTimeout(
                window,
                SafeNativeMethods.WM_NULL,
                UIntPtr.Zero,
                IntPtr.Zero,
                SafeNativeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
                SendMessageTimeout,
                out UIntPtr result);
        }
    }
}

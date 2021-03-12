namespace Applitools.Correlate.Capture
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Security;
    using System.Text.RegularExpressions;
    using Applitools.Utils;
    using Applitools.Utils.Config;
    using Applitools.Utils.Geometry;
    using Applitools.Utils.Gui;

    /// <inheritdoc />
    public sealed class GuiCapturer : IGuiCapturer
    {
        #region Fields

        private readonly object captureLock_ = new object();
        private readonly object eventsLock_ = new object();
        private readonly Logger logger_;
        private readonly ILowLevelCaptureApi captureApi_;
        private readonly int hostId_;

        private volatile WindowCaptureInfo context_;
        private volatile string lastProcessName_;
        private volatile List<GuiEventArgs> hidEvents_ = new List<GuiEventArgs>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="GuiCapturer"/> instance.
        /// </summary>
        public GuiCapturer(Logger logger, ILowLevelCaptureApi captureApi)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(captureApi, nameof(captureApi));

            logger_ = logger;
            captureApi_ = captureApi;
            captureApi_.EventMask =
                GuiEventTypes.WindowActivated |
                GuiEventTypes.WindowMoved |
                GuiEventTypes.WindowResized |
                GuiEventTypes.Hid;
            captureApi_.GuiEvent += CaptureApiGuiEventHandler_;

            using (var host = Process.GetCurrentProcess())
            {
                hostId_ = host.Id;
            }

            MonitorHost = false;
            CapturePadding = new RectangularMargins();
            MonitorProcessesById = -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether to monitor the process hosting this <see cref="GuiCapturer"/> instance.
        /// </summary>
        public bool MonitorHost
        {
            get;
            set;
        }

        /// <summary>
        /// A regex matched against the names of processes to monitor or <c>null</c> to monitor
        /// all processes.
        /// </summary>
        public Regex MonitorProcessesByName
        {
            get;
            set;
        }

        /// <summary>
        /// An id of a process to monitor or -1 to monitor all processes.
        /// </summary>
        public int MonitorProcessesById
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to kill the process of id <see cref="MonitorProcessesById"/> when this object
        /// is disposed.
        /// </summary>
        public bool KillMonitoredProcess
        {
            get;
            set;
        }

        /// <summary>
        /// A regex matched against the titles of windows to monitor or <c>null</c> to monitor
        /// all windows.
        /// </summary>
        public Regex MonitorWindowsByTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Inner window padding defining the region to capture or <c>null</c> to capture the
        /// entire window.
        /// </summary>
        public RectangularMargins CapturePadding
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Public

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "Could not resolve the violation")]
        [SecuritySafeCritical]
        public void Dispose()
        {
            try
            {
                captureApi_.Dispose();
                if (KillMonitoredProcess && MonitorProcessesById != -1)
                {
                    using (var p = Process.GetProcessById(MonitorProcessesById))
                    {
                        p.Kill();
                    }
                }
            }
            catch
            {
                // Don't throw
            }
        }

        /// <inheritdoc />
        [SecurityCritical]
        public void StartMonitoring(bool monitorHid)
        {
            captureApi_.StartMonitoring(monitorHid);
        }

        /// <inheritdoc />
        public void StopMonitoring()
        {
            captureApi_.StopMonitoring();
            ClearCaptureBackground_();
        }

        public Bitmap Capture(IntPtr hWnd)
        {
            lock (captureLock_)
            {
                Bitmap image = captureApi_.CaptureWindow(hWnd, true, CapturePadding, false);
                return image;
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Should not be disposed")]
        [SecurityCritical]
        public GuiCapture Capture(bool standalone = false)
        {
            string traceMsg = Tracer.FormatCall("Capture");

            lock (captureLock_)
            {
                try
                {
                    var context = context_;
                    if (context == null)
                    {
                        return null;
                    }

                    logger_.Verbose(traceMsg);

                    var image = captureApi_.CaptureWindow(context.Handle, true, CapturePadding);
                    if (image == null)
                    {
                        return null;
                    }

                    IEnumerable<GuiEventArgs> hidEvents;
                    lock (eventsLock_)
                    {
                        hidEvents = hidEvents_;
                        hidEvents_ = new List<GuiEventArgs>();
                    }

                    logger_.Verbose("{0} succeeded", traceMsg);

                    if (standalone)
                    {
                        captureApi_.CaptureWindow(IntPtr.Zero, true, CapturePadding, false);
                    }

                    return new GuiCapture(
                        context.Caption, image, context.Client.Location, hidEvents);
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.CaptureScreenshot, ex);
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public void Configure(IReadAccessor<string> reader)
        {
            ArgumentGuard.NotNull(reader, nameof(reader));

            string traceMsg = Tracer.FormatCall("Configure");

            try
            {
                logger_.Log(traceMsg);

#pragma warning disable IDE0018 // Inline variable declaration
                string value;
#pragma warning restore IDE0018 // Inline variable declaration
                if (reader.TryGetValue(this, "MonitorHost", out value))
                {
                    MonitorHost = bool.Parse(value);
                }

                if (reader.TryGetValue(this, "MonitorProcessesByName", out value))
                {
                    MonitorProcessesByName =
                        string.IsNullOrEmpty(value) ? null : new Regex(value);
                }

                if (reader.TryGetValue(this, "MonitorProcessesById", out value))
                {
                    MonitorProcessesById = value.ToInt32();
                }

                if (reader.TryGetValue(this, "MonitorWindowsByTitle", out value))
                {
                    MonitorWindowsByTitle =
                        string.IsNullOrEmpty(value) ? null : new Regex(value);
                }

                if (reader.TryGetValue(this, "CapturePadding", out value))
                {
                    CapturePadding =
                        string.IsNullOrEmpty(value) ? default(RectangularMargins) : RectangularMargins.Parse(value);
                }

                logger_.Verbose("{0} succeeded", traceMsg);
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.CaptureScreenshot, ex);
                throw;
            }
        }

        #endregion

        #region Private

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA2204:Literals should be spelled correctly",
            Justification = "Method name is valid")]
        private void CaptureApiGuiEventHandler_(object sender, GuiEventArgs e)
        {
            var oldContext = context_;
            try
            {
                if (e.IsWindowEvent)
                {
                    HandleWindowEvent_(oldContext, e as WindowEventArgs);
                    if (context_ == null)
                    {
                        logger_.Log("CaptureApiGuiEventHandler_(): context is null!");
                    }
                }
                else if (oldContext == null)
                {
                    return;
                }
                else if (e.IsKeyboardEvent)
                {
                    logger_.Verbose(e.ToString());
                    AddEvent_(e);
                }
                else
                {
                    HandleMouseEvent_(oldContext, e as MouseEventArgs);
                }
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.General, ex);
                throw;
            }
            finally
            {
                if (oldContext != null && context_ == null)
                {
                    ClearCaptureBackground_();
                }
            }
        }

        private void AddEvent_(GuiEventArgs ev)
        {
            lock (eventsLock_)
            {
                hidEvents_.Add(ev);
            }
        }

        private void HandleMouseEvent_(WindowCaptureInfo context, MouseEventArgs ev)
        {
            if (!context.Client.Contains(ev.Location))
            {
                logger_.Verbose("Out of bounds: {0}", ev.ToString());
                return;
            }

            ev.X -= context.Client.X;
            ev.Y -= context.Client.Y;
            ev.WindowSize = context.Client.Size;

            ev.X -= CapturePadding.Left;
            ev.Y -= CapturePadding.Top;

            logger_.Verbose(ev.ToString());
            AddEvent_(ev);
        }

        private void HandleWindowEvent_(WindowCaptureInfo oldContext, WindowEventArgs ev)
        {
            var wi = captureApi_.GetWindowInfo(ev.Window);

            if (wi == null)
            {
                context_ = null;
                return;
            }

            if (ev.EventType == GuiEventTypes.WindowActivated)
            {
                if (!MonitorHost && hostId_ == wi.Process)
                {
                    context_ = null;
                    return;
                }

                if (MonitorProcessesById != -1 && MonitorProcessesById != wi.Process)
                {
                    context_ = null;
                    return;
                }

                if (MonitorWindowsByTitle != null &&
                    !MonitorWindowsByTitle.IsMatch(wi.Caption))
                {
                    context_ = null;
                    return;
                }

                if (MonitorProcessesByName != null)
                {
                    if (oldContext == null || oldContext.Process != wi.Process ||
                        lastProcessName_ == null)
                    {
                        using (var process = Process.GetProcessById(wi.Process))
                        {
                            lastProcessName_ = process.MainModule.ModuleName;
                        }
                    }

                    if (!MonitorProcessesByName.IsMatch(lastProcessName_))
                    {
                        context_ = null;
                        return;
                    }
                }
            }

            // Context must update on activation, resize and move.
            context_ = wi;
            logger_.Log("Capture context updated: {0} => {1}", ev.EventType, context_);
        }

        /// <summary>
        /// Forces the capture background to clear.
        /// </summary>
        /// <remarks>
        /// This is a workaround to get the most out of the current low level capture 
        /// implementation. This could be improved.
        /// </remarks>
        private void ClearCaptureBackground_()
        {
            captureApi_.CaptureWindow(IntPtr.Zero, true, default(RectangularMargins));
        }

        #endregion

        #endregion
    }
}

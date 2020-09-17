using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Applitools.Common;
using Applitools.Fluent;
using Applitools.Correlate.Capture;
using Applitools.Correlate.Capture.Win;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.Utils.Gui.Win;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    /// <summary>
    /// Applitools Eyes Coded UI API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "Disposed on Close or AbortIfNotClosed")]
    [ComVisible(true)]
    public abstract class EyesWindowsBase : EyesBase
    {
        #region Fields

        private readonly HashSet<IntPtr> autWindows_ = new HashSet<IntPtr>();
        protected IntPtr requestedWindowHandle = IntPtr.Zero;
        private WindowsLowLevelCaptureApi captureApi_;
        private GuiCapturer guiCapturer_;
        private Rectangle lastScreenshotBounds_;
        private string title_;
        private bool pidChanged_;
        private int processId_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        protected EyesWindowsBase(Uri serverUrl)
            : base(serverUrl)
        {
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        protected EyesWindowsBase()
            : base()
        {
        }

        #endregion

        #region Configuration

        private Configuration configuration_ = new Configuration();

        protected override Configuration Configuration { get => configuration_; }

        public Configuration GetConfiguration()
        {
            return new Configuration(configuration_);
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The process id of the Application Under Test.
        /// </summary>
        public int ProcessId
        {
            get
            {
                if (guiCapturer_ == null)
                {
                    return 0;
                }

                return guiCapturer_.MonitorProcessesById;
            }

            set
            {
                processId_ = value;
                pidChanged_ = guiCapturer_.MonitorProcessesById != value;
                guiCapturer_.MonitorProcessesById = value;

                // We restart monitoring to make sure that the current active window is reported
                // and not ignored.
                guiCapturer_.StopMonitoring();
                guiCapturer_.StartMonitoring(false);
                Logger.Verbose("AUT Process id set to {0}".Fmt(value));
            }
        }
        protected override bool ViewportSizeRequired => false;

        #endregion

        #region Methods

        public WindowCaptureInfo[] GetWindows(params string[] classNames)
        {
            return GetWindows(Logger, processId_, classNames);
        }

        internal static WindowCaptureInfo[] GetWindows(Logger logger, int processId, params string[] classNames)
        {
            Process process = Process.GetProcessById(processId);
            IntPtr mainWindowHandle = process.MainWindowHandle;
            List<WindowCaptureInfo> windows = new List<WindowCaptureInfo>();

            ArrayList windowHandles = new ArrayList();

            foreach (ProcessThread thread in process.Threads)
            {
                SafeNativeMethods.EnumThreadWindows((uint)thread.Id, EnumWindow_, windowHandles);
            }

            object[] winHandles = windowHandles.ToArray();
            windowHandles.Clear();
            foreach (IntPtr hWnd in winHandles)
            {
                windowHandles.Add(hWnd);
                SafeNativeMethods.EnumChildWindows(hWnd, EnumWindow_, windowHandles);
            }

            foreach (IntPtr hWnd in windowHandles)
            {
                WindowCaptureInfo wi = CreateWindowData(logger, hWnd);
                if (wi != null)
                {
                    string className = wi.ClassName;
                    if (classNames == null || classNames.Length == 0 || Array.Exists(classNames, (n) => n == className))
                    {
                        windows.Add(wi);
                    }
                }
            }
            return windows.ToArray();
        }

        public static WindowCaptureInfo CreateWindowData(Logger logger, IntPtr hWnd)
        {
            if (SafeNativeMethods.IsWindowVisible(hWnd))
            {
                return WindowsLowLevelCaptureApi.GetWindowInfo(logger, hWnd);
            }
            return null;
        }

        private static bool EnumWindow_(IntPtr hWnd, ArrayList handles)
        {
            if (!handles.Contains(hWnd))
            {
                handles.Add(hWnd);
            }
            return true;
        }

        /// <summary>
        /// Takes a snapshot of the application under test, where the capture area and settings
        /// are given by <paramref name="checkSettings"/>.
        /// </summary>
        /// <param name="name">A tag to be associated with the match.</param>
        /// <param name="checkSettings">A settings object defining the capture area and parameters.
        /// Created fluently using the <see cref="Target"/> static class.</param>
        public void Check(string name, ICheckSettings checkSettings)
        {
            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));

            ICheckSettingsInternal checkSettingsInternal = checkSettings as ICheckSettingsInternal;

            Rectangle? targetRegion = checkSettingsInternal.GetTargetRegion();

            CheckWindowBase(targetRegion, checkSettings.WithName(name));
        }

        #region Protected

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="processId">The id of the process of the application under test.</param>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The required application's client area viewport size
        /// or <c>Size.Empty</c> to allow any viewport size.</param>
        protected void OpenBase(
            int processId,
            string appName,
            string testName,
            Size viewportSize)
        {
            autWindows_.Clear();

            if (captureApi_ != null)
            {
                captureApi_.Dispose();
            }

            captureApi_ = new WindowsLowLevelCaptureApi(Logger);

            title_ = null;
            lastScreenshotBounds_ = Rectangle.Empty;

            if (guiCapturer_ != null)
            {
                guiCapturer_.Dispose();
            }

            guiCapturer_ = new GuiCapturer(Logger, captureApi_);
            guiCapturer_.MonitorHost = true;

            processId_ = processId; // this is just a member...

            OpenBase(appName, testName, viewportSize);

            ProcessId = processId; // and this is a property that has a side effect that it also starts monitoring.
            guiCapturer_.StartMonitoring(false);
        }

        /// <summary>
        /// Gets the bounds of the input control relative to the client area of the 
        /// current top level window.
        /// </summary>
        protected Rectangle ScreenToClient(Rectangle control)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            var topWindow = GetTopWindow_();
            var topBounds = topWindow.Client;
            if (!topBounds.Contains(control))
            {
                Logger.Verbose("ScreenToClient(): {0} is not contained in last screenshot!"
                    .Fmt(GeometryUtils.ToString(control)));
            }

            control.Intersect(topBounds);
            if (control == Rectangle.Empty)
            {
                throw new EyesException(
                    "Control is outside the bounds of the top-level window!");
            }

            return new Rectangle(
                control.Left - topBounds.Left,
                control.Top - topBounds.Top,
                control.Width,
                control.Height);
        }

        protected bool ScreenToLastScreenshot(
            Point onScreen, out Point onScreenshot)
        {
            if (!lastScreenshotBounds_.Contains(onScreen))
            {
                Logger.Verbose("ScreenToLastScreenshot(): {0} is not contained in last screenshot!", onScreen);
                onScreenshot = Point.Empty;
                return false;
            }

            onScreen.Offset(-lastScreenshotBounds_.Left, -lastScreenshotBounds_.Top);
            onScreenshot = onScreen;

            return true;
        }

        protected bool ScreenToLastScreenshot(Rectangle control, out Region controlRegion)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            controlRegion = Region.Empty;

            if (!lastScreenshotBounds_.Contains(control))
            {
                Logger.Verbose(
                    "ScreenToLastScreenshot(): {0} is not contained in last screenshot!"
                    .Fmt(GeometryUtils.ToString(control)));
            }

            control.Intersect(lastScreenshotBounds_);
            if (control == Rectangle.Empty)
            {
                return false;
            }

            controlRegion = new Region(
                control.Left - lastScreenshotBounds_.Left,
                control.Top - lastScreenshotBounds_.Top,
                control.Width,
                control.Height);

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA2204:Literals should be spelled correctly",
            Justification = "Method name is valid")]
        protected bool IsRelevantControl(IntPtr control)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            if (IsRelevantWindow_(captureApi_.GetActiveWindow()))
            {
                return true;
            }

            var topParent = SafeNativeMethods.GetAncestor(
                control, SafeNativeMethods.GetAncestorFlags.GA_PARENT);
            if (IsRelevantWindow_(topParent))
            {
                return true;
            }

            Logger.Verbose("IsRelevantControl(): [{0}] is irrelevant".Fmt(control));
            return false;
        }

        protected override Size GetViewportSize()
        {
            //return GetTopWindow_().Client.Size;
            return Size.Empty;
        }

        protected override void SetViewportSize(RectangleSize size)
        {
            ArgumentGuard.NotNull(size, nameof(size));

            Logger.Verbose("SetViewportSize(" + size + ")");

            var topWindow = GetTopWindow_();
            var current = topWindow.Client.Size;
            if (size.ToSize().Equals(current))
            {
                return;
            }

            if (!WindowsGuiUtils.GetWindowRectangles(Logger, topWindow.Handle, out SafeNativeMethods.RECT wr, out SafeNativeMethods.RECT cr))
            {
                throw new EyesException("Failed to obtain window size!");
            }

            int dleft = cr.Left - wr.Left;
            int dright = wr.Right - cr.Right;
            int dtop = cr.Top - wr.Top;
            int dbottom = wr.Bottom - cr.Bottom;

            var resizeFlags = SafeNativeMethods.SetWindowPosFlags.SWP_NOMOVE |
                SafeNativeMethods.SetWindowPosFlags.SWP_NOZORDER;
            if (!SafeNativeMethods.SetWindowPos(
                topWindow.Handle,
                (IntPtr)(-1),
                0,
                0,
                dleft + dright + size.Width,
                dtop + dbottom + size.Height,
                resizeFlags))
            {
                throw new EyesException("Failed to resize window!");
            }

            current = GetViewportSize();
            if (!size.ToSize().Equals(current))
            {
                throw new EyesException("Failed to resize window!");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization",
            "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the process name to be in lowercase")]
        protected override string GetInferredEnvironment()
        {
            var processName = SystemUtils.GetProcessName(processId_, Logger) ?? "-";
            return $"proc:{processName};{OSNames.GetNameAndVersion(Environment.OSVersion)}";
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            IntPtr hWnd = (requestedWindowHandle != IntPtr.Zero) ? requestedWindowHandle : GetTopWindow_().Handle;
            Bitmap screenshot = null;
            try
            {
                screenshot = guiCapturer_.Capture(hWnd);
                EyesWindowsScreenshot eyesWindowsScreenshot = new EyesWindowsScreenshot((Bitmap)screenshot.Clone());
                if (targetRegion.HasValue)
                {
                    return (EyesWindowsScreenshot)eyesWindowsScreenshot.GetSubScreenshot(targetRegion.Value, true);
                }

                return eyesWindowsScreenshot;
            }
            finally
            {
                screenshot?.Dispose();
            }
        }

        protected override string GetTitle()
        {
            return title_;
        }

        protected override void CloseOrAbort(bool aborted)
        {
            if (guiCapturer_ != null)
            {
                guiCapturer_.StopMonitoring();
                guiCapturer_.Dispose();
                guiCapturer_ = null;
            }

            if (captureApi_ != null)
            {
                captureApi_.Dispose();
                captureApi_ = null;
            }

            autWindows_.Clear();
            title_ = null;
        }

        private bool IsRelevantWindow_(IntPtr window)
        {
            if (autWindows_.Contains(window))
            {
                return true;
            }

            if (0 != SafeNativeMethods.GetWindowThreadProcessId(window, out uint pid))
            {
                if ((int)pid == guiCapturer_.MonitorProcessesById)
                {
                    autWindows_.Add(window);
                    return true;
                }
            }

            return false;
        }

        private WindowCaptureInfo GetTopWindow_()
        {
            WindowCaptureInfo wi = null;
            var wh = SafeNativeMethods.GetTopWindow(IntPtr.Zero);
            while (wh != IntPtr.Zero)
            {
                if (SafeNativeMethods.IsWindowVisible(wh))
                {
                    wi = captureApi_.GetWindowInfo(wh);
                    if (wi.Process == ProcessId)
                    {
                        return wi;
                    }
                }

                wh = SafeNativeMethods.GetWindow(wh, SafeNativeMethods.GetWindowCmd.GW_HWNDNEXT);
            }

            throw new EyesException("Failed to find top window for process " + ProcessId);
        }

        #endregion

        #endregion
    }
}

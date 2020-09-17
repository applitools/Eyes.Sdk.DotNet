using System;
using System.Drawing;
using Applitools.Utils;
using Applitools.Utils.Gui.Win;
using Microsoft.VisualStudio.TestTools.UITesting;

using Region = Applitools.Utils.Geometry.Region;
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI
{
    /// <summary>
    /// Applitools Eyes Coded UI API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "Disposed on Close or AbortIfNotClosed")]
    public sealed class Eyes : EyesWindowsBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(Uri serverUrl)
            : base(serverUrl)
        {
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public Eyes()
            : base()
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="mainWindow">The main top-level window of the AUT.</param>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The required application's client area viewport size
        /// or <c>Size.Empty</c> to allow any viewport size.</param>
        public void Open(
            UITestControl mainWindow,
            string appName,
            string testName,
            Size viewportSize)
        {
            ArgumentGuard.NotNull(mainWindow, nameof(mainWindow));

            if (0 == SafeNativeMethods.GetWindowThreadProcessId(mainWindow.WindowHandle, out uint pid))
            {
                var message = "Failed to obtain the process underlying window '{0}'"
                    .Fmt(mainWindow.WindowHandle);
                Logger.Log("Open(): {0}", message);
                throw new EyesException(message);
            }

            OpenBase((int)pid, appName, testName, viewportSize);

            EyesMouse.Attach(Logger, this);
            EyesKeyboard.Attach(Logger, this);
        }

        /// <summary>
        /// Starts a new test that does not dictate the viewport size of the application under
        /// test.
        /// </summary>
        public void Open(
            UITestControl mainWindow,
            string appName,
            string testName)
        {
            Open(mainWindow, appName, testName, Size.Empty);
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(string tag = null)
        {
            CheckWindowBase(null, tag);
        }

        /// <summary>
        /// Takes a snapshot of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(TimeSpan matchTimeout, string tag = null)
        {
            CheckWindowBase(null, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Takes a snapshot of the specified control of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(UITestControl control, string tag = null)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            Rectangle? targetRegion = ScreenToClient(control.BoundingRectangle);
            CheckWindowBase(targetRegion, tag);
        }

        /// <summary>
        /// Takes a snapshot of the specified control of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(UITestControl control, TimeSpan matchTimeout, string tag = null)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            Rectangle? targetRegion = ScreenToClient(control.BoundingRectangle);
            CheckWindowBase(targetRegion, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, string tag = null)
        {
            CheckWindowBase(region, tag);
        }

        /// <summary>
        /// Takes a snapshot of the specified region of the application under test and matches it 
        /// with the expected region output.
        /// </summary>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
        /// <param name="region">The region to check</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, TimeSpan matchTimeout, string tag = null)
        {
            CheckWindowBase(region, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Specifies a region of the current application window.
        /// </summary>
        public InRegion InRegion(Rectangle region)
        {
            return new InRegion(this, InRegionBase(() => region));
        }

        /// <summary>
        /// Specifies a region of a control of the current application window.
        /// </summary>
        public InRegion InRegion(UITestControl control)
        {
            ArgumentGuard.NotNull(control, nameof(control));

            var region = ScreenToClient(control.BoundingRectangle);
            return new InRegion(this, InRegionBase(() => region));
        }

        #endregion

        #region Internal

        internal new Rectangle ScreenToClient(Rectangle control)
        {
            return base.ScreenToClient(control);
        }

        internal new bool ScreenToLastScreenshot(
            Rectangle control,
            out Region controlRegion)
        {
            return base.ScreenToLastScreenshot(control, out controlRegion);
        }

        internal new bool ScreenToLastScreenshot(
            Point onScreen, out Point onScreenshot)
        {
            return base.ScreenToLastScreenshot(onScreen, out onScreenshot);
        }

        internal bool IsRelevantControl(UITestControl control)
        {
            return base.IsRelevantControl(control.WindowHandle);
        }

        #endregion

        #endregion
    }
}

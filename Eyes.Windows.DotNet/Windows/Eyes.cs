using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Applitools.Windows
{
    /// <summary>
    /// Applitools Eyes Windows API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "Disposed on Close or AbortIfNotClosed")]
    [ComVisible(true)]
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
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public Eyes(string serverUrl)
            : base(new Uri(serverUrl))
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
        /// <param name="processId">The process id of the application under test.</param>
        /// <param name="appName">The name of the application under test.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="viewportSize">The required application's client area viewport size
        /// or <c>Size.Empty</c> to allow any viewport size.</param>
        public void Open(int processId, string appName, string testName, Size viewportSize)
        {
            OpenBase(processId, appName, testName, viewportSize);
        }

        /// <summary>
        /// Starts a new test that does not dictate the viewport size of the application under 
        /// test.
        /// </summary>
        public void Open(int processId, string appName, string testName)
        {
            Open(processId, appName, testName, Size.Empty);
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
            CheckWindowBase(Rectangle.Empty, tag);
        }

        /// <summary>
        /// Takes a snapshot of the given window and matches it with the expected output.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckWindow(IntPtr hWnd, string tag = null)
        {
            base.requestedWindowHandle = hWnd;
            CheckWindowBase(Rectangle.Empty, tag);
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
            CheckWindowBase(Rectangle.Empty, tag, (int)matchTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Takes a snapshot of specified region of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="region">The region to check.</param>
        /// <param name="tag">An optional tag to be associated with the snapshot.</param>
        /// <exception cref="TestFailedException">
        /// Thrown if a mismatch is detected and immediate failure reports are enabled.</exception>
        public void CheckRegion(Rectangle region, string tag = null)
        {
            CheckWindowBase(region, tag);
        }

        /// <summary>
        /// Takes a snapshot of specified region of the application under test and matches it with
        /// the expected output.
        /// </summary>
        /// <param name="region">The region to check.</param>
        /// <param name="matchTimeout">The amount of time to retry matching</param>
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
        public InRegionBase InRegion(Rectangle region)
        {
            return InRegionBase(() => region);
        }

        #endregion

        #endregion
    }
}

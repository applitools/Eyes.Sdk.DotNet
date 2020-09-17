using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI.Tests
{
    /// <summary>
    /// Applitools Eyes demo tests based on Calculator.
    /// </summary>
    [CodedUITest]
    public class CalculatorTests
    {
        #region Fields

        private UIMap map_;
        private ApplicationUnderTest calc_;

        #endregion

        #region Constructors

        public CalculatorTests()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        public UIMap UIMap
        {
            get
            {
                if (this.map_ == null)
                {
                    this.map_ = new UIMap();
                }

                return this.map_;
            }
        }

        #endregion

        #region Methods

        #region Setup

        [TestInitialize]
        public void MyTestInitialize()
        {
            calc_ = ApplicationUnderTest.Launch(@"C:\Windows\System32\calc1.exe");

            // Start in standard mode.
            Mouse.Click(UIMap.UICalculatorWindow.UIApplicationMenuBar.UIViewMenuItem
                .UIStandardAlt1MenuItem);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            calc_.Close();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void MiniTest()
        {
            var viewMenu = UIMap.UICalculatorWindow.UIApplicationMenuBar.UIViewMenuItem;
            var eyes = new Eyes();
            eyes.SetLogHandler(new StdoutLogHandler());
            eyes.BranchName = "demo";

            try
            {
                eyes.Open(UIMap.UICalculatorWindow, "Calculator", "MiniTest");
                eyes.CheckWindow("Standard");

                Mouse.Click(new System.Drawing.Point(viewMenu.Left + 100, viewMenu.Top + 200));

                var menu = UIMap.UICalculatorWindow.UIApplicationMenuBar;
                eyes.CheckRegion(menu, TimeSpan.FromSeconds(5), "Menu Bar!");

                Assert.AreEqual("View Edit Help", eyes.InRegion(menu).GetText());

                CollectionAssert.AreEqual(
                    new[] { "View Edit Help", "View Edit Help" }, 
                    eyes.InRegion(menu).And(menu).GetText());

                Mouse.Click(viewMenu);
                eyes.CheckWindow("Standard + View");

                Mouse.Click(viewMenu);
                eyes.CheckWindow("Standard");

                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
            }
        }

        [TestMethod]
        [DeploymentItem("Eyes.Sdk.DotNet.logconfig")]
        public void TestMultipleWindows()
        {
            var helpMenu = UIMap.UICalculatorWindow.UIApplicationMenuBar.UIHelpMenuItem;

            var eyes = new Eyes();
            eyes.SetLogHandler(new TraceLogHandler(true));
            eyes.BranchName = "demo";

            try
            {
                eyes.Open(UIMap.UICalculatorWindow, "Calculator", "TestMultipleWindows");
                eyes.CheckWindow("Standard");

                Mouse.Click(helpMenu);
                eyes.CheckWindow("Standard + Help");

                Mouse.Click(helpMenu.UIAboutCalculatorMenuItem);
                eyes.CheckWindow("About");

                Mouse.Click(UIMap.UIAboutCalculatorWindow.UIOKWindow.UIOKButton);
                eyes.CheckWindow("Standard");

                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
            }
        }

        #endregion

        #endregion
    }
}

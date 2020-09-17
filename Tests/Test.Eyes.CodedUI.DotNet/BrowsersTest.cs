[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI.Tests
{
    using System;
    using Applitools;
    using System.Drawing;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Applitools Eyes demo tests based on Calculator.
    /// </summary>
    [CodedUITest]
    public class BrowserTests
    {
        #region Fields

        private UIMap map_;

        #endregion

        #region Constructors

        public BrowserTests()
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
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestGithub()
        {
            var eyes = new Eyes();
            eyes.SetLogHandler(new StdoutLogHandler(true));
            eyes.BranchName = "demo";

            HtmlHyperlink pricingHyperlink = UIMap.UIGoogleWindowsInterneWindow
                .UIGitHubEnterpriseGitrDocument.UIPricingHyperlink;
            HtmlHyperlink faqHyperlink = UIMap.UIGoogleWindowsInterneWindow
                .UIPricingGitHubEnterprDocument.UIFAQHyperlink;
            HtmlHyperlink supportHyperlink = UIMap.UIGoogleWindowsInterneWindow
                .UIFAQGitHubEnterpriseDocument.UISupportHyperlink;
            HtmlHyperlink contactHyperlink = UIMap.UIGoogleWindowsInterneWindow
                .UISupportGitHubEnterprDocument.UIContactHyperlink;
            HtmlHyperlink homeHyperlink = UIMap.UIGoogleWindowsInterneWindow
                .UIContactusGitHubEnterDocument.UIHomeHyperlink;

            UIMap.UIGoogleWindowsInterneWindow.LaunchUrl(
                new Uri("https://enterprise.github.com/"));

            eyes.Open(
                UIMap.UIGoogleWindowsInterneWindow, "Github", "Browse Tabs", new Size(1000, 700));
            try
            {
                eyes.CheckWindow("Home");

                Mouse.Click(pricingHyperlink, new Point(38, 17));
                eyes.CheckWindow("Pricing");

                Mouse.Click(supportHyperlink, new Point(38, 17));
                eyes.CheckWindow("Support");

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

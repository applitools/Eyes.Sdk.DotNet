using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Applitools.StyleCopRuleChecker",
    "AP1101:NamespaceNameShouldConsistOfOneWord",
    Justification = "OK in this case")]

namespace Applitools.CodedUI.Tests
{
    /// <summary>
    /// Applitools Eyes demo based on Explorer.
    /// </summary>
    [CodedUITest]
    public class ExplorerTests
    {
        private UIMap map_;

        public ExplorerTests()
        {
        }
        
        #region Properties

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
            var path = Path.GetFullPath(@"c:\temp\ExplorerTests");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            WinEdit comboBoxEdit = UIMap.UIRunWindow.UIItemWindow1.UIOpenEdit;

            Keyboard.SendKeys("r", ModifierKeys.Windows);
            Keyboard.SendKeys(comboBoxEdit, path + "{Enter}", ModifierKeys.None);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestClicksAndText()
        {
            WinControl newItemDropDownButton = UIMap.UIExplorerTestsWindow
                .UIItemWindow1.UINewToolBar.UINewitemDropDownButton;
            WinMenuItem textDocumentMenuItem = UIMap.UIExplorerTestsWindow
                .UIItemWindow1.UINewitemClient.UIItemGroup.UITextDocumentMenuItem;
            WinMenuItem folderMenuItem = UIMap.UIExplorerTestsWindow.UIItemWindow1
                .UINewitemClient1.UIItemGroup.UIFolderMenuItem;
            WinEdit fileEdit = UIMap.UIExplorerTestsWindow.UIItemWindow.UIItemEdit;
            WinEdit folderEdit = UIMap.UIExplorerTestsWindow.UIItemWindow2.UIItemEdit;
            WinEdit folder = UIMap.UIExplorerTestsWindow.UIItemWindow3
                .UIWorldListItem.UINameEdit;
            var eyes = new Eyes();
            eyes.SetLogHandler(new StdoutLogHandler(true));
            eyes.BranchName = "demo";
            eyes.Open(UIMap.UIExplorerTestsWindow, "Explorer", "TestClicksAndText", new Size(1000, 700));

            try
            {
                eyes.CheckWindow("Empty folder");

                Mouse.Click(newItemDropDownButton, new Point(60, 15));
                eyes.CheckWindow("New Item");

                Mouse.Click(textDocumentMenuItem);
                eyes.CheckWindow("New Text File");

                fileEdit.Text = "Hello.txt";
                Keyboard.SendKeys(fileEdit, "{Enter}", ModifierKeys.None);
                eyes.CheckWindow("File name set");

                Mouse.Click(newItemDropDownButton, new Point(60, 15));
                eyes.CheckWindow("New Item");

                Mouse.Click(folderMenuItem);
                Mouse.Move(folderEdit, Point.Empty);
                eyes.CheckWindow("New Folder");

                folderEdit.Text = "World";
                Keyboard.SendKeys(folderEdit, "{Enter}", ModifierKeys.None);
                eyes.CheckWindow("Folder name set");

                Mouse.Move(folder, new Point(47, 15));
                eyes.CheckWindow("Folder tool-tip");

                Mouse.Click(folder, MouseButtons.Right, ModifierKeys.None, new Point(47, 15));
                eyes.CheckWindow("Folder context menu");

                Mouse.DoubleClick(folder, new Point(47, 15));
                eyes.CheckWindow("Empty World folder");

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

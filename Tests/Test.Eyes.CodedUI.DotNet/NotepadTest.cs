namespace Applitools.CodedUI.Tests
{
    using CodedUI;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;
    using System.Drawing;

    [CodedUITest]
    public class NotepadTest
    {
        [TestMethod]
        public void TestNotepad()
        {
            Process notepad = Process.Start("notepad");

            // Open notepad
            WinWindow testApp = new WinWindow();
            testApp.SearchProperties[WinWindow.PropertyNames.ClassName] = "Notepad";

            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            eyes.SetLogHandler(new FileLogHandler(@"c:\temp\logs\codedui_notepad_test.log", true, true));

            try
            {
                // Start the test and set the application's viewport size to 800x600
                eyes.Open(testApp, "Hello World!", "My first CodedUI C# test", new Size(800, 600));
                
                // Visual checkpoint #1
                eyes.CheckWindow("Hello!");

                // Write something
                WinEdit edit = new WinEdit(testApp);
                Keyboard.SendKeys("Applitools CodedUI Demo");

                // Visual checkpoint #2
                eyes.CheckWindow("Write!");

                // End the test
                eyes.Close();
            }
            finally
            {
                // Close the app.
                notepad.CloseMainWindow();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.AbortIfNotClosed();
            }
        }
    }
}
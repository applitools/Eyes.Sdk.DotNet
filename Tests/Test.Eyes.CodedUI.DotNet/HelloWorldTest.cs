using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Applitools.CodedUI.Tests
{
    [CodedUITest]
    public class HelloWorldTest
    {
        [TestMethod]
        public void TestHelloWorld()
        {
            // Open a browser
            BrowserWindow testBrowser = BrowserWindow.Launch();

            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            try
            {
                // Start the test and set the browser's viewport size to 800x600
                eyes.Open(testBrowser, "Hello World!", "My first CodedUI C# test", new Size(800, 600));

                // Navigate the browser to the "hello world!" web-site.
                testBrowser.NavigateToUrl(new Uri("https://applitools.com/helloworld"));

                // Visual checkpoint #1
                eyes.CheckWindow("Hello!");

                // Click the "Click me!" button
                HtmlDocument doc = new HtmlDocument(testBrowser);
                HtmlButton button = new HtmlButton(doc);
                Mouse.Click(button);

                // Visual checkpoint #2
                eyes.CheckWindow("Click!");

                // End the test
                eyes.Close();
            }
            finally
            {
                // Close the browser.
                testBrowser.Close();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.AbortIfNotClosed();
            }
        }
    }
}
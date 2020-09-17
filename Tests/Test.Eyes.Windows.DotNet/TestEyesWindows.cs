namespace Applitools.Windows
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using Applitools;
    using Applitools.Utils;
    using NUnit.Framework;
    using System.Windows.Forms;
    using System.Reflection;
    using System.IO;
    using Applitools.Correlate.Capture;
    using Applitools.Tests.Utils;
    using System.Net;

    [TestFixture]
    public class TestEyesWindows
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        //[Test]
        public void TestCheckRegion()
        {
            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            eyes.Open(12148, "Calc", "TestCheckRegion");
            eyes.CheckRegion(new Rectangle(10, 10, 200, 200));
            eyes.Close();
        }

        //[Test]
        public void TestChangePid()
        {
            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            eyes.Open(12148, "Calc", "TestChangePid");
            eyes.CheckWindow("1");
            eyes.ProcessId = 3732;
            eyes.CheckWindow("2");
            eyes.ProcessId = 12148;
            eyes.CheckWindow("1");
            eyes.Close();
        }

        [Test]
        public void TestProcessRestart()
        {
            Process process1 = Process.Start("calc.exe");
            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            eyes.Open(process1.Id, "Calc", "TestProcessRestart-New");
            eyes.CheckWindow(TimeSpan.FromSeconds(5), "process1 = {0}".Fmt(process1.Id));
            process1.Kill();

            // Make sure the new window is already active by the time the process id is changed.
            Process process2 = Process.Start("calc.exe");
            Thread.Sleep(2000);
            eyes.ProcessId = process2.Id;
            eyes.CheckWindow("process2 = {0}".Fmt(process2.Id));
            process2.Kill();

            eyes.Close();
        }

        [Test]
        public void TestInRegion()
        {
            Process process1 = Process.Start("calc.exe");
            ////var eyes = new Eyes("https://eyes2.applitools.com");
            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            TestUtils.SetupLogging(eyes);

            eyes.Open(process1.Id, "Calc", "TestInRegion");
            try
            {
                var text = eyes.InRegion(new Rectangle(0, 0, 120, 19)).GetText();

                var texts = eyes.InRegion(new Rectangle(0, 0, 40, 19))
                    .And(new Rectangle(40, 0, 35, 19))
                    .And(new Rectangle(75, 0, 35, 19)).GetText();

                eyes.Close();

                Assert.AreEqual("View Edit Help", text);
                Assert.AreEqual("View", texts[0]);
                Assert.AreEqual("Edit", texts[1]);
                Assert.AreEqual("HelP", texts[2]);
            }
            finally
            {
                process1.Kill();
                eyes.AbortIfNotClosed();
            }
        }

        [Test]
        public void TestCheckRegionInNotepad()
        {
            Process notepadProcess = Process.Start("notepad");

            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            eyes.IgnoreCaret = true;

            eyes.Open(notepadProcess.Id, "Notepad", "TestCheckRegionInNotepad");

            eyes.CheckWindow("Step 1");

            SendKeys.SendWait("Applitools Native Windows Demo");

            eyes.CheckWindow("Step 2");

            notepadProcess.CloseMainWindow();
            notepadProcess.Kill();

            eyes.Close();
        }

        [Test]
        public void TestWpfApplication()
        {
            string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);
            string currentResourcesDir = Path.Combine(currentAssemblyDir, "Properties", "Resources");
            string demoAppPath = Path.Combine(currentResourcesDir, "Tests.Eyes.Windows.WPF.exe");
            Process demoApp = Process.Start(demoAppPath, "--show-popup");

            int pid = demoApp.Id;
            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            eyes.Open(pid, "WPF Demo App", "WPF Demo App");

            eyes.CheckWindow("Pop Up");

            demoApp.Kill();
            eyes.Close();
        }

        [Test]
        public void TestCheckGetWindowsApi()
        {
            Process notepadProcess = Process.Start("notepad");
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            try
            {
                WindowCaptureInfo[] windows = EyesWindowsBase.GetWindows(logger, notepadProcess.Id, "Notepad");
            }
            finally
            {
                notepadProcess.CloseMainWindow();
                notepadProcess.Kill();
            }
        }

        [Test]
        public void TestGetWindowsOnWpfApplication()
        {
            string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);
            string currentResourcesDir = Path.Combine(currentAssemblyDir, "Properties", "Resources");
            string demoAppPath = Path.Combine(currentResourcesDir, "Tests.Eyes.Windows.WPF.exe");
            Process demoApp = Process.Start(demoAppPath, "--show-popup");

            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());

            int pid = demoApp.Id;
            try
            {
                Thread.Sleep(5000);
                WindowCaptureInfo[] windows = EyesWindowsBase.GetWindows(logger, pid);
                Assert.AreEqual(2, windows.Length, "number of windows");
                Assert.AreEqual("Pop Up", windows[0].Caption, "popup caption");
                Assert.AreEqual("Main Window", windows[1].Caption, "main window caption");
            }
            finally
            {
                demoApp.Kill();
            }
        }

        [Test]
        public void TestCheckHWnd()
        {
            Process notepadProcess = Process.Start("notepad");

            Eyes eyes = new Eyes();
            eyes.Batch = TestUtils.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            try
            {
                eyes.IgnoreCaret = true;
                eyes.Open(notepadProcess.Id, "Notepad", "TestCheckHWnd");

                WindowCaptureInfo[] windows = eyes.GetWindows();

                eyes.CheckWindow(windows[0].Handle, windows[0].Caption);
            }
            finally
            {
                notepadProcess.CloseMainWindow();
                notepadProcess.Kill();

                eyes.Close();
            }
        }
    }
}

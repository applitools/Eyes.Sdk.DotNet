using Applitools.VisualGrid;
using System;

namespace Applitools
{
    public class TestResultContainer
    {
        public TestResults TestResults { get; }
        public RenderBrowserInfo BrowserInfo { get; }
        public Exception Exception { get; }

        public TestResultContainer(TestResults testResults, RenderBrowserInfo browserInfo = null, Exception exception = null)
        {
            TestResults = testResults;
            BrowserInfo = browserInfo;
            Exception = exception;
        }

        public override string ToString()
        {
            string browserInfoStr = BrowserInfo != null ? "\n browserInfo = " + BrowserInfo : string.Empty;
            return "TestResultContainer {" +
                    "\n testResults=" + TestResults +
                     browserInfoStr +
                    "\n exception = " + Exception +
                    '}';
        }
    }
}
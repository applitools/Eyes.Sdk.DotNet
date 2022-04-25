using System.Collections.Generic;
using Applitools.Selenium.Commands;

namespace Applitools
{
    public class CloseAllResult
    {
        public TestResults TestResults { get; set; }
        public object Exception { get; set; }
        //public DesktopBrowserInfo BrowserInfo { get; set; }
    }

    public class EyesManagerCloseManagerResult
    {
        public IList<CloseAllResult> Results { get; set; }

        public int Passed { get; set; }
        public int Unresolved { get; set; }
        public int Failed { get; set; }
        public int Exceptions { get; set; }
        public int? Mismatches { get; set; }
        public int? Missing { get; set; }
        public int? Matches { get; set; }

        public EyesManagerCloseManagerResult()
        {
            Results = new List<CloseAllResult>();
        }
    }

    public class EyesManagerCloseManagerResponsePayload
    {
        public EyesManagerCloseManagerResult Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesManagerCloseManagerResponse : CommandBase
    {
        public EyesManagerCloseManagerResponsePayload Payload { get; set; }
    }
}
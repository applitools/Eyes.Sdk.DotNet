using System.Collections.Generic;
using Applitools.Selenium.Commands;
using Newtonsoft.Json;


namespace Applitools
{
    public class UniversalBrowserInfo
    {
        public string Name { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public UniversalDeviceInfo IosDeviceInfo { get; set; }
        public UniversalDeviceInfo AndroidDeviceInfo { get; set; }
        public UniversalDeviceInfo ChromeEmulationInfo { get; set; }
    }

    public class UniversalDeviceInfo
    {
        public string DeviceName { get; set; }
        public string ScreenOrientation { get; set; }
        public string Version { get; set; }
    }

    public class UniversalException
    {
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Reason { get; set; }
    }

    public class CloseAllResult
    {
        public TestResults TestResults { get; set; }
        public UniversalException Exception { get; set; }
        [JsonProperty]
        public UniversalBrowserInfo BrowserInfo { get; set; }
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
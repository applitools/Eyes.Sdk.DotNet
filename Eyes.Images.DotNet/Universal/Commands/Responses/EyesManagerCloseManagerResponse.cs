using System.Collections.Generic;
using Applitools.Commands;
using Newtonsoft.Json;

namespace Applitools
{
    public class Renderer
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
    
    public class Error
    {
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Reason { get; set; }
    }

    public class CloseAllResult
    {
        public TestResults Result { get; set; }
        public string UserTestId { get; set; }
        public Error Error { get; set; }
        public Renderer Renderer { get; set; }
    }

    public class EyesManagerCloseManagerResult
    {
        public ICollection<CloseAllResult> Results { get; set; }

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

    public class CloseManagerResponsePayload
    {
        public EyesManagerCloseManagerResult Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesManagerCloseManagerResponse : CommandBase
    {
        public CloseManagerResponsePayload Payload { get; set; }
    }
}
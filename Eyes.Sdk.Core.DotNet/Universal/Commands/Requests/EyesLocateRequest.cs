using System.Collections.Generic;
using Applitools.Fluent;

namespace Applitools.Selenium.Commands.Requests
{
    public class LocateSettings
    {
        public object[] LocatorNames { get; set; }
        public bool? FirstOnly { get; set; }
    }

    public class EyesLocateRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public LocateSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }
     
    public class EyesLocateRequest : CommandBase
    {
        public EyesLocateRequestPayload Payload { get; set; }

        public EyesLocateRequest()
        {
            Name = "Eyes.locate";
        }
    }

    public class LocateResponse : CommandBase
    {
        public EyesLocateRequestPayload Payload { get; set; }

        public LocateResponse()
        {
            Name = "Eyes.locate";
        }
    }
}
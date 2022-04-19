using System.Collections.Generic;
namespace Applitools.Selenium.Commands.Responses
{
    public class EyesCloseAbortResponsePayload
    {
        public IList<TestResults> Result { get; set; }
        public ResponsePayloadError Error { get; set; }

        public EyesCloseAbortResponsePayload()
        {
            Result = new List<TestResults>();
        }
    }

    public class EyesCloseAbortResponse : CommandBase
    {
        public EyesCloseAbortResponsePayload Payload { get; set; }
    }
}
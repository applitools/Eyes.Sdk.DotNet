namespace Applitools.Commands.Requests
{
    public class EyesGetResultsRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ResultsSettings Settings { get; set; }
    }

    public class EyesGetResultsRequest : CommandBase
    {
        public EyesGetResultsRequestPayload Payload { get; set; }

        public EyesGetResultsRequest()
        {
            Name = "Eyes.getResults";
        }
    }
}
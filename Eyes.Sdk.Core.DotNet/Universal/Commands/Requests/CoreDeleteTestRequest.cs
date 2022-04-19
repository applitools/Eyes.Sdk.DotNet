namespace Applitools.Selenium.Commands.Requests
{
    public class CoreDeleteTestSettings
    {
        public string TestId { get; set; }
        public string BatchId { get; set; }
        public string ServerUrl { get; set; }
        public string SecretToken { get; set; }
        public string ApiKey { get; set; }
    }

    public class CoreDeleteTestRequestPayload
    {
        public CoreDeleteTestSettings Settings { get; set; }
    }

    public class CoreDeleteTestRequest : CommandBase
    {
        public CoreDeleteTestRequestPayload Payload { get; set; }

        public CoreDeleteTestRequest()
        {
            Name = "Core.deleteTest";
        }
    }
}
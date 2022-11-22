namespace Applitools
{
    public class DeleteTestSettings
    {
        public string TestId { get; set; }
        public string BatchId { get; set; }
        public string ServerUrl { get; set; }
        public string SecretToken { get; set; }
        public string ApiKey { get; set; }
        public ProxySettings Proxy { get; set; }
    }
}
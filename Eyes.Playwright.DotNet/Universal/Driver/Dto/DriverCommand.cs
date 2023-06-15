using Applitools.Commands;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverCommand : CommandBase
    {
        [JsonProperty("payload")]
        public DriverCommandDto Payload { get; set; }
    }
}
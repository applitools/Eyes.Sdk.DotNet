using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverCommandDto
    {
        [JsonProperty("driver")]
        public Reference Driver { get; set; }

        [JsonProperty("context")]
        public Reference Context { get; set; }

        [JsonProperty("element")]
        public Reference Element { get; set; }

        [JsonProperty("parent")]
        public Reference Parent { get; set; }

        [JsonProperty("selector")]
        public Selector Selector { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("arg")]
        public object Arg { get; set; }

        [JsonProperty("size")]
        public RectangleSize Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public override string ToString()
        {
            return "DriverCommandDto{" +
                   "driver=" + Driver +
                   ", context=" + Context +
                   ", element=" + Element +
                   ", parent=" + Parent +
                   ", selector=" + Selector +
                   ", script='" + Script + '\'' +
                   ", arg=" + Arg +
                   ", size=" + Size +
                   ", url='" + Url + '\'' +
                   '}';
        }
    }
}
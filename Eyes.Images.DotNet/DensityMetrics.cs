using Newtonsoft.Json;

namespace Applitools
{
    public class DensityMetrics
    {
        [JsonProperty("scaleRatio")]
        public double? ScaleRatio {get;set;}
        
        [JsonProperty("xdpi")]
        public int? XDpi {get;set;}
        
        [JsonProperty("ydpi")]
        public int? YDpi {get;set;}
    }
}
using Applitools.Utils.Cropping;
using Newtonsoft.Json;

namespace Applitools.Universal.Dto
{
    public class NormalizationDto
    {
        [JsonProperty("cut")]
        public ICutProvider CutProvider { get; set; }

        [JsonProperty("rotation")]
        public int? Rotation { get; set; }
        
        [JsonProperty("scaleRatio")]
        public double? ScaleRatio { get; set; }
    }
}
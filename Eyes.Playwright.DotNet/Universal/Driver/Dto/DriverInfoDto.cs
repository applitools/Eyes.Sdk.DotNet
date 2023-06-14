using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverInfoDto
    {
        [JsonProperty("features")]
        public DriverInfoFeaturesDto Features { get; set; }

        public DriverInfoDto()
        {
            Features = new DriverInfoFeaturesDto();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is DriverInfoDto other))
            {
                return false;
            }

            return Features.Equals(other.Features);
        }

        public override int GetHashCode()
        {
            return Features?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return "DriverInfoDto{" +
                   "features=" + Features +
                   '}';
        }
    }
}
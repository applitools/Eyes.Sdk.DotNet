namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverInfoDto
    {
        private readonly DriverInfoFeaturesDto features_;

        public DriverInfoDto()
        {
            features_ = new DriverInfoFeaturesDto();
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

            return features_.Equals(other.features_);
        }

        public override int GetHashCode()
        {
            return features_?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return "DriverInfoDto{" +
                   "features=" + features_ +
                   '}';
        }
    }
}
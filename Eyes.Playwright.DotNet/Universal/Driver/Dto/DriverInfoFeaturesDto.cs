using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverInfoFeaturesDto
    {
        [JsonProperty("allCookies")]
        public bool AllCookies { get; set; }
        
        [JsonProperty("canExecuteOnlyFunctionScripts")]
        public bool CanExecuteOnlyFunctionScripts { get; set; }

        public DriverInfoFeaturesDto()
        {
            AllCookies = true;
            CanExecuteOnlyFunctionScripts = true;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is DriverInfoFeaturesDto other))
            {
                return false;
            }

            return other.AllCookies.Equals(AllCookies) &&
                   other.CanExecuteOnlyFunctionScripts.Equals(CanExecuteOnlyFunctionScripts);
        }

        public override int GetHashCode()
        {
            return (AllCookies ? 1 : 0) + (CanExecuteOnlyFunctionScripts ? 2 : 0);
        }

        public override string ToString()
        {
            return "DriverInfoFeaturesDto{" +
                   "allCookies=" + AllCookies +
                   ", canExecuteOnlyFunctionScripts=" + CanExecuteOnlyFunctionScripts +
                   '}';
        }
    }
}
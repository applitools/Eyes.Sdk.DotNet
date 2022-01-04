using Applitools;

namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class EyesCheckRequest
    {
        public EyesRef Eyes { get; set; }
        public ICheckSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }
}
namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class EyesCloseRequest
    {
        public string Name => "Eyes.close";
        public EyesRef Eyes { get; set; }
    }
}
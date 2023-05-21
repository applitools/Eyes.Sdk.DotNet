namespace Applitools.Universal
{
    public class WsMessage
    {
        public WsMessage(string message)
        {
            Message = message;
        }
        public string Message { get; }
        public bool IsHandled { get; set; }
    }
}
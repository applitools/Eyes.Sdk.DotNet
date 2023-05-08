using Applitools.Commands;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver
{
    public class PlaywrightSpecDriverMessageAdaptor : ISpecDriverMessageAdaptor
    {
        public TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase
        {
            return JsonConvert.DeserializeObject<TResponse>(message);
        
            // TODO - if not webdriver invoke specdriver to change result
            //          - will invoke specdriver command that will send its result to universal
            // while (response.GetType() != typeof(TResponse)) {
            //    response = invoke specdriver command
            //    Response = sendResponseToUniversal(response);
            // }
        }
    }
}
using System;
using System.Reflection;
using System.Threading.Tasks;
using Eyes.Universal.Sdk.DotNet.Commands.Client;
using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet
{
    public class SocketMessageManager
    {
        private readonly WebSocketClient _socketClient;

        public SocketMessageManager()
        {
            _socketClient = new WebSocketClient();
        }

        public async Task SendRequest(CoreMakeSdk CoreMakeSdk)
        {
        }

        public async Task<CoreMakeManagerResponse> SendRequest(CoreMakeManager coreMakeManager)
        {
            //var coreMakeManager = CreateCoreMakeManager();
            var coreMakeManagerJson = ConvertData(coreMakeManager);

            await _socketClient.SendData(coreMakeManagerJson);

            CoreMakeManagerResponse coreMakeManagerResponse = null;

            do
            {
                var text = await _socketClient.ReceiveMessage();

                coreMakeManagerResponse = JsonConvert.DeserializeObject<CoreMakeManagerResponse>(text);
            } while (coreMakeManagerResponse?.Payload?.Result?.ApplitoolsRefId != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

        public string ConvertData<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        private CoreMakeSdk CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdk();
            result.AddOrUpdatePayload("name", "netSDK");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }
        private CoreMakeManager CreateCoreMakeManager()
        {
            var result = new CoreMakeManager();
            result.Key = Guid.NewGuid().ToString();
            result.AddOrUpdatePayload("type", "classic");

            return result;
        }
    }
}
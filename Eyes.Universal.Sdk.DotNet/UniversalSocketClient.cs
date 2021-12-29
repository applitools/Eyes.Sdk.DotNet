using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using Applitools;
using Eyes.Universal.Sdk.DotNet.Commands.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Eyes.Universal.Sdk.DotNet
{
    public class CoreMakeManagerResponseResult
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }

    public class CoreMakeManagerResponsePayload
    {
        [JsonProperty("result")]
        public CoreMakeManagerResponseResult Result { get; set; }
    }

    public class CoreMakeManagerResponse //: ClientCommandBase
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("payload")]
        public CoreMakeManagerResponsePayload Payload { get; set; }
    }
    
    public class UniversalSocketClient //: IUniversalSocketClient
    {
        private WebSocketClient _socketClient;
        private SocketMessageManager socketMessageManager_;
        private CoreMakeManagerResponse coreMakeManagerResponse;

        public UniversalSocketClient(ILogHandler logHandler = null)
        {
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            _socketClient = new WebSocketClient();
            socketMessageManager_ = new SocketMessageManager();
        }
        
        public void Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            var url = GetSocketUrl(2107);
            _socketClient.ConnectAsync(url).GetAwaiter().GetResult();

            var makeSdk = CreateCoreMakeSdk(); 
            var makeSdkJson = ConvertData(makeSdk);

            _socketClient.SendData(makeSdkJson);

            var coreMakeManager = CreateCoreMakeManager();

            coreMakeManagerResponse = SendRequest(coreMakeManager).Result;

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(coreMakeManagerResponse.Payload.Result.ApplitoolsRefId, driver);
            var eyesManagerOpenEyesJson = ConvertData(eyesManagerOpenEyes);
            
            _socketClient.SendData(eyesManagerOpenEyesJson);

        //    driver.

            //Task.WaitAll(text);

            //Debug.WriteLine(text.Result);
        }

        public void Check(string name, ICheckSettings checkSettings)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            _socketClient.Dispose();
        }

        public void Close()
        { 
            _socketClient.Close();
        }

        public string ConvertData<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return json;
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
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

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(string applitoolsRefId, IWebDriver webDriver)
        {
            var remoteWebDriver = (webDriver as RemoteWebDriver);
            var result = new EyesManagerOpenEyes();
            result.Key = Guid.NewGuid().ToString();
            result.Payload = new EyesManagerOpenEyesRequestPayload
            {
                Manager = new EyesManagerOpenEyesRequestManager
                {
                    ApplitoolsRefId = applitoolsRefId
                },
                Driver = new UniversalDriverConfig
                {
                    ServerUrl = webDriver.Url, 
                    Capabilities = remoteWebDriver?.Capabilities,
                    SessionId = remoteWebDriver?.SessionId
                },
                Config = new EyesConfig
                {
                    ServerUrl = remoteWebDriver?.Url,
                   // AgentId = remoteWebDriver?.a
                }
            };

            return result;
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
            } while (coreMakeManagerResponse?.Key != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

    }
}
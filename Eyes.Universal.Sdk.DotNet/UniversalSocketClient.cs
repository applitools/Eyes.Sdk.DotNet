using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Applitools;
using Eyes.Universal.Sdk.DotNet.Commands.Client;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace Eyes.Universal.Sdk.DotNet
{
    public class UniversalSocketClient //: IUniversalSocketClient
    {
        private WebSocketClient _socketClient;

        public UniversalSocketClient(ILogHandler logHandler = null)
        {
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            _socketClient = new WebSocketClient();
        }
        
        public void Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            var url = GetSocketUrl(2107);
            _socketClient.ConnectAsync(url).GetAwaiter().GetResult();

            var makeSdk = CreateCoreMakeSdk();
            var makeSdkJson = ConvertData(makeSdk);

            _socketClient.SendData(makeSdkJson);

            var coreMakeManager = CreateCoreMakeManager();
            var coreMakeManagerJson = ConvertData(coreMakeManager);

            _socketClient.SendData(coreMakeManagerJson);

            var buffer = new byte[1024];
            Console.WriteLine("Receiving");
            var result = _socketClient.clientWebSocket_.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

            var text = _socketClient.ReceiveMessage();
            Debug.WriteLine(text.Result);
        }

        public string ConvertData<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
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
    }
}
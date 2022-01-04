using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using Applitools;
using Eyes.Universal.Sdk.DotNet.Commands;
using Eyes.Universal.Sdk.DotNet.Commands.Requests;
using Eyes.Universal.Sdk.DotNet.Commands.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Eyes.Universal.Sdk.DotNet
{
    public class UniversalSocketClient //: IUniversalSocketClient
    {
        private WebSocketClient socketClient_;
        private CoreMakeManagerResponse coreMakeManagerResponse_;
        private EyesRef eyes_;

        public UniversalSocketClient(ILogHandler logHandler = null)
        {
            if (logHandler == null) logHandler = NullLogHandler.Instance;
            socketClient_ = new WebSocketClient();
        }
        
        public void Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            var driverUrl = driver.Url;
            var url = GetSocketUrl(2107);
            socketClient_.ConnectAsync(url).GetAwaiter().GetResult();

            var makeSdk = CreateCoreMakeSdk(); 
            var makeSdkJson = ConvertData(makeSdk);

            socketClient_.SendData(makeSdkJson);

            var coreMakeManager = CreateCoreMakeManager();

            coreMakeManagerResponse_ = SendRequest(coreMakeManager).Result;
            
            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(coreMakeManagerResponse_.Payload.Result.ApplitoolsRefId, driver);
            
            eyes_ = SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Result.Payload.Eyes;

            //var eyesManagerOpenEyesJson = ConvertData(eyesManagerOpenEyes);

            //socketClient_.SendData(eyesManagerOpenEyesJson);
            //var text =  socketClient_.ReceiveMessage();

        }

        public void Check(string name, ICheckSettings checkSettings)
        {
            var request = new EyesCheckRequest
            {
                Eyes = eyes_,
                Settings = checkSettings,
                Config = new EyesConfig
                {
                    DisplayName = name
                }
            };

            var matchResults = SendRequest<EyesCheckRequest, MatchResult>(request);
        }
        
        public void Dispose()
        {
            socketClient_.Dispose();
        }

        public void CloseAll()
        { 
            var closeAllEyes = CreateEyesManageCloseAllEyes();

            var testResult = SendRequest(closeAllEyes);

            socketClient_.Close();
        }

        public void Close()
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Eyes = eyes_
            };

            var testResult = SendRequest<EyesCloseRequest, TestResult>(eyesCloseRequest);

            socketClient_.Close();
        }

        public string ConvertData<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }

        private EyesManageCloseAllEyes CreateEyesManageCloseAllEyes()
        {
            var closeAllEyes = new EyesManageCloseAllEyes
            {
                Payload = new EyesManageCloseAllEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = coreMakeManagerResponse_.Payload.Result.ApplitoolsRefId
                    }
                }
            };
            return closeAllEyes;
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
            var result = new CoreMakeManager
            {
                Key = Guid.NewGuid().ToString()
            };
            result.AddOrUpdatePayload("type", "classic");

            return result;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(string applitoolsRefId, IWebDriver webDriver)
        {
            var remoteWebDriver = webDriver as RemoteWebDriver;
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesManagerOpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Driver = new UniversalDriverConfig
                    {
                        ServerUrl = webDriver.Url, 
                        Capabilities = remoteWebDriver?.Capabilities,
                        SessionId = remoteWebDriver?.SessionId.ToString()
                    },
                    Config = new EyesConfig
                    {
                        ServerUrl = remoteWebDriver?.Url
                    }
                }
            };

            return result;
        }
        
        public async Task<CoreMakeManagerResponse> SendRequest(CoreMakeManager coreMakeManager)
        {
            var coreMakeManagerJson = ConvertData(coreMakeManager);

            await socketClient_.SendData(coreMakeManagerJson);

            CoreMakeManagerResponse coreMakeManagerResponse = null;

            do
            {
                var response = await socketClient_.ReceiveMessage();

                coreMakeManagerResponse = JsonConvert.DeserializeObject<CoreMakeManagerResponse>(response);
            } while (coreMakeManagerResponse?.Key != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

        public async Task<TestResult> SendRequest(EyesManageCloseAllEyes data)
        {
            var json = ConvertData(data);

            await socketClient_.SendData(json);

            TestResult result;

            do
            {
                var text = await socketClient_.ReceiveMessage();

                result = JsonConvert.DeserializeObject<TestResult>(text);
            //} while (coreMakeManagerResponse_?.Key != result?.Key);
            } while (result == null);

            return result;
        }
        
        private async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest data)
        {
            var json = ConvertData(data);

            await socketClient_.SendData(json);

            TResponse result;

            do
            {
                var text = await socketClient_.ReceiveMessage();
                result = JsonConvert.DeserializeObject<TResponse>(text);
            } while (result == null);

            return result;
        }
    }
}
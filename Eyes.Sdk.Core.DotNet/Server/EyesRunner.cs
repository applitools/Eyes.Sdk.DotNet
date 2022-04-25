using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Applitools.Selenium;
using Applitools.Selenium.Commands;
using Applitools.Selenium.Commands.Requests;
using Applitools.Selenium.Commands.Responses;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    public abstract class EyesRunner
    {
        private string apiKey;
        private string serverUrl;
        private ProxySettings proxy;
        private string managerApplitoolsRefId_;
        private ManagerConfig config_;

        protected WebSocketClient SocketClient { get; }

        public ManagerConfig Config
        {
            get => config_ ?? (config_ = InitConfig());
            set => config_ = value;
        }

        public bool DontCloseBatches { get; set; }

        public string ApiKey
        {
            get => apiKey;
            set
            {
                apiKey = value;
                //foreach (var eye in GetAllEyes())
                //{
                //    eye.ApiKey = value;
                //}
            }
        }

        public string ServerUrl
        {
            get => serverUrl;
            set
            {
                serverUrl = value;
                foreach (var eye in GetAllEyes())
                {
                    eye.ServerUrl = value;
                }
            }
        }

        public ProxySettings Proxy
        {
            get => proxy;
            set
            {
                proxy = value;
                foreach (var eye in GetAllEyes())
                {
                    eye.Proxy = value;
                }
            }
        }

        public bool IsDisabled { get; set; }

        public bool GetAllTestResultsAlreadyCalled { get; } = false;

        public Logger Logger { get; } = new Logger();

        protected EyesRunner(ILogHandler logHandler)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            UniversalSdkNativeLoader.Start();

            SocketClient = new WebSocketClient();

            var url = GetSocketUrl(21077);
            SocketClient.ConnectAsync(url).GetAwaiter().GetResult();
        }

        public async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest data) 
            where TRequest : CommandBase 
            where TResponse : CommandBase
        {
            var json = data.ToJson();

            await SocketClient.SendData(json);

            TResponse result;

            do
            {
                var response = await SocketClient.ReceiveMessage();
                result = JsonConvert.DeserializeObject<TResponse>(response);
            } while (result == null || result.Key != data.Key);

            return result;
        }

        public async Task SendRequest<TRequest>(TRequest data)
        {
            var json = data.ToJson();

            await SocketClient.SendData(json);
        }

        public TestResultsSummary GetAllTestResults()
        {
            return GetAllTestResults(true);
        }
        
        public TestResultsSummary GetAllTestResults(bool shouldThrowException)
        {
            if (IsDisabled)
            {
                return new TestResultsSummary(new List<TestResultContainer>());
            }

            var results = CloseAllEyes(shouldThrowException);
           
            var testResultContainers = results.Select(i => new TestResultContainer(i)).ToList();

            return new TestResultsSummary(testResultContainers);
        }

        public void SetLogHandler(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
            if (!logHandler.IsOpen)
            {
                logHandler.Open();
            }
        }

        public string GetCoreMakeManager()
        {
            if (managerApplitoolsRefId_ == null)
            {
                var makeSdk = CreateCoreMakeSdk();
                var makeSdkJson = makeSdk.ToJson();

                SocketClient.SendData(makeSdkJson);

                var coreMakeManager = CreateCoreMakeManager();

                var coreMakeManagerResponse = SendRequest<CoreMakeManagerRequest, CoreMakeManagerResponse>(coreMakeManager).Result;
                managerApplitoolsRefId_ = coreMakeManagerResponse.Payload.Result.ApplitoolsRefId;
            }

            return managerApplitoolsRefId_;
        }

        public IList<TestResults> CloseAllEyes(bool shouldThrowException)
        {
            var results = new List<TestResults>();

            if (IsDisabled)
            {
                return results;
            }

            var request = CreateEyesManageCloseAllEyes(shouldThrowException);

            var response = SendRequest<EyesManagerCloseManagerRequest, EyesManagerCloseManagerResponse>(request).Result;
            var payload = response.Payload;

            if (payload != null)
            {
                var error = payload.Error;
                if (error != null)
                {
                    throw new EyesException(error.ToString());
                }

                foreach (var closeAllTestResults in payload.Result.Results)
                {
                    if (closeAllTestResults.Exception != null && shouldThrowException)
                    {
                        throw new EyesException(closeAllTestResults.Exception.ToString());
                    }

                    var testResults = closeAllTestResults.TestResults;
                    if (testResults != null)
                    {
                        testResults.Runner = this;
                        results.Add(testResults);
                    }
                }
            }

            return results;
        }

        protected abstract ManagerConfig InitConfig();

        protected abstract IEnumerable<IEyesBase> GetAllEyes();

        private EyesManagerCloseManagerRequest CreateEyesManageCloseAllEyes(bool shouldThrowException)
        {
            var closeAllEyes = new EyesManagerCloseManagerRequest
            {
                Payload = new EyesManageCloseManagerRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = managerApplitoolsRefId_
                    },
                    ThrowErr = shouldThrowException
                }
            };

            return closeAllEyes;
        }
        
        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }

        private CoreMakeSdkRequest CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdkRequest();
            result.AddOrUpdatePayload("name", "netUniversal4");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }

        private CoreMakeManagerRequest CreateCoreMakeManager()
        {
            var result = new CoreMakeManagerRequest
            {
                Payload =  Config
            };

            return result;
        }
    }
}
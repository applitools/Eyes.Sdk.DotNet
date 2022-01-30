using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Applitools.Selenium;
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

        protected abstract ManagerConfig InitConfig();

        protected abstract IEnumerable<IEyesBase> GetAllEyes();

        public Logger Logger { get; } = new Logger();

        public async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest data)
        {
            var json = data.ToJson();

            await SocketClient.SendData(json);

            TResponse result;

            do
            {
                var response = await SocketClient.ReceiveMessage();
                result = JsonConvert.DeserializeObject<TResponse>(response);
            } while (result == null);

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

            var results = CloseAllEyes();
            if (shouldThrowException)
            {
                var mismatchedResults = results.Where(i => i.Mismatches > 0).ToList();
                if (mismatchedResults.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var mr in mismatchedResults)
                    {
                        sb.AppendLine();
                        sb.AppendFormat("Name: {0}", mr.Name);
                    }

                    throw new EyesException(sb.ToString());
                }
            }

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

        public string ApiKey
        {
            get => apiKey;
            set
            {
                apiKey = value;
                foreach (var eye in GetAllEyes())
                {
                    eye.ApiKey = value;
                }
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

        public bool GetAllTestResultsAlreadyCalled { get; private set; } = false;
         
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

        public IList<TestResults> CloseAllEyes()
        {
            if (IsDisabled)
            {
                return new List<TestResults>();
            }

            var request = CreateEyesManageCloseAllEyes();

            var response = SendRequest<EyesManageCloseAllEyesRequest, EyesCloseResponse>(request).Result;
            if (response.Payload?.Error != null)
            {
                throw new EyesException(response.Payload.Error.Message);
            }

            return response?.Payload?.Result;
        }

        private EyesManageCloseAllEyesRequest CreateEyesManageCloseAllEyes()
        {
            var closeAllEyes = new EyesManageCloseAllEyesRequest
            {
                Payload = new EyesManageCloseAllEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = managerApplitoolsRefId_
                    }
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
            result.AddOrUpdatePayload("name", "netSDK");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
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
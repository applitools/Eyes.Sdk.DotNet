using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Utils;

namespace Applitools
{
    public abstract class EyesRunner
    {
        private string managerApplitoolsRefId_;
        private MakeManagerRequestPayload config_;
        private readonly Assembly _actualAssembly;
        private readonly IList<EyesBaseConfig> _eyeses;

        protected WebSocketClient SocketClient { get; private set; }

        public MakeManagerRequestPayload Config
        {
            get => config_ ?? (config_ = InitConfig());
            set => config_ = value;
        }

        public bool? DontCloseBatches { get; set; }

        public string ApiKey { get; set; }

        public string ServerUrl { get; set; }

        public ProxySettings Proxy { get; set; }

        public bool IsDisabled { get; set; }

        public bool GetAllTestResultsAlreadyCalled { get; } = false;

        public Logger Logger { get; } = new Logger();

        protected EyesRunner(ILogHandler logHandler, bool startServer = true)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            _eyeses = new List<EyesBaseConfig>();
            if (startServer)
            {
                StartServer();
            }

            _actualAssembly = GetActualAssembly();
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest data) 
            where TRequest : CommandBase 
            where TResponse : CommandBase
        {
            var result = SocketClient.SendRequest<TRequest, TResponse>(data);
            return result;
        }

        public void SendRequest<TRequest>(TRequest data)
            where TRequest : CommandBase
        {
            SocketClient.SendData(data);
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
            SocketClient?.SetLogHandler(logHandler);
            if (!logHandler.IsOpen)
            {
                logHandler.Open();
            }
        }

        public void AddEyes(EyesBaseConfig eyesBase)
        {
            _eyeses.Add(eyesBase);
        }

        public string GetCoreMakeManager()
        {
            Logger.Log("CoreMakeManager");

            if (managerApplitoolsRefId_ == null)
            {
                CreateCoreSdk();

                var coreMakeManager = CreateCoreMakeManager();

                var coreMakeManagerResponse = SendRequest<CoreMakeManagerRequest, CoreMakeManagerResponse>(coreMakeManager);
                managerApplitoolsRefId_ = coreMakeManagerResponse.Payload.Result.ApplitoolsRefId;
            }

            return managerApplitoolsRefId_;
        }

        internal void CreateCoreSdk()
        {
            var makeSdk = CreateCoreMakeSdk();
            var makeSdkJson = makeSdk.ToJson();

            SocketClient.SendData(makeSdkJson);
        }

        private IList<CloseAllResult> CloseAllEyes(bool shouldThrowException)
        {
            Logger.Log("Closing All Eyes");

            var results = new List<CloseAllResult>();

            if (IsDisabled)
            {
                return results;
            }

            var request = CreateEyesManageCloseAllEyes(shouldThrowException);

            var response = SendRequest<EyesManagerCloseManagerRequest, EyesManagerCloseManagerResponse>(request);
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
                    if (closeAllTestResults.Error != null && shouldThrowException)
                    {
                        throw new EyesException(closeAllTestResults.Error.ToString());
                    }
                    
                    var testResults = closeAllTestResults.Result;
                    if (testResults != null)
                    {
                        testResults.Runner = this;
                    }
                }

                results.AddRange(payload.Result.Results);
            }

            return results;
        }

        protected abstract MakeManagerRequestPayload InitConfig();

        protected virtual IEnumerable<EyesBaseConfig> GetAllEyes()
        {
            return _eyeses;
        }

        private Assembly GetActualAssembly()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            foreach (StackFrame stackFrame in stackFrames)
            {
                Type callingType = stackFrame.GetMethod().DeclaringType;
                if (callingType.IsAbstract) continue;
                if (callingType.Assembly.GetCustomAttribute<PrivateAssemblyAttribute>() != null) continue;
                return callingType.Assembly;
            }

            return Assembly.GetExecutingAssembly();
        }

        private EyesManagerCloseManagerRequest CreateEyesManageCloseAllEyes(bool shouldThrowException)
        {
            var closeAllEyes = new EyesManagerCloseManagerRequest
            {
                Payload = new CloseManagerRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = managerApplitoolsRefId_
                    },
                    Settings = new CloseSettings
                    {
                        ThrowErr = shouldThrowException
                    }
                }
            };

            return closeAllEyes;
        }

        private void StartServer()
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            SocketClient = new WebSocketClient(Logger?.GetILogHandler());

            var url = GetSocketUrl(serverPort);
            Logger.Log($"Server url: {url}");

            SocketClient.ConnectAsync(url);

            Logger.Log("Connected to Server");
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }

        private CoreMakeSdkRequest CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdkRequest();
            result.AddOrUpdatePayload("name", "netUniversal3");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", _actualAssembly.GetName().Version.ToString());
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
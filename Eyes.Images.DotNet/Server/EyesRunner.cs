using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Universal;
using Applitools.Utils;

namespace Applitools
{
    public abstract class EyesRunner
    {
        private MakeManagerRequestPayload config_;
        protected readonly Assembly ActualAssembly;
        private readonly IList<EyesBaseConfig> eyesConfigs_;

        public CommandExecutor CommandExecutor { get; private set; }

        public MakeManagerRequestPayload Config
        {
            get => config_ ?? (config_ = InitConfig());
            set => config_ = value;
        }

        public string ManagerApplitoolsRefId { get; protected set; }

        public bool? DontCloseBatches { get; set; }

        public string ApiKey { get; set; }

        public string ServerUrl { get; set; }

        public ProxySettings Proxy { get; set; }

        public bool IsDisabled { get; set; }

        public bool GetAllTestResultsAlreadyCalled { get; } = false;

        public Logger Logger { get; } = new Logger();

        public bool? RemoveDuplicateTests { get; private set; }

        protected EyesRunner(ILogHandler logHandler, string agentId, ISpecDriverMessageListener listener)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
                listener.Logger.SetLogHandler(logHandler);
            }

            eyesConfigs_ = new List<EyesBaseConfig>();
            ActualAssembly = GetActualAssembly();
            // ReSharper disable once VirtualMemberCallInConstructor
            CommandExecutor = CommandExecutor.GetInstance(CreateCoreMakeSdk(agentId), listener, logHandler);
        }

        public void SetRemoveDuplicateTests(bool? removeDuplicateTests)
        {
            RemoveDuplicateTests = removeDuplicateTests;
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest data) 
            where TRequest : CommandBase 
            where TResponse : CommandBase
        {
            var result = CommandExecutor.SendRequest<TRequest, TResponse>(data);
            return result;
        }

        public void SendRequest<TRequest>(TRequest data)
            where TRequest : CommandBase
        {
            CommandExecutor.SendData(data);
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
            CommandExecutor?.SetLogHandler(logHandler);
            if (!logHandler.IsOpen)
            {
                logHandler.Open();
            }
        }

        public void AddEyes(EyesBaseConfig eyesBase)
        {
            eyesConfigs_.Add(eyesBase);
        }

        protected string GetCoreMakeManager()
        {
            Logger.Log("CoreMakeManager");

            var coreMakeManager = CreateCoreMakeManager();

            var coreMakeManagerResponse = SendRequest<CoreMakeManagerRequest, CoreMakeManagerResponse>(coreMakeManager);
            var managerApplitoolsRefId = coreMakeManagerResponse.Payload.Result.ApplitoolsRefId;

            return managerApplitoolsRefId;
        }

        internal string CreateCoreSdk(string agentId)
        {
            var makeSdk = CreateCoreMakeSdk(agentId);
            var makeSdkJson = makeSdk.ToJson();

            return makeSdkJson;
        }

        private IList<CloseAllResult> CloseAllEyes(bool shouldThrowException)
        {
            Logger.Log("Closing All Eyes");

            var results = new List<CloseAllResult>();

            if (IsDisabled)
            {
                return results;
            }

            var request = CreateEyesManageCloseAllEyes(shouldThrowException, RemoveDuplicateTests);

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
            return eyesConfigs_;
        }

        private static Assembly GetActualAssembly()
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

        private EyesManagerCloseManagerRequest CreateEyesManageCloseAllEyes(bool shouldThrowException, bool? removeDuplicateTests)
        {
            var closeAllEyes = new EyesManagerCloseManagerRequest
            {
                Payload = new CloseManagerRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = ManagerApplitoolsRefId
                    },
                    Settings = new CloseSettings
                    {
                        ThrowErr = shouldThrowException,
                        RemoveDuplicateTests = removeDuplicateTests
                    }
                }
            };

            return closeAllEyes;
        }

        protected virtual CoreMakeSdkRequest CreateCoreMakeSdk(string name)
        {
            var result = new CoreMakeSdkRequest{
                Payload = new MakeCorePayload
                {
                    AgentId = $"{name}:{ActualAssembly.GetName().Version}",
                    Cwd = Environment.CurrentDirectory,
                    Protocol = "webdriver"
                }
            };

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
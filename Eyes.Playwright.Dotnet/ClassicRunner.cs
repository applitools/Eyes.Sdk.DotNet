using System;
using Applitools.Playwright.Universal;
using Applitools.Commands.Requests;
using Applitools.Playwright.Universal.Driver;
using Applitools.Universal;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright
{
    public class ClassicRunner : EyesRunner
    {
        /// <summary>
        /// name of the client sdk
        /// </summary>
        protected static string BASE_AGENT_ID = "Eyes.Playwright.DotNet";

        /// <summary>
        /// version of the client sdk
        /// </summary>
        protected static string VERSION = typeof(ClassicRunner).Assembly.GetName().Version.ToString();

        /// <summary>
        /// spec-driver
        /// </summary>
        protected static string[] COMMANDS = SpecDriverPlaywright.GetMethodNames();

        private readonly Refer refer_;
        private readonly SpecDriverPlaywright driver_;
        
        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        public ClassicRunner() : this(BASE_AGENT_ID, VERSION)
        {
        }

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        private ClassicRunner(string baseAgentId, string version)
            : this(NullLogHandler.Instance, baseAgentId, version)
        {
        }

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        private ClassicRunner(ILogHandler logHandler, string baseAgentId, string version)
            : base(logHandler, baseAgentId + "-" + version)
        {
            refer_ = new Refer();
            driver_ = new SpecDriverPlaywright(refer_);
        }

        public PlaywrightStaleElementReferenceException GetStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }

        protected override MakeManagerRequestPayload InitConfig()
        {
            throw new NotImplementedException();
        }

        protected override CoreMakeSdkRequest CreateCoreMakeSdk(string name)
        {
            var result = new CoreMakeSdkRequest();
            result.AddOrUpdatePayload("agentId", $"{name}:{ActualAssembly.GetName().Version}");
            result.AddOrUpdatePayload("spec", "playwright");
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }


        private static PlaywrightSpecDriverMessageAdaptor messageAdaptor_ = null;

        protected override ISpecDriverMessageAdaptor SpecDriverMessageAdaptor
        {
            get
            {
                if (messageAdaptor_ == null)
                {
                    messageAdaptor_ = new PlaywrightSpecDriverMessageAdaptor(refer_, Logger);
                }

                return messageAdaptor_;
            }
        }

        protected Refer GtRefer()
        {
            return refer_;
        }
    }
}
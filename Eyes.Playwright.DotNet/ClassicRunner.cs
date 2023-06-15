using System;
using Applitools.Playwright.Universal;
using Applitools.Commands.Requests;
using Applitools.Playwright.Universal.Driver;
using Applitools.Universal;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright
{
    public class ClassicRunner : PlaywrightEyesRunner
    {
        /// <summary>
        /// name of the client sdk
        /// </summary>
        private static readonly string BASE_AGENT_ID = "Eyes.Playwright.DotNet";

        private readonly SpecDriverPlaywright driver_;
        
        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        public ClassicRunner() : this(BASE_AGENT_ID)
        {
        }
  
        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        public ClassicRunner(ILogHandler logHandler) : this(logHandler, BASE_AGENT_ID)
        {
        }

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        private ClassicRunner(string baseAgentId)
            : this(NullLogHandler.Instance, baseAgentId)
        {
        }

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        private ClassicRunner(ILogHandler logHandler, string baseAgentId)
            : base(logHandler, baseAgentId)
        {
            ManagerApplitoolsRefId = GetCoreMakeManager();
            Refer = PlaywrightSpecDriverMessageListener.Instance.Value.Refer;
            driver_ = new SpecDriverPlaywright(Refer);
        }

        public PlaywrightStaleElementReferenceException GetStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }

        protected override MakeManagerRequestPayload InitConfig()
        {
            return new MakeManagerRequestPayload
            {
                Type = "classic"
            };
        }
    }
}
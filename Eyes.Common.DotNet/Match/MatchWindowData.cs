using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Encapsulates the data to be sent to the agent on a "matchWindow" command.
    /// </summary>
    public class MatchWindowData
    {
        /// <summary>
        /// Creates a new <see cref="MatchWindowData"/> instance.
        /// </summary>
        /// <param name="appOutput">The appOutput for the current matchWindow call.</param>
        /// <param name="tag">The step name to use.</param>
        /// <param name="agentSetup">The test setup parameters used.</param>
        public MatchWindowData(AppOutput appOutput, string tag, object agentSetup = null)
        {
            ArgumentGuard.NotNull(appOutput, nameof(appOutput));

            AppOutput = appOutput;
            Tag = tag;
            AgentSetup = agentSetup;
        }

        public string Tag { get; private set; }

        public AppOutput AppOutput { get; private set; }

        public bool IgnoreMismatch { get; set; }

        public ImageMatchOptions Options { get; set; }

        public object AgentSetup { get; set; }
        public string RenderId { get; set; }

    }
}

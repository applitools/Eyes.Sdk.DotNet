namespace Applitools
{
    /// <summary>
    /// The result of a window match by the agent.
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        /// The result is the expected one.
        /// </summary>
        public bool AsExpected { get; set; }

        /// <summary>
        /// The matched window id.
        /// </summary>
        public string WindowId { get; set; }
    }
}

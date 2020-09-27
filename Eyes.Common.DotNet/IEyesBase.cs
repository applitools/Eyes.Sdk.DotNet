using System.Net;

namespace Applitools
{
    public interface IEyesBase
    {
        BatchInfo Batch { get; set; }

        TestResults Abort();

        TestResults AbortIfNotClosed();
        
        void SetLogHandler(ILogHandler logHandler);

        string ServerUrl { get; set; }

        WebProxy Proxy { get; set; }

        Logger Logger { get; }

        void AbortAsync();

        bool IsOpen { get; }

        bool IsDisabled { get; set; }

        string ApiKey { get; set; }

        void AddProperty(string name, string value);

        void ClearProperties();

        string AgentId { get; set; }

        string FullAgentId { get; }
    }
}

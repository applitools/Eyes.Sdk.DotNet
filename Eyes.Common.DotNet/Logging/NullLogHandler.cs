using System;

namespace Applitools
{
    public class NullLogHandler : ILogHandler
    {
        public static NullLogHandler Instance = new NullLogHandler();

        public bool IsOpen => true;

        public void OnMessage(bool verbose, string message, params object[] args)
        {
        }
        public void OnMessage(bool verbose, Func<string> messageProvider)
        {
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}

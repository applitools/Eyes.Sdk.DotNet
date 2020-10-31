using System;

namespace Applitools
{
    public abstract class LogHandlerBase : ILogHandler
    {
        private readonly bool isVerbose_;
        protected bool isOpen_;

        protected LogHandlerBase(bool isVerbose)
        {
            isVerbose_ = isVerbose;
        }

        public virtual void Open() { }

        public virtual void OnMessage(TraceLevel level, string message, params object[] args)
        {
            if (level >= TraceLevel.Notice || isVerbose_)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                OnMessage(message, level);
            }
        }

        public virtual void OnMessage(TraceLevel level, Func<string> messageProvider)
        {
            if (level > TraceLevel.Notice || isVerbose_)
            {
                OnMessage(messageProvider(), level);
            }
        }

        public abstract void OnMessage(string message, TraceLevel level = default);

        public virtual void Close() { }

        public bool IsOpen => isOpen_;
    }

}

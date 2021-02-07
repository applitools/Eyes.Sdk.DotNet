using Newtonsoft.Json;
using System;

namespace Applitools
{
    public abstract class LogHandlerBase : ILogHandler
    {
        protected bool isOpen_;
        private readonly TraceLevel minLevel_;

        protected LogHandlerBase(bool isVerbose)
        {
            minLevel_ = isVerbose ? TraceLevel.Debug : TraceLevel.Notice;
        }

        public virtual void Open() { }

        public virtual void OnMessage(TraceLevel level, string message, params object[] args)
        {
            if (level >= minLevel_)
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
            if (level >= minLevel_)
            {
                OnMessage(messageProvider(), level);
            }
        }

        public abstract void OnMessage(string message, TraceLevel level = default);


        public virtual void OnMessage(ClientEvent @event)
        {
            if (@event.Level >= minLevel_)
            {
                OnMessage(JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }), @event.Level);
            }
        }

        public virtual void Close() { }

        public bool IsOpen => isOpen_;
    }

}

using System;
using System.Collections.Concurrent;
using Applitools.Commands;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools.Universal
{
    public class SpecDriverMessageListener : ISpecDriverMessageListener
    {
        private ConcurrentDictionary<string, WsMessage> messages_;
        protected static JsonSerializer Serializer { get; } = JsonUtils.CreateSerializer();

        internal static Lazy<SpecDriverMessageListener> Instance { get; } 
            = new Lazy<SpecDriverMessageListener>(() => new SpecDriverMessageListener());
        public Logger Logger { get; }

        public virtual string Protocol => "webdriver";

        protected SpecDriverMessageListener()
        {
            Logger = new Logger();
        }
        
        protected SpecDriverMessageListener(Logger logger)
        {
            Logger = logger;
        }

        public virtual void HandleMessage(string message)
        {
            HandleResponse_(message);
        }

        public WebSocketClient WebSocket { get; set; }

        public void SetMessagesDictionary(ConcurrentDictionary<string, WsMessage> messages)
        {
            messages_ = messages;
        }

        protected void HandleResponse_(string message)
        {
            try
            {
                var response = Serializer.Deserialize<CommandResponse>(message);
                if (messages_.TryGetValue(response.Key, out WsMessage wsMessage))
                {
                    wsMessage.IsHandled = true;
                }
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.SpecDriver, e);
            }
        }
    }
}
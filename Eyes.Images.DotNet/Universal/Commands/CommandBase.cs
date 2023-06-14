using System;

namespace Applitools.Commands
{
    public abstract class CommandBase
    {
        public string Name { get; set; }

        public string Key { get; set; }


        protected CommandBase()
        {
            Key = Guid.NewGuid().ToString();
        }
    }

    public class CommandResponse : CommandBase
    {
        public object Payload { get; set; }
    }
}
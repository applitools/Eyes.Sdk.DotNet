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
}
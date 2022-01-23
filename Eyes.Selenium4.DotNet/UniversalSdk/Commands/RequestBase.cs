using System;

namespace Applitools.Selenium.Commands
{
    public abstract class RequestBase
    {
        public abstract string Name { get; }

        public string Key { get; set; }


        protected RequestBase()
        {
            Key = Guid.NewGuid().ToString();
        }
    }
}
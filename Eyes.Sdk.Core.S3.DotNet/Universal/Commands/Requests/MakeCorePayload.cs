using System.Collections.Generic;

namespace Applitools.Commands.Requests
{
    public class MakeCorePayload
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Cwd { get; set; }
        public ICollection<string> Commands { get; set; }

        public MakeCorePayload()
        {
            Commands = new List<string>();
        }
    }
}

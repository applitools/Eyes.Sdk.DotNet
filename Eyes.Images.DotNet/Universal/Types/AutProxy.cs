using System.Collections.Generic;

namespace Applitools
{
    public class AutProxy
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<string> Domains { get; set; }
        public string Mode { get; set; }

        public AutProxy()
        {
            Domains = new List<string>();
        }
    }
}
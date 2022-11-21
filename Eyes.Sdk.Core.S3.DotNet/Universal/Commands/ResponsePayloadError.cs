using System;

namespace Applitools.Commands
{
    public class ResponsePayloadError
    {
        public string Message { get; set; }
        public string Stack { get; set; }

        public override string ToString()
        {
            return $"{Message}{Environment.NewLine}{Stack}";
        }
    }
}
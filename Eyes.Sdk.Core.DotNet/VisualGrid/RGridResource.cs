using Applitools.Utils;
using Newtonsoft.Json;
using System;

namespace Applitools.VisualGrid
{
    public class RGridResource
    {
        private static readonly int MAX_RESOURCE_SIZE = 15 * 1024 * 1024;

        public RGridResource(Uri url, string contentType, byte[] content, Logger logger, string msg, int? errorStatusCode = null)
        {
            Url = url;

            if (content != null)
            {
                if (content.Length > MAX_RESOURCE_SIZE)
                {
                    Array.Copy(content, Content, MAX_RESOURCE_SIZE);
                }
                else
                {
                    Content = content;
                }
                Sha256 = CommonUtils.GetSha256Hash(content).ToLower();
            }

            ContentType = contentType;
            ErrorStatusCode = errorStatusCode;
        }

        [JsonIgnore]
        public Uri Url { get; }

        [JsonProperty("contentType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ContentType { get; }

        [JsonIgnore]
        public byte[] Content { get; }

        [JsonProperty("hash", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Sha256 { get; }

        [JsonProperty("hashFormat")]
        public string HashFormat { get => "sha256"; }

        [JsonProperty("errorStatusCode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ErrorStatusCode { get; }

        public override string ToString()
        {
            return $"{nameof(RGridResource)} - {Sha256} ({ContentType}) [{Url}]";
        }
    }
}
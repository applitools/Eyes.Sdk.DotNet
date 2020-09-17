using Applitools.VisualGrid.Model;
using Applitools.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applitools.VisualGrid
{
    public class RGridDom
    {
        public static readonly string ContentType = "x-applitools-html/cdt";
        private string sha256_;

        public RGridDom()
        {
        }

        public RGridDom(List<CdtData> domNodes, IDictionary<string, RGridResource> resources, Uri url, Logger logger, string msg)
        {
            DomNodes = domNodes;
            Resources = resources;
            Url = url;
            Logger = logger;
            Msg = msg;
        }

        [JsonIgnore]
        public List<CdtData> DomNodes { get; set; }

        [JsonIgnore]
        public IDictionary<string, RGridResource> Resources { get; set; }
        [JsonIgnore]
        public Uri Url { get; set; }
        [JsonIgnore]
        public Logger Logger { get; set; }
        [JsonIgnore]
        public string Msg { get; set; }

        public string HashFormat { get { return "sha256"; } }

        [JsonProperty("hash")]
        public string Sha256
        {
            get
            {
                if (sha256_ == null)
                {
                    sha256_ = CommonUtils.GetSha256Hash(Encoding.UTF8.GetBytes(GetStringObjectMap_())).ToLower();
                }
                return sha256_;
            }
        }

        private string GetStringObjectMap_()
        {
            Dictionary<string, object> map = new Dictionary<string, object> {
                { "domNodes", DomNodes },
                { "resources", Resources }
            };
            JsonSerializer serializer = JsonUtils.CreateSerializer();
            return serializer.Serialize(map);
        }

        public RGridResource AsResource()
        {
            Logger.Verbose("enter");
            string stringObjectMap = GetStringObjectMap_();
            Logger.Verbose("creating RGridResource");
            RGridResource gridResource = new RGridResource(Url, ContentType, Encoding.UTF8.GetBytes(stringObjectMap), Logger, "RGridDom - " + Msg);
            Logger.Verbose("exit");
            return gridResource;
        }
    }
}
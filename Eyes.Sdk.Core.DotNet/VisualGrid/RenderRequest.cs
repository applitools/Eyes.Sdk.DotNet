using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Applitools.VisualGrid
{
    public class RenderRequest
    {
        public RenderRequest(Uri webHook, Uri url, Uri stitchingService, RGridDom dom,
            IDictionary<string, RGridResource> resources, RenderInfo renderInfo,
            string platform, BrowserType browserName, object scriptHooks,
            VisualGridSelector[] selectorsToFindRegionsFor, bool sendDom, VisualGridTask task,
            VisualGridOption[] visualGridOptions)
        {
            Webhook = webHook;
            Url = url;
            StitchingService = stitchingService;
            Dom = dom;
            Resources = resources;
            RenderInfo = renderInfo;
            PlatformName = platform;
            BrowserName = browserName;
            ScriptHooks = scriptHooks;
            SelectorsToFindRegionsFor = selectorsToFindRegionsFor;
            SendDom = sendDom;
            Task = task;
            if (visualGridOptions != null)
            {
                Options = new Dictionary<string, object>();
                foreach (VisualGridOption option in visualGridOptions)
                {
                    Options[option.Key] = option.Value;
                }
            }
        }

        public Uri Webhook { get; private set; }
        public Uri Url { get; set; }
        public Uri StitchingService { get; }
        public RGridDom Dom { get; private set; }
        public IDictionary<string, RGridResource> Resources { get; private set; }
        public Dictionary<string, object> Options { get; private set; }

        public RenderInfo RenderInfo { get; set; }

        [JsonIgnore]
        public string PlatformName { get; set; }

        [JsonIgnore]
        public BrowserType BrowserName { get; set; }

        public object ScriptHooks { get; set; }
        public VisualGridSelector[] SelectorsToFindRegionsFor { get; set; }
        public bool SendDom { get; set; }

        public string AgentId { get; set; }

        [JsonIgnore]
        public VisualGridTask Task { get; set; }

        public string RenderId { get; set; }

        [JsonProperty("browser", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Browser
        {
            get
            {
                Dictionary<string, object> map = new Dictionary<string, object>()
                {
                    { "name", BrowserName }
                };
                return map;
            }
        }

        [JsonProperty("platform", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Platform
        {
            get
            {
                Dictionary<string, object> map = new Dictionary<string, object>()
                {
                    { "name", PlatformName }
                };
                return map;
            }
        }

        public override string ToString()
        {
            return $"{RenderId} ({RenderInfo.Width}x{RenderInfo.Height})";
        }
    }
}
using System;
using System.Collections.Generic;

namespace Applitools.VisualGrid
{
    public class RunningRender
    {
        public RunningRender()
        {
        }

        public RunningRender(string renderId, string jobId, RenderStatus renderStatus, List<Uri> needMoreResources, bool needMoreDom)
        {
            RenderId = renderId;
            JobId = jobId;
            RenderStatus = renderStatus;
            NeedMoreResources = needMoreResources;
            NeedMoreDom = needMoreDom;
        }

        public string RenderId { get; set; }

        public string JobId { get; set; }

        public RenderStatus RenderStatus { get; set; }

        public List<Uri> NeedMoreResources { get; set; }

        public bool NeedMoreDom { get; set; }

        public override string ToString()
        {
            return $"RunningRender{{ renderId='{RenderId}', jobId='{JobId}', renderStatus={RenderStatus}, needMoreResources={NeedMoreResources}, needMoreDom={NeedMoreDom} }}";
        }
    }
}
using Applitools.Fluent;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class ResourceCollectionTask
    {

        private List<RenderRequest> BuildRenderRequests_(FrameData currentFrame, IDictionary<string, RGridResource> resourceMapping)
        {
            Uri url = currentFrame.Url;
            RGridDom dom = new RGridDom(currentFrame.Cdt, resourceMapping, url, logger_, "buildRenderRequests");

            //Create RG requests
            List<RenderRequest> allRequestsForRG = new List<RenderRequest>();
            ICheckSettingsInternal csInternal = (ICheckSettingsInternal)settings_;

            foreach (VisualGridTask task in checkTasks_)
            {
                RenderBrowserInfo browserInfo = task.BrowserInfo;
                RenderInfo renderInfo = new RenderInfo(browserInfo.Width, browserInfo.Height,
                    csInternal.GetSizeMode(), csInternal.GetTargetSelector(),
                    csInternal.GetTargetRegion(), browserInfo.EmulationInfo, browserInfo.IosDeviceInfo);

                List<VisualGridSelector> regionSelectors = new List<VisualGridSelector>();
                if (regionSelectors_ != null)
                {
                    foreach (VisualGridSelector[] vgs in regionSelectors_)
                    {
                        regionSelectors.AddRange(vgs);
                    }
                }

                RenderRequest request = new RenderRequest(renderingInfo_.ResultsUrl, url,
                        renderingInfo_.StitchingServiceUrl, dom,
                        resourceMapping, renderInfo, browserInfo.Platform,
                        browserInfo.BrowserType, csInternal.GetScriptHooks(),
                        regionSelectors.ToArray(), csInternal.GetSendDom() ?? false, task,
                        csInternal.GetVisualGridOptions());

                allRequestsForRG.Add(request);
            }
            return allRequestsForRG;
        }
    }
}

using Applitools.Utils.Geometry;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public interface IEyesConnector : IEyesBase
    {
        void Open(IConfiguration configuration);
        TestResults Close(bool throwEx, IConfiguration config);

        List<RunningRender> Render(RenderRequest[] requests);
        Task<List<RunningRender>> RenderAsync(RenderRequest[] requests);

        List<RenderStatusResults> RenderStatusById(string[] renderIds);

        MatchResult MatchWindow(IConfiguration config, string resultImageURL, string domLocation, ICheckSettings checkSettings,
            IList<IRegion> regions, IList<VisualGridSelector[]> regionSelectors, Location location,
            RenderStatusResults results, string source);

        void SetUserAgent(string userAgent);
        PutFuture RenderPutResource(RunningRender runningRender, RGridResource rGridResource);
        ResourceFuture CreateResourceFuture(RGridResource blob);
        ResourceFuture GetResource(Uri url);
        void SetDeviceSize(RectangleSize deviceSize);
        RenderingInfo GetRenderingInfo();
        void SetRenderInfo(RenderingInfo renderingInfo);
        Dictionary<IosDeviceName, DeviceSize> GetIosDevicesSizes();
        Dictionary<DeviceName, DeviceSize> GetEmulatedDevicesSizes();
        Dictionary<BrowserType, string> GetUserAgents();
    }
}
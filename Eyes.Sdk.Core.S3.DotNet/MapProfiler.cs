using System.Linq;
using Applitools.Metadata;
using AutoMapper;

namespace Applitools
{
    public class MapProfiler : Profile
    {
        public MapProfiler()
        {
            CreateMap<Configuration, OpenConfig>()
                .ForMember(dest => dest.Environment, act => act.MapFrom(
                    src => new BaselineEnv
                                {
                                    Os = src.HostOS,
                                    HostingApp = src.HostApp,
                                    ViewportSize = src.ViewportSize
                                }));
            CreateMap<Configuration, ScreenshotConfig>();
            CreateMap<Configuration, CheckConfig>()
                .ForMember(dest => dest.Renderers, act => act.MapFrom(src => src.GetBrowsersInfo()))
                .ForMember(dest => dest.RetryTimeout, act => act.MapFrom(src => src.MatchTimeout))
                .ForMember(dest => dest.UfgOptions, act => act.MapFrom(src => 
                    src.VisualGridOptions.ToDictionary(k => k.Key, v => v.Value)));
            CreateMap<Configuration, CloseConfig>()
                .ForMember(dest => dest.UpdateBaselineIfNew, act => act.MapFrom(src => src.SaveNewTests))
                .ForMember(dest => dest.UpdateBaselineIfDifferent, act => act.MapFrom(src => src.SaveFailedTests));

            CreateMap<Configuration, OpenSettings>()
                .ForMember(dest => dest.Environment, act => act.MapFrom(
                    src => new BaselineEnv
                    {
                        Os = src.HostOS,
                        HostingApp = src.HostApp,
                        ViewportSize = src.ViewportSize
                    }));
            CreateMap<Configuration, LocateSettings>();
            CreateMap<Configuration, CloseSettings>();
        }
    }
}

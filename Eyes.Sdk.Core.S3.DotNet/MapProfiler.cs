using AutoMapper;

namespace Applitools
{
    public class MapProfiler : Profile
    {
        public MapProfiler()
        {
            CreateMap<Configuration, OpenConfig>();
            CreateMap<Configuration, CheckConfig>()
                .ForMember(dest => dest.Renderers, act => act.MapFrom(src => src.GetBrowsersInfo()))
                .ForMember(dest => dest.RetryTimeout, act => act.MapFrom(src => src.MatchTimeout))
                .ForMember(dest => dest.UfgOptions, act => act.MapFrom(src => src.VisualGridOptions));
            CreateMap<Configuration, CloseConfig>()
                .ForMember(dest => dest.UpdateBaselineIfNew, act => act.MapFrom(src => src.SaveNewTests))
                .ForMember(dest => dest.UpdateBaselineIfDifferent, act => act.MapFrom(src => src.SaveFailedTests));

            CreateMap<Configuration, OpenSettings>();
            CreateMap<Configuration, CloseSettings>();
        }
    }
}

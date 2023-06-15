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
                .ForMember(dest => dest.Environment, act =>
                {
                    act.Condition(src => string.IsNullOrEmpty(src.HostOS) == false || string.IsNullOrEmpty(src.HostApp) == false || src.ViewportSize != null);
                    act.MapFrom(
                        src => new BaselineEnv
                        {
                            Os = src.HostOS,
                            HostingApp = src.HostApp,
                            ViewportSize = src.ViewportSize
                        });
                });
            CreateMap<Configuration, ScreenshotConfig>()
                .ForMember(dest => dest.Fully, act => act.MapFrom(src => src.IsForceFullPageScreenshot));
            CreateMap<Configuration, CheckConfig>()
                .ForMember(dest => dest.Renderers, act => act.MapFrom(src => src.GetBrowsersInfo()))
                .ForMember(dest => dest.RetryTimeout, act => act.MapFrom(src => src.MatchTimeout))
                .ForMember(dest => dest.UfgOptions, act =>
                {
                    act.PreCondition(src => src.VisualGridOptions != null);
                    act.MapFrom(src => src.VisualGridOptions.ToDictionary(k => k.Key, v => v.Value));
                })
                .ForMember(dest => dest.AccessibilitySettings, act =>
                {
                    act.PreCondition(src => src.AccessibilityValidation != null);
                    act.MapFrom(src => new AccessibilitySettings(src.AccessibilityValidation.Level, src.AccessibilityValidation.GuidelinesVersion));
                });
            CreateMap<Configuration, CloseConfig>()
                .ForMember(dest => dest.UpdateBaselineIfNew, act => act.MapFrom(src => src.SaveNewTests))
                .ForMember(dest => dest.UpdateBaselineIfDifferent, act => act.MapFrom(src => src.SaveFailedTests));

            CreateMap<Configuration, ResultsSettings>();

            CreateMap<Configuration, OpenSettings>()
                .ForMember(dest => dest.KeepBatchOpen, act => act.MapFrom(src => src.DontCloseBatches))
                .ForMember(dest => dest.Environment, act => act.MapFrom(
                    src => new BaselineEnv
                    {
                        Os = src.HostOS,
                        HostingApp = src.HostApp,
                        ViewportSize = src.ViewportSize
                    }));
            CreateMap<Configuration, UniversalCheckSettings>()
                .ForMember(dest => dest.AccessibilitySettings, act =>
                    {
                        act.PreCondition(src => src.AccessibilityValidation != null);
                        act.MapFrom(src => new AccessibilitySettings(src.AccessibilityValidation.Level, src.AccessibilityValidation.GuidelinesVersion));
                    });
            CreateMap<Configuration, LocateSettings>();
            CreateMap<Configuration, EGClientSettings>();
            CreateMap<Configuration, CloseSettings>()
                .ForMember(dest => dest.UpdateBaselineIfNew, act => act.MapFrom(src => src.SaveNewTests))
                .ForMember(dest => dest.UpdateBaselineIfDifferent, act => act.MapFrom(src => src.SaveFailedTests));
        }
    }
}

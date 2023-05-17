using Applitools.Playwright.Fluent;
using Applitools.Universal;
using Applitools.Universal.Dto;

namespace Applitools.Playwright.Universal.Mapper
{
    public class PlaywrightCheckSettingsMapper
    {
        public static CheckSettingsDto ToCheckSettingsDto(ICheckSettings checkSettings, Configuration config,
            Refer refer, Reference root)
        {
            if (!(checkSettings is PlaywrightCheckSettings playwrightCheckSettings))
            {
                return null;
            }

            CheckSettingsDto checkSettingsDto = new CheckSettingsDto();
            // CheckSettings
            // checkSettingsDto.Region = TRegionMapper.toTRegionFromCheckSettings(playwrightCheckSettings, refer, root);
            // checkSettingsDto.Frames = TFramesMapper.toTFramesFromCheckSettings(
            //     ((IPlaywrightCheckTarget)playwrightCheckSettings).GetFrameChain(), refer, root);
            // checkSettingsDto.Fully = playwrightCheckSettings.StitchContent;
            // checkSettingsDto.ScrollRootElement =
            //     TRegionMapper.toTRegionDtoFromSRE(
            //         ((IPlaywrightCheckTarget)playwrightCheckSettings).GetScrollRootElement(), refer, root);
            checkSettingsDto.StitchMode = config.StitchMode?.ToString();
            checkSettingsDto.HideScrollbars = config.HideScrollbars ?? false;
            checkSettingsDto.HideCaret = config.HideCaret ?? false;
            // checkSettingsDto.Overlap = config.Overlap;
            // checkSettingsDto.WaitBeforeCapture = playwrightCheckSettings.GetWaitBeforeCapture();
            // checkSettingsDto.LazyLoad = playwrightCheckSettings.GetLazyLoadOptions();
            // checkSettingsDto.IgnoreDisplacements = playwrightCheckSettings.isIgnoreDisplacements();
            // checkSettingsDto.Normalization = toNormalizationDto(
            //     toImageCropRect(config.getCutProvider(), config.getContentInset()),
            //     config.getRotation(), config.getScaleRatio());
            // checkSettingsDto.setDebugImages(new DebugScreenshotHandlerDto(config.getSaveDebugScreenshots(),
            //     config.getDebugScreenshotsPath(), config.getDebugScreenshotsPrefix()));
            // checkSettingsDto.setName(playwrightCheckSettings.getName());
            // checkSettingsDto.setPageId(playwrightCheckSettings.getPageId());
            // checkSettingsDto.setIgnoreRegions(CodedRegionReferenceMapper
            //     .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getIgnoreRegions()),
            //         refer, root));
            // checkSettingsDto.setLayoutRegions(CodedRegionReferenceMapper
            //     .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getLayoutRegions()),
            //         refer, root));
            // checkSettingsDto.setStrictRegions(CodedRegionReferenceMapper
            //     .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getStrictRegions()),
            //         refer, root));
            // checkSettingsDto.setContentRegions(CodedRegionReferenceMapper
            //     .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getContentRegions()),
            //         refer, root));
            // checkSettingsDto.setFloatingRegions(
            //     TFloatingRegionMapper.toTFloatingRegionDtoList(
            //         Arrays.asList(playwrightCheckSettings.getFloatingRegions()), refer, root));
            // checkSettingsDto.setAccessibilityRegions(
            //     TAccessibilityRegionMapper.toTAccessibilityRegionDtoList(
            //         Arrays.asList(playwrightCheckSettings.getAccessibilityRegions()), refer, root));
            // checkSettingsDto.setAccessibilitySettings(
            //     AccessibilitySettingsMapper.toAccessibilitySettingsDto(
            //         ((PlaywrightCheckSettings)checkSettings).getAccessibilityValidation()));
            // checkSettingsDto.setMatchLevel(playwrightCheckSettings.getMatchLevel() == null
            //     ? null
            //     : playwrightCheckSettings.getMatchLevel().getName());
            // checkSettingsDto.setRetryTimeout(config.getMatchTimeout());
            // checkSettingsDto.setSendDom(playwrightCheckSettings.isSendDom());
            // checkSettingsDto.setUseDom(playwrightCheckSettings.isUseDom());
            // checkSettingsDto.setEnablePatterns(playwrightCheckSettings.isEnablePatterns());
            // checkSettingsDto.setIgnoreCaret(playwrightCheckSettings.getIgnoreCaret());
            // checkSettingsDto.setUfgOptions(
            //     VisualGridOptionMapper.toVisualGridOptionDtoList(playwrightCheckSettings
            //         .getVisualGridOptions()));
            // checkSettingsDto.setLayoutBreakpoints(playwrightCheckSettings
            //     .getLayoutBreakpoints().isEmpty()
            //     ? playwrightCheckSettings.isDefaultLayoutBreakpointsSet()
            //     : playwrightCheckSettings.getLayoutBreakpoints());
            // checkSettingsDto.setDisableBrowserFetching(playwrightCheckSettings.isDisableBrowserFetching());
            // checkSettingsDto.setAutProxy(ProxyMapper.toAutProxyDto(config.getAutProxy()));
            // checkSettingsDto.setHooks(playwrightCheckSettings.getScriptHooks());
            // checkSettingsDto.setUserCommandId(playwrightCheckSettings.getVariationGroupId());
            // checkSettingsDto.setDensityMetrics(playwrightCheckSettings.getDensityMetrics());

            return checkSettingsDto;
        }
    }
}
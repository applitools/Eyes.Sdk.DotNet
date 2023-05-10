using Applitools.Playwright.Fluent;
using Applitools.Universal;

namespace Applitools.Playwright.Universal.Mapper
{
    public class PlaywrightCheckSettingsMapper
    {
        public static CheckSettings ToCheckSettingsDto(ICheckSettings checkSettings, Configuration config,
            Refer refer, Reference root)
        {
            if (!(checkSettings is PlaywrightCheckSettings playwrightCheckSettings))
            {
                return null;
            }

            CheckSettingsDto checkSettingsDto = new CheckSettingsDto();
            // CheckSettings
            checkSettingsDto.setRegion(
                TRegionMapper.toTRegionFromCheckSettings(playwrightCheckSettings, refer, root));
            checkSettingsDto.setFrames(
                TFramesMapper.toTFramesFromCheckSettings(playwrightCheckSettings.FrameChain, refer,
                    root));
            checkSettingsDto.setFully(playwrightCheckSettings.getStitchContent());
            checkSettingsDto.setScrollRootElement(
                TRegionMapper.toTRegionDtoFromSRE(playwrightCheckSettings.getScrollRootElement(), refer,
                    root));
            checkSettingsDto.setStitchMode(config.getStitchMode() == null
                ? null
                : config.getStitchMode().getName());
            checkSettingsDto.setHideScrollbars(config.getHideScrollbars());
            checkSettingsDto.setHideCaret(config.getHideCaret());
            checkSettingsDto.setOverlap(config.getOverlap());
            checkSettingsDto.setWaitBeforeCapture(playwrightCheckSettings.getWaitBeforeCapture());
            checkSettingsDto.setLazyLoad(playwrightCheckSettings.getLazyLoadOptions());
            checkSettingsDto.setIgnoreDisplacements(playwrightCheckSettings.isIgnoreDisplacements());
            checkSettingsDto.setNormalization(toNormalizationDto(
                toImageCropRect(config.getCutProvider(), config.getContentInset()),
                config.getRotation(), config.getScaleRatio()));
            checkSettingsDto.setDebugImages(new DebugScreenshotHandlerDto(config.getSaveDebugScreenshots(),
                config.getDebugScreenshotsPath(), config.getDebugScreenshotsPrefix()));
            checkSettingsDto.setName(playwrightCheckSettings.getName());
            checkSettingsDto.setPageId(playwrightCheckSettings.getPageId());
            checkSettingsDto.setIgnoreRegions(CodedRegionReferenceMapper
                .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getIgnoreRegions()),
                    refer, root));
            checkSettingsDto.setLayoutRegions(CodedRegionReferenceMapper
                .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getLayoutRegions()),
                    refer, root));
            checkSettingsDto.setStrictRegions(CodedRegionReferenceMapper
                .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getStrictRegions()),
                    refer, root));
            checkSettingsDto.setContentRegions(CodedRegionReferenceMapper
                .toCodedRegionReferenceList(Arrays.asList(playwrightCheckSettings.getContentRegions()),
                    refer, root));
            checkSettingsDto.setFloatingRegions(
                TFloatingRegionMapper.toTFloatingRegionDtoList(
                    Arrays.asList(playwrightCheckSettings.getFloatingRegions()), refer, root));
            checkSettingsDto.setAccessibilityRegions(
                TAccessibilityRegionMapper.toTAccessibilityRegionDtoList(
                    Arrays.asList(playwrightCheckSettings.getAccessibilityRegions()), refer, root));
            checkSettingsDto.setAccessibilitySettings(
                AccessibilitySettingsMapper.toAccessibilitySettingsDto(
                    ((PlaywrightCheckSettings)checkSettings).getAccessibilityValidation()));
            checkSettingsDto.setMatchLevel(playwrightCheckSettings.getMatchLevel() == null
                ? null
                : playwrightCheckSettings.getMatchLevel().getName());
            checkSettingsDto.setRetryTimeout(config.getMatchTimeout());
            checkSettingsDto.setSendDom(playwrightCheckSettings.isSendDom());
            checkSettingsDto.setUseDom(playwrightCheckSettings.isUseDom());
            checkSettingsDto.setEnablePatterns(playwrightCheckSettings.isEnablePatterns());
            checkSettingsDto.setIgnoreCaret(playwrightCheckSettings.getIgnoreCaret());
            checkSettingsDto.setUfgOptions(
                VisualGridOptionMapper.toVisualGridOptionDtoList(playwrightCheckSettings
                    .getVisualGridOptions()));
            checkSettingsDto.setLayoutBreakpoints(playwrightCheckSettings
                .getLayoutBreakpoints().isEmpty()
                ? playwrightCheckSettings.isDefaultLayoutBreakpointsSet()
                : playwrightCheckSettings.getLayoutBreakpoints());
            checkSettingsDto.setDisableBrowserFetching(playwrightCheckSettings.isDisableBrowserFetching());
            checkSettingsDto.setAutProxy(ProxyMapper.toAutProxyDto(config.getAutProxy()));
            checkSettingsDto.setHooks(playwrightCheckSettings.getScriptHooks());
            checkSettingsDto.setUserCommandId(playwrightCheckSettings.getVariationGroupId());
            checkSettingsDto.setDensityMetrics(playwrightCheckSettings.getDensityMetrics());

            return checkSettingsDto;
        }
    }
}
using System.Reflection;
using Applitools;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;

namespace Applitools.Selenium.Commands
{
    public class RemoteEvents
    {
        public string ServerUrl { get; set; }

        public string AccessKey { get; set; }

        public int? Timeout { get; set; }
    }

    public class EyesConfig
    {
        //export type EyesBaseConfig = {
        //    logs?: LogHandler
        //    debugScreenshots?: DebugScreenshotHandler
        //    agentId?: string
        //    apiKey?: string
        //    serverUrl?: string
        //    proxy?: Proxy
        //    isDisabled?: boolean
        //    connectionTimeout?: number
        //    removeSession?: boolean
        //    remoteEvents?: {serverUrl: string; accessKey?: string; timeout?: number
        //    }
        //}

        public IDebugScreenshotProvider DebugScreenshotProvider { get; set; }

        public string AgentId { get; set; }
        public string ApiKey { get; set; }
        public string ServerUrl { get; set; }
        public Proxy Proxy { get; set; }
        public bool? IsDisabled { get; set; }
        public int? ConnectionTimeout { get; set; }
        public bool? RemoveSession { get; set; }
        public RemoteEvents RemoteEvents { get; set; }

        //export type EyesOpenConfig = {
        //    appName?: string
        //    testName?: string
        //    displayName?: string
        //    viewportSize?: Size
        //    sessionType?: SessionType
        //    properties?: CustomProperty[]
        //    batch?: Batch
        //    defaultMatchSettings?: MatchSettings<Region>
        //                           hostApp?: string
        //    hostOS?: string
        //    hostAppInfo?: string
        //    hostOSInfo?: string
        //    deviceInfo?: string
        //    baselineEnvName?: string
        //    environmentName?: string
        //    branchName?: string
        //    parentBranchName?: string
        //    baselineBranchName?: string
        //    compareWithParentBranch?: boolean
        //    ignoreBaseline?: boolean
        //    saveFailedTests?: boolean
        //    saveNewTests?: boolean
        //    saveDiffs?: boolean
        //    dontCloseBatches?: boolean
        //}

        public string AppName { get; set; }
        public string TestName { get; set; }
        public string DisplayName { get; set; }
        public RectangleSize ViewportSize { get; set; }
        public string SessionType { get; set; }
        public PropertyInfo[] Properties { get; set; }
        public BatchInfo Batch { get; set; }
        public ImageMatchSettings DefaultMatchSettings { get; set; }
        public string HostOs { get; set; }
        public string HostAppInfo { get; set; }
        public string HostOsInfo { get; set; }
        public string DeviceInfo { get; set; }
        public string BaselineEnvName { get; set; }
        public string EnvironmentName { get; set; }
        public string BranchName { get; set; }
        public string ParentBranchName { get; set; }
        public string BaselineBranchName { get; set; }
        public string CompareWithParentBranch { get; set; }
        public bool? IgnoreBaseline { get; set; }
        public bool? SaveFailedTests { get; set; }
        public bool? SaveNewTests { get; set; }
        public bool? SaveDiffs { get; set; }
        public bool? DontCloseBatches { get; set; }

        //  export type EyesCheckConfig = {
        //  sendDom?: boolean
        //  matchTimeout?: number
        //  forceFullPageScreenshot?: boolean
        //}

        public bool? SendDom { get; set; }
        public int? MatchTimeout { get; set; }
        public bool? ForceFullPageScreenshot { get; set; }

        // export type EyesClassicConfig<TElement = unknown, TSelector = unknown> = {
        //  waitBeforeScreenshots?: number
        //  stitchMode?: StitchMode
        //  hideScrollbars?: boolean
        //  hideCaret?: boolean
        //  stitchOverlap?: number
        //  scrollRootElement?: TElement | TSelector
        //  cut?: ImageCropRect | ImageCropRegion
        //  rotation?: ImageRotation
        //  scaleRatio?: number
        //}

        public int? WaitBeforeScreenshots { get; set; }
        public StitchModes? StitchMode { get; set; }
        public bool? HideScrollbars { get; set; }
        public bool? HideCaret { get; set; }
        public int? StitchOverlap { get; set; }

        public IWebElement ScrollRootElement { get; set; }

        //  cut?: ImageCropRect | ImageCropRegion
        //public ImageRotation? rotation { get; set; }
        public int? ScaleRatio { get; set; }

        // export type EyesUFGConfig = {
        //  concurrentSessions?: number
        //  browsersInfo?: (DesktopBrowserRenderer | ChromeEmulationDeviceRenderer | IOSDeviceRenderer)[]
        //  visualGridOptions?: Record<string, any>
        //  layoutBreakpoints?: boolean | number[]
        //  disableBrowserFetching?: boolean
        //}

        public int? ConcurrentSessions { get; set; }

        //  browsersInfo?: (DesktopBrowserRenderer | ChromeEmulationDeviceRenderer | IOSDeviceRenderer)[]
        //  visualGridOptions?: Record<string, any>
        //  layoutBreakpoints?: boolean | number[]
        public bool? DisableBrowserFetching { get; set; }
    }
}
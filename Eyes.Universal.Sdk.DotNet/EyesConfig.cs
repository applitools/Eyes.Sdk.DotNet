using System.Reflection;
using Applitools;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace Eyes.Universal.Sdk.DotNet
{
    public class RemoteEvents
    {
        [JsonProperty("serverUrl")] public string ServerUrl { get; set; }

        [JsonProperty("accessKey")] public string AccessKey { get; set; }

        [JsonProperty("timeout")] public int? Timeout { get; set; }
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

        [JsonProperty("debugScreenshots")] public IDebugScreenshotProvider DebugScreenshotProvider { get; set; }

        [JsonProperty("agentId")] 
        public string AgentId { get; set; }

        [JsonProperty("apiKey")] 
        public string ApiKey { get; set; }

        [JsonProperty("serverUrl")] 
        public string ServerUrl { get; set; }

        [JsonProperty("proxy")] 
        public Proxy Proxy { get; set; }

        [JsonProperty("isDisabled")] 
        public bool? IsDisabled { get; set; }

        [JsonProperty("connectionTimeout")] 
        public int? ConnectionTimeout { get; set; }

        [JsonProperty("removeSession")] 
        public bool? RemoveSession { get; set; }

        [JsonProperty("remoteEvents")] 
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
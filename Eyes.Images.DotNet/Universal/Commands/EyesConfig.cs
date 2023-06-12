using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.Commands
{
    public class RemoteEvents
    {
        public string ServerUrl { get; set; }

        public string AccessKey { get; set; }

        public int? Timeout { get; set; }
    }

    public class DebugScreenshotHandler
    {
        public string Prefix { get; set; }
        public string Path { get; set; }
        public bool Save { get; set; }
    }

    public class EyesConfig
    {
        public DebugScreenshotHandler DebugScreenshots { get; set; }

        public string AgentId { get; set; }
        public string ApiKey { get; set; }
        public string ServerUrl { get; set; }
        public ProxySettings Proxy { get; set; }
        public bool? IsDisabled { get; set; }
        public int? ConnectionTimeout { get; set; }
        public bool? RemoveSession { get; set; }
        public RemoteEvents RemoteEvents { get; set; }

        public string AppName { get; set; }
        public string TestName { get; set; }
        public string DisplayName { get; set; }
        public RectangleSize ViewportSize { get; set; }
        public string SessionType { get; set; }
        public PropertyData[] Properties { get; set; }
        public BatchInfo Batch { get; set; }
        public ImageMatchSettings DefaultMatchSettings { get; set; }
        [JsonProperty("hostOS")]
        public string HostOs { get; set; }
        public string HostAppInfo { get; set; }
        [JsonProperty("hostOSInfo")]
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

        public bool? SendDom { get; set; }
        public int? MatchTimeout { get; set; }
        public bool? ForceFullPageScreenshot { get; set; }

        public IImageCrop Cut { get; set; }

        [Obsolete("Use WaitBeforeCapture")]
        public int? WaitBeforeScreenshots { get; set; }
        public int? WaitBeforeCapture { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StitchModes? StitchMode { get; set; }

        public bool? HideScrollbars { get; set; }
        public bool? HideCaret { get; set; }
        public int? StitchOverlap { get; set; }

        public object ScrollRootElement { get; set; }

        public int? ScaleRatio { get; set; }

        public int? ConcurrentSessions { get; set; }
        public IDictionary<string, object> VisualGridOptions { get; set; }

        public IRenderBrowserInfo[] BrowsersInfo { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public object LayoutBreakpoints { get; set; }

        public EyesConfig()
        {
            Properties = new PropertyData[0];
            VisualGridOptions = new Dictionary<string, object>();
        }

        public EyesConfig(Applitools.Configuration config)
            : this()
        {
            AgentId = config.AgentId;
            ApiKey = config.ApiKey;
            AppName = config.AppName;
            BaselineBranchName = config.BaselineBranchName;
            BaselineEnvName = config.BaselineEnvName;
            Batch = config.Batch;
            BranchName = config.BranchName;
            ParentBranchName = config.ParentBranchName;
            DefaultMatchSettings = config.DefaultMatchSettings;
            DisableBrowserFetching = config.DisableBrowserFetching;
            DisplayName = config.TestName;
            HostOs = config.HostOS;
            ServerUrl = config.ServerUrl;
            TestName = config.TestName;
            HostAppInfo = config.HostApp;
            MatchTimeout = config.MatchTimeout.HasValue ? (int)config.MatchTimeout.Value.TotalMilliseconds : (int?)null;
            Proxy = config.Proxy;
            SaveDiffs = config.SaveDiffs;
            SaveFailedTests = config.SaveFailedTests;
            SaveNewTests = config.SaveNewTests;
            StitchOverlap = config.StitchOverlap;
            ViewportSize = config.ViewportSize;
            HideScrollbars = config.HideScrollbars;
            HideCaret = config.HideCaret;
            EnvironmentName = config.EnvironmentName;
            StitchMode = config.StitchMode;
            WaitBeforeCapture = config.WaitBeforeCapture;
            SendDom = config.SendDom;
            Cut = config.Cut;
            DontCloseBatches = config.DontCloseBatches;
            DebugScreenshots = config.DebugScreenshotProvider;
            ForceFullPageScreenshot = config.IsForceFullPageScreenshot;

            SetLayoutBreakpoints(config);

            if (config.VisualGridOptions?.Length > 0)
            {
                VisualGridOptions = config.VisualGridOptions.ToDictionary(i => i.Key, v => v.Value);
            }

            var browsers = new List<IRenderBrowserInfo>();
            foreach (var browserInfo in config.GetBrowsersInfo())
            {
                if (browserInfo.DesktopBrowserInfo != null)
                {
                    browsers.Add(browserInfo.DesktopBrowserInfo);
                }

                if (browserInfo.IosDeviceInfo != null)
                {
                    browsers.Add(new IosDevice
                    {
                        IosDeviceInfo = browserInfo.IosDeviceInfo
                    });
                }

                if (browserInfo.ChromeEmulationInfo != null)
                {
                    browsers.Add(new ChromeEmulation
                    {
                        ChromeEmulationInfo = (ChromeEmulationInfo)browserInfo.ChromeEmulationInfo
                    });
                }

                if (browserInfo.AndroidDeviceInfo != null)
                {
                    browsers.Add(new AndroidDevice
                    {
                        AndroidDeviceInfo = browserInfo.AndroidDeviceInfo
                    });
                }
            }

            if (browsers.Any())
            {
                BrowsersInfo = browsers.ToArray();
            }
        }

        private void SetLayoutBreakpoints(Applitools.Configuration config)
        {
            LayoutBreakpoints = config.LayoutBreakpoints;
        }
    }

    public class IosDevice : IRenderBrowserInfo
    {
        public IosDeviceInfo IosDeviceInfo { get; set; }
    }

    public class AndroidDevice : IRenderBrowserInfo
    {
        public AndroidDeviceInfo AndroidDeviceInfo { get; set; }
    }

    public class ChromeEmulation : IRenderBrowserInfo
    {
        public ChromeEmulationInfo ChromeEmulationInfo { get; set; }
    }
}
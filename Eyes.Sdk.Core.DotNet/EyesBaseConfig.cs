using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public abstract class EyesBaseConfig
    {

        protected Applitools.Configuration Config { get; private set; }

        public string ApiKey
        {
            get => Config.ApiKey;
            set => Config.SetApiKey(value);
        }

        public string TestName
        {
            get => Config.TestName;
            set => Config.SetTestName(value);
        }

        public string AppName
        {
            get => Config.AppName;
            set => Config.SetAppName(value);
        }

        public RectangleSize ViewportSize
        {
            get => Config.ViewportSize;
            set => Config.SetViewportSize(value);
        }

        public string ServerUrl
        {
            get => Config.ServerUrl;
            set => Config.SetServerUrl(value);
        }

        public ProxySettings Proxy
        {
            get => Config.Proxy;
            set => Config.SetProxy(value);
        }

        #region configuration properties

        public StitchModes? StitchMode
        {
            get => Config.StitchMode;
            set => Config.SetStitchMode(value);
        }

        public int? WaitBeforeScreenshots
        {
            get => Config.WaitBeforeScreenshots;
            set => Config.SetWaitBeforeScreenshots(value);
        }

        public bool? ForceFullPageScreenshot
        {
            get => Config.IsForceFullPageScreenshot;
            set => Config.SetForceFullPageScreenshot(value);
        }

        public bool? HideScrollbars
        {
            get => Config.HideScrollbars;
            set => Config.SetHideScrollbars(value);
        }

        public bool? HideCaret
        {
            get => Config.HideCaret;
            set => Config.SetHideCaret(value);
        }

        public bool? SaveNewTests
        {
            get => Config.SaveNewTests;
            set => Config.SetSaveNewTests(value);
        }

        public string HostOS
        {
            get => Config.HostOS;
            set => Config.SetHostOS(value);
        }

        public string BranchName
        {
            get => Config.BranchName;
            set => Config.SetBranchName(value);
        }

        public string ParentBranchName
        {
            get => Config.ParentBranchName;
            set => Config.SetParentBranchName(value);
        }

        public BatchInfo Batch
        {
            get => Config.Batch;
            set => Config.SetBatch(value);
        }

        #endregion

        public bool IsDisabled { get; set; }

        public bool IsOpen { get; protected set; }


        protected EyesBaseConfig()
        {
            Config = new Applitools.Configuration(new Configuration());
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            ArgumentGuard.NotNull(configuration, nameof(configuration));

            string serverUrl = configuration.ServerUrl;
            if (serverUrl != null)
            {
                ServerUrl = serverUrl;
            }

            string apiKey = configuration.ApiKey;
            if (apiKey != null)
            {
                ApiKey = apiKey;
            }

            ProxySettings proxy = configuration.Proxy;
            if (proxy != null)
            {
                Proxy = proxy;
            }
        }
    }
}

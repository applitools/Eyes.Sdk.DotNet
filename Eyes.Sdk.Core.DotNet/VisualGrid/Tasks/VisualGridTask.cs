using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace Applitools.VisualGrid
{
    public enum TaskType { Open, Check, Close, Abort }

    public class VisualGridTask : ICompletableTask
    {
        private readonly TaskListener listener_;
        private readonly Logger logger_;
        private readonly ICheckSettings checkSettings_;
        private readonly IList<VisualGridSelector[]> regionSelectors_;
        private TestResults testResults_;
        private IConfiguration configuration_;
        private RenderStatusResults renderResult_;
        private readonly string source_;

        internal VisualGridTask(TaskType taskType, Logger logger, RunningTest runningTest)
        {
            logger_ = logger;
            TaskType = taskType;
            RunningTest = runningTest;
        }

        public VisualGridTask(IConfiguration configuration, TestResults testResults,
            IEyesConnector eyesConnector, TaskType taskType, TaskListener runningTestListener,
            ICheckSettings checkSettings, IList<VisualGridSelector[]> regionSelectors,
            RunningTest runningTest, string source, bool throwException = false)
        {
            configuration_ = configuration;
            testResults_ = testResults;
            EyesConnector = eyesConnector;
            TaskType = taskType;
            listener_ = runningTestListener;
            logger_ = runningTest.Logger;
            checkSettings_ = checkSettings?.Clone();
            regionSelectors_ = regionSelectors;
            RunningTest = runningTest;
            isThrowException_ = throwException;
            source_ = source;
        }

        public class TaskListener
        {
            public Action<VisualGridTask> OnTaskComplete;
            public Action<Exception, VisualGridTask> OnTaskFailed;
            public Action OnRenderComplete;

            public TaskListener(Action<VisualGridTask> onTaskComplete, Action<Exception, VisualGridTask> onTaskFailed, Action onRenderComplete)
            {
                OnTaskComplete = onTaskComplete;
                OnTaskFailed = onTaskFailed;
                OnRenderComplete = onRenderComplete;
            }
        }

        public TaskType TaskType { get; private set; }

        public RenderBrowserInfo BrowserInfo
        {
            get { return RunningTest.BrowserInfo; }
        }

        internal bool IsSent { get; set; }
        public Exception Exception { get; internal set; }
        public bool IsTaskComplete { get; private set; }
        public bool IsTaskReadyToCheck => renderResult_ != null || Exception != null;

        public RenderingTask RenderingTask { get; private set; }
        public RunningTest RunningTest { get; }

        private readonly bool isThrowException_;

        public IEyesConnector EyesConnector { get; }

        public TestResultContainer Call(object state)
        {
            try
            {
                logger_.Log("enter - type: {0} ; name: {1}", TaskType, RunningTest.TestName);
                testResults_ = null;
                switch (TaskType)
                {
                    case TaskType.Open:
                        logger_.Log("Task.run opening task");
                        if (renderResult_ != null)
                        {
                            string userAgent = renderResult_.UserAgent;
                            RectangleSize deviceSize = renderResult_.DeviceSize;
                            logger_.Verbose("setting device size: {0}", deviceSize);
                            EyesConnector.SetUserAgent(userAgent);
                            EyesConnector.SetDeviceSize(deviceSize);
                        }
                        else
                        {
                            // We are in exception mode - trying to do eyes.open() without first render
                            RenderBrowserInfo browserInfo = RunningTest.BrowserInfo;
                            EyesConnector.SetDeviceSize(new Size(browserInfo.Width, browserInfo.Height));
                        }
                        EyesConnector.Open(configuration_);
                        break;

                    case TaskType.Check:
                        logger_.Log("Task.run check task");
                        try
                        {
                            string imageLocation = renderResult_.ImageLocation;
                            string domLocation = renderResult_.DomLocation;
                            VGRegion[] vgRegions = renderResult_.SelectorRegions;
                            IList<IRegion> regions = new List<IRegion>();
                            if (vgRegions != null)
                            {
                                foreach (VGRegion reg in vgRegions)
                                {
                                    if (!(string.IsNullOrWhiteSpace(reg.Error)))
                                    {
                                        logger_.Log("Warning: region error: {0}", reg.Error);
                                    }
                                    else
                                    {
                                        regions.Add(reg);
                                    }
                                }
                            }
                            logger_.Verbose(renderResult_.ToString());
                            if (imageLocation == null)
                            {
                                logger_.Log("CHECKING IMAGE WITH NULL LOCATION!");
                            }
                            Location location = null;
                            if (regionSelectors_.Count > 0 && regions.Count > 0)
                            {
                                VisualGridSelector[] targetSelector = regionSelectors_[regionSelectors_.Count - 1];
                                if (targetSelector.Length > 0 && "target".Equals(targetSelector[0].Category))
                                {
                                    location = regions[regions.Count - 1].Location;
                                }
                            }
                            EyesConnector.MatchWindow(configuration_, imageLocation, domLocation, checkSettings_, regions, 
                                regionSelectors_, location, renderResult_, source_);
                        }
                        catch (WebException we)
                        {
                            Stream stream = we.Response.GetResponseStream();
                            byte[] responseBodyBytes = CommonUtils.ReadToEnd(stream);
                            string responseBodyStr = Encoding.UTF8.GetString(responseBodyBytes);
                            logger_.Log($"Error: {we}\n{responseBodyStr}");
                        }
                        catch (Exception e)
                        {
                            logger_.Log("Error: " + e);
                        }
                        break;

                    case TaskType.Close:
                        logger_.Log("Task.run close task");
                        try
                        {
                            testResults_ = EyesConnector.Close(true, configuration_);
                        }
                        catch (TestFailedException tfe)
                        {
                            logger_.Log("Test Failed: " + tfe);
                            Exception = tfe;
                            testResults_ = tfe.TestResults;
                        }
                        catch (Exception e)
                        {
                            logger_.Log("Error: " + e);
                            Exception = e;
                        }
                        break;

                    case TaskType.Abort:
                        logger_.Log("Task.run abort task");
                        testResults_ = EyesConnector.AbortIfNotClosed();
                        break;
                }
                TestResultContainer testResultContainer = new TestResultContainer(testResults_, BrowserInfo, Exception);
                NotifySuccessAllListeners();
                return testResultContainer;
            }
            catch (Exception e)
            {
                logger_.Log("Error: " + e);
                NotifyFailureAllListeners(e);
            }
            finally
            {
                logger_.Verbose("marking {0} task as complete: {1}", TaskType, RunningTest.TestName);
                IsTaskComplete = true;
                //call the callback
            }
            return null;
        }

        //private string CraftUserAgent(RenderBrowserInfo browserInfo)
        //{
        //    BrowserType browserType = browserInfo.BrowserType;
        //    string platform = StringUtils.ToPascalCase(browserInfo.Platform);
        //    switch (browserType)
        //    {
        //        case BrowserType.CHROME: return "Mozilla/5.0 (" + platform + ") Chrome/0.0";
        //        case BrowserType.FIREFOX: return "Mozilla/5.0 (" + platform + ") Firefox/0.0";
        //        case BrowserType.IE_10: return "Mozilla/5.0 (" + platform + "; MSIE 10.0)";
        //        case BrowserType.IE_11: return "Mozilla/5.0 (" + platform + "; MSIE 11.0)";
        //        case BrowserType.EDGE: return "Mozilla/5.0 (" + platform + ") Edge/0.0";
        //    }
        //    return "Mozilla/5.0 (" + platform + "; Unknown)";
        //}

        internal void SetRenderingTask(RenderingTask renderingTask)
        {
            RenderingTask = renderingTask;
        }

        private void NotifySuccessAllListeners()
        {
            listener_.OnTaskComplete(this);
        }

        private void NotifyFailureAllListeners(Exception e)
        {
            listener_.OnTaskFailed(e, this);
        }

        internal void SetRenderError(string renderId, string error, RenderRequest renderRequest)
        {
            logger_.Verbose("enter - renderId: {0}", renderId);
            RenderStatusResults renderResult = new RenderStatusResults();
            string userAgent = GetUserAgent_(renderRequest);
            if (userAgent != null)
            {
                renderResult.UserAgent = userAgent;
            }
            Size deviceSize = GetCorrectDeviceSize_(renderRequest);
            renderResult.DeviceSize = deviceSize;
            renderResult_ = renderResult;
            logger_.Verbose("device size: " + deviceSize);
            listener_.OnTaskFailed(new Exception($"Render Failed for {BrowserInfo} (renderId: {renderId}) with reason: {error}"), this);
            logger_.Verbose("exit - renderId: {0}", renderId);
        }

        private Size GetCorrectDeviceSize_(RenderRequest renderRequest)
        {
            Size deviceSize = configuration_.ViewportSize;
            if (deviceSize.IsEmpty)
            {
                IosDeviceInfo iosDeviceInfo = renderRequest.RenderInfo.IosDeviceInfo;
                ChromeEmulationInfo emulationInfo = renderRequest.RenderInfo.EmulationInfo as ChromeEmulationInfo;
                if (iosDeviceInfo != null)
                {
                    IosDeviceName deviceName = iosDeviceInfo.DeviceName;
                    Dictionary<IosDeviceName, DeviceSize> iosDevicesSizes = EyesConnector.GetIosDevicesSizes();
                    if (!iosDevicesSizes.TryGetValue(deviceName, out DeviceSize deviceSizes))
                    {
                        logger_.Verbose("could not find device in list.");
                        return Size.Empty;
                    }
                    deviceSize = iosDeviceInfo.ScreenOrientation == ScreenOrientation.Portrait
                        ? deviceSizes.Portrait
                        : deviceSizes.LandscapeLeft;
                }
                else if (emulationInfo != null)
                {
                    DeviceName deviceName = emulationInfo.DeviceName;
                    Dictionary<DeviceName, DeviceSize> emulatedDevicesSizes = EyesConnector.GetEmulatedDevicesSizes();
                    if (!emulatedDevicesSizes.TryGetValue(deviceName, out DeviceSize deviceSizes))
                    {
                        logger_.Verbose("could not find device in list.");
                        return Size.Empty;
                    }
                    deviceSize = emulationInfo.ScreenOrientation == ScreenOrientation.Portrait
                        ? deviceSizes.Portrait
                        : deviceSizes.Landscape;

                }
            }
            return deviceSize;
        }

        private string GetUserAgent_(RenderRequest renderRequest)
        {
            if (renderRequest.RenderInfo.EmulationInfo != null || renderRequest.RenderInfo.IosDeviceInfo != null)
            {
                return null;
            }

            BrowserType browser = renderRequest.BrowserName;
            Dictionary<BrowserType, string> userAgents = EyesConnector.GetUserAgents();
            if (!userAgents.TryGetValue(browser, out string userAgent))
            {
                logger_.Verbose("could not find browser {0} in list", browser);
                return null;
            }

            return userAgent;
        }

        internal void SetRenderResult(RenderStatusResults renderResult)
        {
            logger_.Verbose("enter");
            renderResult_ = renderResult;
            NotifyRenderCompleteAllListeners();
            logger_.Verbose("exit");
        }

        private void NotifyRenderCompleteAllListeners()
        {
            listener_.OnRenderComplete();
        }

        internal void SetException(Exception exception)
        {
            logger_.Verbose("setting exception: {0}", exception?.ToString());
            Exception = exception;
        }

        internal void SetExceptionAndAbort(Exception exception)
        {
            logger_.Verbose("aborting task with exception: {0}", exception?.ToString());
            Exception = exception;
            if (TaskType == TaskType.Close)
            {
                TaskType = TaskType.Abort;
            }
            AbortRunningTest(exception);
        }

        private void AbortRunningTest(Exception exception)
        {
            RunningTest.Abort(exception);
        }

        public override string ToString()
        {
            return $"Task - Type: {TaskType} ; Test Id: {RunningTest.TestId} ; Browser Info: {BrowserInfo}";
        }
    }
}
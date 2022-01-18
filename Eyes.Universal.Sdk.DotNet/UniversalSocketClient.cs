using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Eyes.Universal.Sdk.DotNet;
using Eyes.Universal.Sdk.DotNet.Commands;
using Eyes.Universal.Sdk.DotNet.Commands.Requests;
using Eyes.Universal.Sdk.DotNet.Commands.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Applitools.Selenium
{
    public class UniversalSocketClient //: IEyesBase
    {
        private WebSocketClient socketClient_;
        private CoreMakeManagerResponse coreMakeManagerResponse_;
        private string managerApplitoolsRefId_;
        private EyesRef eyes_;
        private readonly string serverUrl_;
        private readonly ILogHandler logHandler_;
        private IWebDriver driver_;
        private UniversalDriverConfig driverRef_;
        private EyesConfig eysConfig_;
        private string _serverUrl;

        public UniversalSocketClient(ILogHandler logHandler = null)
        {
            if (logHandler == null)
            {
                logHandler = NullLogHandler.Instance;
            }

            serverUrl_ = "https://eyesapi.applitools.com/";
            logHandler_ = logHandler;
            socketClient_ = new WebSocketClient();
        }

        public IWebDriver Open(IWebDriver driver)
        {
            return OpenEyes(driver);
        }

        public IWebDriver Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            return OpenEyes(driver, appName, testName, viewportSize);
        }

        public IWebDriver Open(IWebDriver driver, string appName, string testName)
        {
            return OpenEyes(driver, appName, testName);
        }

        public void Check(string name, ICheckSettings checkSettings)
        {
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesCheckRequestPayload
                {
                    Eyes = eyes_,
                    Config = new EyesConfig
                    {
                        DisplayName = name
                    },
                    Settings = CreateUniversalCheckSettings(checkSettings)
                }
            };
            
            var result = SendRequest<EyesCheckRequest, EyesCheckResponse>(request).Result;
        }

        public void Check(params ICheckSettings[] checkSettings)
        {
            foreach (var checkSetting in checkSettings)
            {
                Check(checkSetting);
            }
        }
        
        public void CheckWindow(string tag = null, bool? fully = null)
        {
            Check(tag, Target.Window().Fully(fully ?? false));
        }

        public void CheckWindow(TimeSpan matchTimeout, string tag = null, bool? fully = null)
        {
            Check(tag, Target.Window().Timeout(matchTimeout).Fully(fully ?? false));
        }

        public void CheckRegion(By selector, string tag, bool stitchContent, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Fully(stitchContent).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        public void CheckRegion(By selector, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        public void CheckRegion(By selector, TimeSpan matchTimeout, string tag = null)
        {
            ArgumentGuard.NotNull(selector, nameof(selector));
            Check(tag, Target.Region(selector).Timeout(matchTimeout));
        }

        public void CheckRegion(Rectangle region, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(region).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        public void CheckRegion(Rectangle region, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Region(region).Timeout(matchTimeout));
        }

        public void CheckRegionInFrame(string frameNameOrId, By by, string tag, bool stitchContent, int matchTimeout = -1)
        {
            Check(tag, Target.Frame(frameNameOrId).Region(by).Fully(stitchContent).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        public void CheckElement(By selector, string tag = null, int matchTimeout = -1)
        {
            Check(tag, Target.Region(selector).Timeout(TimeSpan.FromMilliseconds(matchTimeout)));
        }

        public void CheckFrame(string frameNameOrId, string tag = null)
        {
            Check(tag, Target.Frame(frameNameOrId).Fully());
        }

        public void CheckFrame(string frameNameOrId, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameNameOrId).Timeout(matchTimeout).Fully());
        }

        public void CheckFrame(int frameIndex, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameIndex).Timeout(matchTimeout).Fully());
        }

        public void CheckFrame(IWebElement frameReference, TimeSpan matchTimeout, string tag = null)
        {
            Check(tag, Target.Frame(frameReference).Timeout(matchTimeout).Fully());
        }

        public void CheckFrame(string[] framePath, TimeSpan matchTimeout, string tag = null)
        {
            ArgumentGuard.NotNull(framePath, nameof(framePath));
            ArgumentGuard.GreaterThan(framePath.Length, 0, nameof(framePath.Length));

            Fluent.SeleniumCheckSettings settings = Target.Frame(framePath[0]);
            for (int i = 1; i < framePath.Length; i++)
            {
                settings.Frame(framePath[i]);
            }

            Check(tag, settings.Timeout(matchTimeout).Fully());
        }

        public void EyesLocate()
        {
            var request = new EyesLocateRequest
            {
                Payload = new EyesLocateRequestPayload
                {
                    Eyes = eyes_,
                    Settings = new LocateSettings(), // TODO: check it
                    Config = eysConfig_
                }
            };

            SendRequest(request).GetAwaiter(); // TODO: check it
        }

        public IWebDriver GetDriver()
        {
            return driver_;
        }

        public void Dispose()
        {
            socketClient_.Dispose();
        }

        public TestResults Abort()
        {
            var request = new EyesAbortRequest
            {
                Payload = new EyesAbortRequestPayload
                {
                    Eyes = eyes_
                }
            };
             
            var testResults = SendRequest<EyesAbortRequest, TestResults>(request).Result;

            socketClient_.Close();

            return testResults;
        }

        public TestResults AbortIfNotClosed()
        {
            return Abort();
        }

        public void CloseAsync()
        {
            Task.Factory.StartNew(Close);
        }

        public TestResults Close()
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Eyes = eyes_
            };

            var testResults = SendRequest<EyesCloseRequest, TestResults>(eyesCloseRequest).Result;

            socketClient_.Close();

            return testResults;
        }

        public void CloseAll()
        {
            var closeAllEyes = CreateEyesManageCloseAllEyes();
            var testResult = SendRequest<EyesManageCloseAllEyes, TestResult>(closeAllEyes);

            socketClient_.Close();
        }

        public RectangleSize CoreGetViewportSize()
        {
            var request = new CoreGetViewportSizeRequest
            {
                Payload = new DriverRequestPayload
                {
                    Driver = driverRef_
                }
            };

            var result = SendRequest<CoreGetViewportSizeRequest, RectangleSize>(request).Result;
            
            return result;
        }

        public void CoreSetViewportSize(RectangleSize size)
        {
            var request = new CoreSetViewportSizeRequest
            {
                Payload = new CoreSetViewportSizeRequestPayload
                {
                    Driver = driverRef_,
                    Size = size
                }
            };

            SendRequest(request).GetAwaiter();
        }

        //IDictionary<string, IRunningTest> IEyesBase.GetAllTests()
        //{
        //    return activeEyes_.GetAllTests();
        //}

        public void EyesExtractTextRegions()
        {
            var request = new EyesExtractTextRegionsRequest
            {
                Payload = new EyesExtractTextRegionsRequestPayload
                {
                    Eyes = eyes_,
                    Settings = new OCRSearchSettings(), // TODO: check it
                    Config = eysConfig_
                }
            };

            SendRequest(request).GetAwaiter(); // TODO: check it
        }

        public void EyesExtractText()
        {
            var request = new EyesExtractTextRequest
            {
                Payload = new EyesExtractTextRequestPayload
                {
                    Eyes = eyes_,
                    Regions = new OCRExtractSettings[0], // TODO: check it
                    Config = eysConfig_
                }
            };

            SendRequest(request).GetAwaiter(); // TODO: check it
        }

        public void CoreCloseBatches()
        {
            var request = new CoreCloseBatchesRequest
            {
                Payload = new CoreCloseBatchesRequestPayload
                {
                    Eyes = eyes_,
                    Settings = new CloseBatchesSettings
                    {
                        ServerUrl = _serverUrl
                    }, // TODO: check it
                    Config = eysConfig_
                }
            };

            SendRequest(request).GetAwaiter(); // TODO: check it
        }

        //IDictionary<string, IRunningTest> IEyesBase.GetAllTests()
        //{
        //    return activeEyes_.GetAllTests();
        //}

        private IWebDriver OpenEyes(IWebDriver driver, string appName = null, string testName = null, Size? viewportSize = null)
        {
            driver_ = driver;
            var url = GetSocketUrl(2107);
            socketClient_.ConnectAsync(url).GetAwaiter().GetResult();

            var makeSdk = CreateCoreMakeSdk();
            var makeSdkJson = ToJson(makeSdk);

            socketClient_.SendData(makeSdkJson);

            var coreMakeManager = CreateCoreMakeManager();

            coreMakeManagerResponse_ = SendRequest(coreMakeManager).Result;
            managerApplitoolsRefId_ = coreMakeManagerResponse_.Payload.Result.ApplitoolsRefId;

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(managerApplitoolsRefId_, driver, appName, testName, viewportSize);
            driverRef_ = eyesManagerOpenEyes.Payload.Driver;
            eysConfig_ = eyesManagerOpenEyes.Payload.Config;

            var openEyesResponsePayload = SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Result.Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException($"{error.Message}{Environment.NewLine}{error.Stack}");
            }

            return driver;
        }

        private UniversalCheckSettings CreateUniversalCheckSettings(ICheckSettings checkSettings)
        {
            var settings = new UniversalCheckSettings
            {
                Region = GetUniversalRegion(checkSettings)
            };

            return settings;
        }

        private object GetUniversalRegion(ICheckSettings checkSettings)
        {
            object region = null;
            if (checkSettings is ICheckSettingsInternal checkSettingsInternal)
            {
                region = CreateCheckSettingsTargetRegion(checkSettingsInternal);
            }

            if (region == null)
            {
                if (checkSettings is ISeleniumCheckTarget seleniumCheckTarget)
                {
                    region = CreateCheckSettingsRegionSelector(seleniumCheckTarget);
                }
            }

            return region;
        }

        // ICheckSettingsInternal
        private RegionSelector CreateCheckSettingsRegionSelector(ISeleniumCheckTarget settings)
        {
            RegionSelector selector = null;

            var targetSelector = settings.GetTargetSelector();
            if (targetSelector != null)
            {
                selector = new RegionSelector
                {
                    Selector = targetSelector.Criteria,
                    Type = targetSelector.Mechanism
                };
            }

            return selector;
        }

        private UniversalRegion CreateCheckSettingsTargetRegion(ICheckSettingsInternal settings)
        {
            UniversalRegion region = null;

            var rectangle = settings.GetTargetRegion();
            if (rectangle.HasValue)
            {
                region = new UniversalRegion
                {
                    X = rectangle.Value.X,
                    Y = rectangle.Value.Y,
                    Width = rectangle.Value.Width,
                    Height = rectangle.Value.Height
                };
            }

            return region;
        }

        private object TargetReferenceConvert(SeleniumCheckSettings checkSettings)
        {
            var targetSelector = checkSettings.GetTargetSelector();
            if (targetSelector != null)
            {

            }

            return null;
        }

        private object ElementReferenceConvert(SeleniumCheckSettings checkSettings)
        {
            var targetSelector = checkSettings.GetTargetSelector();
            if (targetSelector != null)
            {

            }

            return null;
        }

        private string ToJson<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }

        private EyesManageCloseAllEyes CreateEyesManageCloseAllEyes()
        {
            var closeAllEyes = new EyesManageCloseAllEyes
            {
                Payload = new EyesManageCloseAllEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = managerApplitoolsRefId_
                    }
                }
            };

            return closeAllEyes;
        }

        private CoreMakeSdk CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdk();
            result.AddOrUpdatePayload("name", "netSDK");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }

        private CoreMakeManager CreateCoreMakeManager()
        {
            var result = new CoreMakeManager
            {
                Key = Guid.NewGuid().ToString()
            };
            result.AddOrUpdatePayload("type", "classic");

            return result;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(
            string applitoolsRefId,
            IWebDriver webDriver, 
            string appName, 
            string testName,
            Size? viewportSize)
        {
            var remoteWebDriver = (WebDriver)webDriver;
            _serverUrl = GetServerUrl(remoteWebDriver.CommandExecutor);
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesManagerOpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Driver = new UniversalDriverConfig
                    {
                        ServerUrl = _serverUrl,
                        Capabilities = GetCapabilities(remoteWebDriver),
                        SessionId = remoteWebDriver.SessionId.ToString()
                    },
                    Config = new EyesConfig
                    {
                        AppName = appName,
                        TestName = testName,
                        ServerUrl = serverUrl_,
                        ViewportSize = viewportSize
                    }
                }
            };

            return result;
        }

        private async Task<CoreMakeManagerResponse> SendRequest(CoreMakeManager coreMakeManager)
        {
            var coreMakeManagerJson = ToJson(coreMakeManager);

            await socketClient_.SendData(coreMakeManagerJson);

            CoreMakeManagerResponse coreMakeManagerResponse;

            do
            {
                var response = await socketClient_.ReceiveMessage();

                coreMakeManagerResponse = JsonConvert.DeserializeObject<CoreMakeManagerResponse>(response);
            } while (coreMakeManagerResponse?.Key != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

        private async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest data)
        {
            var json = ToJson(data);

            await socketClient_.SendData(json);

            TResponse result;

            do
            {
                var response = await socketClient_.ReceiveMessage();
                result = JsonConvert.DeserializeObject<TResponse>(response);
            } while (result == null);

            return result;
        }

        private async Task SendRequest<TRequest>(TRequest data)
        {
            var json = ToJson(data);

            await socketClient_.SendData(json);
        }

        private string GetServerUrl(ICommandExecutor commandExecutor)
        {
            var driverServiceCommandExecutor = commandExecutor as DriverServiceCommandExecutor;
            var instance = driverServiceCommandExecutor != null ? driverServiceCommandExecutor.HttpExecutor : commandExecutor;

            return GetPrivateFieldValue<ICommandExecutor, Uri>(instance, "remoteServerUri").AbsoluteUri;
        }

        private IDictionary<string, object> GetCapabilities(WebDriver webDriver)
        {
            return GetPrivateFieldValue<ICapabilities, IDictionary<string, object>>(webDriver.Capabilities, "capabilities");
        }

        private TR GetPrivateFieldValue<T, TR>(T instance, string fieldName)
        {
            return (TR)GetPrivateFieldValue(instance, fieldName);
        }

        private object GetPrivateFieldValue<T>(T instance, string fieldName)
        {
            var instanceType = instance.GetType();
            FieldInfo fieldInfo = instanceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new Exception($"The type {instanceType} does not contain {fieldName}");
            }
            
            return fieldInfo.GetValue(instance);
        }
    }
}
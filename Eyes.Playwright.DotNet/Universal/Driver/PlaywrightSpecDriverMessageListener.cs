using System;
using Applitools.Commands;
using Applitools.Commands.Responses;
using Applitools.Playwright.Universal.Driver.Dto;
using Applitools.Universal;
using Applitools.Utils;
using Newtonsoft.Json.Linq;

namespace Applitools.Playwright.Universal.Driver
{
    public class PlaywrightSpecDriverMessageListener : SpecDriverMessageListener
    {
        internal Refer Refer { get; } = new Refer();
        private readonly SpecDriverPlaywright driver_;
        public override string Protocol => "Playwright";

        internal new static Lazy<PlaywrightSpecDriverMessageListener> Instance { get; }
            = new Lazy<PlaywrightSpecDriverMessageListener>(() => new PlaywrightSpecDriverMessageListener());

        protected PlaywrightSpecDriverMessageListener()
        {
            driver_ = new SpecDriverPlaywright(Refer);
        }

        protected PlaywrightSpecDriverMessageListener(Logger logger) : base(logger)
        {
            driver_ = new SpecDriverPlaywright(Refer);
        }

        public override void HandleMessage(string message)
        {
            var response = Serializer.Deserialize<CommandResponse>(message);
            var commandResponse = new ResponseDto { Key = response.Key, Name = response.Name };
            DriverCommandDto payload = null;
            bool commandHandled = true;
            try
            {
                payload = ((JToken)response.Payload).ToObject<DriverCommandDto>();
                // Logger.Log(TraceLevel.Info, Stage.SpecDriver,
                //     $"Executing {response.Name} with target: {payload}");
                switch (response.Name)
                {
                    case "Driver.getDriverInfo":
                    {
                        var driver = payload.Driver;
                        var driverInfo = (DriverInfoDto)driver_.GetDriverInfo(driver);
                        commandResponse.Payload.Result = driverInfo;
                        break;
                    }
                    case "Driver.executeScript":
                    {
                        object executionResponse = driver_.ExecuteScript(
                            payload.Context,
                            payload.Script,
                            payload.Arg);
                        commandResponse.Payload.Result = executionResponse;
                        break;
                    }
                    case "Driver.getViewportSize":
                    {
                        var driver = payload.Driver;
                        var viewportSize = driver_.GetViewportSize(driver);
                        commandResponse.Payload.Result = viewportSize;
                        break;
                    }
                    case "Driver.setViewportSize":
                    {
                        var driver = payload.Driver;
                        var size = payload.Size;
                        driver_.SetViewportSize(driver, size);
                        commandResponse.Payload.Result = "complete";
                        break;
                    }
                    case "Driver.findElement":
                        var el = driver_.FindElement(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload.Result = el;
                        break;
                    case "Driver.findElements":
                        var els = driver_.FindElements(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload.Result = els;
                        break;
                    case "Driver.takeScreenshot":
                        byte[] screenshot = driver_.TakeScreenshot(payload.Driver);
                        commandResponse.Payload.Result = Convert.ToBase64String(screenshot);
                        break;
                    case "Driver.getTitle":
                        string title = driver_.GetTitle(payload.Driver);
                        commandResponse.Payload.Result = title;
                        break;
                    case "Driver.getUrl":
                        string url = driver_.GetUrl(payload.Driver);
                        commandResponse.Payload.Result = url;
                        break;
                    case "Driver.visit":
                        driver_.Visit(payload.Driver, payload.Url);
                        commandResponse.Payload.Result = "complete";
                        break;
                    case "Driver.getCookies":
                        var cookies = driver_.GetCookies(payload.Driver, payload.Context);
                        commandResponse.Payload.Result = cookies;
                        break;
                    case "Driver.childContext":
                        var childContext = driver_.ChildContext(payload.Context, payload.Element);
                        commandResponse.Payload.Result = childContext;
                        break;
                    case "Driver.mainContext":
                        var mainContext = driver_.MainContext(payload.Context);
                        commandResponse.Payload.Result = mainContext;
                        break;
                    case "Driver.parentContext":
                        var parentContext = driver_.ParentContext(payload.Context);
                        commandResponse.Payload.Result = parentContext;
                        break;
                    default:
                        commandHandled = false;
                        break;
                }

                if (commandHandled)
                {
                    HandleErrorIfExists(commandResponse, payload);
                }
            }
            catch (Exception e)
            {
                commandResponse.Payload.Error = new ResponsePayloadError { Message = e.Message, Stack = e.StackTrace };
                Logger.Log(TraceLevel.Error, Stage.SpecDriver,
                    $"Responding to {response.Name} with {commandResponse.Payload}");
            }

            if (commandHandled)
            {
                string json = Serializer.Serialize(commandResponse);
                WebSocket.SendData(json);
                return;
            }

            switch (response.Name)
            {
                case "Core.makeManager":
                case "Core.locate":
                case "Core.locateText":
                case "Core.extractText":
                case "Core.getViewportSize":
                case "Core.deleteTest":
                case "Core.closeBatch":
                case "EyesManager.openEyes":
                case "Eyes.check":
                case "Eyes.close":
                case "Eyes.getResults":
                case "Eyes.abort":
                case "EyesManager.getResults":
                case "Debug.getHistory":
                    HandleResponse_(message);
                    LogErrorIfExists(response.Name, response.Key, payload);
                    return;
                case "Logger.log":
                    try
                    {
                        string logMsg = $"eyes | [{payload?.Level}] | {payload?.Message}";
                        Logger.Log(TraceLevel.Debug, Stage.General, logMsg);
                    }
                    catch (Exception e)
                    {
                        CommonUtils.LogExceptionStackTrace(Logger, Stage.General, e);
                    }

                    return;

                default:
                    throw new EyesException("Unknown server command " + response.Name);
            }
        }

        private void HandleErrorIfExists(ResponseDto response, DriverCommandDto payload)
        {
            if (payload.Error == null)
            {
                return;
            }

            Logger.Log(TraceLevel.Error, Stage.SpecDriver,
                new CommandResponse { Name = response.Name, Key = response.Key, Payload = payload });
            response.Payload.Result = null;
            response.Payload.Error = payload.Error;
        }

        private void LogErrorIfExists(string name, string key, DriverCommandDto payload)
        {
            if (payload.Error != null)
            {
                Logger.Log(TraceLevel.Error, Stage.SpecDriver,
                    new CommandResponse { Name = name, Key = key, Payload = payload });
            }
        }
    }
}
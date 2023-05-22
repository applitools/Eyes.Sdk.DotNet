using System;
using Applitools.Commands;
using Applitools.Commands.Responses;
using Applitools.Playwright.Universal.Driver.Dto;
using Applitools.Universal;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver
{
    public class PlaywrightSpecDriverMessageListener : SpecDriverMessageListener
    {
        protected internal Refer Refer { get; } = new Refer();
        private readonly SpecDriverPlaywright driver_;

        public PlaywrightSpecDriverMessageListener(Logger logger) : base(logger)
        {
            driver_ = new SpecDriverPlaywright(Refer);
        }

        public override void HandleMessage(string message)
        {
            var response = Serializer.Deserialize<DriverCommand>(message);
            if (response.Payload.Error != null)
            {
                Logger.Log(TraceLevel.Error, Stage.SpecDriver, response);
                throw new EyesException(
                    $"Error handling server command {response.Name}:\n{response.Payload.Error.Message}");
            }

            ResponseDto commandResponse = new ResponseDto { Key = response.Key, Name = response.Name };
            var payload = response.Payload;
            TraceLevel level = TraceLevel.Info;
            try
            {
                Logger.Log(TraceLevel.Info, Stage.SpecDriver,
                    $"Executing {response.Name} with target: {payload}");
                switch (response.Name)
                {
                    case "Driver.getDriverInfo":
                        var driverInfo = (DriverInfoDto)driver_.GetDriverInfo(payload.Driver);
                        commandResponse.Payload.Result = driverInfo;
                        break;
                    case "Driver.executeScript":
                        object executionResponse = driver_.ExecuteScript(
                            payload.Context,
                            payload.Script,
                            payload.Arg);
                        commandResponse.Payload.Result = executionResponse;
                        break;
                    case "Driver.getViewportSize":
                        var viewportSize = driver_.GetViewportSize(payload.Driver);
                        commandResponse.Payload.Result = viewportSize;
                        break;
                    case "Driver.setViewportSize":
                        driver_.SetViewportSize(payload.Driver, payload.Size);
                        commandResponse.Payload.Result = "complete";
                        break;
                    case "Driver.findElement":
                        var el = driver_.FindElement(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload.Result = el;
                        break;
                    case "Driver.findElements":
                        var els = driver_.FindElements(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload.Result = els;
                        break;
                    case "Driver.takeScreenshot":
                        byte[] screenshot = driver_.TakeScreenshot(payload.Context);
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
                    case "Eyes.abort":
                    case "Eyes.getResults":
                    case "EyesManager.getResults":
                    case "Debug.getHistory":
                        HandleResponse_(message);
                        return;

                    case "Logger.log":
                        try
                        {
                            string logMsg = $"eyes | [{payload.Level}] | {payload.Message}";
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
            catch (Exception e)
            {
                commandResponse.Payload.Error = new ResponsePayloadError { Message = e.Message, Stack = e.StackTrace };
                level = TraceLevel.Error;
            }

            Logger.Log(level, Stage.SpecDriver,
                $"Responding to {response.Name} with {commandResponse.Payload}");

            string json = Serializer.Serialize(commandResponse);
            WebSocket.SendData(json);
            //
            // var result = JsonConvert.DeserializeObject<TResponse>(message);

            // TODO - if not webdriver invoke specdriver to change result
            //      - will invoke specdriver command that will send its result to universal
            // while (response.GetType() != typeof(TResponse)) {
            //    response = invoke specdriver command
            //    Response = sendResponseToUniversal(response);
            // }
        }
    }
}
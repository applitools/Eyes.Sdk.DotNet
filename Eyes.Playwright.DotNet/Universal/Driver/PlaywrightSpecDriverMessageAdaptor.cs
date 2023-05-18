using System;
using Applitools.Commands;
using Applitools.Playwright.Universal.Driver.Dto;
using Applitools.Universal;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver
{
    public class PlaywrightSpecDriverMessageAdaptor : ISpecDriverMessageAdaptor
    {
        private readonly SpecDriverPlaywright driver_;
        private readonly Logger logger_;
        
        public WebSocketClient WebSocket { get; set; }

        public PlaywrightSpecDriverMessageAdaptor(Refer refer, Logger logger)
        {
            driver_ = new SpecDriverPlaywright(refer);
            logger_ = logger;
        }

        public TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase
        {
            var response = JsonConvert.DeserializeObject<DriverCommand>(message);
            if (response.Payload.Error != null)
            {
                logger_.Log(TraceLevel.Error, Stage.SpecDriver, response);
                throw new EyesException($"Error handling server command {response.Name}:\n{response.Payload.Error.Message}");
            }
            CommandResponse commandResponse = new CommandResponse { Key = response.Key, Name = response.Name };
            var payload = response.Payload;
            TraceLevel level = TraceLevel.Info;
            try
            {
                logger_.Log(TraceLevel.Info, Stage.SpecDriver,
                    $"Executing {response.Name} with target: {payload}");
                switch (response.Name)
                {
                    case "Driver.getDriverInfo":
                        var driverInfo = (DriverInfoDto)driver_.GetDriverInfo(payload.Driver);
                        commandResponse.Payload = driverInfo;
                        break;
                    case "Driver.executeScript":
                        object executionResponse = driver_.ExecuteScript(
                            payload.Context,
                            payload.Script,
                            payload.Arg);
                        commandResponse.Payload = executionResponse;
                        break;
                    case "Driver.getViewportSize":
                        var viewportSize = driver_.GetViewportSize(payload.Driver);
                        commandResponse.Payload = viewportSize;
                        break;
                    case "Driver.setViewportSize":
                        driver_.SetViewportSize(payload.Driver, payload.Size);
                        commandResponse.Payload = "complete";
                        break;
                    case "Driver.findElement":
                        var el = driver_.FindElement(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload = el;
                        break;
                    case "Driver.findElements":
                        var els = driver_.FindElements(payload.Context, payload.Selector, payload.Parent);
                        commandResponse.Payload = els;
                        break;
                    case "Driver.takeScreenshot":
                        byte[] screenshot = driver_.TakeScreenshot(payload.Context);
                        commandResponse.Payload = Convert.ToBase64String(screenshot);
                        break;
                    case "Driver.getTitle":
                        string title = driver_.GetTitle(payload.Driver);
                        commandResponse.Payload = title;
                        break;
                    case "Driver.getUrl":
                        string url = driver_.GetUrl(payload.Driver);
                        commandResponse.Payload = url;
                        break;
                    case "Driver.visit":
                        driver_.Visit(payload.Driver, payload.Url);
                        commandResponse.Payload = "complete";
                        break;
                    case "Driver.getCookies":
                        var cookies = driver_.GetCookies(payload.Driver, payload.Context);
                        commandResponse.Payload = cookies;
                        break;
                    case "Driver.childContext":
                        var childContext = driver_.ChildContext(payload.Context, payload.Element);
                        commandResponse.Payload = childContext;
                        break;
                    case "Driver.mainContext":
                        var mainContext = driver_.MainContext(payload.Context);
                        commandResponse.Payload = mainContext;
                        break;
                    case "Driver.parentContext":
                        var parentContext = driver_.ParentContext(payload.Context);
                        commandResponse.Payload = parentContext;
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
                        WebSocket.SendRequest<DriverCommand, TResponse>(response);
                        break;
                    case "Logger.log":
                        try
                        {
                            string logMsg = $"eyes | [{payload.Level}] | {payload.Message}";
                            logger_.Log(TraceLevel.Debug, Stage.General, logMsg);
                        }
                        catch (Exception e)
                        {
                            CommonUtils.LogExceptionStackTrace(logger_, Stage.General, e);
                        }

                        break;

                    default:
                        throw new EyesException("Unknown server command " + response.Name);
                }
            }
            catch (Exception e)
            {
                commandResponse.Payload = new Error { Message = e.Message, Stack = e.StackTrace };
                level = TraceLevel.Error;
            }

            logger_.Log(level, Stage.SpecDriver,
                $"Responding to {response.Name} with {commandResponse.Payload}");

            WebSocket.SendRequest<CommandResponse, TResponse>(commandResponse);

            var result = JsonConvert.DeserializeObject<TResponse>(message);

            // TODO - if not webdriver invoke specdriver to change result
            //      - will invoke specdriver command that will send its result to universal
            // while (response.GetType() != typeof(TResponse)) {
            //    response = invoke specdriver command
            //    Response = sendResponseToUniversal(response);
            // }

            return result;
        }
    }
}
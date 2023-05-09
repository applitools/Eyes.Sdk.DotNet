using System;
using System.Buffers.Text;
using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Playwright.Universal.Driver.Dto;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Universal.Driver;
using Applitools.Utils;
using Eyes.Images.DotNet.Universal.Commands.Responses;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Driver
{
    public class PlaywrightSpecDriverMessageAdaptor : ISpecDriverMessageAdaptor
    {
        private readonly SpecDriverPlaywright driver_;
        private readonly WebSocketClient webSocket_;
        public PlaywrightSpecDriverMessageAdaptor(SpecDriverPlaywright driver, WebSocketClient webSocket)
        {
            driver_ = driver;
            webSocket_ = webSocket;
        }
        
        public TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase
        {
            var response = JsonConvert.DeserializeObject<DriverCommand>(message);
            CommandResponse commandResponse = new CommandResponse { Key = response.Key, Name = response.Name };
            var payload = response.Payload;
            switch (response.Name)
            {
                case "Driver.getDriverInfo":
                    var driverInfo = (DriverInfoDto)driver_.GetDriverInfo(payload.Driver);
                    commandResponse.Payload = driverInfo;

                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /* ResponseDto < ?> getDriverInfoResponse = new ResponseDto<>();
                     getDriverInfoResponse.setName(response.getName());
                     getDriverInfoResponse.setKey(response.getKey());
                     try
                     {
                         DriverCommandDto target = objectMapper.readValue(payload,
                             new TypeReference<RequestDto<DriverCommandDto>>()
                             {
                             }).getPayload();

                         logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                             string.format("Executing %s with target: %s", response.getName(), target));
                         DriverInfoDto driverInfo = specDriver_.getDriverInfo(target.getDriver());
                         getDriverInfoResponse.setPayload(new ResponsePayload(driverInfo, null));
                     }
                     catch (Exception e)
                     {
                         ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                             "spec-driver", null);
                         getDriverInfoResponse.setPayload(new ResponsePayload<>(null, err));
                     }

                     logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                         string.format("Responding to %s with: %s", response.getName(), getDriverInfoResponse));
                     webSocket.sendTextFrame(objectMapper.writeValueAsString(getDriverInfoResponse));*/
                    break;
                case "Driver.executeScript":
                    object executionResponse = driver_.ExecuteScript(
                        payload.Context, 
                        payload.Script, 
                        payload.Arg);

                    commandResponse.Payload = executionResponse;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);

/*                    try
                    {
                        DriverCommandDto dto = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), dto));
                        object handleObject =
                            specDriver_.executeScript(dto.getContext(), dto.getScript(), dto.getArg());
                        executeScriptResponse.setPayload(new ResponsePayload(handleObject, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        executeScriptResponse.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), executeScriptResponse));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(executeScriptResponse));*/
                    break;
                case "Driver.getViewportSize":
                    var viewportSize = driver_.GetViewportSize(payload.Driver);
                    commandResponse.Payload = viewportSize;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*   ResponseDto < ?> getViewportSizeResponse = new ResponseDto<>();
                       getViewportSizeResponse.setName(response.getName());
                       getViewportSizeResponse.setKey(response.getKey());
                       try
                       {
                           DriverCommandDto target = objectMapper.readValue(payload,
                               new TypeReference<RequestDto<DriverCommandDto>>()
                               {
                               }).getPayload();
                           logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                               string.format("Executing %s with target: %s", response.getName(), target));
                           RectangleSizeDto viewportSize = specDriver_.getViewportSize(target.getDriver());
                           getViewportSizeResponse.setPayload(new ResponsePayload(viewportSize, null));
                       }
                       catch (Exception e)
                       {
                           ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                               "spec-driver", null);
                           getViewportSizeResponse.setPayload(new ResponsePayload<>(null, err));
                       }
   
                       logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                           string.format("Responding to %s with: %s", response.getName(), getViewportSizeResponse));
                       webSocket.sendTextFrame(objectMapper.writeValueAsString(getViewportSizeResponse));*/
                    break;
                case "Driver.setViewportSize":
                    driver_.SetViewportSize(payload.Driver, payload.Size);
                    commandResponse.Payload = "complete";
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> setViewportSize = new ResponseDto<>();
                    setViewportSize.setName(response.getName());
                    setViewportSize.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        specDriver_.setViewportSize(target.getDriver(), target.getSize());
                        setViewportSize.setPayload(new ResponsePayload("complete", null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        setViewportSize.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), setViewportSize));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(setViewportSize));*/
                    break;
                case "Driver.findElement":
                    var el = driver_.FindElement(payload.Context, payload.Selector, payload.Parent);
                    commandResponse.Payload = el;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*  ResponseDto < ?> findElement = new ResponseDto<>();
                      findElement.setName(response.getName());
                      findElement.setKey(response.getKey());
                      try
                      {
                          DriverCommandDto target = objectMapper.readValue(payload,
                              new TypeReference<RequestDto<DriverCommandDto>>()
                              {
                              }).getPayload();
  
                          logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                              string.format("Executing %s with target: %s", response.getName(), target));
                          Element element = specDriver_.findElement(target.getContext(), target.getSelector(),
                              target.getParent());
                          findElement.setPayload(new ResponsePayload(element, null));
                      }
                      catch (Exception e)
                      {
                          ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                              "spec-driver", null);
                          findElement.setPayload(new ResponsePayload<>(null, err));
                      }
  
                      logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                          string.format("Responding to %s with: %s", response.getName(), findElement));
                      webSocket.sendTextFrame(objectMapper.writeValueAsString(findElement));*/
                    break;
                case "Driver.findElements":
                    var els = driver_.FindElements(payload.Context, payload.Selector, payload.Parent);
                    commandResponse.Payload = els;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> findElements = new ResponseDto<>();
                    findElements.setName(response.getName());
                    findElements.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        List<Reference> elements = specDriver_.findElements(target.getContext(),
                            target.getSelector(), target.getParent());
                        findElements.setPayload(new ResponsePayload(elements, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        findElements.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), findElements));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(findElements));*/
                    break;
                case "Driver.takeScreenshot":
                    byte[] screenshot = driver_.TakeScreenshot(payload.Context);
                    commandResponse.Payload = Convert.ToBase64String(screenshot);
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> takeScreenshot = new ResponseDto<>();
                    takeScreenshot.setName(response.getName());
                    takeScreenshot.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        byte[] screenshot = (byte[])specDriver_.takeScreenshot(target.getDriver());
                        takeScreenshot.setPayload(
                            new ResponsePayload(Base64.getEncoder().encodeToString(screenshot), null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        takeScreenshot.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), takeScreenshot));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(takeScreenshot));*/
                    break;
                case "Driver.getTitle":
                    string title = driver_.GetTitle(payload.Driver);
                    commandResponse.Payload = title;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> getTitle = new ResponseDto<>();
                    getTitle.setName(response.getName());
                    getTitle.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        string title = specDriver_.getTitle(target.getDriver());
                        getTitle.setPayload(new ResponsePayload(title, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        getTitle.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), getTitle));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(getTitle));*/
                    break;
                case "Driver.getUrl":
                    string url = driver_.GetUrl(payload.Driver);
                    commandResponse.Payload = url;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> getUrl = new ResponseDto<>();
                    getUrl.setName(response.getName());
                    getUrl.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        string url = specDriver_.getUrl(target.getDriver());
                        getUrl.setPayload(new ResponsePayload(url, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        getUrl.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), getUrl));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(getUrl));*/
                    break;
                case "Driver.visit":
                    driver_.Visit(payload.Driver, payload.Url);
                    commandResponse.Payload = "complete";
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*    ResponseDto < ?> visit = new ResponseDto<>();
                    visit.setName(response.getName());
                    visit.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        specDriver_.visit(target.getDriver(), target.getUrl());
                        visit.setPayload(new ResponsePayload("complete", null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        visit.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), visit));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(visit));*/
                    break;
                case "Driver.getCookies":
                    var cookies = driver_.GetCookies(payload.Driver, payload.Context);
                    commandResponse.Payload = cookies;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /* ResponseDto < ?> getCookies = new ResponseDto<>();
                     getCookies.setName(response.getName());
                     getCookies.setKey(response.getKey());
                     try
                     {
                         DriverCommandDto target = objectMapper.readValue(payload,
                             new TypeReference<RequestDto<DriverCommandDto>>()
                             {
                             }).getPayload();
 
                         logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                             string.format("Executing %s with target: %s", response.getName(), target));
                         List<ICookie> cookies = specDriver_.getCookies(target.getDriver(), target.getContext());
                         getCookies.setPayload(new ResponsePayload(cookies, null));
                     }
                     catch (Exception e)
                     {
                         ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                             "spec-driver", null);
                         getCookies.setPayload(new ResponsePayload<>(null, err));
                     }
 
                     logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                         string.format("Responding to %s with: %s", response.getName(), getCookies));
                     webSocket.sendTextFrame(objectMapper.writeValueAsString(getCookies));*/
                    break;
                case "Driver.childContext":
                    var childContext = driver_.ChildContext(payload.Context, payload.Element);
                    commandResponse.Payload = childContext;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> childContext = new ResponseDto<>();
                    childContext.setName(response.getName());
                    childContext.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        Context context = specDriver_.childContext(target.getContext(), target.getElement());
                        childContext.setPayload(new ResponsePayload(context, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        childContext.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), childContext));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(childContext));*/
                    break;
                case "Driver.mainContext":
                    var mainContext = driver_.MainContext(payload.Context);
                    commandResponse.Payload = mainContext;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> mainContext = new ResponseDto<>();
                    mainContext.setName(response.getName());
                    mainContext.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        Context context = specDriver_.mainContext(target.getContext());
                        mainContext.setPayload(new ResponsePayload(context, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        mainContext.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), mainContext));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(mainContext));*/
                    break;
                case "Driver.parentContext":
                    var parentContext = driver_.ParentContext(payload.Context);
                    commandResponse.Payload = parentContext;
                    webSocket_.SendRequest<CommandResponse, TResponse>(commandResponse);
                    /*ResponseDto < ?> parentContext = new ResponseDto<>();
                    parentContext.setName(response.getName());
                    parentContext.setKey(response.getKey());
                    try
                    {
                        DriverCommandDto target = objectMapper.readValue(payload,
                            new TypeReference<RequestDto<DriverCommandDto>>()
                            {
                            }).getPayload();

                        logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                            string.format("Executing %s with target: %s", response.getName(), target));
                        Context context = specDriver_.parentContext(target.getContext());
                        parentContext.setPayload(new ResponsePayload(context, null));
                    }
                    catch (Exception e)
                    {
                        ErrorDto err = new ErrorDto(e.getMessage(), Arrays.toString(e.getStackTrace()),
                            "spec-driver", null);
                        parentContext.setPayload(new ResponsePayload<>(null, err));
                    }

                    logger.log(TraceLevel.Info, Stage.SPEC_DRIVER,
                        string.format("Responding to %s with: %s", response.getName(), parentContext));
                    webSocket.sendTextFrame(objectMapper.writeValueAsString(parentContext));*/
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
                    handleResponse(payload, typeReferences.get(response.getName()));
                    break;
                case "Logger.log":
                    try
                    {
                        LogResponseDto serverLogResponse = objectMapper.readValue(payload,
                            new TypeReference<LogResponseDto>()
                            {
                            });
                        string message1 = "eyes | " + new Timestamp(System.currentTimeMillis())
                                                    + " | [" + serverLogResponse.getPayload().getLevel() + "] | "
                                                    + serverLogResponse.getPayload().getMessage();
                        logger.log(TraceLevel.Debug, Stage.GENERAL, message1);
                        System.out.println(message1);
                    }
                    catch (Exception e)
                    {
                        CommonUtils.LogExceptionStackTrace(logger_, Stage.General, e, testIds_);
                    }

                    break;

                default:
                    throw new EyesException("Unknown server command " + response.getName());
            }

            var result = JsonConvert.DeserializeObject<TResponse>(message);

            // TODO - if not webdriver invoke specdriver to change result
            //          - will invoke specdriver command that will send its result to universal
            // while (response.GetType() != typeof(TResponse)) {
            //    response = invoke specdriver command
            //    Response = sendResponseToUniversal(response);
            // }

            return result;
        }
    }
}
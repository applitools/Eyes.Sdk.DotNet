using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Applitools.Playwright.Universal.Driver.Dto;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Universal.Driver;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Driver
{
    public class SpecDriverPlaywright : ISpecDriver
    {
        private readonly Refer refer_;

        public SpecDriverPlaywright(Refer refer)
        {
            refer_ = refer;
        }

        public bool IsDriver(object driver)
        {
            return driver is IPage;
        }

        public bool IsContext(Reference context)
        {
            throw new NotSupportedException("IsContext is not supported");
        }

        public bool IsElement(Reference element)
        {
            throw new NotSupportedException("IsElement is not supported");
        }

        public bool IsSelector(Reference selector)
        {
            throw new NotSupportedException("IsSelector is not supported");
        }

        public bool IsEqualElements()
        {
            throw new NotSupportedException("IsEqualElements is not supported");
        }

        public Reference MainContext(Reference context)
        {
            IFrame mainFrame = ExtractContextUtil_(context);
            while (mainFrame.ParentFrame != null)
            {
                mainFrame = mainFrame.ParentFrame;
            }

            Context frameContext = new Context
            {
                ApplitoolsRefId = refer_.Ref(mainFrame, context)
            };
            return frameContext;
        }

        public Reference ParentContext(Reference context)
        {
            IFrame frame = ExtractContextUtil_(context);

            if (frame.ParentFrame == null)
            {
                return null;
            }

            Context frameContext = new Context
            {
                ApplitoolsRefId = refer_.Ref(frame.ParentFrame, context)
            };
            return frameContext;
        }

        public Reference ChildContext(Reference context, Reference element)
        {
            object root = refer_.Deref(element);

            if (root is IElementHandle eh)
            {
                Context frameContext = new Context();
                IFrame frame = eh.ContentFrameAsync().GetAwaiter().GetResult();
                frameContext.ApplitoolsRefId = refer_.Ref(frame, element);

                return frameContext;
            }

            return null;
        }

        public object ExecuteScript(Reference context, string script, object arg)
        {
            object ctx = refer_.Deref(context);
            IJSHandle res = null;

            object args = DerefArgsUtil(arg);

            if (ctx is IFrame frame)
            {
                res = frame.EvaluateHandleAsync(script, args).GetAwaiter().GetResult();
            }
            else if (ctx is IPage page)
            {
                res = page.EvaluateHandleAsync(script, args).GetAwaiter().GetResult();
            }

            return HandlerToObjectUtil(res, context);
        }

        public Reference FindElement(Reference driver, Reference selector, Reference parent)
        {
            object context = refer_.Deref(driver);
            object root = parent == null ? context : refer_.Deref(parent);
            object locator = refer_.Deref(selector);

            IElementHandle elementHandle = null;
            string selectorPath = ((Selector)selector).SelectorPath;
            if (locator is ILocator loc)
            {
                elementHandle = loc.ElementHandleAsync().GetAwaiter().GetResult();
            }
            else if (root is IFrame frame)
            {
                elementHandle = frame.QuerySelectorAsync(selectorPath).GetAwaiter().GetResult();
            }
            else if (root is IPage page)
            {
                elementHandle = page.QuerySelectorAsync(selectorPath).GetAwaiter().GetResult();
            }

            if (elementHandle == null)
            {
                return null;
            }

            Element element = new Element
            {
                ApplitoolsRefId = refer_.Ref(elementHandle, driver)
            };
            return element;
        }

        public List<Reference> FindElements(Reference driver, Reference selector, Reference parent)
        {
            object context = refer_.Deref(driver);
            object root = parent == null ? context : refer_.Deref(parent);
            object locator = refer_.Deref(selector);

            IEnumerable<IElementHandle> elementHandles = null;
            string selectorPath = ((Selector)selector).SelectorPath;
            if (locator is ILocator loc)
            {
                elementHandles = loc.ElementHandlesAsync().GetAwaiter().GetResult();
            }
            else if (root is IFrame frame)
            {
                elementHandles = frame.QuerySelectorAllAsync(selectorPath).GetAwaiter().GetResult();
            }
            else if (root is IPage page)
            {
                elementHandles = page.QuerySelectorAllAsync(selectorPath).GetAwaiter().GetResult();
            }

            if (elementHandles == null)
            {
                return new List<Reference>();
            }

            List<Reference> result = elementHandles.Select<IElementHandle, Reference>(
                    eh =>
                    {
                        Element e = new Element(eh)
                        {
                            ApplitoolsRefId = refer_.Ref(eh, driver)
                        };
                        return e;
                    })
                .ToList();
            return result;
        }

        public void SetElementText()
        {
            throw new NotSupportedException("SetElementText is not supported");
        }

        public string GetElementText()
        {
            throw new NotSupportedException("GetElementText is not supported");
        }

        public void SetWindowSize(Reference driver, RectangleSize windowSize)
        {
            throw new NotSupportedException("SetWindowSize is not supported");
        }

        public RectangleSize GetWindowSize(Reference driver)
        {
            throw new NotSupportedException("GetWindowSize is not supported");
        }

        public void SetViewportSize(Reference driver, RectangleSize windowSize)
        {
            object page = refer_.Deref(driver);
            ((IPage)page).SetViewportSizeAsync(windowSize.Width, windowSize.Height);
        }

        public RectangleSize GetViewportSize(Reference driver)
        {
            object page = refer_.Deref(driver);
            RectangleSize viewportSize = new RectangleSize();
            PageViewportSizeResult pageVpSize = ((IPage)page).ViewportSize;
            viewportSize.Width = pageVpSize?.Width ?? 0;
            viewportSize.Height = pageVpSize?.Height ?? 0;
            return viewportSize;
        }

        public List<ICookie> GetCookies(Reference driver, Reference context)
        {
            IPage ctx = (IPage)refer_.Deref(driver);
            IReadOnlyList<BrowserContextCookiesResult> cookies = ctx.Context.CookiesAsync().GetAwaiter().GetResult();

            return cookies.Select<BrowserContextCookiesResult, ICookie>(cookie => new TCookie(cookie)).ToList();
        }

        public object GetDriverInfo(Reference driver)
        {
            return new DriverInfoDto();
        }

        public Dictionary<string, object> GetCapabilities(Reference driver)
        {
            throw new NotSupportedException("GetCapabilities is not supported");
        }

        public string GetTitle(Reference driver)
        {
            IPage context = (IPage)refer_.Deref(driver);
            return context.TitleAsync().GetAwaiter().GetResult();
        }

        public string GetUrl(Reference driver)
        {
            IPage context = (IPage)refer_.Deref(driver);
            return context.Url;
        }

        public void Click()
        {
            throw new NotSupportedException("Click is not supported");
        }

        public void Visit(Reference driver, string url)
        {
            IPage context = (IPage)refer_.Deref(driver);
            context.GotoAsync(url);
        }

        public void Hover(Reference context, Reference element)
        {
            throw new NotSupportedException("Hover is not supported");
        }

        public void ScrollIntoView(Reference context, Reference element, bool align)
        {
            throw new NotSupportedException("ScrollIntoView is not supported");
        }

        public void WaitUntilDisplayed(Reference context, Reference selector)
        {
            throw new NotSupportedException("WaitUntilDisplayed is not supported");
        }

        public byte[] TakeScreenshot(Reference driver)
        {
            IPage context = (IPage)refer_.Deref(driver);
            return context.ScreenshotAsync().GetAwaiter().GetResult();
        }
        
        private static readonly Regex jsHandlePattern_ = new Regex("(?:.+@)?(\\w*)(?:\\(\\d+\\))?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex arrayPattern_ = new Regex("array\\(\\d+\\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        private object HandlerToObjectUtil(IJSHandle jsHandle, Reference context)
        {
            if (jsHandle == null)
            {
                return null;
            }

            if (jsHandle is IElementHandle)
            {
                Element element = new Element
                {
                    ApplitoolsRefId = refer_.Ref(jsHandle.AsElement(), context)
                };
                return element;
            }

            string type = "";

            Match jsHandleMatcher = jsHandlePattern_.Match(jsHandle.ToString());
            if (jsHandleMatcher.Success)
            {
                type = jsHandleMatcher.Groups[0].Value.ToLower();
            }

            if (arrayPattern_.Match(type).Success)
            {
                var map = jsHandle.GetPropertiesAsync().GetAwaiter().GetResult();
                var arrayValues = new List<object>();
                foreach (IJSHandle jsHandle1 in map.Values)
                {
                    arrayValues.Add(HandlerToObjectUtil(jsHandle1, context));
                }

                return arrayValues;
            }

            if (type.Equals("object"))
            {
                var map = jsHandle.GetPropertiesAsync().GetAwaiter().GetResult();
                var resultMap = new Dictionary<string, object>();
                foreach (var entry in map)
                {
                    resultMap.Add(entry.Key, HandlerToObjectUtil(entry.Value, context));
                }

                return resultMap;
            }

            return jsHandle.JsonValueAsync<object>().GetAwaiter().GetResult();
        }

        private object DerefArgsUtil(object arg)
        {
            if (arg == null)
            {
                return null;
            }

            var derefArg = new List<object>();

            if (arg is Dictionary<object, object> argsDict)
            {
                var map = new Dictionary<object, object>();
                foreach (var item in argsDict)
                {
                    map.Add(item.Key, DerefArgsUtil(item.Value));
                }

                return map;
            }

            if (arg is IList argsList)
            {
                foreach (object argument in argsList)
                {
                    if (argument is Reference)
                    {
                        derefArg.Add(refer_.Deref(argument));
                    }
                    else
                    {
                        derefArg.Add(DerefArgsUtil(argument));
                    }
                }

                return derefArg;
            }


            return refer_.Deref(arg);
        }

        private IFrame ExtractContextUtil_(Reference context)
        {
            object root = refer_.Deref(context);
            return IsDriver(root) ? ((IPage)root).MainFrame : (IFrame)root;
        }
    }
}
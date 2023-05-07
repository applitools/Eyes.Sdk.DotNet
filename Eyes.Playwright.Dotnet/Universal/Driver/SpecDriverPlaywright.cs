using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Driver
{
    public class SpecDriverPlaywright : ISpecDriver
    {
        private readonly Refer refer_;

        public SpecDriverPlaywright(Refer refer) {
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

        public Reference MainContext(Reference context)
        {
            IFrame mainFrame = ExtractContextUtil_(context);
            while (mainFrame.parentFrame() != null)
            {
                mainFrame = mainFrame.parentFrame();
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

            if (frame.parentFrame() == null)
            {
                return null;
            }

            Context frameContext = new Context
            {
                ApplitoolsRefId = refer_.Ref(frame.parentFrame(), context)
            };
            return frameContext;
        }

        public Reference ChildContext(Reference context, Reference element)
        {
            object root = refer_.Deref(element);

            if (root is IElementHandle eh) {
                Context frameContext = new Context();
                IFrame frame = eh.ContentFrameAsync().GetAwaiter().GetResult();
                frameContext.setApplitoolsRefId(refer_.ref (frame, element));

                return frameContext;
            }

            return null;
        }

        @Override

        public object ExecuteScript(Reference context, string script, object arg)
        {
            object ctx = refer_.deref(context);
            JSHandle res = null;

            object args = derefArgsUtil(arg);

            if (ctx instanceof Frame) {
                res = ((Frame)ctx).evaluateHandle(script, args);
            } else if (ctx instanceof Page) {
                res = ((Page)ctx).evaluateHandle(script, args);
            }

            return handlerToObjectUtil(res, context);
        }

        @Override

        public Element findElement(Reference driver, Reference selector, Reference parent)
        {
            object context = refer_.deref(driver);
            object root = parent == null ? context : refer_.deref(parent);
            object locator = refer_.deref(selector);

            ElementHandle elementHandle = null;
            string _selector = ((Selector)selector).getSelector();
            if (locator instanceof Locator) {
                elementHandle = ((Locator)locator).elementHandle();
            } else if (root instanceof Frame) {
                elementHandle = ((Frame)root).querySelector(_selector);
            } else if (root instanceof Page) {
                elementHandle = ((Page)root).querySelector(_selector);
            }

            if (elementHandle == null)
            {
                return null;
            }

            Element element = new Element();
            element.setApplitoolsRefId(refer_.ref (elementHandle, driver));
            return element;
        }

        @Override

        public List<Reference> FindElements(Reference context, Reference selector, Reference parent)
        {
            object ctx = refer_.deref(context);
            object root = parent == null ? ctx : refer_.deref(parent);
            object locator = refer_.deref(selector);

            List<ElementHandle> elementHandles = new ArrayList<>();
            string _selector = ((Selector)selector).getSelector();
            if (locator instanceof Locator) {
                elementHandles = ((Locator)locator).elementHandles();
            } else if (root instanceof Frame) {
                elementHandles = ((Frame)root).querySelectorAll(_selector);
            } else if (root instanceof Page) {
                elementHandles = ((Page)root).querySelectorAll(_selector);
            }

            return elementHandles.stream()
                .map(elementHandle-> {
                Element element = new Element(elementHandle);
                element.setApplitoolsRefId(refer_.ref (elementHandle, context));
                return element;
            })
            .collect(Collectors.toList());
        }

        @Override

        public void setViewportSize(Reference driver, RectangleSizeDto windowSize)
        {
            object page = refer_.deref(driver);
            ((Page)page).setViewportSize(windowSize.getWidth(), windowSize.getHeight());
        }

        @Override

        public RectangleSizeDto getViewportSize(Reference driver)
        {
            object page = refer_.deref(driver);
            RectangleSizeDto viewportSize = new RectangleSizeDto();
            viewportSize.setWidth(((Page)page).viewportSize().width);
            viewportSize.setHeight(((Page)page).viewportSize().height);
            return viewportSize;
        }

        @Override

        public List<ICookie> getCookies(Reference driver, Reference context)
        {
            Page ctx = (Page)refer_.deref(driver);
            List<Cookie> cookies = ctx.context().cookies();

            return cookies.stream()
                .map(TCookie::new)
                .collect(Collectors.toList());
        }

        @Override

        public DriverInfoDto getDriverInfo(Reference driver)
        {
            return new DriverInfoDto();
        }

        @Override

        public string GetTitle(Reference driver)
        {
            Page context = (Page)refer_.deref(driver);
            return context.title();
        }

        @Override

        public string GetUrl(Reference driver)
        {
            Page context = (Page)refer_.deref(driver);
            return context.url();
        }

        @Override

        public void Visit(Reference driver, string url)
        {
            Page context = (Page)refer_.deref(driver);
            context.navigate(url);
        }

        @Override

        public byte[] takeScreenshot(Reference driver)
        {
            Page context = (Page)refer_.deref(driver);
            return context.screenshot();
        }

        static public string[] getMethodNames()
        {
            Method[] methods = SpecDriverPlaywright.class.getDeclaredMethods();
            return Stream.of(methods)
                .map(Method::getName)
                .filter(name-> !name.contains("getMethodNames") && !name.contains("Util"))
                .toArray(string[]::new);
        }

        private object handlerToObjectUtil(JSHandle jsHandle, Reference context)
        {
            if (jsHandle == null)
            {
                return null;
            }

            string type = "";
            Pattern pattern = Pattern.compile("(?:.+@)?(\\w*)(?:\\(\\d+\\))?", Pattern.CASE_INSENSITIVE);
            Matcher matcher = pattern.matcher(jsHandle.toString());
            if (matcher.find())
            {
                type = matcher.group(0).toLowerCase();
            }

            if (type.matches("array\\(\\d+\\)"))
            {
                Map<string, JSHandle> map = jsHandle.getProperties();
                List<object> arrayValues = new ArrayList<>();
                for (JSHandle jsHandle1:
                map.values()) {
                    arrayValues.add(handlerToObjectUtil(jsHandle1, context));
                }
                return arrayValues;
            }
            else if (type.equals("object"))
            {
                Map<string, JSHandle> map = jsHandle.getProperties();
                Map<string, object> resultMap = new HashMap<>();
                for (Map.Entry < string, JSHandle > entry: map.entrySet())
                {
                    resultMap.put(entry.getKey(), handlerToObjectUtil(entry.getValue(), context));
                }
                return resultMap;
            }
            else if (type.equals("jshandle@node"))
            {
                Element element = new Element();
                element.setApplitoolsRefId(refer_.ref (jsHandle.asElement(), context));
                return element;
            }

            return jsHandle.jsonValue();
        }

        private object derefArgsUtil(object arg)
        {
            if (arg == null)
            {
                return null;
            }

            ArrayList<object> derefArg = new ArrayList<>();

            if (arg instanceof ArrayList) {
                for (object argument:
                (ArrayList)arg) {
                    if (argument instanceof ArrayList) {
                        derefArg.add(derefArgsUtil(argument));
                    } else {
                        derefArg.add(refer_.deref(argument));
                    }
                }
                return derefArg;
            } else if (arg instanceof HashMap){
                HashMap<object, object> map = new HashMap<>();
                for (Map.Entry < ?, ?> entry :
                ((HashMap < ?,  ?>) arg).entrySet()) {
                    map.put(entry.getKey(), refer_.deref(entry.getValue()));
                }
                return map;
            }

            return refer_.deref(arg);
        }

        private IFrame ExtractContextUtil_(Reference context)
        {
            object root = refer_.Deref(context);
            return IsDriver(root) ? ((IPage)root).mainFrame() : (IFrame)root;
        }
    }
}
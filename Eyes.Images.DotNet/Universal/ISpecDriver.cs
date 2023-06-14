using System.Collections.Generic;
using Applitools.Universal.Driver;
using Applitools.Utils.Geometry;

namespace Applitools.Universal
{
    public interface ISpecDriver
    {

    #region UTILITY

    bool IsDriver(object driver);

    bool IsContext(Reference context);

    bool IsElement(Reference element);
    
    bool IsSelector(Reference selector);

    bool IsEqualElements();
    
    #endregion

    #region COMMANDS

    Reference MainContext(Reference context);

    Reference ParentContext(Reference context);

    Reference ChildContext(Reference context, Reference element);

    object ExecuteScript(Reference context, string script, object arg);

    Reference FindElement(Reference driver, Reference selector, Reference parent);

    List<Reference> FindElements(Reference context, Reference selector, Reference parent);

    void SetElementText();

    string GetElementText();

    void SetWindowSize(Reference driver, RectangleSize windowSize);
    
    RectangleSize GetWindowSize(Reference driver);
    
    void SetViewportSize(Reference driver, RectangleSize windowSize);
    
    RectangleSize GetViewportSize(Reference driver);
    
    List<ICookie> GetCookies(Reference driver, Reference context);
    
    object GetDriverInfo(Reference driver);
    
    Dictionary<string, object> GetCapabilities(Reference driver);
    
    string GetTitle(Reference driver);
    
    string GetUrl(Reference driver);
    
    byte[] TakeScreenshot(Reference driver);
    
    void Click();
    
    void Visit(Reference driver, string url);
    
    void Hover(Reference context, Reference element);
    
    void ScrollIntoView(Reference context, Reference element, bool align);
    
    void WaitUntilDisplayed(Reference context, Reference selector);

    #endregion COMMANDS
    }

}
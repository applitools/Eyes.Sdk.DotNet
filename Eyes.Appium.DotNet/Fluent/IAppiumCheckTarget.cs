namespace Applitools.Appium
{
    interface IAppiumCheckTarget : ITargetContainer //: IScrollRootElementContainer
    {
        bool? IsDefaultWebview();
        string GetWebview();
    }
}
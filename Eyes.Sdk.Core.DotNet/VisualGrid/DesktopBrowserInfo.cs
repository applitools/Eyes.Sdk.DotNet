using Applitools.Selenium;
using System;
using System.Drawing;

namespace Applitools.VisualGrid
{
    public class DesktopBrowserInfo : IRenderBrowserInfo, IEquatable<DesktopBrowserInfo>
    {
        public DesktopBrowserInfo(Size viewportSize,
            BrowserType browserType = BrowserType.CHROME,
            string baselineEnvName = null)
        {
            ViewportSize = viewportSize;
            BrowserType = browserType;
            BaselineEnvName = baselineEnvName;
        }

        public DesktopBrowserInfo(int width, int height,
            BrowserType browserType = BrowserType.CHROME,
            string baselineEnvName = null)
            : this(new Size(width, height), browserType, baselineEnvName)
        {
        }

        public Size ViewportSize { get; }
        public int Width { get => ViewportSize.Width; }
        public int Height { get => ViewportSize.Height; }

        public BrowserType BrowserType { get; }
        public string BaselineEnvName { get; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DesktopBrowserInfo);
        }

        public bool Equals(DesktopBrowserInfo other)
        {
            if (other == null) return false;

            return
                ViewportSize == other.ViewportSize &&
                BrowserType == other.BrowserType;
        }

        public override string ToString()
        {
            return $"{nameof(DesktopBrowserInfo)} {{ViewportSize={ViewportSize}, BrowserType={BrowserType}}}";
        }

    }
}
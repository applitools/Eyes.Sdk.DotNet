using Applitools.VisualGrid;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Fluent
{
    public interface ICheckSettingsInternal
    {
        Rectangle? GetTargetRegion();
        int GetTimeout();
        bool? GetStitchContent();
        MatchLevel? GetMatchLevel();

        IGetRegions[] GetIgnoreRegions();
        IGetRegions[] GetStrictRegions();
        IGetRegions[] GetLayoutRegions();
        IGetRegions[] GetContentRegions();
        IGetFloatingRegion[] GetFloatingRegions();
        IGetAccessibilityRegion[] GetAccessibilityRegions();

        bool? GetIgnoreCaret();
        string GetName();
        bool? GetSendDom();
        bool? GetUseDom();
        bool? GetEnablePatterns();
        bool? GetIgnoreDisplacements();
        bool GetReplaceLast();
        bool IsCheckWindow();

        IDictionary<string, string> GetScriptHooks();
        SizeMode GetSizeMode();
        VisualGridSelector GetTargetSelector();
        VisualGridOption[] GetVisualGridOptions();
        Dictionary<string, object> ToSerializableDictionary();
    }
}

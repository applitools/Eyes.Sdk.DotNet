using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Applitools
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BrowserType
    {
        [EnumMember(Value = "chrome")] CHROME,
        [EnumMember(Value = "chrome-1")] CHROME_ONE_VERSION_BACK,
        [EnumMember(Value = "chrome-2")] CHROME_TWO_VERSIONS_BACK,
        [EnumMember(Value = "firefox")] FIREFOX,
        [EnumMember(Value = "firefox-1")] FIREFOX_ONE_VERSION_BACK,
        [EnumMember(Value = "firefox-2")] FIREFOX_TWO_VERSIONS_BACK,
        [EnumMember(Value = "safari")] SAFARI,
        [EnumMember(Value = "safari-1")] SAFARI_ONE_VERSION_BACK,
        [EnumMember(Value = "safari-2")] SAFARI_TWO_VERSIONS_BACK,
        [EnumMember(Value = "ie10")] IE_10,
        [EnumMember(Value = "ie")] IE_11,
        [EnumMember(Value = "edge")] 
        [Obsolete("The 'EDGE' option that is being used in your browsers' configuration will soon be deprecated. Please change it to either \"EDGE_LEGACY\" for the legacy version or to \"EDGE_CHROMIUM\" for the new Chromium-based version.")]
        EDGE,
        [EnumMember(Value = "edgelegacy")] EDGE_LEGACY,
        [EnumMember(Value="edgechromium")] EDGE_CHROMIUM,
        [EnumMember(Value="edgechromium-1")] EDGE_CHROMIUM_ONE_VERSION_BACK,
    }
}

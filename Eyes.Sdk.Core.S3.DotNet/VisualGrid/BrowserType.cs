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
        [EnumMember(Value = "chrome-one-version-back")] CHROME_ONE_VERSION_BACK,
        [EnumMember(Value = "chrome-two-versions-back")] CHROME_TWO_VERSIONS_BACK,
        [EnumMember(Value = "firefox")] FIREFOX,
        [EnumMember(Value = "firefox-one-version-back")] FIREFOX_ONE_VERSION_BACK,
        [EnumMember(Value = "firefox-two-versions-back")] FIREFOX_TWO_VERSIONS_BACK,
        [EnumMember(Value = "safari")] SAFARI,
        [EnumMember(Value = "safari-one-version-back")] SAFARI_ONE_VERSION_BACK,
        [EnumMember(Value = "safari-two-versions-back")] SAFARI_TWO_VERSIONS_BACK,
        [EnumMember(Value = "safari-earlyaccess")] SAFARI_EARLY_ACCESS,
        [EnumMember(Value = "ie10")] IE_10,
        [EnumMember(Value = "ie11")] IE_11,
        [EnumMember(Value = "edge")]
        [Obsolete("The 'EDGE' option that is being used in your browsers' configuration will soon be deprecated. Please change it to either \"EDGE_LEGACY\" for the legacy version or to \"EDGE_CHROMIUM\" for the new Chromium-based version.")]
        EDGE,
        [EnumMember(Value = "edgelegacy")] EDGE_LEGACY,
        [EnumMember(Value = "edgechromium")] EDGE_CHROMIUM,
        [EnumMember(Value = "edgechromium-one-version-back")] EDGE_CHROMIUM_ONE_VERSION_BACK,
        [EnumMember(Value = "edgechromium-two-versions-back")] EDGE_CHROMIUM_TWO_VERSIONS_BACK,
    }
}

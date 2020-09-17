using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IosDeviceName
    {
        [EnumMember(Value = "iPhone 11 Pro")] iPhone_11_Pro,
        [EnumMember(Value = "iPhone 11 Pro Max")] iPhone_11_Pro_Max,
        [EnumMember(Value = "iPhone 11")] iPhone_11,
        [EnumMember(Value = "iPhone XR")] iPhone_XR,
        [EnumMember(Value = "iPhone Xs")] iPhone_XS,
        [EnumMember(Value = "iPhone X")] iPhone_X,
        [EnumMember(Value = "iPhone 8")] iPhone_8,
        [EnumMember(Value = "iPhone 7")] iPhone_7,
        [EnumMember(Value = "iPad Pro (12.9-inch) (3rd generation)")] iPad_Pro_3,
        [EnumMember(Value = "iPad (7th generation)")] iPad_7,
        [EnumMember(Value = "iPad Air (2nd generation)")] iPad_Air_2
    }
}
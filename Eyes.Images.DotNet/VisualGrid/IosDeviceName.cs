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
        [EnumMember(Value = "iPhone 8 Plus")] iPhone_8_Plus,
        [EnumMember(Value = "iPhone SE (1st generation)")] iPhone_SE,
        [EnumMember(Value = "iPhone 7")] iPhone_7,
        [EnumMember(Value = "iPad Pro (12.9-inch) (3rd generation)")] iPad_Pro_3,
        [EnumMember(Value = "iPad (7th generation)")] iPad_7,
        [EnumMember(Value = "iPad Air (2nd generation)")] iPad_Air_2,
        [EnumMember(Value = "iPhone 12 Pro Max")] iPhone_12_Pro_Max,
        [EnumMember(Value = "iPhone 12 Pro")] iPhone_12_Pro,
        [EnumMember(Value = "iPhone 12")] iPhone_12,
        [EnumMember(Value = "iPhone 12 mini")] iPhone_12_mini,
        [EnumMember(Value = "iPhone 14")] iPhone_14,
        [EnumMember(Value = "iPhone 14 Pro Max")] iPhone_14_Pro_Max,
    }
}
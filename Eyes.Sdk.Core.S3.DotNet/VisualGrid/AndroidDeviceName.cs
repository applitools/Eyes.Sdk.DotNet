using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AndroidDeviceName
    {
        [EnumMember(Value = "Pixel 3 XL")] Pixel_3_XL,
        [EnumMember(Value = "Pixel 4")] Pixel_4,    
        [EnumMember(Value = "Pixel 4 XL")] Pixel_4_XL,
        [EnumMember(Value = "Pixel 5")] Pixel_5,
        [EnumMember(Value = "Pixel 6")] Pixel_6,
        [EnumMember(Value = "Galaxy Note 9")] Galaxy_Note_9,
        [EnumMember(Value = "Galaxy Note 10")] Galaxy_Note_10,
        [EnumMember(Value = "Galaxy Note 10 Plus")] Galaxy_Note_10_Plus,
        [EnumMember(Value = "Galaxy Tab S7")] Galaxy_Tab_S7,
        [EnumMember(Value = "Galaxy Tab S8")] Galaxy_Tab_S8,
        [EnumMember(Value = "Galaxy S9")] Galaxy_S9,
        [EnumMember(Value = "Galaxy S9 Plus")] Galaxy_S9_Plus,
        [EnumMember(Value = "Galaxy S10")] Galaxy_S10,
        [EnumMember(Value = "Galaxy S10 Plus")] Galaxy_S10_Plus,
        [EnumMember(Value = "Galaxy S20")] Galaxy_S20,
        [EnumMember(Value = "Galaxy S20 Plus")] Galaxy_S20_PLUS,
        [EnumMember(Value = "Galaxy S21")] Galaxy_S21,
        [EnumMember(Value = "Galaxy S21 Plus")] Galaxy_S21_PLUS,
        [EnumMember(Value = "Galaxy S21 Ultra")] Galaxy_S21_ULTRA,
        [EnumMember(Value = "Galaxy S22")] Galaxy_S22,
        [EnumMember(Value = "Galaxy S22 Plus")] Galaxy_S22_Plus,
        [EnumMember(Value = "Xiaomi Redmi Note 11")] Xiaomi_Redmi_Note_11,
        [EnumMember(Value = "Xiaomi Redmi Note 11 Pro")] Xiaomi_Redmi_Note_11_Pro
    }
}
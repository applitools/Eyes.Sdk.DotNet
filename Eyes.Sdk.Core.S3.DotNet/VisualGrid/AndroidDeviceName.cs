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
        [EnumMember(Value = "Galaxy Note 8")] Galaxy_Note_8,
        [EnumMember(Value = "Galaxy Note 9")] Galaxy_Note_9,
        [EnumMember(Value = "Galaxy S8")] Galaxy_S8,
        [EnumMember(Value = "Galaxy S8 Plus")] Galaxy_S8_Plus,
        [EnumMember(Value = "Galaxy S9")] Galaxy_S9,
        [EnumMember(Value = "Galaxy S9 Plus")] Galaxy_S9_Plus,
        [EnumMember(Value = "Galaxy S10")] Galaxy_S10,
        [EnumMember(Value = "Galaxy S10 Plus")] Galaxy_S10_Plus,
        [EnumMember(Value = "Galaxy Note 10")] Galaxy_Note_10,
        [EnumMember(Value = "Galaxy Note 10 Plus")] Galaxy_Note_10_Plus,
        [EnumMember(Value = "Galaxy S20")] Galaxy_S20,
        [EnumMember(Value = "Galaxy S20 Plus")] Galaxy_S20_PLUS,
        [EnumMember(Value = "Galaxy S21")] Galaxy_S21,
        [EnumMember(Value = "Galaxy S21 Plus")] Galaxy_S21_PLUS,
        [EnumMember(Value = "Galaxy S21 Ultra")] Galaxy_S21_ULTRA
    }
}
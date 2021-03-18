using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceName
    {
        [EnumMember(Value = "iPhone 4")] iPhone_4,
        [EnumMember(Value = "iPhone 5/SE")] iPhone_5SE,
        [EnumMember(Value = "iPhone 6/7/8")] iPhone_6_7_8,
        [EnumMember(Value = "iPhone 6/7/8 Plus")] iPhone_6_7_8_Plus,
        [EnumMember(Value = "iPhone X")] iPhone_X,

        [EnumMember(Value = "iPad")] iPad,
        [EnumMember(Value = "iPad Mini")] iPad_Mini,
        [EnumMember(Value = "iPad Pro")] iPad_Pro,
        
        [EnumMember(Value = "Nexus 4")] Nexus_4,
        [EnumMember(Value = "Nexus 5")] Nexus_5,
        [EnumMember(Value = "Nexus 6")] Nexus_6,
        [EnumMember(Value = "Nexus 5X")] Nexus_5X,
        [EnumMember(Value = "Nexus 6P")] Nexus_6P,
        [EnumMember(Value = "Nexus 7")] Nexus_7,
        [EnumMember(Value = "Nexus 10")] Nexus_10,

        [EnumMember(Value = "Pixel 2")] Pixel_2,
        [EnumMember(Value = "Pixel 2 XL")] Pixel_2_XL,
        [EnumMember(Value = "Pixel 3")] Pixel_3,
        [EnumMember(Value = "Pixel 3 XL")] Pixel_3_XL,
        [EnumMember(Value = "Pixel 4")] Pixel_4,
        [EnumMember(Value = "Pixel 4 XL")] Pixel_4_XL,
        
        [EnumMember(Value = "Galaxy A5")] Galaxy_A5,

        [EnumMember(Value = "Galaxy S III")] Galaxy_S3,
        [EnumMember(Value = "Galaxy S5")] Galaxy_S5,
        [EnumMember(Value = "Galaxy S8")] Galaxy_S8,
        [EnumMember(Value = "Galaxy S8 Fixed")] Galaxy_S8_Fixed,
        [EnumMember(Value = "Galaxy S8 Plus")] Galaxy_S8_Plus,
        [EnumMember(Value = "Galaxy S9")] Galaxy_S9,
        [EnumMember(Value = "Galaxy S9 Plus")] Galaxy_S9_Plus,
        [EnumMember(Value = "Galaxy S10")] Galaxy_S10,

        [EnumMember(Value = "Galaxy Note II")] Galaxy_Note_2,
        [EnumMember(Value = "Galaxy Note 3")] Galaxy_Note_3,
        [EnumMember(Value = "Galaxy Note 4")] Galaxy_Note_4,
        [EnumMember(Value = "Galaxy Note 8")] Galaxy_Note_8,
        [EnumMember(Value = "Galaxy Note 9")] Galaxy_Note_9,
        [EnumMember(Value = "Galaxy Note 10")] Galaxy_Note_10,
        [EnumMember(Value = "Galaxy Note 10 Plus")] Galaxy_Note_10_Plus,

        [EnumMember(Value = "LG G6")] LG_G6,
        [EnumMember(Value = "LG Optimus L70")] LG_Optimus_L70,
        
        [EnumMember(Value = "Nokia N9")] Nokia_N9,
        [EnumMember(Value = "Nokia Lumia 520")] Nokia_Lumia_520,
        [EnumMember(Value = "Microsoft Lumia 550")] Microsoft_Lumia_550,
        [EnumMember(Value = "Microsoft Lumia 950")] Microsoft_Lumia_950,
        
        [EnumMember(Value = "Kindle Fire HDX")] Kindle_Fire_HDX,

        [EnumMember(Value = "BlackBerry Z30")] BlackBerry_Z30,
        [EnumMember(Value = "Blackberry PlayBook")] Blackberry_PlayBook,

        [EnumMember(Value = "OnePlus 7T")] OnePlus_7T,
        [EnumMember(Value = "OnePlus 7T Pro")] OnePlus_7T_Pro,
        [EnumMember(Value = "OnePlus 8")] OnePlus_8,
        [EnumMember(Value = "OnePlus 8 Pro")] OnePlus_8_Pro,

        [EnumMember(Value = "Laptop with touch")] Laptop_with_touch,
        [EnumMember(Value = "Laptop with HiDPI screen")] Laptop_with_HiDPI_screen,
        [EnumMember(Value = "Laptop with MDPI screen")] Laptop_with_MDPI_screen


        //TODO - add the following devices:
        //iphone xr
        //iphone xs
        //iphone xs max
        //iphone 11
        //iphone 11 pro
        //iphone 11 pro max
        //ipad 6th gen
        //ipad 7th gen
        //ipad air 2
        //galaxy s10 plus
        //galaxy s20
    }
}
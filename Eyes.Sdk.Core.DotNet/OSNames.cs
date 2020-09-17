namespace Applitools.Utils
{
    using System;

    /// <summary>
    /// Operating system names.
    /// </summary>
    public static class OSNames
    {
        #region Fields

        /// <summary>
        /// Unknown operating system.
        /// </summary>
        public const string Unknown = "Unknown";

        /// <summary>
        /// Linux operating system.
        /// </summary>
        public const string Linux = "Linux";

        /// <summary>
        /// Max OS X operating system.
        /// </summary>
        public const string MacOSX = "Mac OS X";

        /// <summary>
        /// Chrome OS operating system.
        /// </summary>
        public const string ChromeOS = "Chrome OS";

        /// <summary>
        /// iOS operating system.
        /// </summary>
        public const string IOS = "iOS";

        /// <summary>
        /// Windows operating system.
        /// </summary>
        public const string Windows = "Windows";

        /// <summary>
        /// Android operating system.
        /// </summary>
        public const string Android = "Android";

        /// <summary>
        /// Macintosh operating system.
        /// </summary>
        public const string Macintosh = "Macintosh";

        #endregion

        #region Methods

        /// <summary>
        /// Returns the operating system for the input <see cref="OperatingSystem"/>.
        /// </summary>
        public static string GetName(OperatingSystem operatingSystem)
        {
            ArgumentGuard.NotNull(operatingSystem, nameof(operatingSystem));

            switch (operatingSystem.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    return Windows;
                case PlatformID.MacOSX:
                    return Macintosh;
                default:
                    return Unknown;
            }
        }

        /// <summary>
        /// Returns the operating system name and its version.
        /// </summary>
        public static string GetNameAndVersion(OperatingSystem operatingSystem)
        {
            ArgumentGuard.NotNull(operatingSystem, nameof(operatingSystem));

            return "{0} {1}.{2}".Fmt(
                OSNames.GetName(operatingSystem), 
                operatingSystem.Version.Major, 
                operatingSystem.Version.Minor);
        }

        #endregion
    }
}

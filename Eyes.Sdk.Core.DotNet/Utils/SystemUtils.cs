namespace Applitools.Utils
{
#if NET45
    using System.IO;
    using System.Management;
#endif

    /// <summary>
    /// Operating system utilities.
    /// </summary>
    public static class SystemUtils
    {
        /// <summary>
        /// Returns the name of the process of the input id or <c>null</c> if unknown.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization",
            "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the process name to be in lowercase")]
        public static string GetProcessName(int processId, Logger logger = null)
        {
            string processName = null;

#if NET45
            // We use WMI to get to the process name so we can overcome 32/64 limitations.
            string wmiQuery = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;

            logger = logger ?? new Logger();

            logger.Verbose(wmiQuery);

            using (var searcher = new ManagementObjectSearcher(wmiQuery))
            using (var results = searcher.Get())
            {
                if (results != null)
                {
                    logger.Verbose("Results in set: " + results.Count);

                    foreach (ManagementObject entry in results)
                    {
                        if (entry != null)
                        {
                            string path = (string)entry.GetPropertyValue("ExecutablePath");
                            if (path == null)
                            {
                                logger.Verbose("ManagementObject from result set has no ExecutablePath property, or its value is null.");
                            }
                            else
                            {
                                string filename = Path.GetFileName(path);
                                logger.Verbose("Result set's ManagementObject's filename: " + filename);
                                processName = filename.ToLowerInvariant();
                                break;
                            }
                        }
                        else
                        {
                            logger.Verbose("ManagementObject from result set was null.");
                        }
                    }
                }
                else
                {
                    logger.Verbose("WMI query returned null result.");
                }
            }
#elif NETCOREAPP2_1 || NETCOREAPP3_0 || NETSTANDARD2_0
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processId);
            processName = process?.ProcessName;

#endif
            return processName;
        }
    }
}
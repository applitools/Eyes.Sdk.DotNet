namespace Applitools.Utils
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Standard <see cref="DateTime"/> formats.
    /// </summary>
    public enum StandardDateTimeFormat
    {
        /// <summary>
        /// An RFC1123 <see cref="DateTime"/> format that is commonly used to represent HTTP 
        /// timestamps (e.g., Tue, 22 Aug 2006 06:30:07 GMT)
        /// </summary>
        RFC1123,

        /// <summary>
        /// An RFC3339 <see cref="DateTime"/> format with second resolution used by various
        /// internet protocols (e.g., 2008-04-10T06:30:00-07:00).
        /// This format preserves the time zone information and therefore can be safely used
        /// to recreate a <see cref="DateTime"/> object.
        /// </summary>
        RFC3339,

        /// <summary>
        /// An ISO8601 <see cref="DateTime"/> format with tick (i.e., ten-millionth of a second)
        /// resolution (e.g., 2008-04-10T06:30:00.0000000-07:00).
        /// This format preserves the time zone information and therefore can be safely used
        /// to recreate a <see cref="DateTime"/> object.
        /// </summary>
        ISO8601,
    }
    
    /// <summary>
    /// <see cref="DateTime"/> and <see cref="DateTimeOffset"/> related utilities.
    /// </summary>
    public static class TimeUtils
    {
        #region Fields

        private const string RFC1123Format_ = "r";
        private const string RFC3339Format_ = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";
        private const string ISO8601Format_ = "o";

        #endregion

        #region Properties

        /// <summary>
        /// Returns the maximal time span acceptable by 
        /// <see cref="System.Threading.Thread.Sleep(TimeSpan)"/>.
        /// </summary>
        public static TimeSpan MaxSleepTime
        {
            get { return TimeSpan.FromSeconds(int.MaxValue - 1); }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Returns the maximal timespan of the input time spans.
        /// </summary>
        public static TimeSpan Max(TimeSpan span1, TimeSpan span2)
        {
            return span1 > span2 ? span1 : span2;
        }

        /// <summary>
        /// Returns the minimal timespan of the input timespans.
        /// </summary>
        public static TimeSpan Min(TimeSpan span1, TimeSpan span2)
        {
            return span1 < span2 ? span1 : span2;
        }

        /// <summary>
        /// Returns the time left to expiry based on the input start time (in the past) 
        /// and the input expiry time span.
        /// </summary>
        public static TimeSpan TimeToExpiry(DateTime start, TimeSpan expiry)
        {
            return SubtractNonnegative(start.Add(expiry), DateTime.Now);
        }

        /// <summary>
        /// Returns the time span obtained by subtracting
        /// <c>dt2</c> from <c>dt1</c> unless the result is negative in which case
        /// <c>TimeSpan.Zero</c> is returned.
        /// </summary>
        public static TimeSpan SubtractNonnegative(DateTime time1, DateTime time2)
        {
            if (time1 < time2)
            {
                return TimeSpan.Zero;
            }

            return time1.Subtract(time2);
        }

        /// <summary>
        /// Returns a string representation of the input date time offset of the specified 
        /// format.
        /// </summary>
        public static string ToString(this DateTimeOffset dateTime, StandardDateTimeFormat format)
        {
            if (format == StandardDateTimeFormat.RFC1123 && dateTime.Offset != TimeSpan.Zero)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            return dateTime.ToString(GetFormat_(format), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of the input date time of the specified 
        /// format.
        /// </summary>
        public static string ToString(this DateTime dateTime, StandardDateTimeFormat format)
        {
            if (format == StandardDateTimeFormat.RFC1123 && dateTime.Kind != DateTimeKind.Utc)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            return dateTime.ToString(GetFormat_(format), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses the input date time offset string of the specified format.
        /// </summary>
        public static DateTimeOffset ParseDateTimeOffset(
            string dateTime, StandardDateTimeFormat format)
        {
            return DateTimeOffset.ParseExact(
                dateTime, 
                GetFormat_(format),
                CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// Parses the input date time string of the specified format.
        /// </summary>
        public static DateTime ParseDateTime(
            string dateTime, 
            StandardDateTimeFormat format,
            DateTimeKind kind = DateTimeKind.Unspecified)
        {
            var parsed = DateTime.ParseExact(
                dateTime,
                GetFormat_(format),
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);

            if (parsed.Kind == DateTimeKind.Unspecified)
            {
                parsed = DateTime.SpecifyKind(parsed, kind);
            }

            return parsed;
        }
        
        /// <summary>
        /// Returns a copy of the input date time offset.
        /// </summary>
        /// <param name="source">Object to copy</param>
        /// <param name="millisecond">Whether to copy the millisecond component or use 0</param>
        /// <param name="second">Whether to copy the second component or use 0</param>
        /// <param name="minute">Whether to copy the minute component or use 0</param>
        /// <param name="hour">Whether to copy the hour component or use 0</param>
        /// <param name="day">Whether to copy the day or use 1</param>
        /// <param name="month">Whether to copy the month or use 1</param>
        /// <param name="year">Whether to copy the year or use 1</param>
        /// <param name="offset">Whether to copy the offset or use 0</param>
        public static DateTimeOffset Copy(
            DateTimeOffset source, 
            bool millisecond = true,
            bool second = true, 
            bool minute = true, 
            bool hour = true, 
            bool day = true,
            bool month = true, 
            bool year = true, 
            bool offset = true)
        {
            return new DateTimeOffset(
                year ? source.Year : 1, 
                month ? source.Month : 1,
                day ? source.Day : 1, 
                hour ? source.Hour : 0, 
                minute ? source.Minute : 0,
                second ? source.Second : 0, 
                millisecond ? source.Millisecond : 0,
                offset ? source.Offset : TimeSpan.Zero);
        }

        /// <summary>
        /// Returns a copy of the input date time.
        /// </summary>
        /// <param name="dateTime">Object to copy</param>
        /// <param name="millisecond">Whether to copy the millisecond component or use 0</param>
        /// <param name="second">Whether to copy the second component or use 0</param>
        /// <param name="minute">Whether to copy the minute component or use 0</param>
        /// <param name="hour">Whether to copy the hour component or use 0</param>
        /// <param name="day">Whether to copy the day or use 1</param>
        /// <param name="month">Whether to copy the month or use 1</param>
        /// <param name="year">Whether to copy the year or use 1</param>
        public static DateTime Copy(
            DateTime dateTime, 
            bool millisecond = true,
            bool second = true, 
            bool minute = true, 
            bool hour = true, 
            bool day = true,
            bool month = true,
            bool year = true)
        {
            return new DateTime(
                year ? dateTime.Year : 1, 
                month ? dateTime.Month : 1,
                day ? dateTime.Day : 1, 
                hour ? dateTime.Hour : 0, 
                minute ? dateTime.Minute : 0,
                second ? dateTime.Second : 0, 
                millisecond ? dateTime.Millisecond : 0, 
                dateTime.Kind);
        }
        
        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/> representation of the specified unix timestamp 
        /// at the input offset from UTC.
        /// </summary>
        public static DateTimeOffset FromUnixTime(int unixTime, TimeSpan offset)
        {
            var dt = new DateTimeOffset(1970, 1, 1, 0, 0, 0, offset);
            return dt.AddSeconds(unixTime);
        }

        /// <summary>
        /// Gets the unix time representation at the input offset from UTC of the specified date.
        /// </summary>
        public static int ToUnixTime(this DateTimeOffset dateTime, TimeSpan offset)
        {
            var start = new DateTimeOffset(1970, 1, 1, 0, 0, 0, offset);
            return (int)dateTime.Subtract(start).TotalSeconds;
        }

        /// <summary>
        /// Gets the unix time in milliseconds representation at the input 
        /// offset from UTC of the specified date.
        /// </summary>
        public static long ToUnixTimeMS(this DateTimeOffset dateTime, TimeSpan offset)
        {
            var start = new DateTimeOffset(1970, 1, 1, 0, 0, 0, offset);
            return (long)dateTime.Subtract(start).TotalMilliseconds;
        }

        #endregion

        #region Private

        private static string GetFormat_(StandardDateTimeFormat format)
        {
            if (format == StandardDateTimeFormat.RFC1123)
            {
                return RFC1123Format_;
            }

            if (format == StandardDateTimeFormat.RFC3339)
            {
                return RFC3339Format_;
            }

            if (format == StandardDateTimeFormat.ISO8601)
            {
                return ISO8601Format_;
            }

            throw new NotImplementedException("Unknown format: {0}".Fmt(format));
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Applitools.Utils
{
    /// <summary>
    /// String utilities
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Concatenates the string representations of the input objects delimited by the 
        /// input delimiter.
        /// </summary>
        public static string Concat(
            this IEnumerable objects,
            string delimiter,
            bool isSuffix = false)
        {
            ArgumentGuard.NotNull(objects, nameof(objects));

            IEnumerator enumerator = objects.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(100);
            if (isSuffix)
            {
                sb.Append(delimiter);
            }

            const string NULL = "<null>";
            sb.Append(enumerator.Current ?? NULL);
            while (enumerator.MoveNext())
            {
                sb.Append(delimiter);
                sb.Append(enumerator.Current ?? NULL);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Replaces one or more formats item in this string with the string representation
        /// of a corresponding object in the input array 
        /// (using an <see cref="CultureInfo.InvariantCulture"/>).
        /// </summary>
        public static string Fmt(this string str, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, str, args);
        }

        /// <summary>
        /// Replaces one or more formats item in this string with the string representation
        /// of the input object.
        /// </summary>
        public static string Fmt(this string str, object arg0)
        {
            return string.Format(CultureInfo.InvariantCulture, str, arg0);
        }

        /// <summary>
        /// Determines whether the beginning of this string matches the specified string when
        /// compared using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Performance")]
        public static bool StartsWithOrdinal(this string str, string value)
        {
            return str.StartsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the end of this string matches the specified string when
        /// compared using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Performance")]
        public static bool EndsWithOrdinal(this string str, string value)
        {
            return str.EndsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the <c>InvariantCulture</c> integer represented by this string.
        /// </summary>
        public static int ToInt32(this string value)
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static string EfficientStringReplace(string refIdOpenToken, string refIdCloseToken, string input, IDictionary<string, string> replacements)
        {
            int resultBufferSize = input.Length;
            foreach (string rep in replacements.Values)
            {
                resultBufferSize += rep.Length;
            }
            StringBuilder result = new StringBuilder(resultBufferSize);
            StringBuilder refId = new StringBuilder();
            int inLen = input.Length;
            int refOpenTokenLen = refIdOpenToken.Length;
            int refCloseTokenLen = refIdCloseToken.Length;
            for (int i = 0; i < inLen; ++i)
            {
            start:
                bool refTokenEncountered = input[i] == refIdOpenToken[0];
                if (refTokenEncountered)
                {
                    for (int j = 1; j < refOpenTokenLen; ++j)
                    {
                        if (input[i + j] != refIdOpenToken[j])
                        {
                            refTokenEncountered = false;
                            break;
                        }
                    }
                    if (refTokenEncountered)
                    {
                        refId.Length = 0;
                        for (i += refOpenTokenLen; i < inLen; ++i)
                        {
                            bool refEndTokenEncountered = input[i] == refIdCloseToken[0];
                            if (refEndTokenEncountered)
                            {
                                for (int j = 1; j < refCloseTokenLen; ++j)
                                {
                                    if (input[i + j] != refIdCloseToken[j])
                                    {
                                        refEndTokenEncountered = false;
                                        break;
                                    }
                                }
                                if (refEndTokenEncountered)
                                {
                                    if (replacements.TryGetValue(refId.ToString(), out string rep))
                                    {
                                        result.Append(rep);
                                        i += refCloseTokenLen;
                                        goto start;
                                    }
                                }
                            }
                            if (!refEndTokenEncountered)
                            {
                                refId.Append(input[i]);
                            }
                        }

                    }
                }

                if (!refTokenEncountered)
                {
                    result.Append(input[i]);
                }
            }
            return result.ToString();
        }

        public static string CleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return string.Empty;
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                    case '/':
                        sb.Append('\\').Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + string.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public static bool StringEqualsIgnoreWhiteSpace(string strx, string stry)
        {
            if (strx == null) //stry may contain only whitespace
                return string.IsNullOrWhiteSpace(stry);

            else if (stry == null) //strx may contain only whitespace
                return string.IsNullOrWhiteSpace(strx);

            int ix = 0, iy = 0;
            for (; ix < strx.Length && iy < stry.Length; ix++, iy++)
            {
                char chx = strx[ix];
                char chy = stry[iy];

                //ignore whitespace in strx
                while (char.IsWhiteSpace(chx) && ix < strx.Length - 1)
                {
                    ix++;
                    chx = strx[ix];
                }

                //ignore whitespace in stry
                while (char.IsWhiteSpace(chy) && iy < stry.Length - 1)
                {
                    iy++;
                    chy = stry[iy];
                }

                if (ix == strx.Length && iy != stry.Length)
                { //end of strx, so check if the rest of stry is whitespace
                    for (int iiy = iy + 1; iiy < stry.Length; iiy++)
                    {
                        if (!char.IsWhiteSpace(stry[iiy]))
                            return false;
                    }
                    return true;
                }

                if (ix != strx.Length && iy == stry.Length)
                { //end of stry, so check if the rest of strx is whitespace
                    for (int iix = ix + 1; iix < strx.Length; iix++)
                    {
                        if (!char.IsWhiteSpace(strx[iix]))
                            return false;
                    }
                    return true;
                }

                ////The current chars are not whitespace, so check that they're equal (case-insensitive)
                ////Remove the following two lines to make the comparison case-sensitive.
                //chx = char.ToLowerInvariant(chx);
                //chy = char.ToLowerInvariant(chy);

                if (chx != chy)
                    return false;
            }

            //If strx has more chars than stry
            for (; ix < strx.Length; ix++)
            {
                if (!char.IsWhiteSpace(strx[ix]))
                    return false;
            }

            //If stry has more chars than strx
            for (; iy < stry.Length; iy++)
            {
                if (!char.IsWhiteSpace(stry[iy]))
                    return false;
            }

            return true;
        }

        public static string ToPascalCase(string str)
        {
            ArgumentGuard.NotEmpty(str, nameof(str));
            return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
        }

        /// <summary>
        /// Split string by maximum length.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="n">The maximum number of characters in line. All lines will have that number of characters except (perhaps) the last.</param>
        /// <returns>An enumerable of strings, each represnting a line which is part of the original string.</returns>
        public static IEnumerable<string> Split(this string str, int n)
        {
            ArgumentGuard.NotEmpty(str, nameof(str));
            ArgumentGuard.GreaterOrEqual(n, 1, nameof(n));

            List<string> result = new List<string>();
            int i = 0;
            while (i < str.Length - n)
            {
                result.Add(str.Substring(i, n));
                i += n;
            }
            if (i < str.Length)
            {
                result.Add(str.Substring(i));
            }
            return result.ToArray();
        }

        #region Legacy

        /// <summary>
        /// A legacy replacement for .NET 4.0 <c>string.IsNullOrWhiteSpace</c>
        /// </summary>
        public static bool IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        #endregion
    }
}

namespace Applitools.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utilities for validating method arguments.
    /// </summary>
    public static class ArgumentGuard
    {
        #region Methods

        #region Objects

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the input parameter is <c>null</c>.
        /// </summary>
        public static T NotNull<T>([ValidatedNotNull]T param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, $"'{paramName}' is null");
            }

            return param;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter is not <c>null</c>.
        /// </summary>
        public static void Null<T>([ValidatedNotNull]T param, string paramName)
            where T : class
        {
            if (param != null)
            {
                throw new ArgumentException($"'{paramName}' != null", paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the input parameter is
        /// <c>null</c> and otherwise evaluates the input <c>guard</c> on each of its elements.
        /// </summary>
        public static void ForEach<T>(
            [ValidatedNotNull]IEnumerable<T> param,
            string paramName,
            [ValidatedNotNull]Action<T, string> guard)
        {
            ArgumentGuard.NotNull(param, nameof(param));
            ArgumentGuard.NotNull(guard, nameof(guard));
            int i = 0;
            foreach (var val in param)
            {
                guard(val, "{0}[{1}]".Fmt(paramName, i++));
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the input parameter is false.
        /// </summary>
        public static void IsValidState(bool param, string errMsg)
        {
            if (!param)
            {
                throw new InvalidOperationException(errMsg);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter is not of
        /// the correct type.
        /// </summary>
        public static void NotOfType(object param, Type type, string paramName)
        {
            if (!type.IsAssignableFrom(param.GetType()))
            {
                throw new ArgumentException($"{paramName} is not of type {type.Name}", paramName);
            }
        }

        #endregion

        #region Equality & Comparison

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameters are not equal.
        /// </summary>
        public static void Equals(object param1, object param2, string message)
        {
            if (object.Equals(param1, param2))
            {
                return;
            }

            throw new ArgumentException(message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameters are not equal.
        /// </summary>
        public static void Equals(
            object param1,
            object param2,
            string messageFormat,
            params object[] args)
        {
            if (object.Equals(param1, param2))
            {
                return;
            }

            throw new ArgumentException(messageFormat.Fmt(args));
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter is equal to the
        /// input value.
        /// </summary>
        public static void NotEquals(object param, object value, string paramName)
        {
            if (object.Equals(param, value))
            {
                throw new ArgumentException("'{0}' is '{1}'".Fmt(paramName, value), paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the input parameter is greater
        /// than the input bound.
        /// </summary>
        public static void LowerOrEqual<T>(T param, T bound, string paramName) where T
            : IComparable
        {
            if (param.CompareTo(bound) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, "'{0}' is greater than {1}".Fmt(paramName, bound));
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the input parameter is lower
        /// than the input bound.
        /// </summary>
        public static void GreaterOrEqual<T>(T param, T bound, string paramName)
            where T : IComparable
        {
            if (param.CompareTo(bound) < 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, "'{0}' is lower than {1}".Fmt(paramName, bound));
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the input parameter is lower 
        /// than or equal to the input bound.
        /// </summary>
        public static void GreaterThan<T>(T param, T bound, string paramName)
            where T : IComparable
        {
            if (param.CompareTo(bound) <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, "'{0}' is not greater than {1}".Fmt(paramName, bound));
            }
        }


        #endregion

        #region Strings

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter is empty.
        /// </summary>
        public static void NotEmpty([ValidatedNotNull]string param, string paramName)
        {
            ArgumentGuard.NotNull(param, paramName);

            if (param.Length == 0)
            {
                throw new ArgumentException("'{0}' is empty".Fmt(paramName), paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter consists
        /// only of whitespace characters.
        /// </summary>
        public static void NotWhiteSpace(string param, string paramName)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                ArgumentGuard.NotNull(param, paramName);
                throw new ArgumentException("'{0}' is whitespace".Fmt(paramName), paramName);
            }
        }

        /// <summary>
        /// Trims the input parameter and throws an <see cref="ArgumentNullException"/>
        /// if it is <c>null</c>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1045:DoNotPassTypesByReference", MessageId = "0#",
            Justification = "Required in this case")]
        public static void TrimNotNull([ValidatedNotNull]ref string param, string paramName)
        {
            ArgumentGuard.NotNull(param, paramName);
            param = param.Trim();
        }

        /// <summary>
        /// Trims the input parameter and throws an <see cref="ArgumentNullException"/>
        /// or an <see cref="ArgumentException"/> if it is null or empty (following the trim),
        /// respectively.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1045:DoNotPassTypesByReference", MessageId = "0#",
            Justification = "Required in this case")]
        public static void TrimNotEmpty([ValidatedNotNull]ref string param, string paramName)
        {
            ArgumentGuard.NotNull(param, paramName);
            param = param.Trim();
            ArgumentGuard.NotEmpty(param, paramName);
        }

        /// <summary>
        /// If the input parameter is not <c>null</c>, trims it and throws an
        /// <see cref="ArgumentException"/> if it is empty following the trim.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1045:DoNotPassTypesByReference", MessageId = "0#",
            Justification = "Required in this case")]
        public static void NullOrTrimNotEmpty(ref string param, string paramName)
        {
            if (param == null)
            {
                return;
            }

            param = param.Trim();
            ArgumentGuard.NotEmpty(param, paramName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter is not <c>null</c>
        /// and empty.
        /// </summary>
        public static void NullOrNotEmpty(string param, string paramName)
        {
            if (param != null)
            {
                NotEmpty(param, paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input parameter does not end with
        /// the input suffix.
        /// </summary>
        public static void EndsWith(string param, string suffix, string paramName)
        {
            NotNull(param, paramName);
            if (!param.EndsWithOrdinal(suffix))
            {
                throw new ArgumentException("Does not end with '{0}'".Fmt(suffix), paramName);
            }
        }

        #endregion

        #endregion
    }
}

namespace Applitools.Utils.Config
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Allows reading <see cref="IConfigurable{TValue}"/> items by name.
    /// </summary>
    public interface IReadAccessor<TValue> : IDisposable
    {
        #region Methods

        /// <summary>
        /// Outputs the value of the configuration item of the input name.
        /// </summary>
        /// <param name="client">The object performing the read</param>
        /// <param name="name">The name of the item to read</param>
        /// <param name="value">The items value</param>
        /// <returns>
        /// <c>false</c> if and only if an item of the input name does not exist.
        /// </returns>
        bool TryGetValue(object client, string name, out TValue value);

        #endregion
    }
}

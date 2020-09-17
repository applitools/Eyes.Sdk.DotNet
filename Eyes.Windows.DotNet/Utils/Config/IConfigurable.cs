namespace Applitools.Utils.Config
{
    using System;

    /// <summary>
    /// A configurable object.
    /// </summary>
    public interface IConfigurable<TValue>
    {
        #region Methods

        /// <summary>
        /// Configures this objects given the input configuration reader.
        /// </summary>
        void Configure(IReadAccessor<TValue> reader);

        #endregion
    }
}

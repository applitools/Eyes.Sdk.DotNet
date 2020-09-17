namespace Applitools.Utils
{
    using System;
    using System.Threading;

    /// <summary>
    /// A legacy replacement for .NET 4.0's <c>ThreadLocal&lt;T&gt;</c> class.
    /// </summary>
    public sealed class ThreadLocal<T> : IDisposable
    {
        #region Fields

        private readonly Func<ValueWrapper_> init_;
        private readonly LocalDataStoreSlot dataSlot_ = Thread.AllocateDataSlot();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ThreadLocal&lt;T&gt;"/> instance.
        /// </summary>
        public ThreadLocal(Func<T> valueFactory)
        {
            ArgumentGuard.NotNull(valueFactory, nameof(valueFactory));

            init_ = () => new ValueWrapper_() { Value = valueFactory() };
        }

        /// <summary>
        /// Creates a new <see cref="ThreadLocal&lt;T&gt;"/> instance.
        /// </summary>
        public ThreadLocal()
            : this(() => default(T))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of this instance for the current thread.
        /// </summary>
        public T Value
        {
            get
            {
                var result = (ValueWrapper_)Thread.GetData(dataSlot_);
                if (result == null)
                {
                    result = init_();
                    Thread.SetData(dataSlot_, result);
                }

                return result.Value;
            }

            set
            {
                Thread.SetData(dataSlot_, new ValueWrapper_() { Value = value });
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            Value = default(T);
        }

        #endregion

        #region Classes

        private class ValueWrapper_
        {
            public T Value { get; set; }
        }

        #endregion
    }
}

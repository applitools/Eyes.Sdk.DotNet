namespace Applitools.Utils
{
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Formats method and property invocation messages.
    /// </summary>
    public abstract class TracerMessageFormatter
    {
        #region Fields

        private const string AccessorError_ = "Accessor name already specified";
        private const string Delimiter_ = ", ";
        private readonly StringBuilder sb_;
        private string delimiter_;
        private readonly string suffix_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="TracerMessageFormatter"/> instance.
        /// </summary>
        /// <param name="prefix">Prefixes the formatted message</param>
        /// <param name="suffix">Suffixes the formatted message</param>
        protected TracerMessageFormatter(string prefix, string suffix = "")
        {
            sb_ = new StringBuilder(100);
            sb_.Append(prefix);
            delimiter_ = string.Empty;
            suffix_ = suffix;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="TracerMessageFormatter"/> object to its underlying 
        /// trace string.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Performance")]
        public static implicit operator string(TracerMessageFormatter formatter)
        {
            return formatter.ToString();
        }

        /// <summary>
        /// Format the input argument list
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Performance")]
        public TracerMessageFormatter WithArgs(params object[] args)
        {
            if (args.Length == 0)
            {
                return this;
            }

            return Args_(StringUtils.Concat(args, Delimiter_));
        }

        /// <summary>
        /// Format the input argument
        /// </summary>
        public TracerMessageFormatter WithArg(IEnumerable arg)
        {
            if (arg == null)
            {
                return WithArgs(arg);
            }

            return Args_("[{0}]".Fmt(StringUtils.Concat(arg, Delimiter_)));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            sb_.Append(suffix_);
            return sb_.ToString();
        }

        private TracerMessageFormatter Args_(string args)
        {
            sb_.AppendFormat("{0}{1}", delimiter_, args);
            delimiter_ = Delimiter_;
            return this;
        }

        #endregion
    }
}

namespace Applitools.Utils
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Web;

    [Serializable]
    public class Url : Uri, ISerializable
    {
        private const char EQ_ = '=';
        private const char AND_ = '&';
        private const char Q_ = '?';
        private const char SLASH_ = '/';

        public Url(string baseUrl)
            : this(new Uri(baseUrl))
        {
        }

        public Url(Uri baseUri)
            : base(ArgumentGuard.NotNull(baseUri, nameof(baseUri)).ToString())
        {
        }

        protected Url(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Url QueryElement(string param, string value)
        {
            ArgumentGuard.NotEmpty(param, nameof(param));
            ArgumentGuard.NotNull(value, nameof(value));

            UriBuilder builder = new UriBuilder(this);
            string query = builder.Query;
            query = query.StartsWithOrdinal(Q_.ToString()) ? query.Substring(1) : query;
            builder.Query = AppendQuery_(query, param, value);
            return new Url(builder.Uri);
        }

        public Url SubpathElement(string subpath)
        {
            ArgumentGuard.NotEmpty(subpath, nameof(subpath));

            UriBuilder builder = new UriBuilder(this);
            string path = builder.Path;
            path = path.StartsWithOrdinal(SLASH_.ToString()) ?
                path.Substring(1) : path;
            builder.Path = AppendSubpath_(builder.Path, subpath);
            return new Url(builder.Uri);
        }

        private static string AppendQuery_(string query, string param, string value)
        {
            StringBuilder queryBuilder = new StringBuilder(query);
            if (!string.IsNullOrEmpty(query))
            {
                queryBuilder.Append(AND_);
            }

            queryBuilder.Append(HttpUtility.UrlPathEncode(param));
            queryBuilder.Append(EQ_);
            queryBuilder.Append(HttpUtility.UrlPathEncode(value));
            return queryBuilder.ToString();
        }

        private static string AppendSubpath_(string path, string subpath)
        {
            subpath = subpath.TrimStart(SLASH_);
            StringBuilder pathBuilder = new StringBuilder(path.TrimEnd(SLASH_));
            pathBuilder.Append(SLASH_);
            pathBuilder.Append(subpath.TrimStart(SLASH_));
            return pathBuilder.ToString();
        }
    }
}

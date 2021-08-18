using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Applitools.Utils
{
    /// <summary>
    /// Json formatting utilities.
    /// </summary>
    public static class JsonUtils
    {
        #region Fields

        private static ThreadLocal<JsonSerializer> serializer_ =
            new ThreadLocal<JsonSerializer>(() => CreateSerializer(false, false));

        #endregion

        #region Properties

        /// <summary>
        /// The default (thread-safe) serializer (adds <c>$id</c> and <c>$type</c> attributes).
        /// </summary>
        public static JsonSerializer Serializer
        {
            get
            {
                return serializer_.Value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates, configures and returns a <see cref="JsonSerializer"/> instance.
        /// </summary>
        /// <param name="ids">Whether to mark each object with an <c>$id</c> attribute</param>
        /// <param name="types">Whether to mark objects with a <c>$type</c> attribute</param>
        /// <param name="nulls">Whether to include properties with <c>null</c> values</param>
        /// <param name="indent">Whether to format the output json</param>
        public static JsonSerializer CreateSerializer(
            bool ids = false, bool types = false, bool nulls = false, bool indent = false)
        {
            JsonSerializerSettings settings = CreateSerializerSettings(ids, types, nulls, indent);

            return JsonSerializer.Create(settings);
        }

        public static JsonSerializerSettings CreateSerializerSettings(
            bool ids = false, bool types = false, bool nulls = false, bool indent = false)
        {
            var settings = new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                Formatting = indent ? Formatting.Indented : Formatting.None,
                NullValueHandling = nulls ? NullValueHandling.Include : NullValueHandling.Ignore,
                PreserveReferencesHandling = ids ? PreserveReferencesHandling.Objects : PreserveReferencesHandling.None,
            };

            if (types)
            {
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            }
            else
            {
                settings.TypeNameHandling = TypeNameHandling.None;
            }

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new UriConverter());
            settings.ContractResolver = new PrivateSetterPropertiesContractResolver_();
            return settings;
        }

        /// <summary>
        /// Serializes the input object using the input serializer to the input stream.
        /// </summary>
        public static void Serialize(this JsonSerializer serializer, object value, Stream stream)
        {
            ArgumentGuard.NotNull(serializer, nameof(serializer));
            ArgumentGuard.NotNull(stream, nameof(stream));

            var sw = new StreamWriter(stream);
            serializer.Serialize(sw, value);
            sw.Flush();
        }

        /// <summary>
        /// Serializes the input object using the input serializer and returns its string 
        /// representation.
        /// </summary>
        public static string Serialize(this JsonSerializer serializer, object value)
        {
            ArgumentGuard.NotNull(serializer, nameof(serializer));

            using (var s = new MemoryStream())
            {
                var sr = new StreamReader(s);
                serializer.Serialize(value, s);
                s.Position = 0;
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Serializes the input object and returns its string representation.
        /// </summary>
        public static string Serialize(object value)
        {
            return Serialize(Serializer, value);
        }

        /// <summary>
        /// Deserializes an object of the input type from the input json string.
        /// </summary>
        public static object Deserialize(this JsonSerializer serializer, Type type, string json)
        {
            ArgumentGuard.NotNull(serializer, nameof(serializer));

            using (var sr = new StringReader(json))
            {
                return serializer.Deserialize(sr, type);
            }
        }

        /// <summary>
        /// Deserializes an object of the specified type from the input json string.
        /// </summary>
        public static T Deserialize<T>(this JsonSerializer serializer, string json)
        {
            return (T)Deserialize(serializer, typeof(T), json);
        }

        #endregion

        #region Classes

        private sealed class PrivateSetterPropertiesContractResolver_ : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    if (member is PropertyInfo property)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }

        #endregion
    }
}

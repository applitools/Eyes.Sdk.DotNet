namespace Applitools.Utils
{
    using Newtonsoft.Json;

    /// <summary>
    /// An object with a unique identifier.
    /// </summary>
    public interface IIdentifiable<T>
    {
        /// <summary>
        /// Gets or sets the id of this object.
        /// </summary>
        [JsonProperty]
        T Id { get; set; }
    }
}

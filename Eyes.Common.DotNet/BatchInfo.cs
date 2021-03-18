using System;
using System.Runtime.InteropServices;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    /// <summary>
    /// A batch of tests.
    /// </summary>
    [ComVisible(true)]
    public class BatchInfo
    {
        /// <summary>
        /// Creates a new <see cref="BatchInfo"/> instance.
        /// </summary>
        /// <param name="name">Name of batch or <c>null</c> if anonymous.</param>
        /// <param name="startedAt">Batch start time</param>
        public BatchInfo(string name, DateTimeOffset startedAt)
        {
            Id = CommonUtils.GetEnvVar("APPLITOOLS_BATCH_ID") ?? Guid.NewGuid().ToString();
            Name = name ?? CommonUtils.GetEnvVar("APPLITOOLS_BATCH_NAME");
            SequenceName = CommonUtils.GetEnvVar("APPLITOOLS_BATCH_SEQUENCE");
            NotifyOnCompletion = "true".Equals(CommonUtils.GetEnvVar("APPLITOOLS_BATCH_NOTIFY"), StringComparison.OrdinalIgnoreCase);
            StartedAt = startedAt;
        }

        /// <summary>
        /// Creates a new <see cref="BatchInfo"/> instance.
        /// </summary>
        /// <param name="name">Name of batch or <c>null</c> if anonymous.</param>
        public BatchInfo(string name)
            : this(name, TimeZoneInfo.ConvertTimeToUtc(DateTime.Now))
        {
        }

        /// <summary>
        /// Creates a new <see cref="BatchInfo"/> instance.
        /// </summary>
        public BatchInfo()
            : this(null)
        {
        }

        /// <summary>
        /// The id of the batch.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the name_ of the batch_ or <c>null</c> if anonymous.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the batch start date and time.
        /// </summary>
        public DateTimeOffset StartedAt { get; set; }

        [JsonProperty("BatchSequenceName")]
        public string SequenceName { get; set; }

        public bool NotifyOnCompletion { get; set; }

        [JsonProperty("IsCompleted")]
        public bool IsCompletedSetter { set => IsCompleted = value; }

        [JsonIgnore]
        public bool IsCompleted { get; set; }

        public PropertiesCollection Properties { get; } = new PropertiesCollection();

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} [{Id}] <{SequenceName}> - {TimeUtils.ToString(StartedAt, StandardDateTimeFormat.RFC3339)}";
        }
    }
}

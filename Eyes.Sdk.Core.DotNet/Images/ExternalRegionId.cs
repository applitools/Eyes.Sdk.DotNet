namespace Applitools.Utils.Images
{
    /// <summary>
    /// The identfier of a region in an external system.
    /// </summary>
    public class ExternalRegionId
    {
        #region Properties

        /// <summary>
        /// Identifies the external system.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The id of the region in the <see cref="Target"/> system.
        /// </summary>
        public string Id { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns <c>true</c> if the input objects are equivalent.
        /// </summary>
        public static bool AreEqual(ExternalRegionId id1, ExternalRegionId id2)
        {
            if (id1 == id2)
            {
                return true;
            }

            if (id1 == null || id2 == null)
            {
                return false;
            }

            return id1.Id == id2.Id && id1.Target == id2.Target;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0}:{1}".Fmt(Target, Id);
        }

        #endregion
    }
}

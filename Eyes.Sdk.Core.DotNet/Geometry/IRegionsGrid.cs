namespace Applitools.Utils.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// A grid of regions.
    /// </summary>
    public interface IRegionsGrid
    {
        #region Properties

        /// <summary>
        /// Gets the size of this grid.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Gets all regions of this grid.
        /// </summary>
        IEnumerable<IRegion> Regions { get; }

        /// <summary>
        /// Gets the number of this grid.
        /// </summary>
        int Count { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the region at the input point or <c>null</c> if no such region exists.
        /// </summary>
        IRegion GetRegionAtPoint(int x, int y, int proximity = 0);

        /// <summary>
        /// Adds all regions contained in the input rectangle to the input <c>result</c> list.
        /// </summary>
        void GetContainedRegions(
            Rectangle rectangle, 
            ICollection<IRegion> result, 
            Predicate<IRegion> where = null);

        /// <summary>
        /// Returns a list of regions intersecting the input rectangle.
        /// </summary>
        ICollection<IRegion> GetIntersectingRegions(
            Rectangle rectangle,
            Predicate<IRegion> where = null);

        /// <summary>
        /// Returns <c>true</c> if the input rectangle intersects with a region of this grid
        /// that satisfies the input predicate.
        /// </summary>
        bool IsIntersecting(Rectangle rectangle, Predicate<IRegion> where = null);

        /// <summary>
        /// Returns all region containing the input rectangle sorted by their area
        /// from small to large.
        /// </summary>
        IList<IRegion> GetContainingRegions(
            Rectangle rectangle, Predicate<IRegion> where  = null);

        /// <summary>
        /// Returns <c>true</c> if and only if the input region contains at least one 
        /// other region that meets the input predicate.
        /// </summary>
        bool IsContainer(IRegion region, Predicate<IRegion> where = null);

        /// <summary>
        /// Returns <c>true</c> if and only if the input rectangle is contained by a region
        /// of this grid.
        /// </summary>
        bool IsContained(Rectangle rectangle, Predicate<IRegion> where = null);

        /// <summary>
        /// Returns the region of the input rectangle or <c>null</c> if no such region is 
        /// contained in this grid.
        /// </summary>
        IRegion GetRegion(Rectangle rectangle);

        /// <summary>
        /// Returns <c>true</c> if and only if this grid contains the input region (by reference).
        /// </summary>
        bool HasRegion(IRegion region);

        /// <summary>
        /// Returns <c>true</c> if and only if this grid contains a region similar to the 
        /// input rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle with which similarity is tested</param>
        /// <param name="similarity">The max distance between corresponding edges</param>
        /// <param name="pred">predicate for filtering regions</param>
        bool HasSimilarRegion(
            Rectangle rectangle, int similarity, Predicate<IRegion> pred = null);

        /// <summary>
        /// Returns <c>true</c> if and only if this grid contains a region similar to the 
        /// input rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle with which similarity is tested</param>
        /// <param name="similarity">A number in the range [0, 1] that determines the level of
        /// similarity (i.e., 1 implies exact match)</param>
        bool HasSimilarRegion(Rectangle rectangle, double similarity);

        /// <summary>
        /// Adds all regions that satisfy the input predicate (if not <c>null</c>) 
        /// to the input result list.
        /// </summary>
        void GetRegionsWhere(
            ICollection<IRegion> result,
            Predicate<IRegion> where = null);

        /// <summary>
        /// Returns regions intersecting or adjacent to the input rectangle.
        /// </summary>
        /// <param name="rectangle">Regions adjacent to this rectangle are returned</param>
        /// <param name="adjacency">Adjacency in pixels (0 implies intersection)</param>
        ICollection<IRegion> GetAdjacentRegions(Rectangle rectangle, int adjacency);

        /// <summary>
        /// Returns all regions that are not contained by other regions that are contained by the
        /// input rectangle.
        /// </summary>
        ICollection<IRegion> GetUncontainedRegions(Rectangle rectangle, bool inclusive = true);

        /// <summary>
        /// Returns a copy of the input area of this grid.
        /// </summary>
        IRegionsGrid Copy(Rectangle area);

        /// <summary>
        /// Unifies the input regions with other regions of this grid without modifying the grid.
        /// At completion of each iteration, the regions set is updated to reflect the candidate
        /// regions for that iteration.
        /// </summary>
        /// <param name="regions">Regions to unify</param>
        /// <param name="unifyWith">Regions to unify with the given candidate region of the
        /// specified set of candidates</param>
        /// <param name="iterations">Number of iterations or <c>0</c> to iterate until no 
        /// further unifications occur</param>
        /// <returns>
        /// Regions that were removed from the grid (including new unified regions)
        /// </returns>
        ISet<IRegion> Unify(
            ISet<IRegion> regions,
            Func<IRegion, IEnumerable<IRegion>> unifyWith,
            int iterations = 0);

        #endregion
    }
}

namespace Gobln.Pager;

/// <summary>
/// Extensions for IPage
/// </summary>
public static class IPageExtensions
{
    /// <summary>
    /// Navigation helpers for any <see cref="IPage"/>: the pages around it, and the items on it.
    /// Methods rather than extension properties, so callers on any C# version can use them.
    /// </summary>
    /// <param name="page">The page</param>
    extension(IPage page)
    {
        /// <summary>
        /// Is there a page before this one
        /// </summary>
        public bool HasPreviousPage() => page.PageIndex > 1;

        /// <summary>
        /// Is there a page after this one
        /// </summary>
        public bool HasNextPage() => page.PageIndex < page.TotalPageCount;

        // Both are worked out in long and capped at TotalItemCount, so a large index or size can't overflow

        /// <summary>
        /// The number of the first item on this page in the original collection, counted from 1. 0 when there are no items.
        /// </summary>
        public int FirstItemOnPage() => page.TotalItemCount == 0
            ? 0
            : (int)Math.Min(((long)page.PageIndex - 1) * page.PageSize + 1, page.TotalItemCount);

        /// <summary>
        /// The number of the last item on this page in the original collection, counted from 1. 0 when there are no items.
        /// </summary>
        public int LastItemOnPage() => page.TotalItemCount == 0
            ? 0
            : (int)Math.Min((long)page.PageIndex * page.PageSize, page.TotalItemCount);
    }
}

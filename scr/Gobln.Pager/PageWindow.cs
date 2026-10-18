namespace Gobln.Pager;

/// <summary>
/// Works out which page numbers a pager shows: the first and last pages, the pages around the current one, and an ellipsis where pages are skipped.
/// </summary>
public static class PageWindow
{
    /// <summary>
    /// The page numbers to show, in order. With <paramref name="showEllipsis"/> the list shows the same number of items on every page, and an ellipsis that would hide a single page shows that page instead.
    /// </summary>
    /// <param name="pageIndex">The current page</param>
    /// <param name="totalPageCount">Total amount of pages</param>
    /// <param name="siblingCount">The number of pages to show on each side of the current page. Less than zero counts as zero.</param>
    /// <param name="boundaryCount">The number of pages to always show at the beginning and the end. Less than zero counts as zero.</param>
    /// <param name="showEllipsis">Mark the places where pages are skipped</param>
    /// <returns>The page numbers, with null where an ellipsis goes</returns>
    public static IReadOnlyList<int?> GetPageNumbers(int pageIndex, int totalPageCount, int siblingCount, int boundaryCount, bool showEllipsis)
    {
        siblingCount = Math.Max(siblingCount, 0);
        boundaryCount = Math.Max(boundaryCount, 0);

        var numbers = new List<int?>();

        // An ellipsis takes a place of its own next to each boundary
        var ellipsisModifier = showEllipsis ? 1 : 0;

        var startNumber =
            Math.Max(
                Math.Min(
                    // Original start
                    pageIndex - siblingCount,
                    // Page boundary +/- ellipsis
                    totalPageCount - boundaryCount - siblingCount * 2 - ellipsisModifier
                ),
                // After start +/- ellipsis
                boundaryCount + 1 + ellipsisModifier
            );

        var endNumber =
            Math.Min(
                Math.Max(
                    // Original end
                    pageIndex + siblingCount,
                    // Page boundary +/- ellipsis
                    boundaryCount + siblingCount * 2 + 1 + ellipsisModifier
                ),
                // Before end +/- ellipsis
                totalPageCount - boundaryCount - ellipsisModifier
            );

        var startBoundary = Math.Min(boundaryCount, totalPageCount);

        for (int index = 1; index <= startBoundary; index++)
        {
            numbers.Add(index);
        }

        if (showEllipsis)
        {
            if (startNumber > boundaryCount + 2)
            {
                numbers.Add(null);
            }
            else if (boundaryCount + 1 < totalPageCount - boundaryCount)
            {
                numbers.Add(boundaryCount + 1);
            }
        }

        for (int index = startNumber; index <= endNumber; index++)
        {
            numbers.Add(index);
        }

        if (showEllipsis)
        {
            if (endNumber < totalPageCount - boundaryCount - 1)
            {
                numbers.Add(null);
            }
            else if (totalPageCount - boundaryCount > boundaryCount)
            {
                numbers.Add(totalPageCount - boundaryCount);
            }
        }

        // Start after the first boundary, so overlapping boundaries don't show a page twice
        for (int index = Math.Max(totalPageCount - boundaryCount + 1, startBoundary + 1); index <= totalPageCount; index++)
        {
            numbers.Add(index);
        }

        return numbers;
    }
}

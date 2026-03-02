namespace Gobln.Pager;

/// <remarks>
/// PagerFilter
/// </remarks>
public class PageFilter : IPageFilter
{
    /// <summary>
    /// PagerFilter
    /// </summary>
    public PageFilter()
    {
    }

    /// <summary>
    /// PagerFilter
    /// </summary>
    /// <param name="pageIndex">The index of the page</param>
    /// <param name="pageSize">The size of the page</param>
    public PageFilter(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// The index of the page
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// The size of the page
    /// </summary>
    public int PageSize { get; set; }
}
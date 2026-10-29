namespace Gobln.Pager;

/// <summary>
/// IPageFilter
/// </summary>
public interface IPageFilter
{
    /// <summary>
    /// The index of the page
    /// </summary>
    int PageIndex { get; set; }

    /// <summary>
    /// The size of the page
    /// </summary>
    int PageSize { get; set; }
}
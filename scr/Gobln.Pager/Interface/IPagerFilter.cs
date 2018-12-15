namespace Gobln.Pager
{
    /// <summary>
    /// IPagerFilter
    /// </summary>
    public interface IPagerFilter
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
}
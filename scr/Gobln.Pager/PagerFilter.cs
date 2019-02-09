namespace Gobln.Pager
{
    /// <summary>
    /// PagerFilter
    /// </summary>
    public class PagerFilter : IPagerFilter
    {
        /// <summary>
        /// The index of the page
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// The size of the page
        /// </summary>
        public int PageSize { get; set; }
    }
}
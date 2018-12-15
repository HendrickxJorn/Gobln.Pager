using System.Collections.Generic;

namespace Gobln.Pager
{
    /// <summary>
    /// Interface Page<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPage<out T> : IPage, IEnumerable<T> 
    {
    }

    /// <summary>
    /// Interface Page
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// The index of the current page in the original list
        /// </summary>
        int CurrentPageIndex { get; }

        /// <summary>
        /// The size of the page
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Total amount of items in the original list
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Total amount of pages in the original list
        /// </summary>
        int TotalPageCount { get; }

        /// <summary>
        /// Is paging available (more than one page in the original list)
        /// </summary>
        bool HasPaging { get; }

        /// <summary>
        /// Does the page contain any elements
        /// </summary>
        bool IsEmpty { get; }
    }
}
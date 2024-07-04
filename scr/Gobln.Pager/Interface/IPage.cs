using System.Collections.Generic;

namespace Gobln.Pager
{
    /// <summary>
    /// Interface <see cref="Page{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPage<out T> : IPage, IEnumerable<T> 
    {
    }

    /// <summary>
    /// Interface <see cref="Page"/>
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// The index of the current page in the original collection
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// The size of the page
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Total amount of items in the original collection
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Total amount of pages in the original collection
        /// </summary>
        int TotalPageCount { get; }

        /// <summary>
        /// Is paging available (has more than one page in the original collection)
        /// </summary>
        bool HasPaging { get; }

        /// <summary>
        /// Does the page contain any elements
        /// </summary>
        bool IsEmpty { get; }
    }
}
using System.Collections;
using System.Collections.Generic;

namespace Gobln.Pager
{
    /// <summary>
    /// Interface IPagedList<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IPagedList, IList<T>
    {
        /// <summary>
        /// Retrieves the currently active <see cref="Page{T}"/>
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        Page<T> GetPage();

        /// <summary>
        /// Retrieves the <see cref="Page{T}"/> at the given index
        /// </summary>
        /// <param name="pageIndex">Index of the requested Page</param>
        /// <returns><see cref="Page{T}"/></returns>
        Page<T> GetPage(int pageIndex);

        /// <summary>
        /// Go to the next <see cref="Page"/>. Can not go beyond the last <see cref="Page"/>.
        /// </summary>
        /// <returns><see cref="PagedList{T}"/></returns>
        PagedList<T> NextPage();

        /// <summary>
        /// Go to the previous <see cref="Page"/>. Can not go beyond the fist <see cref="Page"/>.
        /// </summary>
        /// <returns><see cref="PagedList{T}"/></returns>
        PagedList<T> PreviousPage();

        /// <summary>
        /// Get all the Pages
        /// </summary>
        /// <returns><see cref="IEnumerable"/> of <see cref="Page{T}"/></returns>
        IEnumerable<Page<T>> GetPages();
    }

    /// <summary>
    /// Interface IPagedList
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// The current page index
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// The amount of items per page
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// The total page count
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Check if a next page exists
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Check if a previous page exist
        /// </summary>
        bool HasPreviousPage { get; }
    }
}
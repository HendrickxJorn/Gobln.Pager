using System.Collections.Generic;

namespace Gobln.Pager
{
    /// <summary>
    /// Interface IPagedList<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IPagedList, IList<T> //, ICollection<T>, IList, ICollection, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Retrieves the currently active Page
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        Page<T> GetCurrentPage();

        /// <summary>
        /// Retrieves the Page at the given index
        /// </summary>
        /// <param name="pageIndex">Index of the requested Page</param>
        /// <returns><see cref="Page{T}"/></returns>
        Page<T> GetPage(int pageIndex);

        /// <summary>
        /// Loads the next Page if possible and returns it.
        /// </summary>
        /// <returns></returns>
        Page<T> GetNextPage();

        /// <summary>
        /// Loads the previous Page if possible and returns it.
        /// </summary>
        /// <returns></returns>
        Page<T> GetPreviousPage();

        /// <summary>
        /// Returns the next Page if possible without loading it (if the active page is the last Page, it will return the active Page).
        /// </summary>
        /// <returns></returns>
        Page<T> PeakNextPage();

        /// <summary>
        /// Returns the previous Page if possible without loading it (if the active page is the first Page, it will return the active Page).
        /// </summary>
        /// <returns></returns>
        Page<T> PeakPreviousPage();

        /// <summary>
        /// Copies the elements of the PagedList to a new List.
        /// </summary>
        /// <returns>An List containing copies of the elements of the PagedList.</returns>
        List<T> ToList();

        /// <summary>
        /// Copies the elements of the PagedList to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the PagedList.</returns>
        T[] ToArray();
    }

    /// <summary>
    /// Interface IPagedList
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// The current page index
        /// </summary>
        int CurrentPageIndex { get; set; }

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
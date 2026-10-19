using System.Collections.Generic;
using System.Linq;

namespace Gobln.Pager;

/// <summary>
/// Extensions for IQueryable
/// </summary>
/// <remarks>
/// The asynchronous versions are on <see cref="IEnumerable{T}"/> and notice a query, so it is still paged in the query.
/// Gobln.Pager.EntityFrameworkCore adds truly asynchronous ones for Entity Framework Core.
/// </remarks>
public static class IQueryableExtensions
{
    /// <summary>
    /// Paging for a query: the count and the paging are part of the query
    /// </summary>
    /// <typeparam name="T">The type of the items</typeparam>
    /// <param name="source">The elements of <typeparamref name="T"/></param>
    extension<T>(IQueryable<T> source)
    {
        internal Page<T> PrivateToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged = false)
        {
            var pd = new PageDefinition(pageIndex, pageSize, totalItemCount);

            return new Page<T>(
                prePaged || pd.TotalItemCount < 1
                    ? source
                    : source.Skip(pd.SkipIndex).Take(pd.PageSize)
                , pd);
        }

        #region ToPage

        /// <summary>
        /// Counts the query and reads all of it onto one <see cref="Page{T}"/>
        /// </summary>
        /// <returns>One page that holds every item</returns>
        public Page<T> ToPage()
        {
            var c = source.Count();
            return PrivateToPage(source, 1, c, c);
        }

        /// <summary>
        /// Counts the query and reads one <see cref="Page{T}"/> of it
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <returns>The page, with the totals of the whole query</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1</exception>
        public Page<T> ToPage(int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            return PrivateToPage(source, pageIndex, pageSize, source.Count());
        }

        /// <summary>
        /// Counts the query and reads one <see cref="Page{T}"/> of it
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <returns>The page, with the totals of the whole query</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1</exception>
        public Page<T> ToPage(IPageFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());
        }

        /// <summary>
        /// Reads a pre-paged query, which returns only the items of page <paramref name="pageIndex"/>, into a <see cref="Page{T}"/>
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <returns>The page, holding what the query returns</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, true);
        }

        /// <summary>
        /// Reads a pre-paged query, which returns only the items of the page of <paramref name="pagerFilter"/>, into a <see cref="Page{T}"/>
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <returns>The page, holding what the query returns</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true);
        }

        /// <summary>
        /// Reads one <see cref="Page{T}"/> of the query, with a total that is already known, so the query isn't counted
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the query is already paged. If so, it is read as it is.</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged);
        }

        /// <summary>
        /// Reads one <see cref="Page{T}"/> of the query, with a total that is already known, so the query isn't counted
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the query is already paged. If so, it is read as it is.</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Reads the whole query into a <see cref="PagedList{T}"/> of 10 items a page
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <returns>The list</returns>
        public PagedList<T> ToPagedList()
        {
            return PagedList.FromEnumerable(source);
        }

        /// <summary>
        /// Reads the whole query into a <see cref="PagedList{T}"/>
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <param name="pageSize">Number of items on a page</param>
        /// <returns>The list</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageSize"/> is less than 1</exception>
        public PagedList<T> ToPagedList(int pageSize)
        {
            Validator.ValidatePageSize(pageSize);

            return PagedList.FromEnumerable(source, pageSize);
        }

        #endregion ToPagedList
    }
}
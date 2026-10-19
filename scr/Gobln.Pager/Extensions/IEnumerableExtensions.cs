using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gobln.Pager;

/// <summary>
/// Extensions for IEnumerable
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Paging for a collection, in memory
    /// </summary>
    /// <typeparam name="T">The type of the items</typeparam>
    /// <param name="source">The elements of <typeparamref name="T"/></param>
    extension<T>(IEnumerable<T> source)
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
        /// Puts the whole collection on one <see cref="Page{T}"/>
        /// </summary>
        /// <returns>One page that holds every item</returns>
        public Page<T> ToPage()
        {
            var list = source.ToList();
            return PrivateToPage(list, 1, list.Count, list.Count, prePaged: true);
        }

        /// <summary>
        /// Counts the collection and takes one <see cref="Page{T}"/> out of it
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <returns>The page, with the totals of the whole collection</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1</exception>
        public Page<T> ToPage(int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            return PrivateToPage(source, pageIndex, pageSize, source.Count());
        }

        /// <summary>
        /// Counts the collection and takes one <see cref="Page{T}"/> out of it
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <returns>The page, with the totals of the whole collection</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1</exception>
        public Page<T> ToPage(IPageFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());
        }

        /// <summary>
        /// Converts a pre-paged collection, which holds only the items of page <paramref name="pageIndex"/>, to a <see cref="Page{T}"/>
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <returns>The page, holding the collection as it is</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, true);
        }

        /// <summary>
        /// Converts a pre-paged collection, which holds only the items of the page of <paramref name="pagerFilter"/>, to a <see cref="Page{T}"/>
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <returns>The page, holding the collection as it is</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true);
        }

        /// <summary>
        /// Takes one <see cref="Page{T}"/> out of the collection, with a total that is already known, so the collection isn't counted
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the collection is already paged. If so, it is used as it is.</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged);
        }

        /// <summary>
        /// Takes one <see cref="Page{T}"/> out of the collection, with a total that is already known, so the collection isn't counted
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the collection is already paged. If so, it is used as it is.</param>
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
        /// Copies the collection into a <see cref="PagedList{T}"/> of 10 items a page
        /// </summary>
        /// <returns>The list</returns>
        public PagedList<T> ToPagedList()
        {
            return PagedList.FromEnumerable(source);
        }

        /// <summary>
        /// Copies the collection into a <see cref="PagedList{T}"/>
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

        #region ToPage Async

        /// <summary>
        /// Puts the whole collection on one <see cref="Page{T}"/>, on the thread pool
        /// </summary>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>One page that holds every item</returns>
        public async Task<Page<T>> ToPageAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => {
                    var list = source.ToList();
                    return PrivateToPage(list, 1, list.Count, list.Count, prePaged: true);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Counts the collection and takes one <see cref="Page{T}"/> out of it, on the thread pool
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page, with the totals of the whole collection</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1</exception>
        /// <remarks>
        /// Runs on the thread pool. A query is paged in the query. Gobln.Pager.EntityFrameworkCore makes this truly asynchronous for Entity Framework Core.
        /// </remarks>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize);

            return await Task.Run(() =>
                {
                    // The IQueryable overloads moved here, so a query has to be noticed: counting or slicing it in memory would read all of it
                    if (source is IQueryable<T> query)
                    {
                        return query.ToPage(pageIndex, pageSize);
                    }

                    var list = source.ToList();
                    return PrivateToPage(list, pageIndex, pageSize, list.Count);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Counts the collection and takes one <see cref="Page{T}"/> out of it, on the thread pool
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page, with the totals of the whole collection</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1</exception>
        /// <remarks>
        /// Runs on the thread pool. A query is paged in the query. Gobln.Pager.EntityFrameworkCore makes this truly asynchronous for Entity Framework Core.
        /// </remarks>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter);

            return await Task.Run(() =>
                {
                    if (source is IQueryable<T> query)
                    {
                        return query.ToPage(pagerFilter);
                    }

                    var list = source.ToList();
                    return PrivateToPage(list, pagerFilter.PageIndex, pagerFilter.PageSize, list.Count);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts a pre-paged collection, which holds only the items of page <paramref name="pageIndex"/>, to a <see cref="Page{T}"/>, on the thread pool
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page, holding the collection as it is</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, totalItemCount, true), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts a pre-paged collection, which holds only the items of the page of <paramref name="pagerFilter"/>, to a <see cref="Page{T}"/>, on the thread pool
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page, holding the collection as it is</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Takes one <see cref="Page{T}"/> out of the collection, with a total that is already known, so the collection isn't counted, on the thread pool
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the collection is already paged. If so, it is used as it is.</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        /// <remarks>
        /// Runs on the thread pool. A query is paged in the query. Gobln.Pager.EntityFrameworkCore makes this truly asynchronous for Entity Framework Core.
        /// </remarks>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return await Task.Run(() => source is IQueryable<T> query
                    ? query.ToPage(pageIndex, pageSize, totalItemCount, prePaged)
                    : PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Takes one <see cref="Page{T}"/> out of the collection, with a total that is already known, so the collection isn't counted, on the thread pool
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the collection is already paged. If so, it is used as it is.</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagerFilter"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        /// <remarks>
        /// Runs on the thread pool. A query is paged in the query. Gobln.Pager.EntityFrameworkCore makes this truly asynchronous for Entity Framework Core.
        /// </remarks>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return await Task.Run(() => source is IQueryable<T> query
                    ? query.ToPage(pagerFilter, totalItemCount, prePaged)
                    : PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged), cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion ToPage Async

        #region ToPagedList Async

        /// <summary>
        /// Copies the collection into a <see cref="PagedList{T}"/> of 10 items a page, on the thread pool
        /// </summary>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The list</returns>
        public async Task<PagedList<T>> ToPagedListAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => PagedList.FromEnumerable(source), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Copies the collection into a <see cref="PagedList{T}"/>, on the thread pool
        /// </summary>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="cancellationToken">Cancels the work before it starts</param>
        /// <returns>The list</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageSize"/> is less than 1</exception>
        public async Task<PagedList<T>> ToPagedListAsync(int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.ValidatePageSize(pageSize);

            return await Task.Run(() => PagedList.FromEnumerable(source, pageSize), cancellationToken).ConfigureAwait(false);
        }

        #endregion ToPagedList Async
    }
}
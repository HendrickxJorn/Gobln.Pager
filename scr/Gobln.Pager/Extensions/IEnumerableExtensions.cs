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
    internal static Page<T> PrivateToPage<T>(IEnumerable<T> source, int pageIndex, int pageSize, int totalItemCount, bool prePaged = false)
    {
        var pd = new PageDefinition(pageIndex, pageSize, totalItemCount);

        return new Page<T>(
            prePaged || pd.TotalItemCount < 1
                ? source
                : source.Skip(pd.SkipIndex).Take(pd.PageSize)
            , pd);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The elements of <typeparamref name="T"/></param>
    extension<T>(IEnumerable<T> source)
    {
        #region ToPage

        /// <summary>
        /// Converts IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <returns></returns>
        public Page<T> ToPage()
        {
            var list = source.ToList();
            return PrivateToPage(list, 1, list.Count, list.Count, prePaged: true);
        }

        /// <summary>
        /// Converts IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        public Page<T> ToPage(int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            return PrivateToPage(source, pageIndex, pageSize, source.Count());
        }

        /// <summary>
        /// Converts IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());
        }

        /// <summary>
        /// Converts asynchronous a pre-paged IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <returns></returns>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, true);
        }

        /// <summary>
        /// Converts asynchronous a pre-paged IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true);
        }

        /// <summary>
        /// Converts IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="prePaged">Is the IEnumerable already pre-paged. If so the IEnumerable will not be paged anymore.</param>
        /// <returns></returns>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged);
        }

        /// <summary>
        /// Converts IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="prePaged">Is the IEnumerable already pre-paged. If so the IEnumerable will not be paged anymore.</param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Convert IEnumerable to an <see cref="PagedList{T}"/>
        /// </summary>
        /// <returns></returns>
        public PagedList<T> ToPagedList()
        {
            return PagedList.FromEnumerable(source);
        }

        /// <summary>
        /// Convert IEnumerable to an <see cref="PagedList{T}"/>
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedList<T> ToPagedList(int pageSize)
        {
            Validator.ValidatePageSize(pageSize);

            return PagedList.FromEnumerable(source, pageSize);
        }

        #endregion ToPagedList

        #region ToPage Async

        /// <summary>
        /// Converts asynchronous IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => { 
                    var list = source.ToList();
                    return PrivateToPage(list, 1, list.Count, list.Count, prePaged: true);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize);

            return await Task.Run(() => { var list = source.ToList(); return PrivateToPage(list, pageIndex, pageSize, list.Count); }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter);

            return await Task.Run(() => { var list = source.ToList(); return PrivateToPage(list, pagerFilter.PageIndex, pagerFilter.PageSize, list.Count); }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous a pre-paged IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, totalItemCount, true), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous a pre-paged IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="prePaged">Is the IEnumerable already pre-paged. If so the IEnumerable will not be paged anymore.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IEnumerable</param>
        /// <param name="prePaged">Is the IEnumerable already pre-paged. If so the IEnumerable will not be paged anymore.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged), cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion ToPage Async

        #region ToPagedList Async

        /// <summary>
        /// Convert asynchronous IEnumerable to an <see cref="PagedList{T}"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedList<T>> ToPagedListAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => PagedList.FromEnumerable(source), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Convert asynchronous IEnumerable to an <see cref="PagedList{T}"/>
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedList<T>> ToPagedListAsync(int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.ValidatePageSize(pageSize);

            return await Task.Run(() => PagedList.FromEnumerable(source, pageSize), cancellationToken).ConfigureAwait(false);
        }

        #endregion ToPagedList Async
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gobln.Pager;

/// <summary>
/// Extensions for IQueryable
/// </summary>
public static class IQueryableExtensions
{
    internal static Page<T> PrivateToPage<T>(IQueryable<T> collection, int pageIndex, int pageSize, int totalItemCount, bool prePaged = false)
    {
        var pd = new PageDefinition(pageIndex, pageSize, totalItemCount);

        return new Page<T>(
            prePaged || pd.TotalItemCount < 1
                ? collection
                : collection.Skip(pd.SkipIndex).Take(pd.PageSize)
            , pd);
    }

    #region ToPage

    /// <summary>
    /// Converts IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source)
    {
        return PrivateToPage(source, 1, source.Count(), source.Count());
    }

    /// <summary>
    /// Converts IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize)
    {
        Validator.Validate(pageIndex, pageSize);

        return PrivateToPage(source, pageIndex, pageSize, source.Count());
    }

    /// <summary>
    /// Converts IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, IPageFilter pagerFilter)
    {
        Validator.Validate(pagerFilter);

        return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());
    }

    /// <summary>
    /// Converts a pre-paged IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalItemCount)
    {
        Validator.Validate(pageIndex, pageSize, totalItemCount);

        return PrivateToPage(source, pageIndex, pageSize, totalItemCount, true);
    }

    /// <summary>
    /// Converts a pre-paged IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, IPageFilter pagerFilter, int totalItemCount)
    {
        Validator.Validate(pagerFilter, totalItemCount);

        return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true);
    }

    /// <summary>
    /// Converts IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalItemCount, bool prePaged)
    {
        Validator.Validate(pageIndex, pageSize, totalItemCount);

        return PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged);
    }

    /// <summary>
    /// Converts IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
    /// <returns></returns>
    public static Page<T> ToPage<T>(this IQueryable<T> source, IPageFilter pagerFilter, int totalItemCount, bool prePaged)
    {
        Validator.Validate(pagerFilter, totalItemCount);

        return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged);
    }

    #endregion ToPage

    #region ToPagedList

    /// <summary>
    /// Convert IQueryable to an <see cref="PagedList{T}"/>
    /// Warning: This will execute the entire query and load the results in memory!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> source)
    {
        return PagedList.FromEnumerable(source);
    }

    /// <summary>
    /// Convert IQueryable to an <see cref="PagedList{T}"/>
    /// Warning: This will execute the entire query and load the results in memory!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageSize">The number of elements displayed per page</param>
    /// <returns></returns>
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageSize)
    {
        Validator.ValidatePageSize(pageSize);

        return PagedList.FromEnumerable(source, pageSize);
    }

    #endregion ToPagedList

    #region Async

    #region ToPage

    /// <summary>
    /// Converts asynchronous IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => PrivateToPage(source, 1, source.Count(), source.Count()), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous IQueryable toan <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pageIndex, pageSize);

        return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, source.Count()), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, IPageFilter pagerFilter, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pagerFilter);

        return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count()), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous a pre-paged IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalItemCount, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pageIndex, pageSize, totalItemCount);

        return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, totalItemCount, true), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous a pre-paged IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, IPageFilter pagerFilter, int totalItemCount, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pagerFilter, totalItemCount);

        return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageIndex">Index of page</param>
    /// <param name="pageSize">Size of page</param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pageIndex, pageSize, totalItemCount);

        return await Task.Run(() => PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts asynchronous IQueryable to an <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
    /// <param name="totalItemCount">Total item count of IQueryable</param>
    /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, IPageFilter pagerFilter, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
    {
        Validator.Validate(pagerFilter, totalItemCount);

        return await Task.Run(() => PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged), cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion ToPage

    #region ToPagedList

    /// <summary>
    /// Convert asynchronous IQueryable to a <see cref="PagedList{T}"/>
    /// Warning: This will execute the entire query and load the results in memory!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The elements of <typeparamref name="T"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => PagedList.FromEnumerable(source), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Convert asynchronous IQueryable to an <see cref="PagedList{T}"/>
    /// Warning: This will execute the entire query and load the results in memory!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageSize">The number of elements displayed per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageSize, CancellationToken cancellationToken = default)
    {
        Validator.ValidatePageSize(pageSize);

        return await Task.Run(() => PagedList.FromEnumerable(source, pageSize), cancellationToken).ConfigureAwait(false);
    }

    #endregion ToPagedList

    #endregion Async
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Gobln.Pager;

/// <summary>
/// Asynchronous paging for Entity Framework Core. On an EF Core query, <c>ToPageAsync</c> and <c>ToPagedListAsync</c>
/// run <c>CountAsync</c> and <c>ToListAsync</c>, instead of the core package's synchronous queries on the thread pool.
/// </summary>
/// <remarks>
/// The class is in namespace <c>Gobln.Pager</c>, so code that already has <c>using Gobln.Pager;</c> picks these methods up.
/// On a query that isn't from Entity Framework Core, such as <c>list.AsQueryable()</c>, they do what the core package does.
/// </remarks>
public static class EntityFrameworkCoreExtensions
{
    /// <summary>
    /// Asynchronous paging for an Entity Framework Core query
    /// </summary>
    /// <typeparam name="T">The type of the items</typeparam>
    /// <param name="source">The query</param>
    extension<T>(IQueryable<T> source)
    {
        #region ToPageAsync

        /// <summary>
        /// Reads the whole query, asynchronously, onto one page
        /// </summary>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>One page that holds every item</returns>
        public async Task<Page<T>> ToPageAsync(CancellationToken cancellationToken = default)
        {
            if (!IsEntityFrameworkQuery(source))
            {
                return await IEnumerableExtensions.ToPageAsync(source, cancellationToken).ConfigureAwait(false);
            }

            return new Page<T>(await source.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Counts the query and reads one page of it, both asynchronously
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="cancellationToken">Cancels the queries</param>
        /// <returns>The page, with the totals of the whole query</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1</exception>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize);

            if (!IsEntityFrameworkQuery(source))
            {
                return await IEnumerableExtensions.ToPageAsync(source, pageIndex, pageSize, cancellationToken).ConfigureAwait(false);
            }

            var totalItemCount = await source.CountAsync(cancellationToken).ConfigureAwait(false);

            return await PageAsync(source, pageIndex, pageSize, totalItemCount, prePaged: false, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Counts the query and reads one page of it, both asynchronously
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="cancellationToken">Cancels the queries</param>
        /// <returns>The page, with the totals of the whole query</returns>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1</exception>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter);

            return await source.ToPageAsync(pagerFilter.PageIndex, pagerFilter.PageSize, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a query that is already paged, asynchronously, as the page <paramref name="pageIndex"/>
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, CancellationToken cancellationToken = default)
        {
            return source.ToPageAsync(pageIndex, pageSize, totalItemCount, prePaged: true, cancellationToken);
        }

        /// <summary>
        /// Reads a query that is already paged, asynchronously, as the page of <paramref name="pagerFilter"/>
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, CancellationToken cancellationToken = default)
        {
            return source.ToPageAsync(pagerFilter, totalItemCount, prePaged: true, cancellationToken);
        }

        /// <summary>
        /// Reads one page of the query asynchronously, with a total that is already known, so the query isn't counted
        /// </summary>
        /// <param name="pageIndex">Index of the page, from 1. Past the last page, the last page is returned.</param>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the query is already paged. If so, it is read as it is.</param>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public async Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            if (!IsEntityFrameworkQuery(source))
            {
                return await IEnumerableExtensions.ToPageAsync(source, pageIndex, pageSize, totalItemCount, prePaged, cancellationToken).ConfigureAwait(false);
            }

            return await PageAsync(source, pageIndex, pageSize, totalItemCount, prePaged, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads one page of the query asynchronously, with a total that is already known, so the query isn't counted
        /// </summary>
        /// <param name="pagerFilter">The page index and page size</param>
        /// <param name="totalItemCount">Number of items on all pages together</param>
        /// <param name="prePaged">Whether the query is already paged. If so, it is read as it is.</param>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The page</returns>
        /// <exception cref="ArgumentOutOfRangeException">The page index or page size is less than 1, or <paramref name="totalItemCount"/> less than 0</exception>
        public async Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return await source.ToPageAsync(pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, prePaged, cancellationToken).ConfigureAwait(false);
        }

        #endregion ToPageAsync

        #region ToPagedListAsync

        /// <summary>
        /// Reads the whole query asynchronously into a <see cref="PagedList{T}"/> of 10 items a page
        /// </summary>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The list</returns>
        public async Task<PagedList<T>> ToPagedListAsync(CancellationToken cancellationToken = default)
        {
            if (!IsEntityFrameworkQuery(source))
            {
                return await IEnumerableExtensions.ToPagedListAsync(source, cancellationToken).ConfigureAwait(false);
            }

            return new PagedList<T>(await source.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Reads the whole query asynchronously into a <see cref="PagedList{T}"/>
        /// </summary>
        /// <param name="pageSize">Number of items on a page</param>
        /// <param name="cancellationToken">Cancels the query</param>
        /// <returns>The list</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageSize"/> is less than 1</exception>
        public async Task<PagedList<T>> ToPagedListAsync(int pageSize, CancellationToken cancellationToken = default)
        {
            Validator.ValidatePageSize(pageSize);

            if (!IsEntityFrameworkQuery(source))
            {
                return await IEnumerableExtensions.ToPagedListAsync(source, pageSize, cancellationToken).ConfigureAwait(false);
            }

            return new PagedList<T>(await source.ToListAsync(cancellationToken).ConfigureAwait(false), pageSize);
        }

        #endregion ToPagedListAsync
    }

    // EF Core's async methods throw on any other query, such as list.AsQueryable() or a mocked DbSet
    private static bool IsEntityFrameworkQuery<T>(IQueryable<T> source) => source.Provider is IAsyncQueryProvider;

    private static async Task<Page<T>> PageAsync<T>(IQueryable<T> source, int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken)
    {
        // Nothing to read: the core doesn't query for the items either
        if (totalItemCount == 0)
        {
            return new Page<T>();
        }

        var definition = new PageDefinition(pageIndex, pageSize, totalItemCount);
        var query = prePaged ? source : source.Skip(definition.SkipIndex).Take(definition.PageSize);
        var items = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Page<T>(items, pageIndex, pageSize, totalItemCount);
    }
}

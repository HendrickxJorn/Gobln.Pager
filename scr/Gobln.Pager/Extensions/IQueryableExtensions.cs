using System.Linq;

#if !FEATURE_TYPE_NET40

using System.Threading.Tasks;

#endif

namespace Gobln.Pager
{
    /// <summary>
    /// Extensions for IQueryable
    /// </summary>
    public static class IQueryableExtensions
    {
        #region ToPage

        /// <summary>
        /// Converts IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> source)
        {
            return Page<T>.FromEnumerable(source, new PageDefinition(1, source.Count(), source.Count()));
        }

        /// <summary>
        /// Converts IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            var pd = new PageDefinition(pageIndex, pageSize, source.Count());

            return pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> source, IPagerFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            var pd = new PageDefinition(pagerFilter, source.Count());

            return pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="itemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already prepaged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> source, int pageIndex, int pageSize, int itemCount, bool prePaged = false)
        {
            Validator.Validate(pageIndex, pageSize);

            var pd = new PageDefinition(pageIndex, pageSize, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <param name="itemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already prepaged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> source, IPagerFilter pagerFilter, int itemCount, bool prePaged = false)
        {
            Validator.Validate(pagerFilter);

            var pd = new PageDefinition(pagerFilter, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Convert IQueryable to a PagedList.
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source)
        {
            return PagedList<T>.FromEnumerable(source);
        }

        /// <summary>
        /// Convert IQueryable to a PagedList
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageSize)
        {
            Validator.ValidatePageSize(pageSize);

            return PagedList<T>.FromEnumerable(source, pageSize);
        }

        #endregion ToPagedList

#if !FEATURE_TYPE_NET40

        #region Async

        #region ToPage

        /// <summary>
        /// Converts asynchronous IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source)
        {
            return await Task.Run(() => Page<T>.FromEnumerable(source, new PageDefinition(1, source.Count(), source.Count()))).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            var pd = new PageDefinition(pageIndex, pageSize, source.Count());

            return pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, IPagerFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            var pd = new PageDefinition(pagerFilter, source.Count());
            
            return pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="itemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already prepaged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int itemCount, bool prePaged = false)
        {
            Validator.Validate(pageIndex, pageSize);

            var pd = new PageDefinition(pageIndex, pageSize, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IQueryable to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <param name="itemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already prepaged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, IPagerFilter pagerFilter, int itemCount, bool prePaged = false)
        {
            Validator.Validate(pagerFilter);

            var pd = new PageDefinition(pagerFilter, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Convert asynchronous IQueryable to a PagedList.
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source)
        {
            return await Task.Run(() => PagedList<T>.FromEnumerable(source)).ConfigureAwait(false);
        }

        /// <summary>
        /// Convert asynchronous IQueryable to a PagedList
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageSize)
        {
            Validator.ValidatePageSize(pageSize);

            return await Task.Run(() => PagedList<T>.FromEnumerable(source, pageSize)).ConfigureAwait(false);
        }

        #endregion ToPagedList

        #endregion Async

#endif
    }
}
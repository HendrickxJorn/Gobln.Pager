using Gobln.Pager.Infrastructure;
using Gobln.Pager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

#if !FEATURE_TYPE_NET40

using System.Threading.Tasks;

#endif

namespace Gobln.Pager
{
    /// <summary>
    /// Extensions for IEnumerable
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region ToPage

        /// <summary>
        /// Converts IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> source)
        {
            return Page<T>.FromEnumerable(source, new PageDefinition(1, 10, source.Count()));
        }

        /// <summary>
        /// Converts IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", Resources.ExceptionZeroOrLess);

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pageIndex, pageSize, source.Count());

            return pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> source, IPagerFilter pagerFilter)
        {
            if (pagerFilter.PageIndex < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", Resources.ExceptionZeroOrLess);

            if (pagerFilter.PageSize < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());

            return pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="itemCount">Total item count of IEnumerable<typeparamref name="T"/></param>
        /// <param name="prePaged">Is the IEnumerable<typeparamref name="T"/> already prepaged. If so the IEnumerable<typeparamref name="T"/> will not be paged anymore.</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int itemCount, bool prePaged = false)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", Resources.ExceptionZeroOrLess);

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pageIndex, pageSize, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        /// <summary>
        /// Converts IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <param name="itemCount">Total item count of IEnumerable<typeparamref name="T"/></param>
        /// <param name="prePaged">Is the IEnumerable<typeparamref name="T"/> already prepaged. If so the IEnumerable<typeparamref name="T"/> will not be paged anymore.</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> source, IPagerFilter pagerFilter, int itemCount, bool prePaged = false)
        {
            if (pagerFilter.PageIndex < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", Resources.ExceptionZeroOrLess);

            if (pagerFilter.PageSize < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pagerFilter.PageIndex, pagerFilter.PageSize, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? Page<T>.FromEnumerable(source, pd)
                  : Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Convert IEnumerable<typeparamref name="T"/> to a PagedList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source)
        {
            return PagedList<T>.FromEnumerable(source);
        }

        /// <summary>
        /// Convert IEnumerable<typeparamref name="T"/> to a PagedList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);
            
            return PagedList<T>.FromEnumerable(source, pageSize);
        }

        #endregion ToPagedList

#if !FEATURE_TYPE_NET40

        #region Async

        #region ToPage

        /// <summary>
        /// Converts asynchronous IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IEnumerable<T> source)
        {
            //return new Page<T>(source, new PageDefinition(1, 1, source.Count()));
            return await Task.Run(() => Page<T>.FromEnumerable(source, new PageDefinition(1, 1, source.Count()))).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", Resources.ExceptionZeroOrLess);

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pageIndex, pageSize, source.Count());

            return pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagerFilter"><see cref="IPagerFilter"/></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IEnumerable<T> source, IPagerFilter pagerFilter)
        {
            if (pagerFilter.PageIndex < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", Resources.ExceptionZeroOrLess);

            if (pagerFilter.PageSize < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());

            return pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts asynchronous IEnumerable<typeparamref name="T"/> to a Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="itemCount">Total item count of IEnumerable<typeparamref name="T"/></param>
        /// <param name="prePaged">Is the IEnumerable<typeparamref name="T"/> already prepaged. If so the IEnumerable<typeparamref name="T"/> will not be paged anymore.</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int itemCount, bool prePaged = false)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", Resources.ExceptionZeroOrLess);

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            var pd = new PageDefinition(pageIndex, pageSize, itemCount);

            return prePaged || pd.ItemCount < 1
                  ? await Task.Run(() => Page<T>.FromEnumerable(source, pd)).ConfigureAwait(false)
                  : await Task.Run(() => Page<T>.FromEnumerable(source.Skip(pd.SkipIndex).Take(pd.PageSize), pd)).ConfigureAwait(false);
        }

        #endregion ToPage

        #region ToPagedList

        /// <summary>
        /// Convert asynchronous IEnumerable<typeparamref name="T"/> to a PagedList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source)
        {
            //return new PagedList<T>(source);
            return await Task.Run(() => PagedList<T>.FromEnumerable(source)).ConfigureAwait(false);
        }

        /// <summary>
        /// Convert asynchronous IEnumerable<typeparamref name="T"/> to a PagedList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            //return new PagedList<T>(source, pageSize);
            return await Task.Run(() => PagedList<T>.FromEnumerable(source, pageSize)).ConfigureAwait(false);
        }

        #endregion ToPagedList

        #endregion Async

#endif
    }
}
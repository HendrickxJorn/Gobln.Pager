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
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        /// Converts IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <returns></returns>
        public Page<T> ToPage()
        {
            var c = source.Count();
            return PrivateToPage(source, 1, c, c);
        }

        /// <summary>
        /// Converts IQueryable to an <see cref="Page"/>
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
        /// Converts IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter)
        {
            Validator.Validate(pagerFilter);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, source.Count());
        }

        /// <summary>
        /// Converts a pre-paged IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IQueryable</param>
        /// <returns></returns>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, true);
        }

        /// <summary>
        /// Converts a pre-paged IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IQueryable</param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount)
        {
            Validator.Validate(pagerFilter, totalItemCount);

            return PrivateToPage(source, pagerFilter.PageIndex, pagerFilter.PageSize, totalItemCount, true);
        }

        /// <summary>
        /// Converts IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="totalItemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return PrivateToPage(source, pageIndex, pageSize, totalItemCount, prePaged);
        }

        /// <summary>
        /// Converts IQueryable to an <see cref="Page"/>
        /// </summary>
        /// <param name="pagerFilter"><see cref="IPageFilter"/></param>
        /// <param name="totalItemCount">Total item count of IQueryable</param>
        /// <param name="prePaged">Is the IQueryable already pre-paged. If so the IQueryable will not be paged anymore.</param>
        /// <returns></returns>
        public Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount, bool prePaged)
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
        /// <returns></returns>
        public PagedList<T> ToPagedList()
        {
            return PagedList.FromEnumerable(source);
        }

        /// <summary>
        /// Convert IQueryable to an <see cref="PagedList{T}"/>
        /// Warning: This will execute the entire query and load the results in memory!
        /// </summary>
        /// <param name="pageSize">The number of elements displayed per page</param>
        /// <returns></returns>
        public PagedList<T> ToPagedList(int pageSize)
        {
            Validator.ValidatePageSize(pageSize);

            return PagedList.FromEnumerable(source, pageSize);
        }

        #endregion ToPagedList
    }
}
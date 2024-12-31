using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gobln.Pager
{
    // todo https://stackoverflow.com/questions/5877808/what-is-serializable-and-when-should-i-use-it
    // todo add possible exeptions to the comments
    // bug when TotalItemCount is 0 or new Page<>( null array )

    /// <summary>
    /// Represents a strongly typed <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Page<T> : IPage<T>, IEnumerable
    {
        #region Members

        internal T[] _items = [];

        #endregion Members

        #region Properties

        /// <summary>
        /// The index of the current page in the original collection
        /// </summary>
        public int PageIndex { get; private set; } = 1;

        /// <summary>
        /// The size of the page
        /// </summary>
        public int PageSize { get; private set; } = 0;

        /// <summary>
        /// Total amount of items in the original collection
        /// </summary>
        public int TotalItemCount { get; private set; } = 0;

        /// <summary>
        /// Total amount of pages in the original collection
        /// </summary>
        public int TotalPageCount { get; private set; } = 1;

        /// <summary>
        /// Is paging available (has more than one page in the original collection)
        /// </summary>
        public bool HasPaging => TotalPageCount > 1;

        /// <summary>
        /// Does the page contain any elements
        /// </summary>
        public bool IsEmpty => this == null || _items.Length == 0;

        #endregion Properties

        #region Initialize

        /// <summary>
        /// Creates an empty <see cref="Page"/>
        /// </summary>
        public Page()
        {
        }

        /// <summary>
        /// Converts a collection of <typeparamref name="T"/> to a single <see cref="Page"/>
        /// <param name="collection">The elements of <typeparamref name="T"/> of the current page</param>
        /// </summary>
        public Page(IEnumerable<T> collection)
        {
            var pageDefinitions = new PageDefinition(1, collection.Count(), collection.Count());

            FillPageData(collection, pageDefinitions);
        }

        /// <summary>
        /// Converts a pre-paged collection to a specific <see cref="Page"/>
        /// </summary>
        /// <param name="collection">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="totalItemCount">Total item count in the original collection</param>
        /// <exception cref="ArgumentOutOfRangeException">Can not be less then zero or zero</exception>
        public Page(IEnumerable<T> collection, int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            var pageDefinitions = new PageDefinition(pageIndex, pageSize, totalItemCount);

            FillPageData(collection, pageDefinitions);
        }

        /// <summary>
        /// Converts a collection to a specific <see cref="Page"/>
        /// </summary>
        /// <param name="collection">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageDefinitions">The definition of the current page and its position in the original collection</param>
        /// <exception cref="ArgumentOutOfRangeException">Can not be less then zero or zero</exception>
        internal Page(IEnumerable<T> collection, PageDefinition pageDefinitions)
        {
            FillPageData(collection, pageDefinitions);
        }

        #endregion Initialize

        private void FillPageData(IEnumerable<T> collection, PageDefinition pageDefinitions)
        {
            if (collection != null && pageDefinitions.TotalItemCount > 0 && collection.Any())
            {
                _items = collection.ToArray();

                PageIndex = pageDefinitions.PageIndex;
                TotalPageCount = pageDefinitions.PageCount;
                PageSize = pageDefinitions.PageSize;
                TotalItemCount = pageDefinitions.TotalItemCount;
            }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns>Returns <see cref="IEnumerable{T}"/></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable"/>
        /// </summary>
        /// <returns>Returns <see cref="IEnumerable"/></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Implementation of IEnumerable
    }

    /// <summary>
    /// Represents a strongly typed <see cref="Page"/>
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Converts a collection of <typeparamref name="T"/> to an single <see cref="Page"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Can not be less then zero or zero</exception>
        public static Page<T> FromEnumerable<T>(IEnumerable<T> source)
        {
            return source == null
                ? new Page<T>()
                : new Page<T>(source);
        }

        /// <summary>
        /// Converts collection of <typeparamref name="T"/> to an single <see cref="Page"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <exception cref="ArgumentOutOfRangeException">Can not be less then zero or zero</exception>
        /// <returns></returns>
        public static Page<T> FromEnumerable<T>(IEnumerable<T> collection, int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            return collection == null
                ? new Page<T>()
                : new Page<T>(collection, new PageDefinition(pageIndex, pageSize, collection.Count()));
        }

        /// <summary>
        /// Converts a pre-paged collection of <typeparamref name="T"/> to an single <see cref="Page"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="totalItemCount">Total item count in the original collection</param>
        /// <returns></returns>
        public static Page<T> FromEnumerable<T>(IEnumerable<T> collection, int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize, totalItemCount);

            return collection == null
                ? new Page<T>()
                : new Page<T>(collection, new PageDefinition(pageIndex, pageSize, totalItemCount));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.Pager
{
    //https://stackoverflow.com/questions/5877808/what-is-serializable-and-when-should-i-use-it

    /// <summary>
    /// Represents a strongly typed <see cref="Page"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> : IPage<T>, IEnumerable
    {
        #region Members

        private readonly T[] _items = [];

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
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// </summary>
        public Page(IEnumerable<T> source)
        {
            if (source != null && source.Any())
            {
                _items = source.ToArray();

                PageSize = _items.Length;
                TotalItemCount = _items.Length;
            }
        }

        /// <summary>
        /// Converts a pre-paged collection to a specific <see cref="Page"/>
        /// </summary>
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="totalItemCount">Total item count in the original collection</param>
        public Page(IEnumerable<T> source, int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize);

            var definitions = new PageDefinition(pageIndex, pageSize, totalItemCount);

            if (source != null && definitions.TotalItemCount > 0 && source.Any())
            {
                _items = source.ToArray();

                PageIndex = definitions.PageIndex;
                TotalPageCount = definitions.PageCount;
                PageSize = definitions.PageSize;
                TotalItemCount = definitions.TotalItemCount;
            }
        }

        /// <summary>
        /// Converts a collection to a specific <see cref="Page"/>
        /// </summary>
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageDefinitions">The definition of the current page and its position in the original collection</param>
        internal Page(IEnumerable<T> source, PageDefinition pageDefinitions)
        {
            if (source != null && pageDefinitions.TotalItemCount > 0 && source.Any())
            {
                _items = source.ToArray();

                PageIndex = pageDefinitions.PageIndex;
                TotalPageCount = pageDefinitions.PageCount;
                PageSize = pageDefinitions.PageSize;
                TotalItemCount = pageDefinitions.TotalItemCount;
            }
        }

        #endregion Initialize

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
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <returns></returns>
        public static Page<T> FromEnumerable<T>(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            Validator.Validate(pageIndex, pageSize);

            return source == null
                ? new Page<T>()
                : new Page<T>(source, new PageDefinition(pageIndex, pageSize, source.Count()));
        }

        /// <summary>
        /// Converts a pre-paged collection of <typeparamref name="T"/> to an single <see cref="Page"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The elements of <typeparamref name="T"/> of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original collection</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="totalItemCount">Total item count in the original collection</param>
        /// <returns></returns>
        public static Page<T> FromEnumerable<T>(IEnumerable<T> source, int pageIndex, int pageSize, int totalItemCount)
        {
            Validator.Validate(pageIndex, pageSize);

            return source == null
                ? new Page<T>()
                : new Page<T>(source, new PageDefinition(pageIndex, pageSize, totalItemCount));
        }
    }
}
using Gobln.Pager.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.Pager
{
    /// <summary>
    /// Page Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> : IPage<T>, IEnumerable
    {
        #region Members

        private List<T> _list;

        #endregion Members

        #region Properties

        /// <summary>
        /// The index of the current page in the original list
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// The size of the page
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Total amount of items in the original list
        /// </summary>
        public int TotalItemCount { get; private set; }

        /// <summary>
        /// Total amount of pages in the original list
        /// </summary>
        public int TotalPageCount { get; private set; }

        /// <summary>
        /// Is paging avaible (more than one page in the original list)
        /// </summary>
        public bool HasPaging => TotalPageCount > 1;

        /// <summary>
        /// Does the page contain any elements
        /// </summary>
        public bool IsEmpty => this == null ? true : !_list.Any();

        #endregion Properties

        #region Init

        /// <summary>
        /// Creates an empty page
        /// </summary>
        public Page()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Converts a collection to a single page.
        /// <param name="source">the elements of the current page</param>
        /// </summary>
        public Page(IEnumerable<T> source)
        {
            if (source != null)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    _list = new List<T>(arrSource);

                    CurrentPageIndex = 1;
                    TotalPageCount = 1;
                    PageSize = _list.Count();
                    TotalItemCount = _list.Count();
                }
                else
                {
                    _list = new List<T>();
                }
            }
            else
            {
                _list = new List<T>();
            }
        }

        /// <summary>
        /// Converts a collection to a specific Page
        /// </summary>
        /// <param name="source">the elements of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original list</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="itemCount">Total item count in the original list</param>
        public Page(IEnumerable<T> source, int pageIndex, int pageSize, int itemCount)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", "Can not be less then zero or zero.");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Can not be less then zero or zero.");

            var definitions = new PageDefinition(pageIndex, pageSize, itemCount);

            if (source != null && itemCount > 0)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    _list = new List<T>(arrSource);
                }
                else
                {
                    _list = new List<T>();
                }
            }
            else
            {
                _list = new List<T>();
            }

            CurrentPageIndex = definitions.PageIndex;
            TotalPageCount = definitions.PageCount;
            PageSize = definitions.PageSize;
            TotalItemCount = definitions.ItemCount;
        }

        /// <summary>
        /// Converts a collection to a specific Page
        /// </summary>
        /// <param name="source">the elements of the current page</param>
        /// <param name="definitions">the definition of the current page and its position in the original list</param>
        internal Page(IEnumerable<T> source, PageDefinition definitions)
        {
            if (source != null && definitions.ItemCount > 0)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    _list = new List<T>(arrSource);
                }
                else
                {
                    _list = new List<T>();
                }
            }
            else
            {
                _list = new List<T>();
            }

            CurrentPageIndex = definitions.PageIndex;
            TotalPageCount = definitions.PageCount;
            PageSize = definitions.PageSize;
            TotalItemCount = definitions.ItemCount;
        }

        #endregion Init

        #region Factory

        /// <summary>
        /// Converts a collection to a single page.
        /// <param name="source">the elements of the current page</param>
        /// </summary>
        public static Page<T> FromEnumerable(IEnumerable<T> source)
        {
            var page = new Page<T>();

            if (source != null)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    page._list = new List<T>(arrSource);

                    page.CurrentPageIndex = 1;
                    page.TotalPageCount = 1;
                    page.PageSize = arrSource.Count();
                    page.TotalItemCount = arrSource.Count();
                }
                else
                {
                    page._list = new List<T>();
                }
            }
            else
            {
                page._list = new List<T>();
            }

            return page;
        }

        /// <summary>
        /// Converts a collection to a specific Page
        /// </summary>
        /// <param name="source">the elements of the current page</param>
        /// <param name="pageIndex">The currently selected page in the original list</param>
        /// <param name="pageSize">Size of an individual page</param>
        /// <param name="itemCount">Total item count in the original list</param>
        public static Page<T> FromEnumerable(IEnumerable<T> source, int pageIndex, int pageSize, int itemCount)
        {
            var page = new Page<T>();

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", "Can not be less then zero or zero.");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Can not be less then zero or zero.");

            var definitions = new PageDefinition(pageIndex, pageSize, itemCount);

            if (source != null && itemCount > 0)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    page._list = new List<T>(arrSource);
                }
                else
                {
                    page._list = new List<T>();
                }
            }
            else
            {
                page._list = new List<T>();
            }

            page.CurrentPageIndex = definitions.PageIndex;
            page.TotalPageCount = definitions.PageCount;
            page.PageSize = definitions.PageSize;
            page.TotalItemCount = definitions.ItemCount;

            return page;
        }

        /// <summary>
        /// Converts a collection to a specific Page
        /// </summary>
        /// <param name="source">the elements of the current page</param>
        /// <param name="definitions">the definition of the current page and its position in the original list</param>
        internal static Page<T> FromEnumerable(IEnumerable<T> source, PageDefinition definitions)
        {
            var page = new Page<T>();

            if (source != null && definitions.ItemCount > 0)
            {
                var arrSource = source.ToArray();

                if (arrSource.Any())
                {
                    page._list = new List<T>(arrSource);
                }
                else
                {
                    page._list = new List<T>();
                }
            }
            else
            {
                page._list = new List<T>();
            }

            page.CurrentPageIndex = definitions.PageIndex;
            page.TotalPageCount = definitions.PageCount;
            page.PageSize = definitions.PageSize;
            page.TotalItemCount = definitions.ItemCount;

            return page;
        }

        #endregion Factory

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.List.
        /// </summary>
        /// <returns>A System.Collections.Generic.List.Enumerator for the Page.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.List.
        /// </summary>
        /// <returns>A System.Collections.Generic.List.Enumerator for the Page.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Implementation of IEnumerable
    }
}
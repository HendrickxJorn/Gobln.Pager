using Gobln.Pager.Infrastructure;
using Gobln.Pager.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gobln.Pager
{
    /// <summary>
    /// Represents a strongly typed pagedlist of objects that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [Serializable]
#if FEATURE_TYPE_NET40
    public class PagedList<T> : IPagedList<T>
#else
    public class PagedList<T> : IPagedList<T>, IReadOnlyList<T>
#endif
    {
        #region Members

        private List<T> _list = new List<T>();
        private int _currentPageIndex;
        private int _pageSize;
        private const int _defaultPageSize = 10;

        #endregion Members

        #region Init

        /// <summary>
        /// Initializes a new empty instance of the Gobln.Pager.PagedList class with default initial page size.
        /// </summary>
        public PagedList()
            : this(_defaultPageSize)
        {
        }

        /// <summary>
        /// Initializes a new empty instance of the Gobln.Pager.PagedList class with given initial page size.
        /// </summary>
        /// <param name="pageSize">The number of elements displayed per page</param>
        public PagedList(int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            _currentPageIndex = 1;
            _pageSize = pageSize;
        }

        /// <summary>
        /// Initializes a new instance of the Gobln.Pager.PagedList class from the given list with the default page size.
        /// </summary>
        /// <param name="collection">The source list to paginate</param>
        public PagedList(IEnumerable<T> collection)
            : this(collection, _defaultPageSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Gobln.Pager.PagedList class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// And where the pageSize can be defined.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new pageList.</param>
        /// <param name="pageSize">The size that will be displayed on the page.</param>
        public PagedList(IEnumerable<T> collection, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            _list = new List<T>(collection);
            _pageSize = pageSize;
            _currentPageIndex = 1;

            RecalculatePageCount();
        }

        #endregion Init

        #region Factory

        /// <summary>
        /// Initializes a new instance of the Gobln.Pager.PagedList class from the given list with the default page size.
        /// </summary>
        /// <param name="collection">The source list to paginate</param>
        public static PagedList<T> FromEnumerable(IEnumerable<T> collection)
        {
            return FromEnumerable(collection, _defaultPageSize);
        }

        /// <summary>
        /// Initializes a new instance of the Gobln.Pager.PagedList class from the given list with the given page size.
        /// </summary>
        /// <param name="collection">The source list to paginate</param>
        /// <param name="pageSize">The number of elements displayed per page</param>
        public static PagedList<T> FromEnumerable(IEnumerable<T> collection, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", Resources.ExceptionZeroOrLess);

            var pagedList = new PagedList<T>() { _list = new List<T>(collection) };
            pagedList._pageSize = pageSize;
            pagedList._currentPageIndex = 1;

            pagedList.RecalculatePageCount();

            return pagedList;
        }

        #endregion Factory

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the Gobln.Pager.PagedList.
        /// </summary>
        /// <returns>A Gobln.Pager.PagedList.Enumerator for the Gobln.Pager.PagedList.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Gobln.Pager.PagedList.
        /// </summary>
        /// <returns>A Gobln.Pager.PagedList.Enumerator for the Gobln.Pager.PagedList.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Implementation of IEnumerable

        #region Implementation of ICollection<T>

        /// <summary>
        /// Adds an object to the end of the Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="item">The object to be added to the end of the Gobln.Pager.PagedList. The value can be null for reference types.</param>
        public void Add(T item)
        {
            _list.Add(item);
            RecalculatePageCount();
        }

        /// <summary>
        /// Removes all elements from the Gobln.Pager.PagedList.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            RecalculatePageCount();
        }

        /// <summary>
        /// Determines whether an element is in the Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="item">The object to locate in the Gobln.Pager.PagedList. The value can be null for reference types.</param>
        /// <returns>true if item is found in the Gobln.Pager.PagedList; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies the entire Gobln.Pager.PagedList to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from Gobln.Pager.PagedList. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">The number of elements in the source Gobln.Pager.PagedList is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="item">The object to remove from the Gobln.Pager.PagedList. The value can be null for reference types.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the Gobln.Pager.PagedList.</returns>
        public bool Remove(T item)
        {
            var result = _list.Remove(item);
            RecalculatePageCount();
            return result;
        }

        /// <summary>
        /// Gets the number of elements actually contained in the Gobln.Pager.PagedList.
        /// </summary>
        /// <returns>The number of elements actually contained in the Gobln.Pager.PagedList.</returns>
        public int Count => _list.Count;

#if FEATURE_TYPE_NET40
        int ICollection<T>.Count => Count;
#else
        int IReadOnlyCollection<T>.Count => Count;
#endif

        /// <summary>
        /// Is the Gobln.Pager.PagedList readonly
        /// </summary>
        public bool IsReadOnly => false;

        #endregion Implementation of ICollection<T>

        #region Implementation of IList<T>

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="item">The object to locate in the Gobln.Pager.PagedList. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire Gobln.Pager.PagedList, if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        ///  Inserts an element into the Gobln.Pager.PagedList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than Gobln.Pager.PagedList.Count.</exception>
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            RecalculatePageCount();
        }

        /// <summary>
        /// Removes the element at the specified index of the Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than Gobln.Pager.PagedList.Count.</exception>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            RecalculatePageCount();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than Gobln.Pager.PagedList.Count.</exception>
        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion Implementation of IList<T>

        #region Implementation of IPagedList

        /// <summary>
        /// The current page index
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                if (value < 0)
                    value = 1;
                else if (value > PageCount)
                    value = PageCount;

                _currentPageIndex = value;
            }
        }

        /// <summary>
        /// The amount of items per page
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value < 0)
                    value = 1;
                else if (value == 0)
                    value = int.MaxValue;

                _pageSize = value;
                RecalculatePageCount();
            }
        }

        /// <summary>
        /// The total page count
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// Check if a next page exists
        /// </summary>
        public bool HasNextPage => _currentPageIndex < PageCount;

        /// <summary>
        /// Check if a previous page exist
        /// </summary>
        public bool HasPreviousPage => _currentPageIndex > 1;

        #endregion Implementation of IPagedList

        #region Implementation of IPagedList<T>

        /// <summary>
        /// Retrieves the currently active <see cref="Page{T}"/>
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        public Page<T> GetCurrentPage() => _list.ToPage(CurrentPageIndex, PageSize);

        /// <summary>
        /// Retrieves the <see cref="Page{T}"/> at the given index
        /// </summary>
        /// <param name="pageIndex">Index of the requested page</param>
        /// <returns><see cref="Page{T}"/></returns>
        public Page<T> GetPage(int pageIndex)
        {
            CurrentPageIndex = pageIndex;
            return GetCurrentPage();
        }

        /// <summary>
        /// Loads the next <see cref="Page{T}"/> if possible and returns it.
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        public Page<T> GetNextPage()
        {
            CurrentPageIndex += 1;
            return GetCurrentPage();
        }

        /// <summary>
        /// Loads the previous Page if possible and returns it.
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        public Page<T> GetPreviousPage()
        {
            CurrentPageIndex -= 1;
            return GetCurrentPage();
        }

        /// <summary>
        /// Returns the next Page if possible without loading it (if the active page is the last page, it will return the active page).
        /// </summary>
        /// <returns><see cref="Page{T}"/></returns>
        public Page<T> PeakNextPage()
        {
            if (HasNextPage)
                return _list.ToPage(CurrentPageIndex + 1, PageSize);
            else
                return GetCurrentPage();
        }

        /// <summary>
        /// /// Returns the previous Page if possible without loading it (if the active page is the first page, it will return the active page).
        /// </summary>
        /// <returns></returns>
        public Page<T> PeakPreviousPage()
        {
            if (HasPreviousPage)
                return _list.ToPage(CurrentPageIndex - 1, PageSize);
            else
                return GetCurrentPage();
        }

        /// <summary>
        /// Copies the elements of the <see cref="PagedList{T}"/> to a new List.
        /// </summary>
        /// <returns>An List containing copies of the elements of the <see cref="PagedList{T}"/>.</returns>
        public List<T> ToList()
        {
            return new List<T>(_list);
        }

        /// <summary>
        /// Copies the elements of the <see cref="PagedList{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="PagedList{T}"/>.</returns>
        public T[] ToArray()
        {
            return _list.ToArray();
        }

        #endregion Implementation of IPagedList<T>

        #region Additional

        /// <summary>
        ///  Adds the elements of the specified collection to the end of the Gobln.Pager.PagedList.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the Gobln.Pager.PagedList. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        /// <exception cref="System.ArgumentNullException">collection is null.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            _list.AddRange(collection);
            RecalculatePageCount();
        }

        /// <summary>
        /// Returns a read-only System.Collections.Generic.IList wrapper for the current collection.
        /// </summary>
        /// <returns>A System.Collections.ObjectModel.ReadOnlyCollection that acts as a read-only wrapper around the current Gobln.Pager.PagedList.</returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return _list.AsReadOnly();
        }

        #endregion Additional

        #region Private

        private void RecalculatePageCount()
        {
            PageCount = Calculate.CalculatePageCount(Count, PageSize);

            //By reassigning the CurrentPageIndex to itself, we force the validation again so the CurrentPageIndex remains in scope.
            CurrentPageIndex = CurrentPageIndex;
        }

        #endregion Private
    }
}
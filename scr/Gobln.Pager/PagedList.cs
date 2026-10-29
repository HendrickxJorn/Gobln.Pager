using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Gobln.Pager;

/// <summary>
/// Represents a strongly typed <see cref="PagedList{T}"/>
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class PagedList<T> : IPagedList<T>, IReadOnlyList<T>
{
    #region Members

    internal List<T> _list;
    internal int _currentPageIndex;
    internal int _pageSize;

    #endregion Members

    #region Initialize

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class that is empty and has a page size of 10.
    /// </summary>
    public PagedList()
        : this(10)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class that is empty and has a page size of <see href="pageSize"/>.
    /// </summary>
    /// <param name="pageSize">The number of elements per page</param>
    public PagedList(int pageSize)
    {
        Validator.ValidatePageSize(pageSize);

        _currentPageIndex = 1;
        _pageSize = pageSize;
        _list = [];

        // Without this an empty list reports PageCount 0, and the PageIndex setter then
        // clamps every assignment down to 0. Clear() already establishes 1 as the page
        // count of an empty list; construction has to agree with it.
        RecalculatePageCount();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified source and has a page size of 10.
    /// And where the pageSize can be defined.
    /// </summary>
    /// <param name="collection">The source whose elements are copied to the new <see cref="PagedList{T}"/></param>
    public PagedList(IEnumerable<T> collection)
        : this(collection, 10)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified source and has a page size of <see href="pageSize"/>.
    /// And where the pageSize can be defined.
    /// </summary>
    /// <param name="collection">The source whose elements are copied to the new <see cref="PagedList{T}"/></param>
    /// <param name="pageSize">The number of elements per page</param>
    public PagedList(IEnumerable<T> collection, int pageSize)
        : this(pageSize)
    {
        _list = [.. collection ?? []];

        RecalculatePageCount();
    }

    #endregion Initialize

    #region Implementation of IEnumerable

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <returns>A Enumerator for the <see cref="PagedList{T}"/></returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <returns>A Enumerator for the <see cref="PagedList{T}"/></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion Implementation of IEnumerable

    #region Implementation of ICollection<T>

    /// <summary>
    /// Adds an object to the end of the <see cref="PagedList{T}"/>
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="PagedList{T}"/>. The value can be null for reference types</param>
    public void Add(T item)
    {
        _list.Add(item);
        RecalculatePageCount();
    }

    /// <summary>
    /// Removes all elements from the <see cref="PagedList{T}"/>
    /// </summary>
    public void Clear()
    {
        _list.Clear();
        RecalculatePageCount();
    }

    /// <summary>
    /// Determines whether an element is in the <see cref="PagedList{T}"/>
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="PagedList{T}"/>. The value can be null for reference types</param>
    /// <returns>true if item is found in the <see cref="PagedList{T}"/>; otherwise, false</returns>
    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    /// <summary>
    /// Copies the entire <see cref="PagedList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array
    /// </summary>
    /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="PagedList{T}"/>. The System.Array must have zero-based indexing</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
    /// <exception cref="System.ArgumentNullException">array is null</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source <see cref="PagedList{T}"/> is greater than the available space from arrayIndex to the end of the destination array</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="PagedList{T}"/>. The value can be null for reference types</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the <see cref="PagedList{T}"/></returns>
    public bool Remove(T item)
    {
        var result = _list.Remove(item);
        RecalculatePageCount();
        return result;
    }

    /// <summary>
    /// Gets the number of elements actually contained in the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <returns>The number of elements actually contained in the <see cref="PagedList{T}"/></returns>
    public int Count => _list.Count;

    int IReadOnlyCollection<T>.Count => Count;

    /// <summary>
    /// Is the <see cref="PagedList{T}"/> readonly
    /// </summary>
    public bool IsReadOnly => false;

    #endregion Implementation of ICollection<T>

    #region Implementation of IList<T>

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="PagedList{T}"/>. The value can be null for reference types</param>
    /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="PagedList{T}"/>, if found; otherwise, –1</returns>
    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    /// <summary>
    ///  Inserts an element into the <see cref="PagedList{T}"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted</param>
    /// <param name="item">The object to insert. The value can be null for reference types</param>
    /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0 or the index is greater than amount of items in the <see cref="PagedList{T}"/></exception>
    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
        RecalculatePageCount();
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove</param>
    /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than Gobln.Pager.PagedList.Count</exception>
    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
        RecalculatePageCount();
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set</param>
    /// <returns>The element at the specified index</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than Gobln.Pager.PagedList.Count</exception>
    public T this[int index]
    {
        get
        {
            return _list[index];
        }
        set
        {
            _list[index] = value;
        }
    }

    #endregion Implementation of IList<T>

    #region Implementation of IPagedList

    /// <summary>
    /// The page index
    /// This value can not be lower then 1 or greater then the <see cref="PageCount" />
    /// </summary>
    public int PageIndex
    {
        get
        {
            return _currentPageIndex;
        }
        set
        {
            if (value < 1)
            {
                value = 1;
            }
            else if (PageCount < value)
            {
                value = PageCount;
            }

            _currentPageIndex = value;
        }
    }

    /// <summary>
    /// The amount of items per page
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 1, as in the constructor</exception>
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            Validator.ValidatePageSize(value);

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
    /// Retrieves the <see cref="Page"/> at the given index
    /// </summary>
    /// <param name="pageIndex">Index of the requested page</param>
    /// <returns><see cref="Page{T}"/></returns>
    public Page<T> GetPage(int pageIndex) => _list.ToPage(pageIndex, PageSize);

    /// <summary>
    /// Retrieves the currently active <see cref="Page"/>
    /// </summary>
    /// <returns><see cref="Page{T}"/></returns>
    public Page<T> GetPage() => GetPage(PageIndex);

    /// <summary>
    /// Go to the next <see cref="Page"/>. Can not go beyond the last <see cref="Page"/>.
    /// </summary>
    /// <returns><see cref="PagedList{T}"/></returns>
    public PagedList<T> NextPage()
    {
        PageIndex += 1;
        return this;
    }

    /// <summary>
    /// Go to the previous <see cref="Page"/>. Can not go beyond the fist <see cref="Page"/>.
    /// </summary>
    /// <returns><see cref="PagedList{T}"/></returns>
    public PagedList<T> PreviousPage()
    {
        PageIndex -= 1;
        return this;
    }

    /// <summary>
    /// Get all the Pages
    /// </summary>
    /// <returns><see cref="IEnumerable"/> of <see cref="Page{T}"/></returns>
    public IEnumerable<Page<T>> GetPages()
    {
        for (int i = 1; i <= PageCount; i++)
        {
            yield return this.GetPage(i);
        }
    }

    #endregion Implementation of IPagedList<T>

    #region Additional

    /// <summary>
    ///  Adds the elements of the specified collection to the end of the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added to the end of the <see cref="PagedList{T}"/>. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type</param>
    /// <exception cref="System.ArgumentNullException">collection is null</exception>
    public void AddRange(IEnumerable<T> collection)
    {
        _list.AddRange(collection);
        RecalculatePageCount();
    }

    /// <summary>
    /// Returns a read-only <see cref="IList"/> wrapper for the current collection.
    /// </summary>
    /// <returns>A System.Collections.ObjectModel.ReadOnlyCollection that acts as a read-only wrapper around the current <see cref="PagedList{T}"/></returns>
    public ReadOnlyCollection<T> AsReadOnly()
    {
        return new List<T>(_list).AsReadOnly();
    }

    #endregion Additional

    #region Private

    internal void RecalculatePageCount()
    {
        PageCount = Count == 0 ? 1 : ((Count - 1) / PageSize) + 1;

        //By reassigning the CurrentPageIndex to itself, we force the validation again so the CurrentPageIndex remains in scope.
        PageIndex = PageIndex;
    }

    #endregion Private
}

/// <summary>
/// Creates instances of <see cref="PagedList{T}"/>
/// </summary>
public static class PagedList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class from the given list with the given page size.
    /// </summary>
    /// <remarks>
    /// A plain static method, not an extension: <c>ToPagedList()</c> is the extension that does the same.
    /// </remarks>
    /// <typeparam name="T">The type of the items</typeparam>
    /// <param name="source">The items</param>
    /// <param name="pageSize">The number of elements displayed per page</param>
    /// <returns>A list holding a copy of the items</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageSize"/> is less than 1</exception>
    public static PagedList<T> FromEnumerable<T>(IEnumerable<T> source, int pageSize = 10)
    {
        Validator.ValidatePageSize(pageSize);

        return new PagedList<T>(source, pageSize);
    }
}
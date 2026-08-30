using Gobln.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// The paging parameter rules -- pageIndex >= 1, pageSize >= 1, totalItemCount >= 0 --
/// are enforced by Validator on every public entry point. These pin them.
///
/// Plain int sequences are used on purpose: nothing here depends on the element type,
/// and ints keep the assertions about the rule rather than about the data.
/// </summary>
public class ValidationTest
{
    private static IEnumerable<int> Source => Enumerable.Range(1, 100);

    private static IQueryable<int> QuerySource => Enumerable.Range(1, 100).AsQueryable();

    #region Page<T> constructor

    [Fact]
    public void Page_Constructor_PageIndexBelowOne_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<int>(Source, 0, 10, 100));

        Assert.Equal("pageIndex", ex.ParamName);
    }

    [Fact]
    public void Page_Constructor_PageSizeBelowOne_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<int>(Source, 1, 0, 100));

        Assert.Equal("pageSize", ex.ParamName);
    }

    [Fact]
    public void Page_Constructor_NegativeTotalItemCount_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<int>(Source, 1, 10, -1));

        Assert.Equal("totalItemCount", ex.ParamName);
    }

    #endregion Page<T> constructor

    #region Page.FromEnumerable

    [Fact]
    public void Page_FromEnumerable_PageIndexBelowOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Page.FromEnumerable(Source, 0, 10));
    }

    [Fact]
    public void Page_FromEnumerable_PageSizeBelowOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Page.FromEnumerable(Source, 1, 0));
    }

    [Fact]
    public void Page_FromEnumerable_NegativeTotalItemCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Page.FromEnumerable(Source, 1, 10, -1));
    }

    #endregion Page.FromEnumerable

    #region IEnumerable ToPage

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void Enumerable_ToPage_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(pageIndex, pageSize));
    }

    [Fact]
    public void Enumerable_ToPage_NegativeTotalItemCount_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(1, 10, -1));

        Assert.Equal("totalItemCount", ex.ParamName);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public void Enumerable_ToPage_InvalidPageFilter_Throws(int pageIndex, int pageSize)
    {
        var filter = new PageFilter(pageIndex, pageSize);

        Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(filter));
    }

    [Fact]
    public void Enumerable_ToPagedList_PageSizeBelowOne_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPagedList(0));

        Assert.Equal("pageSize", ex.ParamName);
    }

    #endregion IEnumerable ToPage

    #region IQueryable ToPage

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public void Queryable_ToPage_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => QuerySource.ToPage(pageIndex, pageSize));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public void Queryable_ToPage_InvalidPageFilter_Throws(int pageIndex, int pageSize)
    {
        var filter = new PageFilter(pageIndex, pageSize);

        Assert.Throws<ArgumentOutOfRangeException>(() => QuerySource.ToPage(filter));
    }

    [Fact]
    public void Queryable_ToPagedList_PageSizeBelowOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => QuerySource.ToPagedList(0));
    }

    #endregion IQueryable ToPage

    #region Async

    // The async overloads validate before their first await, so the exception surfaces
    // on the returned Task rather than at the call -- ThrowsAsync, not Throws.

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public async Task Enumerable_ToPageAsync_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => Source.ToPageAsync(pageIndex, pageSize));
    }

    [Fact]
    public async Task Enumerable_ToPagedListAsync_PageSizeBelowOne_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => Source.ToPagedListAsync(0));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public async Task Queryable_ToPageAsync_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => QuerySource.ToPageAsync(pageIndex, pageSize));
    }

    #endregion Async

    #region PagedList<T>

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PagedList_Constructor_PageSizeBelowOne_Throws(int pageSize)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<int>(pageSize));

        Assert.Equal("pageSize", ex.ParamName);
    }

    [Fact]
    public void PagedList_ConstructorWithCollection_PageSizeBelowOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PagedList<int>(Source, 0));
    }

    [Fact]
    public void PagedList_AddRange_Null_Throws()
    {
        var pagedList = new PagedList<int>(Source, 10);

        Assert.Throws<ArgumentNullException>(() => pagedList.AddRange(null!));
    }

    #endregion PagedList<T>
}

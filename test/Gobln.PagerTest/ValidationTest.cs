using Gobln.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// The paging parameter rules -- pageIndex >= 1, pageSize >= 1 (or 0 without a totalItemCount,
/// or with one of 0), totalItemCount >= 0 -- are enforced by Validator on every public entry
/// point. These pin them.
///
/// Plain int sequences are used on purpose: nothing here depends on the element type,
/// and ints keep the assertions about the rule rather than about the data.
/// </summary>
public class ValidationTest
{
    private static IEnumerable<int> Source => Enumerable.Range(1, 100);

    private static IQueryable<int> QuerySource => Enumerable.Range(1, 100).AsQueryable();

    // A page size of 0 without a total makes an empty page as with a total of 0: nothing is counted or read
    private static void AssertEmptyWithoutPageSize(Page<int> page)
    {
        Assert.Empty(page);
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(0, page.PageSize);
        Assert.Equal(0, page.TotalItemCount);
        Assert.Equal(1, page.TotalPageCount);
    }

    // Throws when it is counted or read
    private sealed class UnreadableSource : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => throw new InvalidOperationException("The source was read");

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

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

    [Fact]
    public void Page_Constructor_PageWithoutAPageSize_CanBeMadeAgainFromItsOwnValues()
    {
        // new Page<T>() reports a PageSize of 0, which is allowed when there are no items in total
        var page = new Page<int>();

        var copy = new Page<int>(page, page.PageIndex, page.PageSize, page.TotalItemCount);

        Assert.Equal(0, copy.PageSize);
        Assert.Equal(0, copy.TotalItemCount);
        Assert.Equal(1, copy.TotalPageCount);
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
    public void Page_FromEnumerable_NegativePageSize_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Page.FromEnumerable(Source, 1, -1));
    }

    [Fact]
    public void Page_FromEnumerable_PageSizeZero_IsAnEmptyPage_WithoutReadingTheSource()
    {
        AssertEmptyWithoutPageSize(Page.FromEnumerable(new UnreadableSource(), 1, 0));
    }

    [Fact]
    public void Page_FromEnumerable_NegativeTotalItemCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Page.FromEnumerable(Source, 1, 10, -1));
    }

    [Fact]
    public void Page_FromEnumerable_PageSizeZero_WithTotalItemCountZero_IsAllowed()
    {
        Assert.Equal(0, Page.FromEnumerable(Source, 1, 0, 0).PageSize);
    }

    #endregion Page.FromEnumerable

    #region IEnumerable ToPage

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
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

    [Fact]
    public void Enumerable_ToPage_PageSizeZero_IsAnEmptyPage_WithoutReadingTheSource()
    {
        AssertEmptyWithoutPageSize(new UnreadableSource().ToPage(5, 0));
        AssertEmptyWithoutPageSize(new UnreadableSource().ToPage(new PageFilter(5, 0)));
    }

    [Fact]
    public void Enumerable_ToPage_PageSizeZero_WithTotalItemCountZero_IsAllowed()
    {
        Assert.Equal(0, Source.ToPage(1, 0, 0).PageSize);
        Assert.Equal(0, Source.ToPage(1, 0, 0, prePaged: false).PageSize);
        Assert.Equal(0, Source.ToPage(new PageFilter(1, 0), 0).PageSize);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(-1, 0)]
    public void Enumerable_ToPage_WithTotalItemCount_InvalidPageSize_Throws(int pageSize, int totalItemCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(1, pageSize, totalItemCount));
        Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(new PageFilter(1, pageSize), totalItemCount));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, -1)]
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
    [InlineData(1, -1)]
    public void Queryable_ToPage_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => QuerySource.ToPage(pageIndex, pageSize));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, -1)]
    public void Queryable_ToPage_InvalidPageFilter_Throws(int pageIndex, int pageSize)
    {
        var filter = new PageFilter(pageIndex, pageSize);

        Assert.Throws<ArgumentOutOfRangeException>(() => QuerySource.ToPage(filter));
    }

    [Fact]
    public void Queryable_ToPage_PageSizeZero_IsAnEmptyPage_WithoutRunningAQuery()
    {
        var query = RecordingQueryable.From(Source, out var provider);

        AssertEmptyWithoutPageSize(query.ToPage(5, 0));
        AssertEmptyWithoutPageSize(query.ToPage(new PageFilter(5, 0)));
        Assert.Empty(provider.Executed);
        Assert.Empty(provider.Enumerated);
    }

    [Fact]
    public void Queryable_ToPage_PageSizeZero_WithTotalItemCountZero_IsAllowed()
    {
        Assert.Equal(0, QuerySource.ToPage(1, 0, 0).PageSize);
        Assert.Equal(0, QuerySource.ToPage(new PageFilter(1, 0), 0).PageSize);
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
    [InlineData(1, -1)]
    public async Task Enumerable_ToPageAsync_InvalidPaging_Throws(int pageIndex, int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => Source.ToPageAsync(pageIndex, pageSize));
    }

    [Fact]
    public async Task Enumerable_ToPageAsync_PageSizeZero_IsAnEmptyPage_WithoutReadingTheSource()
    {
        AssertEmptyWithoutPageSize(await new UnreadableSource().ToPageAsync(5, 0));
        AssertEmptyWithoutPageSize(await new UnreadableSource().ToPageAsync(new PageFilter(5, 0)));
    }

    [Fact]
    public async Task Queryable_ToPageAsync_PageSizeZero_IsAnEmptyPage_WithoutRunningAQuery()
    {
        var query = RecordingQueryable.From(Source, out var provider);

        AssertEmptyWithoutPageSize(await query.ToPageAsync(5, 0));
        AssertEmptyWithoutPageSize(await query.ToPageAsync(new PageFilter(5, 0)));
        Assert.Empty(provider.Executed);
        Assert.Empty(provider.Enumerated);
    }

    [Fact]
    public async Task Enumerable_ToPageAsync_PageSizeZero_WithTotalItemCountZero_IsAllowed()
    {
        Assert.Equal(0, (await Source.ToPageAsync(1, 0, 0)).PageSize);
        Assert.Equal(0, (await Source.ToPageAsync(new PageFilter(1, 0), 0)).PageSize);
    }

    [Fact]
    public async Task Enumerable_ToPagedListAsync_PageSizeBelowOne_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => Source.ToPagedListAsync(0));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, -1)]
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PagedList_PageSizeSetter_BelowOne_Throws(int pageSize)
    {
        // The same rule as the constructor, which throws too
        var pagedList = new PagedList<int>(Source, 10);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => pagedList.PageSize = pageSize);

        Assert.Equal("pageSize", ex.ParamName);
        Assert.Equal(10, pagedList.PageSize);
    }

    #endregion PagedList<T>

    #region Exceptions

    [Fact]
    public void NullPageFilter_ThrowsArgumentNull()
    {
        Assert.Equal("pagerFilter", Assert.Throws<ArgumentNullException>(() => Source.ToPage((IPageFilter)null!)).ParamName);
        Assert.Equal("pagerFilter", Assert.Throws<ArgumentNullException>(() => QuerySource.ToPage((IPageFilter)null!, 100)).ParamName);
    }

    [Fact]
    public async Task NullPageFilter_Async_ThrowsArgumentNull()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => Source.ToPageAsync((IPageFilter)null!));

        Assert.Equal("pagerFilter", ex.ParamName);
    }

    [Theory]
    [InlineData(0, 10, "pagerFilter.PageIndex")]
    [InlineData(1, -1, "pagerFilter.PageSize")]
    public void InvalidPageFilter_NamesTheParameterAndItsProperty(int pageIndex, int pageSize, string paramName)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(new PageFilter(pageIndex, pageSize)));

        Assert.Equal(paramName, ex.ParamName);
    }

    [Fact]
    public void OutOfRange_CarriesTheValueThatWasPassed()
    {
        Assert.Equal(-3, Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(-3, 10)).ActualValue);
        Assert.Equal(-1, Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(new PageFilter(1, -1))).ActualValue);
        Assert.Equal(-1, Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(1, 10, -1)).ActualValue);
    }

    [Fact]
    public void OutOfRange_SaysWhatIsAllowed()
    {
        Assert.StartsWith("Must be greater than zero.", Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(0, 10)).Message);
        Assert.StartsWith("Can't be less than zero.", Assert.Throws<ArgumentOutOfRangeException>(() => Source.ToPage(1, 10, -1)).Message);
    }

    #endregion Exceptions
}

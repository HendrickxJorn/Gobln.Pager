using Gobln.Pager;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// Paging state and the IList/ICollection surface of <see cref="PagedList{T}"/>.
///
/// PageIndex clamps on assignment (a PageSize below 1 throws, see ValidationTest), and every mutation recalculates
/// the page count -- which can pull PageIndex back when the list shrinks. Plain int
/// sequences keep the assertions about that state machine rather than about the data.
/// </summary>
public class PagedListStateTest
{
    private static PagedList<int> Create(int itemCount = 25, int pageSize = 10)
        => new(Enumerable.Range(1, itemCount), pageSize);

    #region PageIndex clamping

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void PageIndex_BelowOne_ClampsToOne(int pageIndex)
    {
        var pagedList = Create();

        pagedList.PageIndex = pageIndex;

        Assert.Equal(1, pagedList.PageIndex);
    }

    [Fact]
    public void PageIndex_AbovePageCount_ClampsToPageCount()
    {
        var pagedList = Create();

        pagedList.PageIndex = 999;

        Assert.Equal(pagedList.PageCount, pagedList.PageIndex);
        Assert.Equal(3, pagedList.PageIndex);
    }

    [Fact]
    public void PageIndex_OnEmptyList_StaysOne()
    {
        // An empty list still has one (empty) page, so 1 has to remain assignable.
        var pagedList = new PagedList<int>(10);

        Assert.Equal(1, pagedList.PageCount);

        pagedList.PageIndex = 1;

        Assert.Equal(1, pagedList.PageIndex);
    }

    #endregion PageIndex clamping

    #region PageSize

    [Fact]
    public void PageSize_Change_RecalculatesPageCount()
    {
        var pagedList = Create(itemCount: 25, pageSize: 10);

        Assert.Equal(3, pagedList.PageCount);

        pagedList.PageSize = 5;

        Assert.Equal(5, pagedList.PageCount);
    }

    #endregion PageSize

    #region Recalculation on mutation

    [Fact]
    public void Removing_Items_ShrinksPageCount_AndPullsBackPageIndex()
    {
        var pagedList = Create(itemCount: 25, pageSize: 10);

        pagedList.PageIndex = 3;

        for (var i = 0; i < 10; i++)
        {
            pagedList.RemoveAt(pagedList.Count - 1);
        }

        Assert.Equal(15, pagedList.Count);
        Assert.Equal(2, pagedList.PageCount);
        Assert.Equal(2, pagedList.PageIndex);
    }

    [Fact]
    public void Clear_EmptiesListAndResetsPaging()
    {
        var pagedList = Create();

        pagedList.PageIndex = 3;
        pagedList.Clear();

        Assert.Equal(0, pagedList.Count);
        Assert.Equal(1, pagedList.PageCount);
        Assert.Equal(1, pagedList.PageIndex);
    }

    [Fact]
    public void Add_GrowsPageCount()
    {
        var pagedList = new PagedList<int>(10);

        Assert.Equal(1, pagedList.PageCount);

        for (var i = 1; i <= 11; i++)
        {
            pagedList.Add(i);
        }

        Assert.Equal(2, pagedList.PageCount);
    }

    #endregion Recalculation on mutation

    #region Navigation flags

    [Fact]
    public void HasPreviousPage_FalseOnFirstPage_TrueAfterwards()
    {
        var pagedList = Create();

        pagedList.PageIndex = 1;
        Assert.False(pagedList.HasPreviousPage);

        pagedList.PageIndex = 2;
        Assert.True(pagedList.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_TrueUntilLastPage()
    {
        var pagedList = Create();

        pagedList.PageIndex = 1;
        Assert.True(pagedList.HasNextPage);

        pagedList.PageIndex = pagedList.PageCount;
        Assert.False(pagedList.HasNextPage);
    }

    [Fact]
    public void NextPage_And_PreviousPage_DoNotRunPastTheEnds()
    {
        var pagedList = Create();

        pagedList.PageIndex = 1;
        pagedList.PreviousPage();
        Assert.Equal(1, pagedList.PageIndex);

        pagedList.PageIndex = pagedList.PageCount;
        pagedList.NextPage();
        Assert.Equal(pagedList.PageCount, pagedList.PageIndex);
    }

    #endregion Navigation flags

    #region IList / ICollection surface

    [Fact]
    public void IsReadOnly_IsFalse()
    {
        Assert.False(Create().IsReadOnly);
    }

    [Fact]
    public void Indexer_GetsAndSetsByAbsoluteIndex()
    {
        var pagedList = Create();

        Assert.Equal(1, pagedList[0]);

        pagedList[0] = 99;

        Assert.Equal(99, pagedList[0]);
    }

    [Fact]
    public void Contains_And_IndexOf_FindItems()
    {
        var pagedList = Create();

        Assert.True(pagedList.Contains(7));
        Assert.False(pagedList.Contains(999));
        Assert.Equal(6, pagedList.IndexOf(7));
        Assert.Equal(-1, pagedList.IndexOf(999));
    }

    [Fact]
    public void Insert_PlacesItemAtIndex()
    {
        var pagedList = Create();

        pagedList.Insert(0, 99);

        Assert.Equal(99, pagedList[0]);
        Assert.Equal(26, pagedList.Count);
    }

    [Fact]
    public void Remove_RemovesFirstMatchAndReportsResult()
    {
        var pagedList = Create();

        Assert.True(pagedList.Remove(7));
        Assert.False(pagedList.Contains(7));
        Assert.Equal(24, pagedList.Count);

        Assert.False(pagedList.Remove(999));
        Assert.Equal(24, pagedList.Count);
    }

    [Fact]
    public void CopyTo_CopiesEveryItem()
    {
        var pagedList = Create();

        var target = new int[pagedList.Count];

        pagedList.CopyTo(target, 0);

        Assert.Equal(Enumerable.Range(1, 25), target);
    }

    [Fact]
    public void AsReadOnly_ReturnsSnapshotDetachedFromLaterChanges()
    {
        var pagedList = Create();

        var readOnly = pagedList.AsReadOnly();

        Assert.Equal(25, readOnly.Count);

        pagedList.Add(26);

        Assert.Equal(25, readOnly.Count);
    }

    #endregion IList / ICollection surface

    #region Pages

    [Fact]
    public void GetPage_ReturnsRequestedSlice()
    {
        var pagedList = Create(itemCount: 25, pageSize: 10);

        Assert.Equal(Enumerable.Range(1, 10), pagedList.GetPage(1));
        Assert.Equal(Enumerable.Range(11, 10), pagedList.GetPage(2));
        Assert.Equal(Enumerable.Range(21, 5), pagedList.GetPage(3));
    }

    [Fact]
    public void GetPage_WithoutArgument_ReturnsCurrentPage()
    {
        var pagedList = Create(itemCount: 25, pageSize: 10);

        pagedList.PageIndex = 2;

        Assert.Equal(Enumerable.Range(11, 10), pagedList.GetPage());
    }

    [Fact]
    public void GetPages_YieldsEveryPageOnce()
    {
        var pagedList = Create(itemCount: 25, pageSize: 10);

        var pages = pagedList.GetPages().ToList();

        Assert.Equal(3, pages.Count);
        Assert.Equal(Enumerable.Range(1, 25), pages.SelectMany(p => p));
    }

    #endregion Pages

    #region FromEnumerable

    [Fact]
    public void FromEnumerable_IsAStaticMethod_NotAnExtensionOnEverySequence()
    {
        // As an extension it showed up on every IEnumerable, next to ToPagedList, which does the same
        var method = typeof(PagedList).GetMethod(nameof(PagedList.FromEnumerable));

        Assert.NotNull(method);
        Assert.False(method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), inherit: false));
    }

    [Fact]
    public void FromEnumerable_MakesAPagedList()
    {
        var pagedList = PagedList.FromEnumerable(Enumerable.Range(1, 25), pageSize: 5);

        Assert.Equal(25, pagedList.Count);
        Assert.Equal(5, pagedList.PageCount);
    }

    #endregion FromEnumerable
}

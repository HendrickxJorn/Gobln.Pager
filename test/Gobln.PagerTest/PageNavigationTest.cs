using Gobln.Pager;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// The navigation helpers every <see cref="IPage"/> gets: whether there is a page before or
/// after it, and which items it holds, as in "Showing 11-20 of 25".
/// </summary>
public class PageNavigationTest
{
    private sealed class CustomPage : IPage
    {
        public int PageIndex { get; init; }

        public int PageSize { get; init; }

        public int TotalItemCount { get; init; }

        public int TotalPageCount { get; init; }

        public bool HasPaging => TotalPageCount > 1;

        public bool IsEmpty => false;
    }

    [Theory]
    [InlineData(1, false, true, 1, 10)]
    [InlineData(2, true, true, 11, 20)]
    [InlineData(3, true, false, 21, 25)]
    public void Page_KnowsItsNeighboursAndItsItems(int pageIndex, bool hasPrevious, bool hasNext, int first, int last)
    {
        // 25 items at 10 per page: the last page holds 21 to 25
        var page = Enumerable.Range(1, 25).ToPage(pageIndex, 10);

        Assert.Equal(hasPrevious, page.HasPreviousPage());
        Assert.Equal(hasNext, page.HasNextPage());
        Assert.Equal(first, page.FirstItemOnPage());
        Assert.Equal(last, page.LastItemOnPage());
    }

    [Fact]
    public void SinglePage_HasNoNeighbours()
    {
        var page = Enumerable.Range(1, 5).ToPage(1, 10);

        Assert.False(page.HasPreviousPage());
        Assert.False(page.HasNextPage());
        Assert.Equal(1, page.FirstItemOnPage());
        Assert.Equal(5, page.LastItemOnPage());
    }

    [Fact]
    public void EmptyPage_HasNoItems()
    {
        var page = new List<int>().ToPage(1, 10);

        Assert.False(page.HasPreviousPage());
        Assert.False(page.HasNextPage());
        Assert.Equal(0, page.FirstItemOnPage());
        Assert.Equal(0, page.LastItemOnPage());
    }

    [Fact]
    public void CustomPage_GetsTheHelpersToo()
    {
        // They are extension members: any IPage has them, not only Page<T>
        IPage page = new CustomPage { PageIndex = 2, PageSize = 20, TotalItemCount = 45, TotalPageCount = 3 };

        Assert.True(page.HasPreviousPage());
        Assert.True(page.HasNextPage());
        Assert.Equal(21, page.FirstItemOnPage());
        Assert.Equal(40, page.LastItemOnPage());
    }
}

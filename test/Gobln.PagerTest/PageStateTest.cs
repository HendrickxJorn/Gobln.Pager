using Gobln.Pager;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// The two computed flags on <see cref="Page{T}"/> -- IsEmpty and HasPaging -- plus the
/// metadata a default-constructed page reports.
/// </summary>
public class PageStateTest
{
    [Fact]
    public void DefaultPage_IsEmpty_AndReportsASinglePage()
    {
        var page = new Page<int>();

        Assert.True(page.IsEmpty);
        Assert.False(page.HasPaging);
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(1, page.TotalPageCount);
        Assert.Equal(0, page.PageSize);
        Assert.Equal(0, page.TotalItemCount);
        Assert.Empty(page);
    }

    [Fact]
    public void EmptySource_ProducesAnEmptyPage()
    {
        var page = new List<int>().ToPage(1, 10);

        Assert.True(page.IsEmpty);
        Assert.False(page.HasPaging);
        Assert.Equal(0, page.TotalItemCount);
    }

    [Fact]
    public void PageWithItems_IsNotEmpty()
    {
        var page = Enumerable.Range(1, 5).ToPage(1, 10);

        Assert.False(page.IsEmpty);
    }

    [Fact]
    public void HasPaging_IsFalse_WhenEverythingFitsOnOnePage()
    {
        var page = Enumerable.Range(1, 5).ToPage(1, 10);

        Assert.False(page.HasPaging);
        Assert.Equal(1, page.TotalPageCount);
    }

    [Fact]
    public void HasPaging_IsTrue_WhenTheSourceSpansSeveralPages()
    {
        var page = Enumerable.Range(1, 25).ToPage(1, 10);

        Assert.True(page.HasPaging);
        Assert.Equal(3, page.TotalPageCount);
        Assert.Equal(25, page.TotalItemCount);
        Assert.Equal(Enumerable.Range(1, 10), page);
    }

    [Fact]
    public void LastPartialPage_IsNotEmpty_AndStillReportsPaging()
    {
        // 25 items at 10 per page leaves 5 on the last page.
        var page = Enumerable.Range(1, 25).ToPage(3, 10);

        Assert.False(page.IsEmpty);
        Assert.True(page.HasPaging);
        Assert.Equal(3, page.PageIndex);
        Assert.Equal(Enumerable.Range(21, 5), page);
    }
}

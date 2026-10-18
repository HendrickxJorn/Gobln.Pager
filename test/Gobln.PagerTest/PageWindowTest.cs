using Gobln.Pager;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// Which page numbers a pager shows. The MVC helper and the Blazor component both render
/// what PageWindow gives them, so these pin the numbers for both.
/// </summary>
public class PageWindowTest
{
    /// <summary>
    /// The numbers as text, with "..." for an ellipsis.
    /// </summary>
    private static string Numbers(int pageIndex, int totalPageCount, int siblingCount, int boundaryCount, bool showEllipsis)
    {
        return string.Join(" ", PageWindow
            .GetPageNumbers(pageIndex, totalPageCount, siblingCount, boundaryCount, showEllipsis)
            .Select(number => number?.ToString() ?? "..."));
    }

    [Theory]
    [InlineData(1, 10, "1 2 3 4 5 ... 10")]
    [InlineData(5, 10, "1 ... 4 5 6 ... 10")]
    [InlineData(10, 10, "1 ... 6 7 8 9 10")]
    public void WithEllipsis_ShowsTheBoundariesAndThePagesAroundTheCurrentOne(int pageIndex, int totalPageCount, string expected)
    {
        Assert.Equal(expected, Numbers(pageIndex, totalPageCount, siblingCount: 1, boundaryCount: 1, showEllipsis: true));
    }

    [Fact]
    public void WithEllipsis_AnEllipsisThatWouldHideOnePage_ShowsThatPageInstead()
    {
        // Between boundary page 1 and the window 3 4 5 only page 2 is skipped
        Assert.Equal("1 2 3 4 5 ... 10", Numbers(4, 10, siblingCount: 1, boundaryCount: 1, showEllipsis: true));
    }

    [Fact]
    public void WithEllipsis_ShowsTheSameNumberOfItemsOnEveryPage()
    {
        // The pager then doesn't change width while paging
        var failures = new List<string>();

        foreach (var totalPageCount in Enumerable.Range(1, 30))
        foreach (var boundaryCount in Enumerable.Range(0, 4))
        foreach (var siblingCount in Enumerable.Range(0, 3))
        {
            var counts = Enumerable.Range(1, totalPageCount)
                .Select(pageIndex => PageWindow.GetPageNumbers(pageIndex, totalPageCount, siblingCount, boundaryCount, showEllipsis: true).Count)
                .Distinct()
                .ToArray();

            if (counts.Length != 1)
            {
                failures.Add($"pages={totalPageCount} boundary={boundaryCount} sibling={siblingCount}: {string.Join(", ", counts)} items");
            }
        }

        Assert.True(failures.Count == 0, $"{failures.Count} combinations failed:\n{string.Join("\n", failures.Take(20))}");
    }

    [Theory]
    [InlineData(1, "1 4 5 6 10")]
    [InlineData(0, "4 5 6")]
    public void WithoutEllipsis_SkipsPagesWithoutMarkingIt(int boundaryCount, string expected)
    {
        Assert.Equal(expected, Numbers(5, 10, siblingCount: 1, boundaryCount, showEllipsis: false));
    }

    [Theory]
    [InlineData(1, 1, "1")]
    [InlineData(3, 2, "1 2 3")]
    [InlineData(5, 3, "1 2 3 4 5")]
    public void OverlappingBoundaries_ShowEachPageOnce(int totalPageCount, int boundaryCount, string expected)
    {
        Assert.Equal(expected, Numbers(1, totalPageCount, siblingCount: 1, boundaryCount, showEllipsis: true));
    }

    [Fact]
    public void NegativeCounts_AreTreatedAsZero()
    {
        Assert.Equal("5", Numbers(5, 10, siblingCount: 0, boundaryCount: 0, showEllipsis: false));
        Assert.Equal("5", Numbers(5, 10, siblingCount: -1, boundaryCount: -1, showEllipsis: false));
    }

    [Fact]
    public void NoPages_ShowsNothing()
    {
        Assert.Empty(PageWindow.GetPageNumbers(1, 0, siblingCount: 1, boundaryCount: 1, showEllipsis: true));
    }

    [Fact]
    public void NoCombination_ShowsAPageTwiceOutOfOrderOrOutOfRange()
    {
        var failures = new List<string>();

        foreach (var totalPageCount in Enumerable.Range(1, 12))
        foreach (var boundaryCount in Enumerable.Range(0, 4))
        foreach (var siblingCount in Enumerable.Range(0, 3))
        foreach (var showEllipsis in new[] { true, false })
        foreach (var pageIndex in Enumerable.Range(1, totalPageCount))
        {
            var window = PageWindow.GetPageNumbers(pageIndex, totalPageCount, siblingCount, boundaryCount, showEllipsis);

            var numbers = window.Where(number => number is not null).Select(number => number!.Value).ToArray();

            var strictlyIncreasing = numbers.Zip(numbers.Skip(1), (a, b) => a < b).All(ok => ok);
            var inRange = numbers.All(number => number >= 1 && number <= totalPageCount);
            var showsCurrentPage = numbers.Contains(pageIndex);
            var ellipsisOnlyWhenAsked = showEllipsis || numbers.Length == window.Count;

            if (!(strictlyIncreasing && inRange && showsCurrentPage && ellipsisOnlyWhenAsked))
            {
                failures.Add($"pages={totalPageCount} boundary={boundaryCount} sibling={siblingCount} ellipsis={showEllipsis} page={pageIndex}: {Numbers(pageIndex, totalPageCount, siblingCount, boundaryCount, showEllipsis)}");
            }
        }

        Assert.True(failures.Count == 0, $"{failures.Count} combinations failed:\n{string.Join("\n", failures.Take(20))}");
    }

    [Fact]
    public void AnEllipsis_AlwaysHidesMoreThanOnePage()
    {
        var failures = new List<string>();

        foreach (var totalPageCount in Enumerable.Range(1, 12))
        foreach (var boundaryCount in Enumerable.Range(0, 4))
        foreach (var siblingCount in Enumerable.Range(0, 3))
        foreach (var pageIndex in Enumerable.Range(1, totalPageCount))
        {
            var window = PageWindow.GetPageNumbers(pageIndex, totalPageCount, siblingCount, boundaryCount, showEllipsis: true);

            for (var i = 0; i < window.Count; i++)
            {
                if (window[i] is not null)
                {
                    continue;
                }

                // An ellipsis at an end of the list (no boundary pages) hides everything up to that end
                var before = i > 0 ? window[i - 1] : 0;
                var after = i < window.Count - 1 ? window[i + 1] : totalPageCount + 1;

                if (before is null || after is null || after - before - 1 < 2)
                {
                    failures.Add($"pages={totalPageCount} boundary={boundaryCount} sibling={siblingCount} page={pageIndex}: {Numbers(pageIndex, totalPageCount, siblingCount, boundaryCount, true)}");
                }
            }
        }

        Assert.True(failures.Count == 0, $"{failures.Count} combinations failed:\n{string.Join("\n", failures.Take(20))}");
    }
}

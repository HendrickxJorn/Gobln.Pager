using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using System.Text.RegularExpressions;
using Xunit;

namespace Gobln.Pager.MvcTest;

/// <summary>
/// Which page numbers the pager renders, independent of the markup around them.
/// </summary>
public class PageNumbersTest
{
    private static readonly Regex PageLinkText = new("class=\"page-link\"[^>]*>([^<]*)<");

    private readonly HelperHtml _helperHtml = new();

    /// <summary>
    /// Renders only the numbered links (no First/Previous/Next/Last) and returns their
    /// texts in order, ellipses included.
    /// </summary>
    private string[] RenderPageNumbers(int totalPages, int pageIndex, Action<PagerOptions>? configure = null)
    {
        return RenderPageNumbers(Enumerable.Range(1, totalPages).ToPage(pageIndex, 1), configure);
    }

    private string[] RenderPageNumbers(IPage page, Action<PagerOptions>? configure = null)
    {
        var options = new PagerOptions
        {
            HRefUrl = "#",
            ShowPager = WhenToShowEnum.Always,
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonPreviousLink = WhenToShowEnum.Never,
            ShowButtonNextLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        };

        configure?.Invoke(options);

        var html = _helperHtml.CreateHtmlHelper().Pager(page, options).ToHtmlString();

        return PageLinkText.Matches(html).Select(m => m.Groups[1].Value).ToArray();
    }

    [Theory]
    [InlineData(3, 2, "1 2 3")]
    [InlineData(5, 3, "1 2 3 4 5")]
    [InlineData(1, 1, "1")]
    public void OverlappingBoundaries_RenderEachPageOnce(int totalPages, int boundaryCount, string expected)
    {
        var numbers = RenderPageNumbers(totalPages, 1, o => o.BoundaryCount = boundaryCount);

        Assert.Equal(expected, string.Join(" ", numbers));
    }

    [Fact]
    public void NegativeCounts_AreTreatedAsZero()
    {
        var numbers = RenderPageNumbers(10, 5, o =>
        {
            o.SiblingCount = -1;
            o.BoundaryCount = -1;
            o.ShowEllipsis = false;
        });

        Assert.Equal(["5"], numbers);
    }

    [Fact]
    public void ShowIndexAsRange_LastRangeEndsAtTheLastItem()
    {
        var page = Enumerable.Range(1, 12).ToPage(1, 5);

        var numbers = RenderPageNumbers(page, o => o.ShowIndexAsRange = true);

        Assert.Equal(["1 - 5", "6 - 10", "11 - 12"], numbers);
    }

    [Fact]
    public void ShowPagerButtonLinksIfNecessary_ShowsTheNumbersOnlyWhenThereIsMoreThanOnePage()
    {
        // Same meaning as IfNecessary on ShowPager: only when there is somewhere else to go
        var single = RenderPageNumbers(1, 1, o => o.ShowPagerButtonLinks = WhenToShowEnum.IfNecessary);
        var several = RenderPageNumbers(3, 1, o => o.ShowPagerButtonLinks = WhenToShowEnum.IfNecessary);

        Assert.Empty(single);
        Assert.Equal(["1", "2", "3"], several);
    }

    [Fact]
    public void NoCombination_RendersAPageTwiceOrOutOfOrder()
    {
        var failures = new List<string>();

        foreach (var totalPages in Enumerable.Range(1, 12))
        foreach (var boundaryCount in Enumerable.Range(0, 4))
        foreach (var siblingCount in Enumerable.Range(0, 3))
        foreach (var showEllipsis in new[] { true, false })
        foreach (var pageIndex in Enumerable.Range(1, totalPages))
        {
            var numbers = RenderPageNumbers(totalPages, pageIndex, o =>
                {
                    o.BoundaryCount = boundaryCount;
                    o.SiblingCount = siblingCount;
                    o.ShowEllipsis = showEllipsis;
                })
                .Where(text => text != "...")
                .Select(int.Parse)
                .ToArray();

            var strictlyIncreasing = numbers.Zip(numbers.Skip(1), (a, b) => a < b).All(ok => ok);

            if (!strictlyIncreasing)
            {
                failures.Add($"pages={totalPages} boundary={boundaryCount} sibling={siblingCount} ellipsis={showEllipsis} page={pageIndex}: {string.Join(" ", numbers)}");
            }
        }

        Assert.True(failures.Count == 0, $"{failures.Count} combinations failed:\n{string.Join("\n", failures.Take(20))}");
    }
}

using Bunit;
using Gobln.Pager;
using Gobln.Pager.Blazor;
using Gobln.Pager.BlazorTest.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.Pager.BlazorTest;

public class PagerComponentTests02 : BunitContext
{
    /// <summary>
    /// The texts of the numbered items in order, ellipses included, without
    /// First/Previous/Next/Last.
    /// </summary>
    private static string PageNumbers(IRenderedComponent<Blazor.Pager> cut)
    {
        return string.Join(" ", cut.FindAll("li").Select(li => li.TextContent.Trim()).Skip(2).SkipLast(2));
    }

    [Fact]
    public void PageNumbers_AreTheBoundariesAndThePagesAroundTheCurrentOne()
    {
        var testPage = new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.SiblingCount, 1)
              .Add(c => c.BoundaryCount, 1)
        );

        Assert.Equal("1 4 5 6 10", PageNumbers(cut));
    }

    [Fact]
    public void PageNumbers_WithEllipsis_AreTheSameAsTheMvcPagers()
    {
        // Both pagers get their numbers from PageWindow. Between page 1 and the pages around
        // page 4 only page 2 is skipped, so page 2 is shown instead of an ellipsis.
        var testPage = new TestPage { PageIndex = 4, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.SiblingCount, 1)
              .Add(c => c.BoundaryCount, 1)
              .Add(c => c.ShowEllipsis, true)
        );

        Assert.Equal("1 2 3 4 5 ... 10", PageNumbers(cut));
    }

    [Fact]
    public void NegativeCounts_AreTreatedAsZero()
    {
        var testPage = new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.SiblingCount, -1)
              .Add(c => c.BoundaryCount, -1)
        );

        Assert.Equal("5", PageNumbers(cut));
    }

    [Fact]
    public void BasicBunit_Test01()
    {
        var testPage = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(parameters => parameters.Add(c => c.Page, testPage));

        // Every item is a link, the active page included (Bootstrap 5.3)
        Assert.Equal(["First", "Previous", "2", "3", "4", "Next", "Last"], cut.FindAll("a").Select(a => a.TextContent.Trim()));

        var activePage = cut.Find("li.active > a");

        Assert.Equal("3", activePage.TextContent.Trim());
        Assert.Equal("page", activePage.GetAttribute("aria-current"));
    }

    [Fact]
    public void InvokePageChange_UpdatesPageIndexAndInvokesCallback()
    {
        var testPage = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true };

        var pageChanged = 0;

        Action<int> onPageChanged = c => { pageChanged = c; };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.PageIndexChanged, onPageChanged)
        );

        // The anchors are: First, Previous, 2, 3, 4, Next, Last
        cut.FindAll("a")[4].Click();

        Assert.Equal(4, pageChanged);

        // The component tracks the new index itself, so 4 is now the active page.
        var activePage = cut.Find("li.active > a");

        Assert.Equal("4", activePage.TextContent.Trim());
        Assert.Equal("page", activePage.GetAttribute("aria-current"));
    }

    [Fact]
    public void NewPage_FromTheParent_IsAdopted_EvenWithTheSameIndex()
    {
        // The pager moved to page 2 on a click, then the parent hands over a new Page on
        // page 1 (for example after resetting a filter). The parent's Page wins.
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.PageIndexChanged, (int _) => { })
        );

        cut.FindAll("a").First(a => a.TextContent.Trim() == "2").Click();

        cut.Render(parameters => parameters.Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true }));

        Assert.Equal("1", cut.Find("li.active > a").TextContent.Trim());
    }

    [Fact]
    public void SamePage_FromTheParent_KeepsTheClickedPage()
    {
        // A parent that loads the new page asynchronously re-renders first with the Page it
        // already had. The pager keeps the clicked page meanwhile, instead of jumping back.
        var testPage = new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.PageIndexChanged, (int _) => { })
        );

        cut.FindAll("a").First(a => a.TextContent.Trim() == "2").Click();

        cut.Render(parameters => parameters.Add(c => c.Page, testPage));

        Assert.Equal("2", cut.Find("li.active > a").TextContent.Trim());
    }

    [Fact]
    public void AriaCurrent_IsOnlyOnTheActivePagesLink()
    {
        // Bootstrap 5.3 puts aria-current on the current page's link. On every li, a screen
        // reader would announce each item as the current page.
        var testPage = new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.BoundaryCount, 1)
              .Add(c => c.ShowEllipsis, true)
        );

        var current = Assert.Single(cut.FindAll("[aria-current]"));

        Assert.Equal("A", current.TagName);
        Assert.Equal("page", current.GetAttribute("aria-current"));
        Assert.Equal("5", current.TextContent.Trim());
        Assert.True(current.ParentElement!.ClassList.Contains("active"));
    }

    [Fact]
    public void DisabledItems_AreSpans()
    {
        // A disabled link is still announced as a working link and can still be focused, so
        // First/Previous on the first page and the ellipsis are spans (Bootstrap 5.3).
        var testPage = new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.BoundaryCount, 1)
              .Add(c => c.ShowEllipsis, true)
        );

        var disabled = cut.FindAll("li.disabled");

        Assert.Equal(["First", "Previous", "..."], disabled.Select(li => li.TextContent.Trim()));

        Assert.All(disabled, li =>
        {
            var content = Assert.Single(li.Children);

            Assert.Equal("SPAN", content.TagName);
            Assert.True(content.ClassList.Contains("page-link"));
        });

        // Everything else stays a link
        Assert.All(cut.FindAll("li:not(.disabled)"), li => Assert.Equal("A", Assert.Single(li.Children).TagName));
    }

    [Fact]
    public void DisabledItems_HaveNoClickHandler()
    {
        // Next/Last on the last page and the ellipsis must not react to a click at all
        var testPage = new TestPage { PageIndex = 10, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.BoundaryCount, 1)
              .Add(c => c.ShowEllipsis, true)
        );

        var disabled = cut.FindAll("li.disabled");

        Assert.Equal(["...", "Next", "Last"], disabled.Select(li => li.TextContent.Trim()));

        Assert.All(disabled, li => Assert.DoesNotContain("blazor:onclick", li.OuterHtml));

        // ... while the items that go somewhere keep theirs
        Assert.All(cut.FindAll("li:not(.disabled) > a"), a => Assert.True(a.HasAttribute("blazor:onclick")));
    }

    [Fact]
    public void ShowPager_ByDefault_RendersASinglePage()
    {
        // The default is Always: a single page still gets a pager, as it always did
        var cut = Render<Blazor.Pager>(parameters => parameters.Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = 1 }));

        Assert.Single(cut.FindAll("nav"));
    }

    [Fact]
    public void ShowPager_Never_RendersNothing()
    {
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.ShowPager, WhenToShowEnum.Never)
        );

        Assert.Equal("", cut.Markup.Trim());
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void ShowPager_IfNecessary_RendersOnlyForMoreThanOnePage(int totalPageCount, bool rendered)
    {
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = totalPageCount })
              .Add(c => c.ShowPager, WhenToShowEnum.IfNecessary)
        );

        Assert.Equal(rendered, cut.FindAll("nav").Count == 1);
    }

    /// <summary>
    /// The texts of all items in order, First/Previous/Next/Last included
    /// </summary>
    private static string[] Items(IRenderedComponent<Blazor.Pager> cut)
    {
        return [.. cut.FindAll("li").Select(li => li.TextContent.Trim())];
    }

    [Fact]
    public void ShowButtons_Never_HidesThem()
    {
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.ShowFirstButton, WhenToShowEnum.Never)
              .Add(c => c.ShowLastButton, WhenToShowEnum.Never)
        );

        Assert.Equal(["Previous", "4", "5", "6", "Next"], Items(cut));
    }

    [Theory]
    [InlineData(1, new[] { "1", "2", "3", "Next", "Last" })]
    [InlineData(5, new[] { "First", "Previous", "4", "5", "6", "Next", "Last" })]
    [InlineData(10, new[] { "First", "Previous", "8", "9", "10" })]
    public void ShowButtons_IfNecessary_OnlyShowsTheButtonsThatGoSomewhere(int pageIndex, string[] expected)
    {
        // First and Previous only when not on the first page, Next and Last only when not on the last
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = pageIndex, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.ShowFirstButton, WhenToShowEnum.IfNecessary)
              .Add(c => c.ShowPreviousButton, WhenToShowEnum.IfNecessary)
              .Add(c => c.ShowNextButton, WhenToShowEnum.IfNecessary)
              .Add(c => c.ShowLastButton, WhenToShowEnum.IfNecessary)
        );

        Assert.Equal(expected, Items(cut));
    }

    [Fact]
    public void Texts_ReplaceTheButtonLabels()
    {
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.FirstText, "Eerste")
              .Add(c => c.PreviousText, "Vorige")
              .Add(c => c.NextText, "Volgende")
              .Add(c => c.LastText, "Laatste")
        );

        Assert.Equal(["Eerste", "Vorige", "4", "5", "6", "Volgende", "Laatste"], Items(cut));

        // Without an icon the text is read out itself, so it needs no aria-label
        Assert.All(cut.FindAll("li > *"), item => Assert.False(item.HasAttribute("aria-label")));
    }

    [Fact]
    public void Icon_IsShownInsteadOfTheText_WhichBecomesTheAriaLabel()
    {
        // As in Bootstrap's example and the Mvc pager: the icon is hidden from screen readers,
        // which read the text from the aria-label instead
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.PreviousText, "Vorige")
              .Add(c => c.PreviousIcon, "&laquo;")
        );

        var previous = cut.Find("li:nth-child(2) > a");

        Assert.Equal("Vorige", previous.GetAttribute("aria-label"));
        Assert.Equal("«", previous.TextContent.Trim());

        var icon = Assert.Single(previous.Children);

        Assert.Equal("SPAN", icon.TagName);
        Assert.Equal("true", icon.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Icon_OnADisabledButton_IsShownToo()
    {
        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true })
              .Add(c => c.PreviousText, "Vorige")
              .Add(c => c.PreviousIcon, "&laquo;")
        );

        var previous = cut.Find("li.disabled:nth-child(2) > span.page-link");

        Assert.Equal("Vorige", previous.GetAttribute("aria-label"));
        Assert.Equal("«", previous.TextContent.Trim());
    }

    [Fact]
    public void AriaLabel_LabelsTheNav()
    {
        var testPage = new TestPage { PageIndex = 1, TotalPageCount = 10, HasPaging = true };

        var byDefault = Render<Blazor.Pager>(parameters => parameters.Add(c => c.Page, testPage));

        var custom = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.AriaLabel, "Search results pages")
        );

        // The same default as the Mvc pager
        Assert.Equal("Page navigation", byDefault.Find("nav").GetAttribute("aria-label"));
        Assert.Equal("Search results pages", custom.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void ClickHandler_IsOnTheLink_NotOnTheListItem()
    {
        // The handler belongs on the element that is clicked, next to its preventDefault.
        // On the li, a click on its padding would change the page too.
        var testPage = new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true };

        var cut = Render<Blazor.Pager>(parameters => parameters.Add(c => c.Page, testPage));

        Assert.All(cut.FindAll("a"), a => Assert.True(a.HasAttribute("blazor:onclick")));
        Assert.All(cut.FindAll("li"), li => Assert.False(li.HasAttribute("blazor:onclick")));
    }
}
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

        Assert.All(disabled, li => Assert.False(li.HasAttribute("blazor:onclick")));

        // ... while the items that go somewhere keep theirs
        Assert.All(cut.FindAll("li:not(.disabled)"), li => Assert.True(li.HasAttribute("blazor:onclick")));
    }
}
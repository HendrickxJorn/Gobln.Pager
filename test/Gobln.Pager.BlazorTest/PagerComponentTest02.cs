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
    public async Task BasicBunit_Test01()
    {
        var testPage = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true };

        using var ctx = new BunitContext();

        var pageChanged = 0;

        Action<int> onPageChanged = c => { pageChanged = c; };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.PageIndexChanged, onPageChanged)
        );

        var links = cut.FindAll("a");

        var expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>First</a>
                        """;

        links[0].MarkupMatches(expectedHtml);

        expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>Previous</a>
                        """;

        links[1].MarkupMatches(expectedHtml);

        expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>2</a>
                        """;

        links[2].MarkupMatches(expectedHtml);

        expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>4</a>
                        """;

        links[3].MarkupMatches(expectedHtml);

        expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>Next</a>
                        """;

        links[4].MarkupMatches(expectedHtml);

        expectedHtml = """
                        <a class="page-link" blazor:onclick:preventDefault diff:ignore>Last</a>
                        """;

        links[5].MarkupMatches(expectedHtml);

        var activePage = cut.Find("span");

        expectedHtml = """
                        <span class="page-link pointer" diff:ignore>3</span>
                        """;

        activePage.MarkupMatches(expectedHtml);
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

        // The active page renders as a span, so the anchors are:
        // First, Previous, 2, 4, Next, Last
        cut.FindAll("a")[3].Click();

        Assert.Equal(4, pageChanged);

        // The component tracks the new index itself, so 4 is now the active page.
        cut.Find("span").MarkupMatches("""
                        <span class="page-link pointer" diff:ignore>4</span>
                        """);
    }
}
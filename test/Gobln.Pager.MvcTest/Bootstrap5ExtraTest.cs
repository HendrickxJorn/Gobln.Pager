using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Xunit;

namespace Gobln.Pager.MvcTest;

public class Bootstrap5ExtraTest
{
    private readonly HelperHtml _helperHtml;
    private readonly IPage _page1;
    private readonly IPage _page3;
    private readonly IPage _page5;

    public Bootstrap5ExtraTest()
    {
        _helperHtml = new HelperHtml();
        _page1 = new int[] { 1 }.ToPage(1, 1);
        _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);
        _page5 = new int[] { 1, 2, 3, 4, 5 }.ToPage(1, 1);
    }

    [Fact]
    public void DisabledAndActiveTest01()
    {
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="disabled page-item">
                    <span class="page-link">Previous</span>
                </li>
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">1</a>
                </li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="page-item"><a class="page-link" href="#">3</a></li>
                <li class="page-item">
                    <a class="page-link" href="#">Next</a>
                </li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void NotPageblePagerTest01()
    {
        var target = "";

        var page = new int[] { 1 }.ToPage();

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(page);

        Assert.Equal(target, result.ToHtmlString());
    }

    [Fact]
    public void NotPageblePagerTest02()
    {
        var target = "";

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(null);

        Assert.Equal(target, result.ToHtmlString());
    }

    [Fact]
    public void NullOptionsTest01()
    {
        // Null options are refused up front, naming the caller's argument
        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var exception = Assert.Throws<ArgumentNullException>(() => mockHtmlHelper.Pager(_page3, (IPagerOptions)null!));

        Assert.Equal("pagerOptions", exception.ParamName);
    }

    [Fact]
    public void NotPageblePagerTest03()
    {
        var target = """
         <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="disabled page-item">
                    <span class="page-link">Previous</span>
                </li>
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">1</a>
                </li>
                <li class="disabled page-item">
                    <span class="page-link">Next</span>
                </li>
            </ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page1, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            ShowPager = WhenToShowEnum.Always,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
            SiblingCount = 0,
            BoundaryCount = 0,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void UrlContextPagerTest01()
    {
        var target = """
         <nav aria-label="Page navigation example">
         	<ul class="pagination">
         		<li class="disabled page-item">
         			<span class="page-link">Previous</span>
         		</li>
         		<li class="active page-item">
         			<a aria-current="page" class="page-link" href="http://pager.com/?pageIndex=1">1</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/?pageIndex=2">2</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/?pageIndex=3">3</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/?pageIndex=2">Next</a>
         		</li>
         	</ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper(url: "http://pager.com/");

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = null,

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void UrlContextPagerTest02()
    {
        var target = """
         <nav aria-label="Page navigation example">
         	<ul class="pagination">
         		<li class="disabled page-item">
         			<span class="page-link">Previous</span>
         		</li>
         		<li class="active page-item">
         			<a aria-current="page" class="page-link" href="http://pager.com/search?pageIndex=1">1</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/search?pageIndex=2">2</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/search?pageIndex=3">3</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/search?pageIndex=4">4</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/search?pageIndex=5">5</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pager.com/search?pageIndex=2">Next</a>
         		</li>
         	</ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper(url: "http://pager.com/search");

        var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = null,

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void UrlContextSharedOptionsTest01()
    {
        // Options are often a static shared between views. Resolving HRefUrl from the
        // current request must not write it back into them, or the first request's url
        // gets pinned onto every later one.
        var shared = new PagerOptions() { HRefUrl = null };

        var first = _helperHtml.CreateHtmlHelper(url: "http://first.test/orders?customer=1")
            .Pager(_page3, shared)
            .ToHtmlString();

        var second = _helperHtml.CreateHtmlHelper(url: "http://second.test/products")
            .Pager(_page3, shared)
            .ToHtmlString();

        Assert.Null(shared.HRefUrl);

        Assert.Contains("http://first.test/orders?customer=1", first);
        Assert.Contains("http://second.test/products?pageIndex=2", second);
        Assert.DoesNotContain("first.test", second);
    }

    [Fact]
    public void UrlParameterPagerTest01()
    {
        var target = """
         <nav aria-label="Page navigation example">
         	<ul class="pagination">
         		<li class="disabled page-item">
         			<span class="page-link">Previous</span>
         		</li>
         		<li class="active page-item">
         			<a aria-current="page" class="page-link" href="http://pagerlist.com/?pageIndex=1">1</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/?pageIndex=2">2</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/?pageIndex=3">3</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/?pageIndex=2">Next</a>
         		</li>
         	</ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = "http://pagerList.com/",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void UrlParameterPagerTest02()
    {
        var target = """
         <nav aria-label="Page navigation example">
         	<ul class="pagination">
         		<li class="disabled page-item">
         			<span class="page-link">Previous</span>
         		</li>
         		<li class="active page-item">
         			<a aria-current="page" class="page-link" href="http://pagerlist.com/search?pageIndex=1">1</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/search?pageIndex=2">2</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/search?pageIndex=3">3</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/search?pageIndex=4">4</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/search?pageIndex=5">5</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" href="http://pagerlist.com/search?pageIndex=2">Next</a>
         		</li>
         	</ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = "http://pagerList.com/search",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void DataParameterPagerTest01()
    {
        var target = """
         <nav aria-label="Page navigation example">
         	<ul class="pagination">
         		<li class="disabled page-item">
         			<span class="page-link">First</span>
         		</li>
         		<li class="disabled page-item">
         			<span class="page-link">Previous</span>
         		</li>
         		<li class="active page-item">
         			<a aria-current="page" class="page-link" data-pageindex="1" href="#">1</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="2" href="#">2</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="3" href="#">3</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="4" href="#">4</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="5" href="#">5</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="2" href="#">Next</a>
         		</li>
         		<li class="page-item">
         			<a class="page-link" data-pageindex="5" href="#">Last</a>
         		</li>
         	</ul>
         </nav>
         """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            PageIndexDataName = "pageindex",
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void AllowDisabledPageLinkFalseTest01()
    {
        // Without disabled styling, First and Previous on the first page must stay working
        // links, not spans that look like buttons but can't be clicked.
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="page-item"><a class="page-link" href="#">First</a></li>
                <li class="page-item"><a class="page-link" href="#">Previous</a></li>
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">1</a>
                </li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="page-item"><a class="page-link" href="#">3</a></li>
                <li class="page-item"><a class="page-link" href="#">Next</a></li>
                <li class="page-item"><a class="page-link" href="#">Last</a></li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            AllowDisabledPageLink = false,

            HRefUrl = "#",
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void AllowDisabledPageLinkFalseTest02()
    {
        // ... and the same for Next and Last on the last page.
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="page-item"><a class="page-link" href="#">First</a></li>
                <li class="page-item"><a class="page-link" href="#">Previous</a></li>
                <li class="page-item"><a class="page-link" href="#">1</a></li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">3</a>
                </li>
                <li class="page-item"><a class="page-link" href="#">Next</a></li>
                <li class="page-item"><a class="page-link" href="#">Last</a></li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(new int[] { 1, 2, 3 }.ToPage(3, 1), new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            AllowDisabledPageLink = false,

            HRefUrl = "#",
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void DisabledWithoutActiveTest01()
    {
        // Disabled items are spans, not links: pointer-events: none only stops the mouse,
        // a screen reader would still announce a working link.
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="disabled page-item"><span class="page-link">First</span></li>
                <li class="disabled page-item"><span class="page-link">Previous</span></li>
                <li class="disabled page-item"><span class="page-link">1</span></li>
                <li class="page-item"><a class="page-link" href="http://pager.test/list?pageIndex=2">2</a></li>
                <li class="page-item"><a class="page-link" href="http://pager.test/list?pageIndex=3">3</a></li>
                <li class="page-item"><a class="page-link" href="http://pager.test/list?pageIndex=2">Next</a></li>
                <li class="page-item"><a class="page-link" href="http://pager.test/list?pageIndex=3">Last</a></li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            DisplayActive = false,

            HRefUrl = "http://pager.test/list",
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void LabelsAreEscapedTest01()
    {
        // Labels are text (often from a translation file), so they are escaped on every
        // target. Only icons are inserted as markup.
        var result = _helperHtml.CreateHtmlHelper().Pager(_page3, new PagerOptions()
        {
            ButtonPreviousLinkLabel = "Prev & <less>",
            ButtonNextLinkLabel = "Next & <more>",

            HRefUrl = "#",
        }).ToHtmlString();

        Assert.Contains("Prev &amp; &lt;less&gt;", result);
        Assert.Contains("Next &amp; &lt;more&gt;", result);
        Assert.DoesNotContain("<less>", result);
        Assert.DoesNotContain("<more>", result);
    }

    [Fact]
    public void ActivePageTest01()
    {
        // Bootstrap 5.3: the current page is a working link, and aria-current goes on the
        // link, not on the li (https://getbootstrap.com/docs/5.3/components/pagination/#active)
        var result = _helperHtml.CreateHtmlHelper().Pager(new int[] { 1, 2, 3 }.ToPage(2, 1), new PagerOptions()
        {
            HRefUrl = "http://pager.test/list",
        }).ToHtmlString();

        Assert.Contains("""<li class="active page-item"><a aria-current="page" class="page-link" href="http://pager.test/list?pageIndex=2">2</a></li>""", result);
    }

    [Fact]
    public void NoTabindexTest01()
    {
        // Disabled items, the active page and the ellipsis are spans, which can't be focused
        // anyway, and every link works: nothing needs a tabindex.
        var result = _helperHtml.CreateHtmlHelper().Pager(Enumerable.Range(1, 100).ToPage(1, 10), new PagerOptions()
        {
            HRefUrl = "#",
        }).ToHtmlString();

        Assert.Contains("...", result);
        Assert.DoesNotContain("tabindex", result);
    }

    [Fact]
    public void NavAriaLabelTest01()
    {
        // Without a label, the pager's nav can't be told apart from the site's other navigation
        var result = _helperHtml.CreateHtmlHelper().Pager(_page3, new PagerOptions()
        {
            HRefUrl = "#",
        });

        Assert.StartsWith("""<nav aria-label="Page navigation">""", result.ToHtmlString());
    }

    [Fact]
    public void NavAriaLabelTest02()
    {
        // Other nav attributes keep the default label
        var result = _helperHtml.CreateHtmlHelper().Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { @class = "my-pager" },

            HRefUrl = "#",
        });

        Assert.StartsWith("""<nav aria-label="Page navigation" class="my-pager">""", result.ToHtmlString());
    }

    [Fact]
    public void NavAriaLabelTest03()
    {
        // A label set by the caller, either way, replaces the default
        var labelled = _helperHtml.CreateHtmlHelper().Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Search results pages" },

            HRefUrl = "#",
        });

        var labelledBy = _helperHtml.CreateHtmlHelper().Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_labelledby = "results-heading" },

            HRefUrl = "#",
        });

        Assert.StartsWith("""<nav aria-label="Search results pages">""", labelled.ToHtmlString());
        Assert.StartsWith("""<nav aria-labelledby="results-heading">""", labelledBy.ToHtmlString());
    }

    [Fact]
    public void ShowButtonNextLinkNeverTest01()
    {
        // Next is driven by its own option; hiding it must leave Previous alone.
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="disabled page-item">
                    <span class="page-link">Previous</span>
                </li>
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">1</a>
                </li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="page-item"><a class="page-link" href="#">3</a></li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = "#",

            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
            ShowButtonNextLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void ShowButtonPreviousLinkNeverTest01()
    {
        // ... and hiding Previous must leave Next alone.
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="active page-item">
                    <a aria-current="page" class="page-link" href="#">1</a>
                </li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="page-item"><a class="page-link" href="#">3</a></li>
                <li class="page-item">
                    <a class="page-link" href="#">Next</a>
                </li>
            </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            HRefUrl = "#",

            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
            ShowButtonPreviousLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }
}
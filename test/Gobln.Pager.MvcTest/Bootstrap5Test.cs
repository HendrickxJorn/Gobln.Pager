using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Xunit;

namespace Gobln.Pager.MvcTest;

public class Bootstrap5Test
{
    private readonly HelperHtml _helperHtml;
    private readonly IPage _page3;

    public Bootstrap5Test()
    {
        _helperHtml = new HelperHtml();
        _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);
    }

    [Fact]
    public void Default01()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="pagination-lg pagination">
            <li class="page-item">
                <a class="page-link" href="#">Previous</a>
            </li>
            <li class="page-item">
                <a class="page-link" href="#">1</a>
            </li>
            <li class="page-item">
                <a class="page-link" href="#">2</a>
            </li>
            <li class="page-item">
                <a class="page-link" href="#">3</a>
            </li>
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
            UlTagHtmlAttributes = new { @class = "pagination-lg" },

            AllowDisabledPageLink = false,
            AllowDisabledPageLinkTab = false,

            DisplayActive = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void WithIcons01()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="pagination">
            <li class="page-item">
              <a aria-label="Previous" class="page-link" href="#">
                <span aria-hidden="true">&amp;laquo;</span>
              </a>
            </li>
            <li class="page-item"><a class="page-link" href="#">1</a></li>
            <li class="page-item"><a class="page-link" href="#">2</a></li>
            <li class="page-item"><a class="page-link" href="#">3</a></li>
            <li class="page-item">
              <a aria-label="Next" class="page-link" href="#">
                <span aria-hidden="true">&amp;raquo;</span>
              </a>
            </li>
          </ul>
        </nav>
        """;

        var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

        var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },

            AllowDisabledPageLink = false,
            AllowDisabledPageLinkTab = false,

            DisplayActive = false,

            ButtonPreviousLinkIcon = "&laquo;",
            ButtonNextLinkIcon = "&raquo;",

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void DisabledAndActive01()
    {
        var target = """
        <nav aria-label="Page navigation example">
            <ul class="pagination">
                <li class="disabled page-item">
                    <span class="page-link">Previous</span>
                </li>
                <li aria-current="page" class="active page-item">
                    <span class="page-link">1</span>
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

            AllowDisabledPageLinkTab = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void Sizing01()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="pagination-lg pagination">
            <li aria-current="page" class="active page-item">
              <span class="page-link">1</span>
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
            UlTagHtmlAttributes = new { @class = "pagination-lg" },

            AllowDisabledPageLinkTab = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
            ShowButtonPreviousLink = WhenToShowEnum.Never,
            ShowButtonNextLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void Sizing02()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="pagination-sm pagination">
            <li aria-current="page" class="active page-item">
              <span class="page-link">1</span>
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
            UlTagHtmlAttributes = new { @class = "pagination-sm" },

            AllowDisabledPageLinkTab = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
            ShowButtonPreviousLink = WhenToShowEnum.Never,
            ShowButtonNextLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void Alignment01()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="justify-content-center pagination">
            <li class="page-item">
              <a class="page-link" href="#">Previous</a>
            </li>
            <li class="page-item"><a class="page-link" href="#">1</a></li>
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
            UlTagHtmlAttributes = new { @class = "justify-content-center" },

            AllowDisabledPageLink = false,
            AllowDisabledPageLinkTab = false,

            DisplayActive = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }

    [Fact]
    public void Alignment02()
    {
        var target = """
        <nav aria-label="Page navigation example">
          <ul class="justify-content-end pagination">
            <li class="page-item">
              <a class="page-link" href="#">Previous</a>
            </li>
            <li class="page-item"><a class="page-link" href="#">1</a></li>
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
            UlTagHtmlAttributes = new { @class = "justify-content-end" },

            AllowDisabledPageLink = false,
            AllowDisabledPageLinkTab = false,

            DisplayActive = false,

            HRefUrl = "#",

            //new 
            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        });

        Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
    }
}
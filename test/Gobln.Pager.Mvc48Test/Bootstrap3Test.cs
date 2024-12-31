using Gobln.Pager.Mvc;
using Gobln.Pager.Mvc48Test.Helper;
using Xunit;

namespace Gobln.Pager.Mvc48Test
{
    public class Bootstrap3Test
    {
        //https://getbootstrap.com/docs/3.4/components/#pagination

        private readonly HelperHtml _helperHtml;
        private readonly IPage _page5;

        public Bootstrap3Test()
        {
            _helperHtml = new HelperHtml();
            _page5 = new int[] { 1, 2, 3, 4, 5 }.ToPage(1, 1);
        }

        [Fact]
        public void Default01()
        {
            var target = """
            <nav aria-label="Page navigation example">
              <ul class="pagination">
                <li>
                  <a aria-label="Previous" href="#">
                    <span aria-hidden="true">&laquo;</span>
                  </a>
                </li>
                <li><a href="#">1</a></li>
                <li><a href="#">2</a></li>
                <li><a href="#">3</a></li>
                <li><a href="#">4</a></li>
                <li><a href="#">5</a></li>
                <li>
                  <a aria-label="Next" href="#">
                    <span aria-hidden="true">&raquo;</span>
                  </a>
                </li>
              </ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var pagerOptions = new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                PreviousPageLinkAlwaysShow = true,
                NextPageLinkAlwaysShow = true,

                AllowDisabledPageLink = false,
                AllowDisabledPageLinkTab = false,

                DisplayActive = false,

                PreviousPageLinkIconLabel = "&laquo;",
                NextPageLinkIconLabel = "&raquo;",

                HRefUrl = "#",
            };

            var result = mockHtmlHelper.Pager(_page5, new Bootstrap3Pager(pagerOptions));

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToString());
        }

        [Fact]
        public void DisabledAndActive01()
        {
            // readable target
            var target = """
            <nav aria-label="Page navigation">
              <ul class="pagination">
                <li class="disabled"><a href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a></li>
                <li class="active"><a href="#">1 <span class="sr-only">(current)</span></a></li>
                <li><a href="#">2</a></li>
                <li><a href="#">3</a></li>
                <li><a href="#">4</a></li>
                <li><a href="#">5</a></li>
                <li>
                  <a href="#" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                  </a>
                </li>
              </ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = false,

                PreviousPageLinkAlwaysShow = true,
                NextPageLinkAlwaysShow = true,

                HRefUrl = "#",
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToString());
        }

        [Fact]
        public void Sizing01()
        {
            var target = """
            <nav aria-label="Page navigation">
              <ul class="pagination pagination-lg">
                <li>
                  <a href="#" aria-label="Previous">
                    <span aria-hidden="true">&laquo;</span>
                  </a>
                </li>
                <li><a href="#">1</a></li>
                <li><a href="#">2</a></li>
                <li><a href="#">3</a></li>
                <li><a href="#">4</a></li>
                <li><a href="#">5</a></li>
                <li>
                  <a href="#" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                  </a>
                </li>
              </ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation" },
                UlTagHtmlAttributes = new { @class = "pagination-lg" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                PreviousPageLinkAlwaysShow = true,
                NextPageLinkAlwaysShow = true,

                AllowDisabledPageLink = false,
                AllowDisabledPageLinkTab = false,

                DisplayActive = false,

                HRefUrl = "#",
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToString());
        }

        [Fact]
        public void Sizing02()
        {
            var target = """
            <nav aria-label="Page navigation">
              <ul class="pagination pagination-sm">
                <li>
                  <a href="#" aria-label="Previous">
                    <span aria-hidden="true">&laquo;</span>
                  </a>
                </li>
                <li><a href="#">1</a></li>
                <li><a href="#">2</a></li>
                <li><a href="#">3</a></li>
                <li><a href="#">4</a></li>
                <li><a href="#">5</a></li>
                <li>
                  <a href="#" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                  </a>
                </li>
              </ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var result = mockHtmlHelper.Pager(_page5, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation" },
                UlTagHtmlAttributes = new { @class = "pagination-sm" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                PreviousPageLinkAlwaysShow = true,
                NextPageLinkAlwaysShow = true,

                AllowDisabledPageLink = false,
                AllowDisabledPageLinkTab = false,

                DisplayActive = false,

                HRefUrl = "#",
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToString());
        }
    }
}

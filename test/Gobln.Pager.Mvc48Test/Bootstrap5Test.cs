using Gobln.Pager.Mvc;
using Gobln.Pager.Mvc48Test.Helper;
using Moq;
using Xunit;

namespace Gobln.Pager.Mvc48Test
{
    public class Bootstrap5Test
    {
        // https://getbootstrap.com/docs/5.3/components/pagination/

        // https://github.com/HtmlTags/htmltags check for testing

        //todo:
        //  - if disabled add tabindex="-1"
        //  - if 2 is active, previous and next can not be disabled
        //  - Add test with first and end

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
              <ul class="pagination pagination-lg">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
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
                    <span aria-hidden="true">&laquo;</span>
                  </a>
                </li>
                <li class="page-item"><a class="page-link" href="#">1</a></li>
                <li class="page-item"><a class="page-link" href="#">2</a></li>
                <li class="page-item"><a class="page-link" href="#">3</a></li>
                <li class="page-item">
                  <a aria-label="Next" class="page-link" href="#">
                    <span aria-hidden="true">&raquo;</span>
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

                PreviousPageLinkIconLabel = "&laquo;",
                NextPageLinkIconLabel = "&raquo;",

                HRefUrl = "#",

                //new 
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void DisabledAndActive01()
        {
            var target = """
            <nav aria-label="Page navigation example">
                <ul class="pagination">
                    <li class="page-item disabled">
                        <span class="page-link">Previous</span>
                    </li>
                    <li aria-current="page" class="page-item active">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Sizing01()
        {
            var target = """
            <nav aria-label="Page navigation example">
              <ul class="pagination pagination-lg">
                <li aria-current="page" class="page-item active">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
                ShowPreviousButtonLink = WhenToShowEnum.Never,
                ShowNextButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Sizing02()
        {
            var target = """
            <nav aria-label="Page navigation example">
              <ul class="pagination pagination-sm">
                <li aria-current="page" class="page-item active">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
                ShowPreviousButtonLink = WhenToShowEnum.Never,
                ShowNextButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Alignment01()
        {
            var target = """
            <nav aria-label="Page navigation example">
              <ul class="pagination justify-content-center">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Alignment02()
        {
            var target = """
            <nav aria-label="Page navigation example">
              <ul class="pagination justify-content-end">
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
                ShowFirstButtonLink = WhenToShowEnum.Never,
                ShowLastButtonLink = WhenToShowEnum.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }
    }
}
using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Xunit;

namespace Gobln.Pager.MvcTest
{
    public class Bootstrap5ExtraTest
    {
        //https://mui.com/material-ui/react-pagination/
        // todo get extra's form here
        //          -> siblingCount={1} boundaryCount={1}

        private readonly HelperHtml _helperHtml;
        private readonly IPage _page1;
        private readonly IPage _page3;
        private readonly IPage _page5;
        private readonly IPage _page20;

        public Bootstrap5ExtraTest()
        {
            _helperHtml = new HelperHtml();
            _page1 = new int[] { 1 }.ToPage(1, 1);
            _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);
            _page5 = new int[] { 1, 2, 3, 4, 5 }.ToPage(1, 1);
            _page20 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }.ToPage(1, 1);
        }

        [Fact]
        public void DisabledAndActiveTest01()
        {
            var target = """
            <nav aria-label="Page navigation example">
                <ul class="pagination">
                    <li class="disabled page-item">
                        <span class="page-link" tabindex="-1">Previous</span>
                    </li>
                    <li aria-current="page" class="active page-item">
                        <span class="page-link" tabindex="-1">1</span>
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
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                HRefUrl = "#",
                
                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
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
        public void NotPageblePagerTest03()
        {
            var target = """
             <nav aria-label="Page navigation example">
                <ul class="pagination">
                    <li class="disabled page-item">
                        <span class="page-link" tabindex="-1">Previous</span>
                    </li>
                    <li aria-current="page" class="active page-item">
                        <span class="page-link" tabindex="-1">1</span>
                    </li>
                    <li class="disabled page-item">
                        <span class="page-link" tabindex="-1">Next</span>
                    </li>
                </ul>
             </nav>
             """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var result = mockHtmlHelper.Pager(_page1, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                AlwaysShowPager = true,

                HRefUrl = "#",

                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
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
             			<span class="page-link" tabindex="-1">Previous</span>
             		</li>
             		<li aria-current="page" class="active page-item">
             			<span class="page-link" tabindex="-1">1</span>
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
             """
            ;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper(url: "http://pager.com/");

            var result = mockHtmlHelper.Pager(_page3, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                AlwaysShowPager = true,

                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
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
             			<span class="page-link" tabindex="-1">Previous</span>
             		</li>
             		<li aria-current="page" class="active page-item">
             			<span class="page-link" tabindex="-1">1</span>
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
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                AlwaysShowPager = true,

                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void UrlParameterPagerTest01()
        {
            var target = """
             <nav aria-label="Page navigation example">
             	<ul class="pagination">
             		<li class="disabled page-item">
             			<span class="page-link" tabindex="-1">Previous</span>
             		</li>
             		<li aria-current="page" class="active page-item">
             			<span class="page-link" tabindex="-1">1</span>
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
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                AlwaysShowPager = true,

                HRefUrl = "http://pagerList.com/",

                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
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
             			<span class="page-link" tabindex="-1">Previous</span>
             		</li>
             		<li aria-current="page" class="active page-item">
             			<span class="page-link" tabindex="-1">1</span>
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
                ShowItemOrder = [PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink],

                AllowDisabledPageLinkTab = true,

                AlwaysShowPager = true,

                HRefUrl = "http://pagerList.com/search",

                //new 
                ShowFirstButtonLink = ShowPageLink.Never,
                ShowLastButtonLink = ShowPageLink.Never,
            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Default02()
        {
            var target = """
            <nav aria-label="Page navigation example">
                <ul class="pagination-lg pagination">
            		<li class="page-item">
            	        <a class="page-link" href="#">First</a>
                    </li>
            		<li class="page-item">
            			<a class="page-link" href="#">Previous</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">1</a>
            		</li>
            		<li class="disabled page-item">
            			<span class="page-link" tabindex="-1">...</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">5</a>
            		</li>
            		<li aria-current="page" class="active page-item">
            			<span class="page-link" tabindex="-1">6</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">7</a>
            		</li>
            		<li class="disabled page-item">
            			<span class="page-link" tabindex="-1">...</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">20</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">Next</a>
            		</li>
            		<li class="page-item">
            	        <a class="page-link" href="#">Last</a>
                    </li>
            	</ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var page20 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }.ToPage(6, 1);

            var result = mockHtmlHelper.Pager(page20, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                UlTagHtmlAttributes = new { @class = "pagination-lg" },

                ShowItemOrder = [PageLinkItem.FirstPageLink, PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink, PageLinkItem.LastPageLink],

                HRefUrl = "#",

            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Default03()
        {
            var target = """
            <nav aria-label="Page navigation example">
                <ul class="pagination-lg pagination">
            		<li class="page-item">
            	        <a class="page-link" href="#">First</a>
                    </li>
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
            		<li aria-current="page" class="active page-item">
            			<span class="page-link" tabindex="-1">4</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">5</a>
            		</li>
            		<li class="disabled page-item">
            			<span class="page-link" tabindex="-1">...</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">20</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">Next</a>
            		</li>
            		<li class="page-item">
            	        <a class="page-link" href="#">Last</a>
                    </li>
            	</ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var page20 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }.ToPage(4, 1);

            var result = mockHtmlHelper.Pager(page20, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                UlTagHtmlAttributes = new { @class = "pagination-lg" },

                ShowItemOrder = [PageLinkItem.FirstPageLink, PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink, PageLinkItem.LastPageLink],

                HRefUrl = "#",

            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }

        [Fact]
        public void Default04()
        {
            var target = """
            <nav aria-label="Page navigation example">
                <ul class="pagination-lg pagination">
            		<li class="page-item">
            	        <a class="page-link" href="#">First</a>
                    </li>
            		<li class="page-item">
            			<a class="page-link" href="#">Previous</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">1</a>
            		</li>
            		<li class="disabled page-item">
            			<span class="page-link" tabindex="-1">...</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">16</a>
            		</li>
            		<li aria-current="page" class="active page-item">
            			<span class="page-link" tabindex="-1">17</span>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">18</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">19</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">20</a>
            		</li>
            		<li class="page-item">
            			<a class="page-link" href="#">Next</a>
            		</li>
            		<li class="page-item">
            	        <a class="page-link" href="#">Last</a>
                    </li>
            	</ul>
            </nav>
            """;

            var mockHtmlHelper = _helperHtml.CreateHtmlHelper();

            var page20 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }.ToPage(17, 1);

            var result = mockHtmlHelper.Pager(page20, new PagerOptions()
            {
                NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
                UlTagHtmlAttributes = new { @class = "pagination-lg" },

                ShowItemOrder = [PageLinkItem.FirstPageLink, PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink, PageLinkItem.LastPageLink],

                HRefUrl = "#",

            });

            Assert.Equal(HtmlStringHelper.Minify(target), result.ToHtmlString());
        }
    }
}

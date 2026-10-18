using Gobln.Pager;
using Gobln.Pager.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Encodings.Web;
using WebApplicationMvc.Models;

namespace WebApplicationMvc.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private static readonly int[] Items = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14];

        private const int PageSize = 1;

        private static readonly PagerOptions JsonPagerOptions = new()
        {
            HRefUrl = "#",
            PageIndexDataName = "pageindex",
        };

        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Like Index, but with a single pager whose page index lives in the url: the pager's
        /// links add ?pageIndex=N and model binding reads it back into <paramref name="pageIndex"/>.
        /// </summary>
        public IActionResult UrlPaging(int pageIndex = 1)
        {
            return View(GetPage(pageIndex));
        }

        /// <summary>
        /// A page whose script fetches every page as JSON from <see cref="JsonPage"/>.
        /// </summary>
        public IActionResult JsonPaging()
        {
            return View();
        }

        /// <summary>
        /// Page <paramref name="pageIndex"/> as JSON: the page itself, written by the
        /// PageJsonConverter registered in Program.cs, and the pager markup to show with it.
        /// </summary>
        public IActionResult JsonPage(int pageIndex = 1)
        {
            var page = GetPage(pageIndex);

            return Json(new { page, pager = RenderPager(page) });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Page <paramref name="pageIndex"/> of the demo items. The index comes from the request,
        /// so it can be anything: keep it within the pages there are, since ToPage throws below
        /// page 1 and past the last page returns no items.
        /// </summary>
        private static Page<int> GetPage(int pageIndex)
        {
            var lastPage = (int)Math.Ceiling(Items.Length / (double)PageSize);

            return Items.ToPage(Math.Clamp(pageIndex, 1, lastPage), PageSize);
        }

        /// <summary>
        /// The pager markup as a string, for a JSON response rather than a view. With "#" as the
        /// url the links don't navigate; they carry data-pageindex for the page's script instead.
        /// </summary>
        private static string RenderPager(IPage page)
        {
            using var writer = new StringWriter();

            new PagerRenderer().Render(page, JsonPagerOptions, "#").WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}

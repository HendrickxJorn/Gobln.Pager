#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

#endif

namespace Gobln.Pager.Mvc;

public static class HtmlExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="html">The HTML helper instance that this method extends.</param>
#if NETFRAMEWORK
    extension(HtmlHelper html)
#else
    extension(IHtmlHelper html)
#endif
    {
        /// <summary>
        /// Create pager with default <see cref="PagerOptions"/>
        /// </summary>
        /// <param name="page">Page that will be converted to an page</param>
        /// <returns></returns>
#if NETFRAMEWORK
        public MvcHtmlString Pager(IPage page)
#else
        public IHtmlContent Pager(IPage page)
#endif
        {
            return Pager(html, page, PagerSettings.DefaultPagerOptions);
        }

        /// <summary>
        /// Create pager
        /// </summary>
        /// <param name="page">Page that will be converted to an page</param>
        /// <param name="pagerOptions">PageOptions</param>
        /// <returns></returns>
#if NETFRAMEWORK
        public MvcHtmlString Pager(IPage page, IPagerOptions pagerOptions)
#else
        public IHtmlContent Pager(IPage page, IPagerOptions pagerOptions)
#endif
        {
            return Pager(html, page, pagerOptions, new PagerRenderer());
        }

#if NETFRAMEWORK
        public MvcHtmlString Pager<T>(IPage page, T pagerOptions, IPagerRenderer<T> pagerRenderer)
#else
        public IHtmlContent Pager<T>(IPage page, T pagerOptions, IPagerRenderer<T> pagerRenderer)
#endif
            where T : IPagerOptions
        {
#if NETFRAMEWORK
            if (pagerOptions is null)
            {
                throw new ArgumentNullException(nameof(pagerOptions));
            }
#else
            ArgumentNullException.ThrowIfNull(pagerOptions);
#endif

            pagerRenderer.SetOptions(pagerOptions, ResolveHRefUrl(html, pagerOptions.HRefUrl));

            return Pager(html, page, pagerRenderer);
        }

        /// <summary>
        /// Create pager
        /// </summary>
        /// <param name="page">Page that will be converted to an page</param>
        /// <param name="pagerRenderer"><see cref="IPagerRenderer"/></param>
        /// <returns></returns>
#if NETFRAMEWORK
        public MvcHtmlString Pager(IPage page, IPagerRenderer pagerRenderer)
#else
        public IHtmlContent Pager(IPage page, IPagerRenderer pagerRenderer)
#endif
        {
            return pagerRenderer.Render(page);
        }
    }

    /// <summary>
    /// The url the page links are built on for this request. Returned as a new value and
    /// never written back into the options: those are often shared between views and
    /// requests, and would pin the first request's url onto every later one.
    /// </summary>
#if NETFRAMEWORK
    private static string ResolveHRefUrl(HtmlHelper html, string? hRefUrl)
#else
    private static string ResolveHRefUrl(IHtmlHelper html, string? hRefUrl)
#endif
    {
        if (hRefUrl is null || string.IsNullOrWhiteSpace(hRefUrl))
        {
            return Infrastructure.UrlHelper.HasCurrentUrlString(html)
                ? Infrastructure.UrlHelper.GetCurrentUrlString(html)
                : "#";
        }

        if (hRefUrl is "#")
        {
            return hRefUrl;
        }

        return Uri.TryCreate(hRefUrl, UriKind.Absolute, out Uri? uriResult)
            ? uriResult.ToString()
            : hRefUrl;
    }
}
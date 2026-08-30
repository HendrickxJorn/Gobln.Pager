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
            // Clone, because rendering resolves and writes back HRefUrl on the options
            // instance. Handing out the shared default would pin the first request's url
            // into every later one, and race when requests overlap.
            return Pager(html, page, PagerSettings.DefaultPagerOptions.Clone());
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
            return Pager(html, page, pagerOptions, new PagerFactory());
        }

#if NETFRAMEWORK
        public MvcHtmlString Pager<T>(IPage page, T pagerOptions, IPagerFactory<T> pagerFactory)
#else
        public IHtmlContent Pager<T>(IPage page, T pagerOptions, IPagerFactory<T> pagerFactory)
#endif
            where T : IPagerOptions
        {
            if (pagerOptions is not null && pagerOptions.HRefUrl is not "#")
            {
                if (string.IsNullOrWhiteSpace(pagerOptions.HRefUrl))
                {
                    pagerOptions.HRefUrl = !Infrastructure.UrlHelper.HasCurrentUrlString(html)
                        ? "#"
                        : Infrastructure.UrlHelper.GetCurrentUrlString(html);
                }
                else
                {
                    pagerOptions.HRefUrl = Uri.TryCreate(pagerOptions.HRefUrl, UriKind.Absolute, out Uri? uriResult)
                        ? uriResult.ToString()
                        : pagerOptions.HRefUrl;
                }
            }

            pagerFactory.SetOptions(pagerOptions);

            return Pager(html, page, pagerFactory);
        }

        /// <summary>
        /// Create pager
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="page">Page that will be converted to an page</param>
        /// <param name="pagerFactory"><see cref="IPagerFactory"></param>
        /// <returns></returns>
#if NETFRAMEWORK
        public MvcHtmlString Pager(IPage page, IPagerFactory pagerFactory)
#else
        public IHtmlContent Pager(IPage page, IPagerFactory pagerFactory)
#endif
        {
            return pagerFactory.Render(page);
        }
    }
}
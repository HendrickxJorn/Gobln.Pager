using Gobln.Pager.Mvc.Contracts;

#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

#endif

namespace Gobln.Pager.Mvc
{
    public static class HtmlExtension
    {
        /// <summary>
        /// Create pager with default <see cref="PagerOptions"/>
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="page">Page that will be converted to an page</param>
        /// <returns></returns>
#if NETFRAMEWORK
        public static MvcHtmlString Pager(this HtmlHelper html, IPage page)
#else
        public static IHtmlContent Pager(this IHtmlHelper html, IPage page)
#endif
        {
            return Pager(html, page, Settings.DefaultPagerOptions);
        }

        /// <summary>
        /// Create pager
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="page">Page that will be converted to an page</param>
        /// <param name="pagerOptions">PageOptions</param>
        /// <returns></returns>
#if NETFRAMEWORK

        public static MvcHtmlString Pager(this HtmlHelper html, IPage page, IPagerOptions pagerOptions)
#else
        public static IHtmlContent Pager(this IHtmlHelper html, IPage page, IPagerOptions pagerOptions)
#endif
        {
            if (pagerOptions.HRefUrl is not "#")
            {
                if (string.IsNullOrWhiteSpace(pagerOptions.HRefUrl))
                {
                    if (!Infrastructure.UrlHelper.HasCurrentUrlString(html))
                    {
                        pagerOptions.HRefUrl = "#";
                    }
                    else
                    {
                        pagerOptions.HRefUrl = Infrastructure.UrlHelper.GetCurrentUrlString(html);
                    }
                }
                else
                {
                    pagerOptions.HRefUrl = Uri.TryCreate(pagerOptions.HRefUrl, UriKind.Absolute, out Uri? uriResult)
                        ? uriResult.ToString()
                        : null;
                }
            }

            return Pager(html, page, new PagerFactory(pagerOptions));
        }

        /// <summary>
        /// Create pager
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="page">Page that will be converted to an page</param>
        /// <param name="pagerFactory">IPagerFactory</param>
        /// <returns></returns>
#if NETFRAMEWORK

        public static MvcHtmlString Pager(this HtmlHelper html, IPage page, IPagerFactory pagerFactory)
#else
        public static IHtmlContent Pager(this IHtmlHelper html, IPage page, IPagerFactory pagerFactory)
#endif
        {
            pagerFactory.Validate();

            return pagerFactory.Render(page);
        }
    }
}
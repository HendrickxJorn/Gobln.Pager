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
    /// Create pager with default <see cref="PagerOptions"/>
    /// </summary>
    /// <param name="html">The HTML helper instance that this method extends.</param>
    /// <param name="page">Page that will be converted to an page</param>
    /// <returns></returns>
#if NETFRAMEWORK
    public static MvcHtmlString Pager(this HtmlHelper html, IPage page)
    {
#else
    public static IHtmlContent Pager(this IHtmlHelper html, IPage page)

    {
#endif
        return Pager(html, page, PagerSettings.DefaultPagerOptions);
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
        return Pager(html, page, pagerOptions, new PagerFactory());
    }

#if NETFRAMEWORK

    public static MvcHtmlString Pager<T>(this HtmlHelper html, IPage page, T pagerOptions, IPagerFactory<T> pagerFactory)
        where T : IPagerOptions
#else
    public static IHtmlContent Pager<T>(this IHtmlHelper html, IPage page, T pagerOptions, IPagerFactory<T> pagerFactory)
        where T : IPagerOptions
#endif
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
                    : null;
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

    public static MvcHtmlString Pager(this HtmlHelper html, IPage page, IPagerFactory pagerFactory)
#else
    public static IHtmlContent Pager(this IHtmlHelper html, IPage page, IPagerFactory pagerFactory)
#endif
    {
        return pagerFactory.Render(page);
    }
}
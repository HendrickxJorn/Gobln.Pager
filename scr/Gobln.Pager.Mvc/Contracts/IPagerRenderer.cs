#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;

#endif

namespace Gobln.Pager.Mvc;

/// <summary>
/// A renderer that is handed the options and the url with each render. A render gets
/// everything it needs as arguments, so a renderer has no need to keep any of it.
/// </summary>
public interface IPagerRenderer<T>
{
    /// <summary>
    /// Render the pager for <paramref name="page"/>
    /// </summary>
    /// <param name="page">The page to render the pager for</param>
    /// <param name="options">The options. Read only, never changed: they may be shared between requests.</param>
    /// <param name="hRefUrl">The url the page links are built on, already resolved for the current request. "#" makes every link "#".</param>
#if NETFRAMEWORK
    MvcHtmlString Render(IPage page, T options, string hRefUrl);
#else
    IHtmlContent Render(IPage page, T options, string hRefUrl);
#endif
}

/// <summary>
/// A renderer that needs nothing but the page
/// </summary>
public interface IPagerRenderer
{
#if NETFRAMEWORK
    MvcHtmlString Render(IPage page);
#else
    IHtmlContent Render(IPage page);
#endif
}

#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;

#endif

namespace Gobln.Pager.Mvc;

public interface IPagerRenderer<T>
    : IPagerRenderer
{
    /// <summary>
    /// Set the options and url for the next <see cref="IPagerRenderer.Render(IPage)"/>
    /// </summary>
    /// <param name="options">The options. Read only, never changed: they may be shared between requests.</param>
    /// <param name="hRefUrl">The url the page links are built on, already resolved for the current request. "#" makes every link "#".</param>
    void SetOptions(T options, string hRefUrl);
}

public interface IPagerRenderer
{
#if NETFRAMEWORK
    MvcHtmlString Render(IPage page);
#else
    IHtmlContent Render(IPage page);
#endif
}

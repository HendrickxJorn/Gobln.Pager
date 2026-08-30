#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;

#endif

namespace Gobln.Pager.Mvc;

public interface IPagerRenderer<T>
    : IPagerRenderer
{
    void SetOptions(T options);
}

public interface IPagerRenderer
{
#if NETFRAMEWORK
    MvcHtmlString Render(IPage page);
#else
    IHtmlContent Render(IPage page);
#endif
}

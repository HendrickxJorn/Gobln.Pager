#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;

#endif

namespace Gobln.Pager.Mvc;

public interface IPagerFactory<T>
    : IPagerFactory
{
    void SetOptions(T options);
}

public interface IPagerFactory
{
#if NETFRAMEWORK
    MvcHtmlString Render(IPage page);
#else
    IHtmlContent Render(IPage page);
#endif
}

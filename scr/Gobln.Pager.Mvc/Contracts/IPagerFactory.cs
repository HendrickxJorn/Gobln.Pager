#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;

#endif

namespace Gobln.Pager.Mvc.Contracts
{
    public interface IPagerFactory
    {
        //todo remove ???
        void Validate();

#if NETFRAMEWORK
        MvcHtmlString Render(IPage page);
#else
        IHtmlContent Render(IPage page);
#endif
    }
}

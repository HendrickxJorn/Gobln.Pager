#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Mvc.Rendering;

#endif

namespace Gobln.Pager.Mvc.Infrastructure;

internal static class TagBuilderHelper
{
#if NETFRAMEWORK

    internal static void InnerHtmlAppend(this TagBuilder tb, TagBuilder htmlContent)
    {
        tb.InnerHtml += htmlContent.ToString(TagRenderMode.Normal);
    }

#else

    internal static void InnerHtmlAppend(this TagBuilder tb, TagBuilder htmlContent)
    {
        tb.InnerHtml.AppendHtml(htmlContent);
    }

#endif

#if NETFRAMEWORK

    internal static void InnerHtmlAppend(this TagBuilder tb, string content)
    {
        tb.InnerHtml += content;
    }

#else

    internal static void InnerHtmlAppend(this TagBuilder tb, string content)
    {
        tb.InnerHtml.Append(content);
    }

#endif
}
#if NETFRAMEWORK

using System.Web;
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

    /// <summary>
    /// Append <paramref name="content"/> as text, escaped
    /// </summary>
#if NETFRAMEWORK

    internal static void InnerHtmlAppend(this TagBuilder tb, string content)
    {
        tb.InnerHtml += HttpUtility.HtmlEncode(content);
    }

#else

    internal static void InnerHtmlAppend(this TagBuilder tb, string content)
    {
        tb.InnerHtml.Append(content);
    }

#endif

    /// <summary>
    /// Append <paramref name="html"/> as markup, unescaped. Only for markup the developer
    /// wrote, such as icons, never for text.
    /// </summary>
#if NETFRAMEWORK

    internal static void InnerHtmlAppendHtml(this TagBuilder tb, string html)
    {
        tb.InnerHtml += html;
    }

#else

    internal static void InnerHtmlAppendHtml(this TagBuilder tb, string html)
    {
        tb.InnerHtml.AppendHtml(html);
    }

#endif
}
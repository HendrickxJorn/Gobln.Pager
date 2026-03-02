using System.Web;

#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;


#endif


namespace Gobln.Pager.Mvc.Infrastructure;

internal static class UrlHelper
{
#if NETFRAMEWORK

    public static bool HasCurrentUrlString(HtmlHelper html) => (html.ViewContext.HttpContext.Request.Url?.AbsoluteUri ?? "#") != "#";

    public static string GetCurrentUrlString(HtmlHelper html, string[]? excludeParameters = null)
    { 
        var uriBuilder = new UriBuilder(html.ViewContext.HttpContext.Request.Url.AbsoluteUri);

#else
    public static bool HasCurrentUrlString(IHtmlHelper html) => html.ViewContext?.HttpContext?.Request is not null;

    public static string GetCurrentUrlString(IHtmlHelper html, string[]? excludeParameters = null)
    {
        var absoluteUri = new Uri($"{html.ViewContext.HttpContext.Request.Scheme}://{html.ViewContext.HttpContext.Request.Host}{html.ViewContext.HttpContext.Request.Path}{html.ViewContext.HttpContext.Request.QueryString}");

        var uriBuilder = new UriBuilder(absoluteUri);
#endif

        var parseQueryString = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (parseQueryString.HasKeys())
        {
            foreach (var param in new string[] { "x-requested-with", "xmlhttprequest" })
            {
                parseQueryString.Remove(param);
            }

            if (excludeParameters != null)
            {
                foreach (var param in excludeParameters)
                {
                    parseQueryString.Remove(param);
                }
            }
        }

        uriBuilder.Query = parseQueryString.ToString();

        return uriBuilder.Uri.AbsoluteUri;
    }

    public static string AddQueryValuesToUrlString(string url, Dictionary<string, string> queryValues)
    {
        var uriBuilder = new UriBuilder(url);

        var parseQueryString = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (parseQueryString.HasKeys())
        {
            if (queryValues != null)
            {
                foreach (var param in queryValues.Select(c => c.Key))
                {
                    parseQueryString.Remove(param);
                }
            }
        }

        if (queryValues != null)
        {
            foreach (var param in queryValues)
            {
                parseQueryString.Add(param.Key, param.Value);
            }
        }

        uriBuilder.Query = parseQueryString.ToString();

        return uriBuilder.Uri.AbsoluteUri;
    }
}

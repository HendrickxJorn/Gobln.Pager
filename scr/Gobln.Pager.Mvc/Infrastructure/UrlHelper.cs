using System.Web;

#if NETFRAMEWORK

using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Mvc.Rendering;

#endif

namespace Gobln.Pager.Mvc.Infrastructure;

internal static class UrlHelper
{
    // The current url is given back relative: its path and query only. The host of a request
    // comes from its Host header, which the client controls, so it must not end up in the page.

#if NETFRAMEWORK

    public static bool HasCurrentUrlString(HtmlHelper html) => html.ViewContext.HttpContext.Request.Url is not null;

    public static string GetCurrentUrlString(HtmlHelper html, string[]? excludeParameters = null)
    {
        // PathAndQuery includes the application path of an app in an IIS virtual directory
        var currentUrl = html.ViewContext.HttpContext.Request.Url.PathAndQuery;

#else

    public static bool HasCurrentUrlString(IHtmlHelper html) => html.ViewContext?.HttpContext?.Request is not null;

    public static string GetCurrentUrlString(IHtmlHelper html, string[]? excludeParameters = null)
    {
        var request = html.ViewContext.HttpContext.Request;

        // PathBase is the path the app is mounted on (an IIS virtual directory, UsePathBase).
        // It is not part of Path, and links without it leave the app.
        var currentUrl = $"{request.PathBase}{request.Path}{request.QueryString}";
#endif

        // Built on the placeholder, so a path such as "//other.test/x" can't become a link to another host
        var uriBuilder = new UriBuilder(new Uri(PlaceholderBase, currentUrl));

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

        return uriBuilder.Uri.PathAndQuery;
    }

    // Only there to give a relative url something to build on; it never ends up in a link
    private static readonly Uri PlaceholderBase = new("http://placeholder/");

    /// <summary>
    /// Is <paramref name="url"/> an absolute http(s) url. Scheme checked, because on Linux and
    /// macOS a path such as "/products" parses as an absolute file:// uri.
    /// </summary>
    public static bool IsAbsoluteHttpUrl(string url, out Uri? uri)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return true;
        }

        uri = null;
        return false;
    }

    public static string AddQueryValuesToUrlString(string url, Dictionary<string, string> queryValues)
    {
        // UriBuilder only takes an absolute url. A relative one is built on a placeholder and
        // given back relative, so its links stay on whatever host serves the page.
        var isAbsolute = IsAbsoluteHttpUrl(url, out var absoluteUri);

        var uriBuilder = new UriBuilder(isAbsolute ? absoluteUri! : new Uri(PlaceholderBase, url));

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

        return isAbsolute
            ? uriBuilder.Uri.AbsoluteUri
            : uriBuilder.Uri.PathAndQuery + uriBuilder.Uri.Fragment;
    }
}

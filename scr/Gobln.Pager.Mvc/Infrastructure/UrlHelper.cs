using System.Web;

#if NETFRAMEWORK

using System.Web.Mvc;

#else

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
    // A request without a Host header (HTTP/1.0) has no absolute url to build on
    public static bool HasCurrentUrlString(IHtmlHelper html) => html.ViewContext?.HttpContext?.Request is { Host.HasValue: true };

    public static string GetCurrentUrlString(IHtmlHelper html, string[]? excludeParameters = null)
    {
        var request = html.ViewContext.HttpContext.Request;

        // PathBase is the path the app is mounted on (an IIS virtual directory, UsePathBase).
        // It is not part of Path, and links without it leave the app.
        var absoluteUri = new Uri($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}");

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

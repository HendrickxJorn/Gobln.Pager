using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace Gobln.Pager.MvcTest.Helper
{
    internal static class HtmlContentExtensions
    {
        public static string ToHtmlString(this IHtmlContent content)
        {
            return ToHtmlString(content, HtmlEncoder.Default);
        }

        public static string ToHtmlString(this IHtmlContent content, HtmlEncoder? encoder = null)
        {
            if (content is HtmlString htmlString)
            {
                return htmlString.Value ?? string.Empty;
            }

            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, encoder ?? HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}
using WebMarkupMin.Core;

namespace Gobln.Pager.Mvc48Test.Helper
{
    internal static class HtmlStringHelper
    {
        private static readonly HtmlMinifier _htmlMinifier = new(new HtmlMinificationSettings(false)
        {
            AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes,
            RemoveRedundantAttributes = false,
            RemoveOptionalEndTags = false,
            WhitespaceMinificationMode = WhitespaceMinificationMode.Aggressive,
        });

        public static string Minify(string value)
        {
            return _htmlMinifier.Minify(value).MinifiedContent;
        }
    }
}
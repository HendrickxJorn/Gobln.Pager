using Gobln.Pager.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gobln.Pager.Test.Helper
{
    internal class Bootstrap3Pager() : PagerRenderer()
    {
        public override TagBuilder TagCreatorLi(bool isDisabled = false, bool isActive = false, Dictionary<string, string?>? attributes = null)
        {
            var tagBuilder = new TagBuilder("li");

            if (attributes is not null && attributes.Count > 0)
            {
                tagBuilder.MergeAttributes(attributes);
            }

            if (isDisabled && !isActive)
            {
                tagBuilder.AddCssClass("disabled");
            }

            if (isActive)
            {
                tagBuilder.AddCssClass("active");
            }

            return tagBuilder;
        }

        public override TagBuilder TagCreatorLink(string label, int pageIndex, string? icon = null, bool disableTabing = false, string? dataIndexName = null, Dictionary<string, string?>? attributes = null)
        {
            var tagBuilder = new TagBuilder("a");

            if (!string.IsNullOrWhiteSpace(dataIndexName))
            {
                tagBuilder.Attributes.Add($"data-{dataIndexName}", pageIndex.ToString());
            }

            if (disableTabing)
            {
                tagBuilder.Attributes.Add("tabindex", "-1");
            }

            var url = "#";

            if (PagerOptions.HRefUrl is not "#")
            {
                var tempUrl = UrlCreator(pageIndex);

                if (!string.IsNullOrWhiteSpace(tempUrl))
                {
                    url = tempUrl;
                }
            }

            tagBuilder.Attributes.Add("href", url);

            if (icon is null || string.IsNullOrWhiteSpace(icon))
            {
                tagBuilder.InnerHtml.Append(label);
            }
            else
            {
                tagBuilder.InnerHtml.AppendHtml(TagCreatorSpanIcon(icon));
                tagBuilder.MergeAttribute("aria-label", label);
            }

            return tagBuilder;
        }

        public override TagBuilder TagCreatorSpan(string label, string? icon = null, bool disableTabing = false, bool isActive = false, Dictionary<string, string?>? attributes = null)
        {
            var tagBuilder = new TagBuilder("span");

            if (attributes is not null && attributes.Count > 0)
            {
                tagBuilder.MergeAttributes(attributes);
            }

            if (disableTabing)
            {
                tagBuilder.Attributes.Add("tabindex", "-1");
            }

            if (icon is not null && !string.IsNullOrWhiteSpace(icon))
            {
                tagBuilder.InnerHtml.AppendHtml(TagCreatorSpanIcon(icon));
            }
            else if(isActive)
            {
                tagBuilder.InnerHtml.Append(label + " ");

                var attributesCurrentSpan = new Dictionary<string, string?>()
                {
                    { "class", "sr-only" }
                };

                tagBuilder.InnerHtml.AppendHtml(TagCreatorSpan("(current)", attributes: attributesCurrentSpan));
    }
            else
            {
                tagBuilder.InnerHtml.Append(label);
            }

            return tagBuilder;
        }

    }
}
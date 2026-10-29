using Gobln.Pager.Mvc;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Gobln.Pager.Mvc48Test.Helper;

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

    public override TagBuilder TagCreatorLink(string label, int pageIndex, string? icon = null, string? dataIndexName = null, bool isActive = false, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = new TagBuilder("a");

        if (!string.IsNullOrWhiteSpace(dataIndexName))
        {
            tagBuilder.Attributes.Add($"data-{dataIndexName}", pageIndex.ToString());
        }

        var url = UrlCreator(pageIndex);

        tagBuilder.Attributes.Add("href", string.IsNullOrWhiteSpace(url) ? "#" : url);

        if (icon is null || string.IsNullOrWhiteSpace(icon))
        {
            tagBuilder.InnerHtml += label;

            if (isActive)
            {
                // Bootstrap 3 marks the current page with screen reader text, not aria-current
                tagBuilder.InnerHtml += " " + TagCreatorSpan("(current)", attributes: new() { { "class", "sr-only" } });
            }
        }
        else
        {
            tagBuilder.InnerHtml += TagCreatorSpanIcon(icon);
            tagBuilder.MergeAttribute("aria-label", label);
        }

        return tagBuilder;
    }

    public override TagBuilder TagCreatorSpan(string label, string? icon = null, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = new TagBuilder("span");

        if (attributes is not null && attributes.Count > 0)
        {
            tagBuilder.MergeAttributes(attributes);
        }

        if (icon is not null && !string.IsNullOrWhiteSpace(icon))
        {
            tagBuilder.InnerHtml += TagCreatorSpanIcon(icon);
        }
        else
        {
            tagBuilder.InnerHtml += label;
        }

        return tagBuilder;
    }

}
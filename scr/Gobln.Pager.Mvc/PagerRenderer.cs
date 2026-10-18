using Gobln.Pager.Mvc.Infrastructure;

#if NETFRAMEWORK
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif

namespace Gobln.Pager.Mvc;

public class PagerRenderer() : IPagerRenderer<IPagerOptions>
{
    // The page, options and url below are those of the render in progress. Outside a render
    // they are an empty page, the default options and no url.

    internal IPage InternalPage { get; private set; } = new Page();

    public IPagerOptions PagerOptions { get; private set; } = PagerSettings.DefaultPagerOptions;

    /// <summary>
    /// The url the page links are built on for this render: <see cref="IPagerOptions.HRefUrl"/>,
    /// or the current request's url when that is not set. "#" makes every link "#".
    /// </summary>
    public string HRefUrl { get; private set; } = "#";

    /// <summary>
    /// Render the pager for <paramref name="page"/>. Nothing of a render is kept on the
    /// renderer, so one renderer can be shared between views and requests.
    /// </summary>
    /// <param name="page">The page to render the pager for</param>
    /// <param name="options">The options. Read only, never changed: they may be shared between requests.</param>
    /// <param name="hRefUrl">The url the page links are built on, already resolved for the current request. "#" makes every link "#".</param>
#if NETFRAMEWORK
    public virtual MvcHtmlString Render(IPage page, IPagerOptions options, string hRefUrl)
#else
    public virtual IHtmlContent Render(IPage page, IPagerOptions options, string hRefUrl)
#endif
    {
#if NETFRAMEWORK
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (hRefUrl is null)
        {
            throw new ArgumentNullException(nameof(hRefUrl));
        }
#else
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(hRefUrl);
#endif

        // No page renders nothing, even with Always: there is nothing to show a pager for
        if (page is null
            || (options.ShowPager != WhenToShowEnum.Always
                && !(page.HasPaging && options.ShowPager == WhenToShowEnum.IfNecessary)))
        {
#if NETFRAMEWORK
            return MvcHtmlString.Create(string.Empty);
#else
            return new HtmlContentBuilder();
#endif
        }

        // The page, options and url of this render go on a copy of the renderer, never on the
        // renderer itself. Renders that share a renderer then can't get each other's.
        var renderer = (PagerRenderer)MemberwiseClone();

        renderer.InternalPage = page;
        renderer.PagerOptions = options;
        renderer.HRefUrl = hRefUrl;

#if NETFRAMEWORK
        return MvcHtmlString.Create(renderer.RenderPager().ToString(TagRenderMode.Normal));
#else
        return renderer.RenderPager();
#endif
    }

    private TagBuilder RenderPager()
    {
        var tagBuilders = new List<TagBuilder>();

        if( PagerOptions.ShowButtonFirstLink == WhenToShowEnum.Always
            || (PagerOptions.ShowButtonFirstLink == WhenToShowEnum.IfNecessary && InternalPage.PageIndex > 1))
        {
            tagBuilders.Add(CreateFirstTag());
        }

        if (PagerOptions.ShowButtonPreviousLink == WhenToShowEnum.Always 
            || (PagerOptions.ShowButtonPreviousLink == WhenToShowEnum.IfNecessary && InternalPage.PageIndex > 1))
        {
            tagBuilders.Add(CreatePreviousTag());
        }

        if (PagerOptions.ShowPagerButtonLinks == WhenToShowEnum.Always
            || (PagerOptions.ShowPagerButtonLinks == WhenToShowEnum.IfNecessary && InternalPage.HasPaging))
        {
            tagBuilders.AddRange(PageNumbersCreator());
        }

        if (PagerOptions.ShowButtonNextLink == WhenToShowEnum.Always
            || (PagerOptions.ShowButtonNextLink == WhenToShowEnum.IfNecessary && InternalPage.PageIndex < InternalPage.TotalPageCount))
        {
            tagBuilders.Add(CreateNextTag());
        }

        if (PagerOptions.ShowButtonLastLink == WhenToShowEnum.Always
            || (PagerOptions.ShowButtonLastLink == WhenToShowEnum.IfNecessary && InternalPage.PageIndex < InternalPage.TotalPageCount))
        {
            tagBuilders.Add(CreateLastTag());
        }

        var ul = TagCreatorUl();

        tagBuilders.ForEach(ul.InnerHtmlAppend);

        var nav = TagCreatorNav();

        nav.InnerHtmlAppend(ul);

        return nav;
    }

    public virtual TagBuilder CreateFirstTag()
    {
        return PrivateButtonCreator(1, PagerOptions.ButtonFirstLinkLabel, PagerOptions.ButtonFirstLinkIcon);
    }

    public virtual TagBuilder CreatePreviousTag()
    {
        var index = InternalPage.PageIndex - 1;

        index = index < 1 ? 1 : index;

        return PrivateButtonCreator(index, PagerOptions.ButtonPreviousLinkLabel, PagerOptions.ButtonPreviousLinkIcon);
    }

    public virtual TagBuilder CreateNextTag()
    {
        var index = InternalPage.PageIndex + 1;

        index = index > InternalPage.TotalPageCount ? InternalPage.TotalPageCount : index;

        return PrivateButtonCreator(index, PagerOptions.ButtonNextLinkLabel, PagerOptions.ButtonNextLinkIcon);
    }

    public virtual TagBuilder CreateLastTag()
    {
        return PrivateButtonCreator(InternalPage.TotalPageCount, PagerOptions.ButtonLastLinkLabel, PagerOptions.ButtonLastLinkIcon);
    }

    #region Generate

    private TagBuilder PrivateTagCreator(string name, object? attributes = null, string[]? cssClasses = null)
    {
        var tagBuilder = new TagBuilder(name);

        if (attributes is not null)
        {
            if (attributes is IDictionary<string, string?> dict)
            {
                tagBuilder.MergeAttributes(dict);
            }
            else
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
            }
        }

        if(cssClasses is not null)
        {
            tagBuilder.AddCssClass(string.Join(" ", cssClasses.Distinct().Where(c => !string.IsNullOrWhiteSpace(c))));
        }

        return tagBuilder;
    }

    /// <summary>
    /// A First/Previous/Next/Last button. When it points at the current page and
    /// <see cref="IPagerOptions.AllowDisabledPageLink"/> is set, it is disabled: styled as
    /// disabled and rendered as a span, so it can't be clicked. Otherwise it is a working link.
    /// </summary>
    private TagBuilder PrivateButtonCreator(int index, string label, string? icon)
    {
        var isDisabled = InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink;

        var li = TagCreatorLi(isDisabled);

        li.InnerHtmlAppend(
            isDisabled
            ? TagCreatorSpan(label, icon: icon)
            : TagCreatorLink(label, index, icon: icon, dataIndexName: PagerOptions.PageIndexDataName)
        );

        return li;
    }

    private TagBuilder PrivatePageNumberCreator(int index)
    {
        var isActive = PagerOptions.DisplayActive && InternalPage.PageIndex == index;

        var text = PagerOptions.ShowIndexAsRange
            ? $"{((index - 1) * InternalPage.PageSize) + 1} - {Math.Min(index * InternalPage.PageSize, InternalPage.TotalItemCount)}"
            : index.ToString();

        var isDisabled = InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink;

        var li = TagCreatorLi(
            isDisabled,
            isActive: isActive
            );

        // A disabled page is a span: a disabled link would still be announced as working.
        // The active page stays a link, as in Bootstrap 5.3.
        var link = isDisabled && !isActive
            ? TagCreatorSpan(text)
            : TagCreatorLink(text, index, dataIndexName: PagerOptions.PageIndexDataName, isActive: isActive);

        li.InnerHtmlAppend(link);

        return li;
    }

    public virtual TagBuilder TagCreatorNav()
    {
        var tagBuilder = PrivateTagCreator("nav", attributes: PagerOptions.NavTagHtmlAttributes);

        // A label tells the pager apart from the site's other navigation. The caller's own label wins.
        if (!tagBuilder.Attributes.ContainsKey("aria-label") && !tagBuilder.Attributes.ContainsKey("aria-labelledby"))
        {
            tagBuilder.Attributes.Add("aria-label", "Page navigation");
        }

        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorUl()
    {
        return PrivateTagCreator("ul", attributes: PagerOptions.UlTagHtmlAttributes, cssClasses: ["pagination"]);
    }

    public virtual TagBuilder TagCreatorLi(bool isDisabled = false, bool isActive = false, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("li", attributes: attributes);

        if (isDisabled && !isActive)
        {
            tagBuilder.AddCssClass("disabled");
        }

        if (isActive)
        {
            tagBuilder.AddCssClass("active");
        }

        tagBuilder.AddCssClass("page-item");


        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorLink(string label, int pageIndex, string? icon = null, string? dataIndexName = null, bool isActive = false, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("a", attributes: attributes, cssClasses: [ "page-link" ]);

        if (isActive)
        {
            tagBuilder.MergeAttribute("aria-current", "page");
        }

        if (!string.IsNullOrWhiteSpace(dataIndexName) && !(pageIndex > InternalPage.TotalPageCount || pageIndex < 1))
        {
            tagBuilder.Attributes.Add($"data-{dataIndexName}", pageIndex.ToString());
        }

        var url = UrlCreator(pageIndex);

        tagBuilder.Attributes.Add("href", string.IsNullOrWhiteSpace(url) ? "#" : url);

        if (icon is null || string.IsNullOrWhiteSpace(icon))
        {
            tagBuilder.InnerHtmlAppend(label);
        }
        else
        {
            tagBuilder.InnerHtmlAppend(TagCreatorSpanIcon(icon));
            tagBuilder.MergeAttribute("aria-label", label);
        }

        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorSpan(string label, string? icon = null, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("span", attributes: attributes, cssClasses: ["page-link"]);

        if (icon is null || string.IsNullOrWhiteSpace(icon))
        {
            tagBuilder.InnerHtmlAppend(label);
        }
        else
        {
            tagBuilder.InnerHtmlAppend(TagCreatorSpanIcon(icon));
            tagBuilder.MergeAttribute("aria-label", label);
        }

        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorSpanIcon(string label, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("span", attributes: attributes);

        tagBuilder.Attributes.Add("aria-hidden", "true");

        // Icons are markup (an entity or an icon font element), so they are not escaped
        tagBuilder.InnerHtmlAppendHtml(label);

        return tagBuilder;
    }

    public virtual IEnumerable<TagBuilder> PageNumbersCreator()
    {
        var tagBuilderList = new List<TagBuilder>();

        var pageNumbers = PageWindow.GetPageNumbers(
            InternalPage.PageIndex,
            InternalPage.TotalPageCount,
            PagerOptions.SiblingCount,
            PagerOptions.BoundaryCount,
            PagerOptions.ShowEllipsis);

        foreach (var pageNumber in pageNumbers)
        {
            if (pageNumber is int index)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(index));
            }
            else
            {
                var li = TagCreatorLi(true);
                li.InnerHtmlAppend(TagCreatorSpan("..."));
                tagBuilderList.Add(li);
            }
        }

        return tagBuilderList;
    }

    /// <summary>
    /// Generate Url link for <paramref name="pageIndex"/>
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public virtual string? UrlCreator(int pageIndex)
    {
        if (HRefUrl is "#" || InternalPage.TotalPageCount < pageIndex || pageIndex < 1)
        {
            return null;
        }

        return Gobln.Pager.Mvc.Infrastructure.UrlHelper.AddQueryValuesToUrlString(
            HRefUrl,
            new Dictionary<string, string> { { PagerOptions.PageIndexUrlName, pageIndex.ToString() } }
            );
    }

#endregion Generate

}
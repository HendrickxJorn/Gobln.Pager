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
    internal IPage InternalPage { get; set; } = default!;

    public IPagerOptions PagerOptions { get; internal set; } = default!;

    /// <summary>
    /// The url the page links are built on for this render: <see cref="IPagerOptions.HRefUrl"/>,
    /// or the current request's url when that is not set. "#" makes every link "#".
    /// </summary>
    public string HRefUrl { get; internal set; } = "#";

    public void SetOptions(IPagerOptions options, string hRefUrl)
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
        PagerOptions = options;
        HRefUrl = hRefUrl;
    }

#if NETFRAMEWORK
    public virtual MvcHtmlString Render(IPage page)
#else
    public virtual IHtmlContent Render(IPage page)
#endif
    {
        if (PagerOptions.ShowPager != WhenToShowEnum.Always
            && (page == null
                || !(page.HasPaging && PagerOptions.ShowPager == WhenToShowEnum.IfNecessary)))
        {
#if NETFRAMEWORK
            return MvcHtmlString.Create(string.Empty);
#else
            return new HtmlContentBuilder();
#endif
        }

        InternalPage = page;

#if NETFRAMEWORK
        return MvcHtmlString.Create(RenderPager().ToString(TagRenderMode.Normal));
#else
        return RenderPager();
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
     
        var elipseModifier = PagerOptions.ShowEllipsis ? 1 : 0;

        var startNumber =
            Math.Max(
                Math.Min(
                    // Original start
                    InternalPage.PageIndex - PagerOptions.SiblingCount,
                    // Page boundery +/- elipse
                    InternalPage.TotalPageCount - PagerOptions.BoundaryCount - PagerOptions.SiblingCount * 2 - elipseModifier
                ),
                // After start +/- elipse
                PagerOptions.BoundaryCount + 1 + elipseModifier
            );

        var endNumber = 
            Math.Min(
                Math.Max(
                    // Original end
                    InternalPage.PageIndex + PagerOptions.SiblingCount,
                    // Page boundery +/- elipse
                    PagerOptions.BoundaryCount + PagerOptions.SiblingCount * 2 + 1 + elipseModifier

                ),
                // Before end +/- elipse
                InternalPage.TotalPageCount - PagerOptions.BoundaryCount - elipseModifier
            );

        var firstIndex = Math.Min(PagerOptions.BoundaryCount, InternalPage.TotalPageCount) - 1 + 1;
        var lastIndex = InternalPage.TotalPageCount - Math.Max(InternalPage.TotalPageCount - PagerOptions.BoundaryCount + 1, PagerOptions.BoundaryCount + 1) + 1;

        if (firstIndex > 0)
        {
            for (int index = 1; index <= firstIndex; index++)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(index));
            }
        }

        if (PagerOptions.ShowEllipsis)
        {
            if (startNumber > PagerOptions.BoundaryCount + 2)
            {
                var li = TagCreatorLi(true);
                li.InnerHtmlAppend(TagCreatorSpan("..."));
                tagBuilderList.Add(li);
            }
            else if (PagerOptions.BoundaryCount + 1 < InternalPage.TotalPageCount - PagerOptions.BoundaryCount)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(PagerOptions.BoundaryCount + 1));
            }
        }

        for (int index = startNumber; index <= endNumber; index++)
        {
            tagBuilderList.Add(PrivatePageNumberCreator(index));
        }

        if (PagerOptions.ShowEllipsis)
        {
            if (endNumber < InternalPage.TotalPageCount - PagerOptions.BoundaryCount - 1)
            {
                var li = TagCreatorLi(true);
                li.InnerHtmlAppend(TagCreatorSpan("..."));
                tagBuilderList.Add(li);
            }
            else if (InternalPage.TotalPageCount - PagerOptions.BoundaryCount > PagerOptions.BoundaryCount)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(InternalPage.TotalPageCount - PagerOptions.BoundaryCount));
            }
        }

        if (lastIndex <= InternalPage.TotalPageCount)
        {
            // Start after the first boundary, so overlapping boundaries don't render a page twice
            for (int index = Math.Max(InternalPage.TotalPageCount - PagerOptions.BoundaryCount + 1, firstIndex + 1); index <= InternalPage.TotalPageCount; index++)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(index));
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
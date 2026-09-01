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

    public IPagerOptions PagerOptions { get; internal set; }

    public void SetOptions(IPagerOptions options)
    {
#if NETFRAMEWORK
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }
#else
        ArgumentNullException.ThrowIfNull(options);
#endif
        PagerOptions = options;
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
            || PagerOptions.ShowPagerButtonLinks == WhenToShowEnum.IfNecessary)
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
        var index = 1;

        var isActive = InternalPage.PageIndex == index && PagerOptions.DisplayActive;

        var li = TagCreatorLi(InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink);

        li.InnerHtmlAppend(
            isActive
            ? TagCreatorSpan(PagerOptions.ButtonFirstLinkLabel,
                icon: PagerOptions.ButtonFirstLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
            : TagCreatorLink(PagerOptions.ButtonFirstLinkLabel, index,
                icon: PagerOptions.ButtonFirstLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                dataIndexName: PagerOptions.PageIndexDataName)
        );

        return li;
    }

    public virtual TagBuilder CreatePreviousTag()
    {
        var index = InternalPage.PageIndex - 1;

        index = index < 1 ? 1 : index;

        var isActive = InternalPage.PageIndex == index && PagerOptions.DisplayActive;

        var li = TagCreatorLi(InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink);

        li.InnerHtmlAppend(
            isActive
            ? TagCreatorSpan(PagerOptions.ButtonPreviousLinkLabel,
                icon: PagerOptions.ButtonPreviousLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
            : TagCreatorLink(PagerOptions.ButtonPreviousLinkLabel, index,
                icon: PagerOptions.ButtonPreviousLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                dataIndexName: PagerOptions.PageIndexDataName)
        );

        return li;
    }

    public virtual TagBuilder CreateNextTag()
    {
        var index = InternalPage.PageIndex + 1;

        index = index > InternalPage.TotalPageCount ? InternalPage.TotalPageCount : index;

        var isActive = InternalPage.PageIndex == index && PagerOptions.DisplayActive;

        var li = TagCreatorLi(InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink);

        li.InnerHtmlAppend(
            isActive
            ? TagCreatorSpan(PagerOptions.ButtonNextLinkLabel,
                icon: PagerOptions.ButtonNextLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
            : TagCreatorLink(PagerOptions.ButtonNextLinkLabel, index,
                icon: PagerOptions.ButtonNextLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                dataIndexName: PagerOptions.PageIndexDataName)
        );

        return li;
    }

    public virtual TagBuilder CreateLastTag()
    {
        var index = InternalPage.TotalPageCount;

        var isActive = InternalPage.PageIndex == index && PagerOptions.DisplayActive;

        var li = TagCreatorLi(InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink);

        li.InnerHtmlAppend(
            isActive
            ? TagCreatorSpan(PagerOptions.ButtonLastLinkLabel,
                icon: PagerOptions.ButtonLastLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
            : TagCreatorLink(PagerOptions.ButtonLastLinkLabel, index,
                icon: PagerOptions.ButtonLastLinkIcon,
                disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                dataIndexName: PagerOptions.PageIndexDataName)
        );

        return li;
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

    private TagBuilder PrivatePageNumberCreator(int index)
    {
        var isActive = PagerOptions.DisplayActive && InternalPage.PageIndex == index;

        var text = PagerOptions.ShowIndexAsRange
            ? $"{((index - 1) * InternalPage.PageSize) + 1} - {((index - 1) * InternalPage.PageSize) + InternalPage.PageSize}"
            : index.ToString();

        var li = TagCreatorLi(
            InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLink,
            isActive: isActive
            );

        var link = isActive
            ? TagCreatorSpan(text,
                    disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                    isActive: isActive)
            : TagCreatorLink(text, index,
                    disableTabing: InternalPage.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                    dataIndexName: PagerOptions.PageIndexDataName);

        li.InnerHtmlAppend(link);

        return li;
    }

    public virtual TagBuilder TagCreatorNav()
    {
        return PrivateTagCreator("nav", attributes: PagerOptions.NavTagHtmlAttributes);
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

            tagBuilder.MergeAttributes(new Dictionary<string, string?> { { "aria-current", "page" } });
        }

        tagBuilder.AddCssClass("page-item");


        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorLink(string label, int pageIndex, string? icon = null, bool disableTabing = false, string? dataIndexName = null, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("a", attributes: attributes, cssClasses: [ "page-link" ]);

        if (!string.IsNullOrWhiteSpace(dataIndexName) && !(pageIndex > InternalPage.TotalPageCount || pageIndex < 1))
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
            tagBuilder.InnerHtmlAppend(label);
        }
        else
        {
            tagBuilder.InnerHtmlAppend(TagCreatorSpanIcon(icon));
            tagBuilder.MergeAttribute("aria-label", label);
        }

        return tagBuilder;
    }

    public virtual TagBuilder TagCreatorSpan(string label, string? icon = null, bool disableTabing = false, bool isActive = false, Dictionary<string, string?>? attributes = null)
    {
        var tagBuilder = PrivateTagCreator("span", attributes: attributes, cssClasses: ["page-link"]);

        if (disableTabing)
        {
            tagBuilder.Attributes.Add("tabindex", "-1");
        }

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

        tagBuilder.InnerHtmlAppend(label);

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
                li.InnerHtmlAppend(TagCreatorSpan("...", disableTabing: true));
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
                li.InnerHtmlAppend(TagCreatorSpan("...", disableTabing: true));
                tagBuilderList.Add(li);
            }
            else if (InternalPage.TotalPageCount - PagerOptions.BoundaryCount > PagerOptions.BoundaryCount)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(InternalPage.TotalPageCount - PagerOptions.BoundaryCount));
            }
        }

        if (lastIndex <= InternalPage.TotalPageCount)
        {
            for (int index = InternalPage.TotalPageCount - PagerOptions.BoundaryCount + 1; index <= InternalPage.TotalPageCount; index++)
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
        if (InternalPage.TotalPageCount < pageIndex || pageIndex < 1)
        {
            return null;
        }

        return Gobln.Pager.Mvc.Infrastructure.UrlHelper.AddQueryValuesToUrlString(
            PagerOptions.HRefUrl ?? "#",
            new Dictionary<string, string> { { PagerOptions.PageIndexUrlName, pageIndex.ToString() } }
            );
    }

#endregion Generate

}
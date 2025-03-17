using Gobln.Pager.Mvc.Infrastructure;

#if NETFRAMEWORK
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif

// TODO Rename????
// TODO make functions virtual
namespace Gobln.Pager.Mvc
{
   public class PagerFactory() : IPagerFactory<IPagerOptions>
    {
        internal IPage _page { get; set; }

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

            _page = page;

#if NETFRAMEWORK
            return MvcHtmlString.Create(RenderPager().ToString(TagRenderMode.Normal));
#else
            return RenderPager();
#endif
        }

        private TagBuilder RenderPager()
        {
            var tagBuilders = new List<TagBuilder>();

            if( PagerOptions.ShowFirstButtonLink == WhenToShowEnum.Always
                || (PagerOptions.ShowFirstButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex > 1))
            {
                tagBuilders.Add(CreateFirstTag());
            }

            if (PagerOptions.ShowPreviousButtonLink == WhenToShowEnum.Always 
                || (PagerOptions.ShowPreviousButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex > 1))
            {
                tagBuilders.Add(CreatePreviousTag());
            }

            if (PagerOptions.ShowPagerButtonLinks == WhenToShowEnum.Always
                || PagerOptions.ShowPagerButtonLinks == WhenToShowEnum.IfNecessary)
            {
                tagBuilders.AddRange(PageNumbersCreator());
            }

            if (PagerOptions.ShowPreviousButtonLink == WhenToShowEnum.Always
                || (PagerOptions.ShowPreviousButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex < _page.TotalPageCount))
            {
                tagBuilders.Add(CreateNextTag());
            }

            if (PagerOptions.ShowLastButtonLink == WhenToShowEnum.Always
                || (PagerOptions.ShowLastButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex < _page.TotalPageCount))
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

            var isActive = _page.PageIndex == index && PagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && PagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(PagerOptions.FirstPageLinkLabel,
                    icon: PagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(PagerOptions.FirstPageLinkLabel, index,
                    icon: PagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                    dataIndexName: PagerOptions.PageIndexDataName)
            );

            return li;
        }

        public virtual TagBuilder CreatePreviousTag()
        {
            var index = _page.PageIndex - 1;

            index = index < 1 ? 1 : index;

            var isActive = _page.PageIndex == index && PagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && PagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(PagerOptions.PreviousPageLinkLabel,
                    icon: PagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(PagerOptions.PreviousPageLinkLabel, index,
                    icon: PagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                    dataIndexName: PagerOptions.PageIndexDataName)
            );

            return li;
        }

        public virtual TagBuilder CreateNextTag()
        {
            var index = _page.PageIndex + 1;

            index = index > _page.TotalPageCount ? _page.TotalPageCount : index;

            var isActive = _page.PageIndex == index && PagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && PagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(PagerOptions.NextPageLinkLabel,
                    icon: PagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(PagerOptions.NextPageLinkLabel, index,
                    icon: PagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                    dataIndexName: PagerOptions.PageIndexDataName)
            );

            return li;
        }

        public virtual TagBuilder CreateLastTag()
        {
            var index = _page.TotalPageCount;

            var isActive = _page.PageIndex == index && PagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && PagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(PagerOptions.LastPageLinkLabel,
                    icon: PagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(PagerOptions.LastPageLinkLabel, index,
                    icon: PagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
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
            var isActive = PagerOptions.DisplayActive && _page.PageIndex == index;

            var text = PagerOptions.ShowIndexAsRange
                ? $"{((index - 1) * _page.PageSize) + 1} - {((index - 1) * _page.PageSize) + _page.PageSize}"
                : index.ToString();

            var li = TagCreatorLi(
                _page.PageIndex == index && PagerOptions.AllowDisabledPageLink,
                isActive: isActive
                );

            var link = isActive
                ? TagCreatorSpan(text,
                        disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
                        isActive: isActive)
                : TagCreatorLink(text, index,
                        disableTabing: _page.PageIndex == index && PagerOptions.AllowDisabledPageLinkTab,
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

            if (!string.IsNullOrWhiteSpace(dataIndexName) && !(pageIndex > _page.TotalPageCount || pageIndex < 1))
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
                        _page.PageIndex - PagerOptions.SiblingCount,
                        // Page boundery +/- elipse
                        _page.TotalPageCount - PagerOptions.BoundaryCount - PagerOptions.SiblingCount * 2 - elipseModifier
                    ),
                    // After start +/- elipse
                    PagerOptions.BoundaryCount + 1 + elipseModifier
                );

            var endNumber = 
                Math.Min(
                    Math.Max(
                        // Original end
                        _page.PageIndex + PagerOptions.SiblingCount,
                        // Page boundery +/- elipse
                        PagerOptions.BoundaryCount + PagerOptions.SiblingCount * 2 + 1 + elipseModifier

                    ),
                    // Before end +/- elipse
                    _page.TotalPageCount - PagerOptions.BoundaryCount - elipseModifier
                );

            var firstIndex = Math.Min(PagerOptions.BoundaryCount, _page.TotalPageCount) - 1 + 1;
            var lastIndex = _page.TotalPageCount - Math.Max(_page.TotalPageCount - PagerOptions.BoundaryCount + 1, PagerOptions.BoundaryCount + 1) + 1;

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
                else if (PagerOptions.BoundaryCount + 1 < _page.TotalPageCount - PagerOptions.BoundaryCount)
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
                if (endNumber < _page.TotalPageCount - PagerOptions.BoundaryCount - 1)
                {
                    var li = TagCreatorLi(true);
                    li.InnerHtmlAppend(TagCreatorSpan("...", disableTabing: true));
                    tagBuilderList.Add(li);
                }
                else if (_page.TotalPageCount - PagerOptions.BoundaryCount > PagerOptions.BoundaryCount)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(_page.TotalPageCount - PagerOptions.BoundaryCount));
                }
            }

            if (lastIndex <= _page.TotalPageCount)
            {
                for (int index = _page.TotalPageCount - PagerOptions.BoundaryCount + 1; index <= _page.TotalPageCount; index++)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(index));
                }
            }

            return tagBuilderList;
        }

        /// <summary>
        /// Generate Url link for <see cref="pageIndex">
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public virtual string? UrlCreator(int pageIndex)
        {
            if (_page.TotalPageCount < pageIndex || pageIndex < 1)
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
}
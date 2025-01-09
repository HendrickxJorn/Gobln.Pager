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
    public class PagerFactory(IPagerOptions _pagerOptions)
        : IPagerFactory
    {
        internal IPage _page { get; set; }

        public virtual void Validate()
        {
#if NETFRAMEWORK
            if (_pagerOptions is null)
            {
                throw new ArgumentNullException(nameof(_pagerOptions));
            }
#else
            ArgumentNullException.ThrowIfNull(_pagerOptions);
#endif
        }

#if NETFRAMEWORK
        public virtual MvcHtmlString Render(IPage page)
#else
        public virtual IHtmlContent Render(IPage page)
#endif
        {
            if (_pagerOptions.ShowPager != WhenToShowEnum.Always
                && (page == null
                    || !(page.HasPaging && _pagerOptions.ShowPager == WhenToShowEnum.IfNecessary)))
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

            if( _pagerOptions.ShowFirstButtonLink == WhenToShowEnum.Always
                || (_pagerOptions.ShowFirstButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex > 1))
            {
                tagBuilders.Add(CreateFirstTag());
            }

            if (_pagerOptions.ShowPreviousButtonLink == WhenToShowEnum.Always 
                || (_pagerOptions.ShowPreviousButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex > 1))
            {
                tagBuilders.Add(CreatePreviousTag());
            }

            if (_pagerOptions.ShowPagerButtonLinks == WhenToShowEnum.Always
                || _pagerOptions.ShowPagerButtonLinks == WhenToShowEnum.IfNecessary)
            {
                tagBuilders.AddRange(PageNumbersCreator());
            }

            if (_pagerOptions.ShowPreviousButtonLink == WhenToShowEnum.Always
                || (_pagerOptions.ShowPreviousButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex < _page.TotalPageCount))
            {
                tagBuilders.Add(CreateNextTag());
            }

            if (_pagerOptions.ShowLastButtonLink == WhenToShowEnum.Always
                || (_pagerOptions.ShowLastButtonLink == WhenToShowEnum.IfNecessary && _page.PageIndex < _page.TotalPageCount))
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

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(_pagerOptions.FirstPageLinkLabel, index,
                    icon: _pagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(_pagerOptions.FirstPageLinkLabel, index,
                    icon: _pagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return li;
        }

        public virtual TagBuilder CreatePreviousTag()
        {
            var index = _page.PageIndex - 1;

            index = index < 1 ? 1 : index;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(_pagerOptions.PreviousPageLinkLabel, index,
                    icon: _pagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(_pagerOptions.PreviousPageLinkLabel, index,
                    icon: _pagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return li;
        }

        public virtual TagBuilder CreateNextTag()
        {
            var index = _page.PageIndex + 1;

            index = index > _page.TotalPageCount ? _page.TotalPageCount : index;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(_pagerOptions.NextPageLinkLabel, index,
                    icon: _pagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(_pagerOptions.NextPageLinkLabel, index,
                    icon: _pagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return li;
        }

        public virtual TagBuilder CreateLastTag()
        {
            var index = _page.TotalPageCount;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var li = TagCreatorLi(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            li.InnerHtmlAppend(
                isActive
                ? TagCreatorSpan(_pagerOptions.LastPageLinkLabel, index,
                    icon: _pagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(_pagerOptions.LastPageLinkLabel, index,
                    icon: _pagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return li;
        }

        public virtual TagBuilder CreateListtTag()
        {

            return new TagBuilder("");
        }

        #region Generate

        private TagBuilder PrivateTagCreator(string name, object? attibutes = null, string[]? classes = null)
        {
            var tagBuilder = new TagBuilder(name);

            if (attibutes is not null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attibutes));
            }

            if(classes is not null)
            {
                tagBuilder.AddCssClass(string.Join(" ", classes.Where(c => !string.IsNullOrWhiteSpace(c))));
            }

            return tagBuilder;
        }

        private TagBuilder PrivatePageNumberCreator(int index, bool numberRange)
        {
            var isActive = _pagerOptions.DisplayActive && _page.PageIndex == index;

            var text = _pagerOptions.ShowIndexAsRange
                ? $"{((index - 1) * _page.PageSize) + 1} - {((index - 1) * _page.PageSize) + _page.PageSize}"
                : index.ToString();

            var li = TagCreatorLi(
                _page.PageIndex == index && _pagerOptions.AllowDisabledPageLink,
                isActive: isActive,
                attributes: isActive
                        ? new Dictionary<string, string?> { { "aria-current", "page" } }
                        : null);

            var link = isActive
                ? TagCreatorSpan(text, index,
                        disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : TagCreatorLink(text, index,
                        disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab);

            li.InnerHtmlAppend(link);

            return li;
        }

        public virtual TagBuilder TagCreatorNav()
        {
            return PrivateTagCreator("nav", attibutes: _pagerOptions.NavTagHtmlAttributes);
        }

        public virtual TagBuilder TagCreatorUl()
        {
            return PrivateTagCreator("ul", attibutes: _pagerOptions.UlTagHtmlAttributes, classes: ["pagination"]);
        }

        //todo isActive omdraaien van bedoeling
        //todo change isDisabled and isActive to dictionary<>  ?????
        public virtual TagBuilder TagCreatorLi(bool isDisabled = false, bool isActive = false, Dictionary<string, string?>? attributes = null)
        {
            var tagBuilder = PrivateTagCreator("li");

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

            tagBuilder.AddCssClass("page-item");


            return tagBuilder;
        }

        public virtual TagBuilder TagCreatorLink(string label, int pageIndex, string? icon = null, bool disableTabing = false)
        {
            var tagBuilder = PrivateTagCreator("a");

            tagBuilder.AddCssClass("page-link");

            //todo: make optional
            if (_pagerOptions.DataIndexName is not null && !(pageIndex > _page.TotalPageCount || pageIndex < 1))
            {
                tagBuilder.Attributes.Add($"data-{_pagerOptions.DataIndexName}", pageIndex.ToString());
            }

            if (disableTabing)
            {
                tagBuilder.Attributes.Add("tabindex", "-1");
            }

            var url = "#";

            if (_pagerOptions.HRefUrl is not "#")
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

        public virtual TagBuilder TagCreatorSpan(string label, int pageIndex, string? icon = null, bool disableTabing = false)
        {
            var tagBuilder = new TagBuilder("span");

            tagBuilder.AddCssClass("page-link");

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

        public virtual TagBuilder TagCreatorSpanIcon(string label)
        {
            var tagBuilder = new TagBuilder("span");

            tagBuilder.Attributes.Add("aria-hidden", "true");

            tagBuilder.InnerHtmlAppend(label);

            return tagBuilder;
        }

        public virtual IEnumerable<TagBuilder> PageNumbersCreator(bool numberRange = false)
        {
            var tagBuilderList = new List<TagBuilder>();
         
            var elipseModifier = _pagerOptions.ShowEllipsis ? 1 : 0;

            var startNumber =
                Math.Max(
                    Math.Min(
                        // Original start
                        _page.PageIndex - _pagerOptions.SiblingCount,
                        // Page boundery +/- elipse
                        _page.TotalPageCount - _pagerOptions.BoundaryCount - _pagerOptions.SiblingCount * 2 - elipseModifier
                    ),
                    // After start +/- elipse
                    _pagerOptions.BoundaryCount + 1 + elipseModifier
                );

            var endNumber = 
                Math.Min(
                    Math.Max(
                        // Original end
                        _page.PageIndex + _pagerOptions.SiblingCount,
                        // Page boundery +/- elipse
                        _pagerOptions.BoundaryCount + _pagerOptions.SiblingCount * 2 + 1 + elipseModifier

                    ),
                    // Before end +/- elipse
                    _page.TotalPageCount - _pagerOptions.BoundaryCount - elipseModifier
                );

            var firstIndex = Math.Min(_pagerOptions.BoundaryCount, _page.TotalPageCount) - 1 + 1;
            var lastIndex = _page.TotalPageCount - Math.Max(_page.TotalPageCount - _pagerOptions.BoundaryCount + 1, _pagerOptions.BoundaryCount + 1) + 1;

            if (firstIndex > 0)
            {
                for (int index = 1; index <= firstIndex; index++)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(index, numberRange));
                }
            }

            if (_pagerOptions.ShowEllipsis)
            {
                if (startNumber > _pagerOptions.BoundaryCount + 2)
                {
                    var li = TagCreatorLi(true);
                    li.InnerHtmlAppend(TagCreatorSpan("...", 0, disableTabing: true));
                    tagBuilderList.Add(li);
                }
                else if (_pagerOptions.BoundaryCount + 1 < _page.TotalPageCount - _pagerOptions.BoundaryCount)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(_pagerOptions.BoundaryCount + 1, numberRange));
                }
            }

            for (int index = startNumber; index <= endNumber; index++)
            {
                tagBuilderList.Add(PrivatePageNumberCreator(index, numberRange));
            }

            if (_pagerOptions.ShowEllipsis)
            {
                if (endNumber < _page.TotalPageCount - _pagerOptions.BoundaryCount - 1)
                {
                    var li = TagCreatorLi(true);
                    li.InnerHtmlAppend(TagCreatorSpan("...", 0, disableTabing: true));
                    tagBuilderList.Add(li);
                }
                else if (_page.TotalPageCount - _pagerOptions.BoundaryCount > _pagerOptions.BoundaryCount)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(_page.TotalPageCount - _pagerOptions.BoundaryCount, numberRange));
                }
            }

            if (lastIndex <= _page.TotalPageCount)
            {
                for (int index = _page.TotalPageCount - _pagerOptions.BoundaryCount + 1; index <= _page.TotalPageCount; index++)
                {
                    tagBuilderList.Add(PrivatePageNumberCreator(index, numberRange));
                }
            }

            return tagBuilderList;
        }

        /// <summary>
        /// Generat Url link for pageindex
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private string? UrlCreator(int pageIndex)
        {
            //todo: return null if  page index larger than total page count or smaller then 1
            if (pageIndex > _page.TotalPageCount || pageIndex < 1)
            {
                return null;
            }

            //todo maybe overkill the #
            return Gobln.Pager.Mvc.Infrastructure.UrlHelper.AddQueryValuesToUrlString(_pagerOptions.HRefUrl ?? "#", new Dictionary<string, string> { { _pagerOptions.PageIndexName, pageIndex.ToString() } });
        }

#endregion Generate

    }
}
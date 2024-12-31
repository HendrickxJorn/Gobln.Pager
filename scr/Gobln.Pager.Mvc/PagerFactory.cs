using Gobln.Pager.Mvc.Infrastructure;
using Gobln.Pager.Mvc.Contracts;
using System.Collections.Generic;
using System.Reflection;


#if NETFRAMEWORK

using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

#else

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

#endif

// TODO Rename????
// TODO make functions virtual
namespace Gobln.Pager.Mvc
{
    //public class PagerFactory(IPage _page, IPagerOptions _pagerOptions)
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

            if (_pagerOptions.ShowItemOrder is null)
            {
                throw new ArgumentNullException(nameof(_pagerOptions.ShowItemOrder));
            }

#else

            ArgumentNullException.ThrowIfNull(_pagerOptions);
            ArgumentNullException.ThrowIfNull(_pagerOptions.ShowItemOrder);

#endif

        }

#if NETFRAMEWORK
        public virtual MvcHtmlString Render(IPage page)
#else
        public virtual IHtmlContent Render(IPage page)
#endif
        {
            _page = page;

            if (_page == null || (!_page.HasPaging && !_pagerOptions.AlwaysShowPager))
            {
#if NETFRAMEWORK
                return MvcHtmlString.Create(string.Empty);
#else
                return new HtmlContentBuilder();
#endif
            }

#if NETFRAMEWORK
            return MvcHtmlString.Create(RenderPager().ToString(TagRenderMode.Normal));
#else
            return RenderPager();
#endif
        }

        //todo put every thing in the tag creator, => ul containt everyting of ul and lower, nav => contain everything of nav and lower
        //          Then methode Render() only need to call nav and return it.
        private TagBuilder RenderPager()
        {
            var ul = UlTagCreator();

            foreach (var item in _pagerOptions.ShowItemOrder)
            {
                switch (item)
                {
                    case PageLinkItem.FirstPageLink:
                        //todo VisableItemsPerSide
                        //if (_pagerOptions.FirstPageLinkAlwaysShow || _page.PageIndex - _pagerOptions.VisableItemsPerSide > 1)
                        //{
                        ul.InnerHtmlAppend(CreateFirstTag());
                        //}
                        break;

                    case PageLinkItem.PreviousPageLink:
                        if (_pagerOptions.PreviousPageLinkAlwaysShow || _page.PageIndex > 1)
                        {
                            ul.InnerHtmlAppend(CreatePreviousTag());
                        }
                        break;

                            case PageLinkItem.EllipsisPreviousPageLink:
                    //            if (_pagerOptions.AlwaysShowJumpPageItem || _page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide > 2)
                    //            {
                    //                var previous = ((_page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide) / 2) + 1;

                    //                previous = previous < 1 ? 1 : previous;

                    //                var il = GenerateIl(_page.CurrentPageIndex == previous);

                    //                il.InnerHtmlAppendHtml(GenerateLinkTag(_pagerOptions.LabelJumpPageItem, previous, string.Empty, disableTabing: _page.CurrentPageIndex == previous));

                    //                ul.InnerHtmlAppendHtml(il);
                    //            }
                                break;

                    case PageLinkItem.PageLinks:
                        foreach (var pNumber in PageNumbersCreator())
                        {
                            ul.InnerHtmlAppend(pNumber);
                        }
                        break;

                    //        case ItemShow.PagesItemsRange:
                    //            foreach (var pNumber in GeneratePageNumbers(true))
                    //            {
                    //                ul.InnerHtmlAppendHtml(pNumber);
                    //            }
                    //            break;

                          case PageLinkItem.EllipsisNextPageLink:
                    //            if (_pagerOptions.AlwaysShowJumpPageItem || _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide < _page.TotalPageCount - 1)
                    //            {
                    //                var next = _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide;

                    //                next = ((_page.TotalPageCount - next) / 2) + next;

                    //                next = next > _page.TotalPageCount ? _page.TotalPageCount : next;

                    //                var il = GenerateIl(next == _page.CurrentPageIndex);

                    //                il.InnerHtmlAppendHtml(GenerateLinkTag(_pagerOptions.LabelJumpPageItem, next, string.Empty, disableTabing: _page.CurrentPageIndex == next));

                    //                ul.InnerHtmlAppendHtml(il);
                    //            }
                                break;

                    case PageLinkItem.NextPageLink:
                        if (_pagerOptions.NextPageLinkAlwaysShow || _page.PageIndex < _page.TotalPageCount)
                        {
                            ul.InnerHtmlAppend(CreateNextTag());
                        }
                        break;

                    case PageLinkItem.LastPageLink:
                        //todo VisableItemsPerSide
                        //if (_pagerOptions.LastPageLinkAlwaysShow || _page.PageIndex + _pagerOptions.VisableItemsPerSide < _page.TotalPageCount)
                        //{
                        ul.InnerHtmlAppend(CreateLastTag());
                        //}
                        break;

                    default:
                        break;
                }
            }

            var nav = NavTagCreator();

            nav.InnerHtmlAppend(ul);

            return nav;
        }

        public virtual TagBuilder CreateFirstTag()
        {
            var index = 1;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var Li = LiTagCreator(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            Li.InnerHtmlAppend(
                isActive
                ? SpanTagCreator(_pagerOptions.FirstPageLinkLabel, index,
                    icon: _pagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : LinkTagCreator(_pagerOptions.FirstPageLinkLabel, index,
                    icon: _pagerOptions.FirstPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return Li;
        }

        public virtual TagBuilder CreatePreviousTag()
        {
            var index = _page.PageIndex - 1;

            index = index < 1 ? 1 : index;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var Li = LiTagCreator(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            Li.InnerHtmlAppend(
                isActive
                ? SpanTagCreator(_pagerOptions.PreviousPageLinkLabel, index,
                    icon: _pagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : LinkTagCreator(_pagerOptions.PreviousPageLinkLabel, index,
                    icon: _pagerOptions.PreviousPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return Li;
        }

        public virtual TagBuilder CreateNextTag()
        {
            var index = _page.PageIndex + 1;

            index = index > _page.TotalPageCount ? _page.TotalPageCount : index;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var Li = LiTagCreator(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            Li.InnerHtmlAppend(
                isActive
                ? SpanTagCreator(_pagerOptions.NextPageLinkLabel, index,
                    icon: _pagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : LinkTagCreator(_pagerOptions.NextPageLinkLabel, index,
                    icon: _pagerOptions.NextPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return Li;
        }

        public virtual TagBuilder CreateLastTag()
        {
            var index = _page.TotalPageCount;

            var isActive = _page.PageIndex == index && _pagerOptions.DisplayActive;

            var Li = LiTagCreator(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink);

            Li.InnerHtmlAppend(
                isActive
                ? SpanTagCreator(_pagerOptions.LastPageLinkLabel, index,
                    icon: _pagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                : LinkTagCreator(_pagerOptions.LastPageLinkLabel, index,
                    icon: _pagerOptions.LastPageLinkIconLabel,
                    disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
            );

            return Li;
        }

        public virtual TagBuilder CreateListtTag()
        {

            return new TagBuilder("");
        }

        #region Generate

        private TagBuilder PrivateTagBuilder(string name, object? attibutes = null)
        {
            var tagBuilder = new TagBuilder(name);

            if (attibutes is not null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attibutes));
            }

            return tagBuilder;
        }

        public virtual TagBuilder NavTagCreator()
        {
            return PrivateTagBuilder("nav", _pagerOptions.NavTagHtmlAttributes);
        }

        public virtual TagBuilder UlTagCreator()
        {
            var tagBuilder = PrivateTagBuilder("ul", _pagerOptions.UlTagHtmlAttributes);

            tagBuilder.AddCssClass("pagination");

            return tagBuilder;
        }

        //todo isActive omdraaien van bedoeling
        //todo change isDisabled and isActive to dictionary<>  ?????
        public virtual TagBuilder LiTagCreator(bool isDisabled, bool isActive = false, Dictionary<string, string?>? attributes = null)
        {
            var tagBuilder = PrivateTagBuilder("li");

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

        public virtual TagBuilder LinkTagCreator(string label, int pageIndex, string? icon = null, bool disableTabing = false)
        {
            var tagBuilder = PrivateTagBuilder("a");

            tagBuilder.AddCssClass("page-link");

            //todo make optional
            //tagBuilder.Attributes.Add($"data-{_pagerOptions.DataIndexName}", pageIndex.ToString());

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

            if (string.IsNullOrWhiteSpace(icon))
            {
                tagBuilder.InnerHtmlAppend(label);
            }
            else
            {
                tagBuilder.InnerHtmlAppend(SpanIconTagCreator(icon));
                tagBuilder.MergeAttribute("aria-label", label);
            }

            return tagBuilder;
        }

        public virtual TagBuilder SpanTagCreator(string label, int pageIndex, string? icon = null, bool isDisabled = false, bool disableTabing = false)
        {
            var tagBuilder = new TagBuilder("span");

            tagBuilder.AddCssClass("page-link");

            if (disableTabing)
            {
                tagBuilder.Attributes.Add("tabindex", "-1");
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                tagBuilder.InnerHtmlAppend(label);
            }
            else
            {
                tagBuilder.InnerHtmlAppend(SpanIconTagCreator(icon));
                tagBuilder.MergeAttribute("aria-label", label);
            }

            return tagBuilder;
        }

        public virtual TagBuilder SpanTagCreator(string label, string classNames, bool isAreaHidden = false)
        {
            var tagBuilder = new TagBuilder("span");

            if (!string.IsNullOrWhiteSpace(classNames))
            {
                tagBuilder.AddCssClass(classNames);
            }

            if (isAreaHidden)
            {
                tagBuilder.Attributes.Add("aria-hidden", "true");
            }

            tagBuilder.InnerHtmlAppend(label);

            return tagBuilder;
        }

        public virtual TagBuilder SpanIconTagCreator(string label)
        {
            var tagBuilder = new TagBuilder("span");

            tagBuilder.Attributes.Add("aria-hidden", "true");

            tagBuilder.InnerHtmlAppend(label);

            return tagBuilder;
        }

        public virtual IEnumerable<TagBuilder> PageNumbersCreator(bool numberRange = false)
        {
            //todo VisableItemsPerSide
            //var startIndex = _page.PageIndex - _pagerOptions.VisableItemsPerSide;
            //var endIndex = _page.PageIndex + _pagerOptions.VisableItemsPerSide;

            var startIndex = _page.PageIndex;
            var endIndex = _page.PageIndex;

            if (startIndex < 1)
            {
                startIndex = 1;
            }

            //todo
            //if (endIndex > _page.TotalPageCount)
            //{
            endIndex = _page.TotalPageCount;
            //}

            for (int index = startIndex; index <= endIndex; index++)
            {
                var isActive = _pagerOptions.DisplayActive && _page.PageIndex == index;

                var text = numberRange
                    ? $"{((index - 1) * _page.PageSize) + 1} - {((index - 1) * _page.PageSize) + _page.PageSize}"
                    : index.ToString();

                Dictionary<string, string?> attibutes = [];

                if (isActive)
                {
                    attibutes.Add("aria-current", "page");
                }

                var il = LiTagCreator(_page.PageIndex == index && _pagerOptions.AllowDisabledPageLink, isActive, attibutes);

                var link = isActive
                    ? SpanTagCreator(text, index,
                            disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab)
                    : LinkTagCreator(text, index,
                            disableTabing: _page.PageIndex == index && _pagerOptions.AllowDisabledPageLinkTab);

                il.InnerHtmlAppend(link);

                yield return il;
            }
        }

        /// <summary>
        /// Generat Url link for pageindex
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private string? UrlCreator(int pageIndex)
        {
            //return null if  page index larger than total page count or smaller then 1
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
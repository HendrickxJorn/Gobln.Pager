//#if !NETFRAMEWORK

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Gobln.Pager.Mvc.Infrastructure;
//using Gobln.Pager.Mvc.Interface;

//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace Gobln.Pager.Mvc
//{
//    internal class PagerFactoryCore
//    {
//        internal IPage _page { get; private set; }
//        internal IPagerOptions _pagerOptions { get; private set; }

//        internal PagerFactoryCore(IPage page, IPagerOptions pagerOptions)
//        {
//            _pagerOptions = pagerOptions ?? new PagerOptions();
//            _page = page ?? new EmptyPage();
//        }

//        public IHtmlContent Render()
//        {
//            return RenderPager();
//        }

//        #region Private

//        private IHtmlContent RenderPager()
//        {
//            var ul = GenerateUl();

//            foreach (var item in _pagerOptions.ItemShowOrder)
//            {
//                switch (item)
//                {
//                    case ItemShow.FirstItem:
//                        if (_pagerOptions.AlwaysShowFirstPageItem || _page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide > 1)
//                        {
//                            var il = GenerateIl(_page.CurrentPageIndex == 1);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelFirstPageItem, "&laquo;&laquo;", 1, disableTabing: _page.CurrentPageIndex == 1));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    case ItemShow.PreviousItem:
//                        if (_pagerOptions.AlwaysShowPreviousPageItem || _page.CurrentPageIndex > 1)
//                        {
//                            var previous = _page.CurrentPageIndex - 1;

//                            previous = previous < 1 ? 1 : previous;

//                            var il = GenerateIl(_page.CurrentPageIndex == previous);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelPreviousPageItem, "&laquo;", previous, disableTabing: _page.CurrentPageIndex == previous));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    case ItemShow.JumpPreviousItem:
//                        if (_pagerOptions.AlwaysShowJumpPageItem || _page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide > 2)
//                        {
//                            var previous = ((_page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide) / 2) + 1;

//                            previous = previous < 1 ? 1 : previous;

//                            var il = GenerateIl(_page.CurrentPageIndex == previous);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelJumpPageItem, string.Empty, previous, disableTabing: _page.CurrentPageIndex == previous));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    case ItemShow.PagesItems:
//                        foreach (var pNumber in GeneratePageNumbers())
//                        {
//                            ul.InnerHtml.AppendHtml(pNumber);
//                        }
//                        break;

//                    case ItemShow.PagesItemsRange:
//                        foreach (var pNumber in GeneratePageNumbers(true))
//                        {
//                            ul.InnerHtml.AppendHtml(pNumber);
//                        }
//                        break;

//                    case ItemShow.JumpNextItem:
//                        if (_pagerOptions.AlwaysShowJumpPageItem || _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide < _page.TotalPageCount - 1)
//                        {
//                            var next = _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide;

//                            next = ((_page.TotalPageCount - next) / 2) + next;

//                            next = next > _page.TotalPageCount ? _page.TotalPageCount : next;

//                            var il = GenerateIl(next == _page.CurrentPageIndex);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelJumpPageItem, string.Empty, next, disableTabing: _page.CurrentPageIndex == next));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    case ItemShow.NextItem:
//                        if (_pagerOptions.AlwaysShowNextPageItem || _page.CurrentPageIndex < _page.TotalPageCount)
//                        {
//                            var next = _page.CurrentPageIndex + 1;

//                            next = next > _page.TotalPageCount ? _page.TotalPageCount : next;

//                            var il = GenerateIl(next == _page.CurrentPageIndex);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelNextPageItem, "&raquo;", next, disableTabing: _page.CurrentPageIndex == next));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    case ItemShow.LastItem:
//                        if (_pagerOptions.AlwaysShowLastPageItem || _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide < _page.TotalPageCount)
//                        {
//                            var il = GenerateIl(_page.CurrentPageIndex == _page.TotalPageCount);

//                            il.InnerHtml.AppendHtml(GenerateLinkTag(_pagerOptions.LabelLastPageItem, "&raquo;&raquo;", _page.TotalPageCount, disableTabing: _page.CurrentPageIndex == _page.TotalPageCount));

//                            ul.InnerHtml.AppendHtml(il);
//                        }
//                        break;

//                    default:
//                        break;
//                }
//            }

//            var nav = GenerateNav();

//            nav.InnerHtml.AppendHtml(ul);

//            return nav;
//        }

//        #region Generate

//        private TagBuilder GenerateNav()
//        {
//            var tagBuilder = new TagBuilder("nav");

//            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(_pagerOptions.NavTagHtmlAttributes));

//            return tagBuilder;
//        }

//        private TagBuilder GenerateUl()
//        {
//            var tagBuilder = new TagBuilder("ul");

//            tagBuilder.AddCssClass("pagination");

//            switch (_pagerOptions.PagerSize)
//            {
//                case PagerSizeEnum.Large:
//                    tagBuilder.AddCssClass("pagination-lg");
//                    break;

//                case PagerSizeEnum.Small:
//                    tagBuilder.AddCssClass("pagination-sm");
//                    break;
//            }

//            switch (_pagerOptions.PagerAlignment)
//            {
//                case PagerAlignmentEnum.Center:
//                    tagBuilder.AddCssClass("justify-content-center");
//                    break;

//                case PagerAlignmentEnum.Right:
//                    tagBuilder.AddCssClass("justify-content-end");
//                    break;
//            }

//            return tagBuilder;
//        }

//        private TagBuilder GenerateIl(bool isDisabled, bool isActive = false)
//        {
//            var tagBuilder = new TagBuilder("li");

//            tagBuilder.AddCssClass("page-item");

//            if (isDisabled && !isActive)
//                tagBuilder.AddCssClass("disabled");

//            if (isActive)
//                tagBuilder.AddCssClass("active");

//            return tagBuilder;
//        }

//        private TagBuilder GenerateLinkTag(string label, string icon, int pageIndex, bool isDisabled = false, bool disableTabing = false)
//        {
//            var tagBuilder = new TagBuilder("a");

//            tagBuilder.AddCssClass("page-link");

//            tagBuilder.Attributes.Add($"data-{_pagerOptions.DataIndexName}", pageIndex.ToString());

//            if (disableTabing)
//                tagBuilder.Attributes.Add("tabindex", "-1");

//            if (isDisabled)
//            {
//                tagBuilder.Attributes.Add("disabled", "disabled");
//            }
//            else
//            {
//                var url = "#";

//                if (!_pagerOptions.UrlDisable)
//                {
//                    var tempUrl = GenerateUrl(pageIndex);

//                    if (!string.IsNullOrWhiteSpace(tempUrl))
//                    {
//                        url = tempUrl;
//                    }
//                }

//                tagBuilder.Attributes.Add("href", url);
//            }

//            if (_pagerOptions.UseIcons && !string.IsNullOrWhiteSpace(icon))
//            {
//                tagBuilder.InnerHtml.AppendHtml(GenerateSpanIconTag(icon));
//                tagBuilder.InnerHtml.AppendHtml(GenerateSpanTag(label, "sr-only"));
//                tagBuilder.MergeAttribute("aria-label", label);
//            }
//            else
//            {
//                tagBuilder.InnerHtml.Append(label);
//            }

//            return tagBuilder;
//        }

//        private TagBuilder GenerateSpanTag(string label, string icon, int pageIndex, bool isDisabled = false, bool disableTabing = false)
//        {
//            var tagBuilder = new TagBuilder("span");

//            tagBuilder.AddCssClass("page-link");

//            if (disableTabing)
//                tagBuilder.Attributes.Add("tabindex", "-1");

//            if (_pagerOptions.UseIcons && !string.IsNullOrWhiteSpace(icon))
//            {
//                tagBuilder.InnerHtml.AppendHtml(GenerateSpanIconTag(icon));
//                tagBuilder.InnerHtml.AppendHtml(GenerateSpanTag(label, "sr-only"));
//                tagBuilder.MergeAttribute("aria-label", label);
//            }
//            else
//            {
//                tagBuilder.InnerHtml.Append(label);
//            }

//            return tagBuilder;
//        }

//        private static TagBuilder GenerateSpanTag(string label, string classNames, bool isAreaHidden = false)
//        {
//            var tagBuilder = new TagBuilder("span");

//            if (!string.IsNullOrWhiteSpace(classNames))
//                tagBuilder.AddCssClass(classNames);

//            if (isAreaHidden)
//                tagBuilder.Attributes.Add("aria-hidden", "true");

//            tagBuilder.InnerHtml.Append(label);

//            return tagBuilder;
//        }

//        private static TagBuilder GenerateSpanIconTag(string label)
//        {
//            var tagBuilder = new TagBuilder("span");

//            tagBuilder.Attributes.Add("aria-hidden", "true");

//            tagBuilder.InnerHtml.AppendHtml(label);

//            return tagBuilder;
//        }

//        private IEnumerable<TagBuilder> GeneratePageNumbers(bool numberRange = false)
//        {
//            var startIndex = _page.CurrentPageIndex - _pagerOptions.VisableItemsPerSide;
//            var endIndex = _page.CurrentPageIndex + _pagerOptions.VisableItemsPerSide;

//            if (startIndex < 1)
//                startIndex = 1;

//            if (endIndex > _page.TotalPageCount)
//                endIndex = _page.TotalPageCount;

//            for (int index = startIndex; index <= endIndex; index++)
//            {
//                var text = numberRange
//                    ? $"{((index - 1) * _page.PageSize) + 1} - {((index - 1) * _page.PageSize) + _page.PageSize}"
//                    : index.ToString();

//                var il = GenerateIl(_page.CurrentPageIndex == index, _pagerOptions.ActiveDisplay && _page.CurrentPageIndex == index);

//                var link = _pagerOptions.ActiveDisplayAsLink && _page.CurrentPageIndex == index
//                    ? GenerateSpanTag(text, string.Empty, index)
//                    : GenerateLinkTag(text, string.Empty, index);

//                if (_pagerOptions.ActiveDisplay && _page.CurrentPageIndex == index)
//                    link.InnerHtml.AppendHtml(GenerateSpanTag("(current)", "sr-only"));

//                il.InnerHtml.AppendHtml(link);

//                yield return il;
//            }
//        }

//        /// <summary>
//        /// Generat Url link for pageindex
//        /// </summary>
//        /// <param name="pageIndex"></param>
//        /// <returns></returns>
//        private string GenerateUrl(int pageIndex)
//        {
//            //return null if  page index larger than total page count or smaller then 1
//            if (pageIndex > _page.TotalPageCount || pageIndex < 1)
//                return null;

//            return Helper.AddQueryValuesToUrlString(_pagerOptions.Url, new Dictionary<string, string> { { _pagerOptions.DataIndexName, pageIndex.ToString() } });
//        }

//        #endregion Generate

//        #endregion Private
//    }
//}
//#endif
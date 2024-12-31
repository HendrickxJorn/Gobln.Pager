using Gobln.Pager.Mvc.Contracts;

namespace Gobln.Pager.Mvc
{
    public class PagerOptions : IPagerOptions
    {
        public bool AlwaysShowPager { get; set; }

        /// <summary>
        /// Defines which page items will be displayed and in what order.
        /// </summary>
        /// 
        //todo change ItemShow.JumpPreviousItem and ItemShow.JumpNextItem to ItemShow.Diffider => will not be clickable, 
        public PageLinkItem[] ShowItemOrder { get; set; } = [PageLinkItem.FirstPageLink, PageLinkItem.PreviousPageLink, PageLinkItem.EllipsisPreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.EllipsisNextPageLink, PageLinkItem.NextPageLink, PageLinkItem.LastPageLink];



        public ShowPageLink FirstPageLinkShow = ShowPageLink.IfNecessary;

        public ShowPageLink PreviousPageLinkShow = ShowPageLink.IfNecessary;

        public ShowPageLink DividerPreviousItemPageLinkShow = ShowPageLink.IfNecessary;

        //????
        public ShowPageLink PagesItemsPageLinkShow = ShowPageLink.IfNecessary;

        public ShowPageLink DividerNextPageLinkShow = ShowPageLink.IfNecessary;

        public ShowPageLink NextPageLinkShow = ShowPageLink.IfNecessary;

        public ShowPageLink LastPageLinkShow = ShowPageLink.IfNecessary;


        // todo : show range of items instread of page index

        // todo :Add page items to show text as "Page {0} of {1}."[=> 0= pageindex, 1=> max page] or "Showing items {0} through {1} of {2}." [=> 0= pageindex, 1=> max item in page, 2=> maxitem]
        // todo :can do this whit a replace "Page {PageIndex} of {PagerSize}." => "Page 1 of 10."

        // forsee text for FirstPageLink
        // may be empty
        public string FirstPageLinkLabel { get; set; } = "<<";

        public string PreviousPageLinkLabel { get; set; } = "Previous";

        public string? PreviousPageLinkIconLabel { get; set; } = null;


        public bool PreviousPageLinkAlwaysShow { get; set; }

        public bool AllowDisabledPageLink { get; set; } = true;

        public bool AllowDisabledPageLinkTab { get; set; } = true;


        public string NextPageLinkLabel { get; set; } = "Next";

        public string? NextPageLinkIconLabel { get; set; } = null;

        public bool NextPageLinkAlwaysShow { get; set; }


        public bool FirstPageLinkAlwaysShow { get; set; }


        public string? FirstPageLinkIconLabel { get; set; } = null;

        public string? LastPageLinkIconLabel { get; set; } = null;



        public string LastPageLinkLabel { get; set; } = "Last";

        public bool LastPageLinkAlwaysShow { get; set; }

        /// <summary>
        /// The url to use for the page links
        /// If null, the url of the page is used
        /// If "#" then all links are "#"
        /// </summary>
        public string? HRefUrl { get; set; } = null;



        //todo ???  rename
        /// <summary>
        /// Nav Tag Attributes
        /// </summary>
        public object? NavTagHtmlAttributes { get; set; } = null;

        //todo ???  rename
        /// <summary>
        /// Ul Tag Attributes
        /// </summary>
        public object? UlTagHtmlAttributes { get; set; } = null;


        // todo Rename
        public bool DisplayActive { get; set; } = true;


        // Todo Remane
        public string PageIndexName { get; set; } = "pageIndex";

    }
}

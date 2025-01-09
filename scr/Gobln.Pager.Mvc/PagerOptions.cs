using Gobln.Pager.Mvc.Contracts;

namespace Gobln.Pager.Mvc
{
    public class PagerOptions : IPagerOptions
    {
        public PagerOptions()
        {
        }

        public bool AlwaysShowPager { get; set; }

        /// <summary>
        /// Defines which page items will be displayed and in what order.
        /// </summary>
        /// 
        //todo change ItemShow.JumpPreviousItem and ItemShow.JumpNextItem to ItemShow.Diffider => will not be clickable, 
        //todo Remove and use ShowFirstButton, ShowPreviousButton, ShowNextButton, ShowLastButton
        //      Elipse will be used in the Pagelinks in combo with siblingCount {default 1} and boundaryCount {default 1}
        //      see https://mui.com/material-ui/react-pagination/#pagination-ranges
        public PageLinkItem[] ShowItemOrder { get; set; } = [PageLinkItem.FirstPageLink, PageLinkItem.PreviousPageLink, PageLinkItem.PageLinks, PageLinkItem.NextPageLink, PageLinkItem.LastPageLink];

        /// <summary>
        /// The number of sibling pages to show on each side of the current page.
        /// </summary>
        public int SiblingCount { get; set; } = 1;

        /// <summary>
        /// The number of always visible pages at the beginning and end.
        /// </summary>
        public int BoundaryCount { get; set; } = 1;

        public bool ShowEllipsis { get; set; } = true;

        public ShowPageLink ShowFirstButtonLink { get; set; } = ShowPageLink.Always;

        public ShowPageLink ShowPreviousButtonLink { get; set; } = ShowPageLink.Always;

        public ShowPageLink ShowPagerButtonLinks { get; set; } = ShowPageLink.Always;

        public ShowPageLink ShowNextButtonLink { get; set; } = ShowPageLink.Always;

        public ShowPageLink ShowLastButtonLink { get; set; } = ShowPageLink.Always;


        // todo : show range of items instread of page index

        // todo :Add page items to show text as "Page {0} of {1}."[=> 0= pageindex, 1=> max page] or "Showing items {0} through {1} of {2}." [=> 0= pageindex, 1=> max item in page, 2=> maxitem]
        // todo :can do this whit a replace "Page {PageIndex} of {PagerSize}." => "Page 1 of 10."

        public string PreviousPageLinkLabel { get; set; } = "Previous";

        //rename to LabelPreviousButtonLinkIcon
        public string? PreviousPageLinkIconLabel { get; set; } = null;

        public bool AllowDisabledPageLink { get; set; } = true;

        public bool AllowDisabledPageLinkTab { get; set; } = true;


        public string NextPageLinkLabel { get; set; } = "Next";

        //todo ?? rename to LabelNextButtonLinkIcon
        public string? NextPageLinkIconLabel { get; set; } = null;

        public string FirstPageLinkLabel { get; set; } = "First";

        public string? FirstPageLinkIconLabel { get; set; } = null;

        public string? LastPageLinkIconLabel { get; set; } = null;



        public string LastPageLinkLabel { get; set; } = "Last";

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

        // Todo Remane
        public string? DataIndexName { get; set; } = null;

        public PagerOptions Clone()
        {
            return new PagerOptions(this);
        }

        protected PagerOptions(PagerOptions other)
        {
            this.ShowNextButtonLink = other.ShowNextButtonLink;
            this.ShowLastButtonLink = other.ShowLastButtonLink;
            this.ShowPagerButtonLinks = other.ShowPagerButtonLinks;
            this.ShowPreviousButtonLink = other.ShowPreviousButtonLink;
            this.ShowFirstButtonLink = other.ShowFirstButtonLink;
            this.ShowItemOrder = other.ShowItemOrder;
            this.AlwaysShowPager = other.AlwaysShowPager;
            this.NavTagHtmlAttributes = other.NavTagHtmlAttributes;
            this.UlTagHtmlAttributes = other.UlTagHtmlAttributes;
            this.FirstPageLinkLabel = other.FirstPageLinkLabel;
            this.PreviousPageLinkLabel = other.PreviousPageLinkLabel;
            this.AllowDisabledPageLink = other.AllowDisabledPageLink;
            this.AllowDisabledPageLinkTab = other.AllowDisabledPageLinkTab;
            this.NextPageLinkLabel = other.NextPageLinkLabel;
            this.DisplayActive = other.DisplayActive;
            this.PreviousPageLinkIconLabel = other.PreviousPageLinkIconLabel;
            this.NextPageLinkIconLabel = other.NextPageLinkIconLabel;
            this.FirstPageLinkIconLabel = other.FirstPageLinkIconLabel;
            this.LastPageLinkIconLabel = other.LastPageLinkIconLabel;
            this.LastPageLinkLabel = other.LastPageLinkLabel;
            this.PageIndexName = other.PageIndexName;
            this.DataIndexName = other.DataIndexName;
            this.HRefUrl = other.HRefUrl;
            this.SiblingCount = other.SiblingCount;
            this.BoundaryCount = other.BoundaryCount;
            this.ShowEllipsis = other.ShowEllipsis;
        }

    }
}

namespace Gobln.Pager.Mvc.Contracts
{
    public interface IPagerOptions
    {
        bool AlwaysShowPager { get; set; }

        /// <summary>
        /// Nav Tag Attributes
        /// </summary>
        object NavTagHtmlAttributes { get; set; }

        /// <summary>
        /// Ul Tag Attributes
        /// </summary>
        object UlTagHtmlAttributes { get; set; }

        PageLinkItem[] ShowItemOrder { get; set; }

        string FirstPageLinkLabel { get; set; }

        string PreviousPageLinkLabel { get; set; }

        bool AllowDisabledPageLink { get; set; }

        bool AllowDisabledPageLinkTab { get; set; }

        string NextPageLinkLabel { get; set; }

        bool DisplayActive { get; set; }

        string? PreviousPageLinkIconLabel { get; set; }

        string? NextPageLinkIconLabel { get; set; }

        string? FirstPageLinkIconLabel { get; set; }

        string? LastPageLinkIconLabel { get; set; }

        string LastPageLinkLabel { get; set; }

        string PageIndexName { get; set; }

        string? DataIndexName { get; set; }

        /// <summary>
        /// The url to use for the page links
        /// If null, the url of the page is used
        /// If "#" then all links are "#"
        /// </summary>
        string? HRefUrl { get; set; }


        ShowPageLink ShowFirstButtonLink { get; set; }

        ShowPageLink ShowPreviousButtonLink { get; set; }

        ShowPageLink ShowPagerButtonLinks { get; set; }

        ShowPageLink ShowNextButtonLink { get; set; }

        ShowPageLink ShowLastButtonLink { get; set; }

        /// <summary>
        /// The number of sibling pages to show on each side of the current page.
        /// </summary>
        int SiblingCount { get; set; }

        /// <summary>
        /// The number of boundary pages to show on each side of the pager.
        /// </summary>
        int BoundaryCount { get; set; }

        bool ShowEllipsis { get; set; }
    }
}
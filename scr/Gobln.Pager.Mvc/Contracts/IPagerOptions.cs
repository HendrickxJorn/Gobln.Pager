namespace Gobln.Pager.Mvc
{
    public interface IPagerOptions
    {
        /// <summary>
        /// Show the pager
        /// </summary>
        WhenToShowEnum ShowPager { get; set; }

        /// <summary>
        /// Show the first button link
        /// </summary>
        WhenToShowEnum ShowFirstButtonLink { get; set; }

        /// <summary>
        /// Show the previous button link
        /// </summary>
        WhenToShowEnum ShowPreviousButtonLink { get; set; }

        /// <summary>
        /// Show the pager button links
        /// </summary>
        WhenToShowEnum ShowPagerButtonLinks { get; set; }

        /// <summary>
        /// Show the next button link
        /// </summary>
        WhenToShowEnum ShowNextButtonLink { get; set; }

        /// <summary>
        /// Show the last button link
        /// </summary>
        WhenToShowEnum ShowLastButtonLink { get; set; }

        /// <summary>
        /// Nav Tag Attributes
        /// </summary>
        object? NavTagHtmlAttributes { get; set; }

        /// <summary>
        /// Ul Tag Attributes
        /// </summary>
        object? UlTagHtmlAttributes { get; set; }

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

        /// <summary>
        /// The number of sibling pages to show on each side of the current page.
        /// </summary>
        int SiblingCount { get; set; }

        /// <summary>
        /// The number of boundary pages to show on each side of the pager.
        /// </summary>
        int BoundaryCount { get; set; }

        /// <summary>
        /// Show ellipsis
        /// </summary>
        bool ShowEllipsis { get; set; }

        /// <summary>
        /// Show the range of items instead of the page index
        /// </summary>
        bool ShowIndexAsRange { get; set; }
    }
}
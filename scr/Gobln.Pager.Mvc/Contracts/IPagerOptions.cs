namespace Gobln.Pager.Mvc;

public interface IPagerOptions
{
    /// <summary>
    /// Show the pager
    /// </summary>
    WhenToShowEnum ShowPager { get; set; }

    /// <summary>
    /// Show the first button link
    /// </summary>
    WhenToShowEnum ShowButtonFirstLink { get; set; }

    /// <summary>
    /// Show the previous button link
    /// </summary>
    WhenToShowEnum ShowButtonPreviousLink { get; set; }

    /// <summary>
    /// Show the numbered page links. IfNecessary shows them only when there is more than one page.
    /// </summary>
    WhenToShowEnum ShowPagerButtonLinks { get; set; }

    /// <summary>
    /// Show the next button link
    /// </summary>
    WhenToShowEnum ShowButtonNextLink { get; set; }

    /// <summary>
    /// Show the last button link
    /// </summary>
    WhenToShowEnum ShowButtonLastLink { get; set; }

    /// <summary>
    /// Nav Tag Attributes
    /// </summary>
    object? NavTagHtmlAttributes { get; set; }

    /// <summary>
    /// Ul Tag Attributes
    /// </summary>
    object? UlTagHtmlAttributes { get; set; }

    string ButtonFirstLinkLabel { get; set; }

    string ButtonPreviousLinkLabel { get; set; }

    bool AllowDisabledPageLink { get; set; }

    string ButtonNextLinkLabel { get; set; }

    bool DisplayActive { get; set; }

    string? ButtonPreviousLinkIcon { get; set; }

    string? ButtonNextLinkIcon { get; set; }

    string? ButtonFirstLinkIcon { get; set; }

    string? ButtonLastLinkIcon { get; set; }

    string ButtonLastLinkLabel { get; set; }

    /// <summary>
    /// The name of the url to use for the page index
    /// </summary>
    string PageIndexUrlName { get; set; }

    /// <summary>
    /// The name of the data to use for the page index
    /// </summary>
    string? PageIndexDataName { get; set; }

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
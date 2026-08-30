namespace Gobln.Pager.Mvc;

public class PagerOptions : IPagerOptions
{
    public PagerOptions()
    {
    }

    protected PagerOptions(PagerOptions other)
    {
        this.ShowButtonNextLink = other.ShowButtonNextLink;
        this.ShowButtonLastLink = other.ShowButtonLastLink;
        this.ShowPagerButtonLinks = other.ShowPagerButtonLinks;
        this.ShowButtonPreviousLink = other.ShowButtonPreviousLink;
        this.ShowButtonFirstLink = other.ShowButtonFirstLink;
        this.ShowPager = other.ShowPager;
        this.NavTagHtmlAttributes = other.NavTagHtmlAttributes;
        this.UlTagHtmlAttributes = other.UlTagHtmlAttributes;
        this.ButtonFirstLinkLabel = other.ButtonFirstLinkLabel;
        this.ButtonPreviousLinkLabel = other.ButtonPreviousLinkLabel;
        this.AllowDisabledPageLink = other.AllowDisabledPageLink;
        this.AllowDisabledPageLinkTab = other.AllowDisabledPageLinkTab;
        this.ButtonNextLinkLabel = other.ButtonNextLinkLabel;
        this.DisplayActive = other.DisplayActive;
        this.ButtonPreviousLinkIcon = other.ButtonPreviousLinkIcon;
        this.ButtonNextLinkIcon = other.ButtonNextLinkIcon;
        this.ButtonFirstLinkIcon = other.ButtonFirstLinkIcon;
        this.ButtonLastLinkIcon = other.ButtonLastLinkIcon;
        this.ButtonLastLinkLabel = other.ButtonLastLinkLabel;
        this.PageIndexUrlName = other.PageIndexUrlName;
        this.PageIndexDataName = other.PageIndexDataName;
        this.HRefUrl = other.HRefUrl;
        this.SiblingCount = other.SiblingCount;
        this.BoundaryCount = other.BoundaryCount;
        this.ShowEllipsis = other.ShowEllipsis;
        this.ShowIndexAsRange = other.ShowIndexAsRange;
    }

    /// <summary>
    /// Show the pager
    /// </summary>
    public WhenToShowEnum ShowPager { get; set; } = WhenToShowEnum.IfNecessary;

    /// <summary>
    /// Show the first button link
    /// </summary>
    public WhenToShowEnum ShowButtonFirstLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the previous button link
    /// </summary>
    public WhenToShowEnum ShowButtonPreviousLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the pager button links
    /// </summary>
    public WhenToShowEnum ShowPagerButtonLinks { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the next button link
    /// </summary>
    public WhenToShowEnum ShowButtonNextLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the last button link
    /// </summary>
    public WhenToShowEnum ShowButtonLastLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// The number of sibling pages to show on each side of the current page.
    /// </summary>
    public int SiblingCount { get; set; } = 1;

    /// <summary>
    /// The number of always visible pages at the beginning and end.
    /// </summary>
    public int BoundaryCount { get; set; } = 1;

    /// <summary>
    /// Show ellipsis
    /// </summary>
    public bool ShowEllipsis { get; set; } = true;

    /// <summary>
    /// Show the range of items instead of the page index
    /// </summary>
    public bool ShowIndexAsRange { get; set; }

    public string ButtonPreviousLinkLabel { get; set; } = "Previous";

    public string? ButtonPreviousLinkIcon { get; set; } = null;

    public bool AllowDisabledPageLink { get; set; } = true;

    public bool AllowDisabledPageLinkTab { get; set; } = true;

    public string ButtonNextLinkLabel { get; set; } = "Next";

    public string? ButtonNextLinkIcon { get; set; } = null;

    public string ButtonFirstLinkLabel { get; set; } = "First";

    public string? ButtonFirstLinkIcon { get; set; } = null;

    public string? ButtonLastLinkIcon { get; set; } = null;

    public string ButtonLastLinkLabel { get; set; } = "Last";

    /// <summary>
    /// The url to use for the page links
    /// If null, the url of the page is used
    /// If "#" then all links are "#"
    /// </summary>
    public string? HRefUrl { get; set; } = null;

    /// <summary>
    /// Nav Tag Attributes
    /// </summary>
    public object? NavTagHtmlAttributes { get; set; } = null;

    /// <summary>
    /// Ul Tag Attributes
    /// </summary>
    public object? UlTagHtmlAttributes { get; set; } = null;

    public bool DisplayActive { get; set; } = true;

    /// <summary>
    /// The name of the pageindex for the href in the a-link
    /// </summary>
    public string PageIndexUrlName { get; set; } = "pageIndex";

    /// <summary>
    /// The name of the pageindex for the data in the a-link
    /// </summary>
    public string? PageIndexDataName { get; set; } = null;

    public PagerOptions Clone()
    {
        return new PagerOptions(this);
    }
}
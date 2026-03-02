namespace Gobln.Pager.Mvc;

public class PagerOptions : IPagerOptions
{
    public PagerOptions()
    {
    }

    protected PagerOptions(PagerOptions other)
    {
        this.ShowNextButtonLink = other.ShowNextButtonLink;
        this.ShowLastButtonLink = other.ShowLastButtonLink;
        this.ShowPagerButtonLinks = other.ShowPagerButtonLinks;
        this.ShowPreviousButtonLink = other.ShowPreviousButtonLink;
        this.ShowFirstButtonLink = other.ShowFirstButtonLink;
        this.ShowPager = other.ShowPager;
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
    public WhenToShowEnum ShowFirstButtonLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the previous button link
    /// </summary>
    public WhenToShowEnum ShowPreviousButtonLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the pager button links
    /// </summary>
    public WhenToShowEnum ShowPagerButtonLinks { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the next button link
    /// </summary>
    public WhenToShowEnum ShowNextButtonLink { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// Show the last button link
    /// </summary>
    public WhenToShowEnum ShowLastButtonLink { get; set; } = WhenToShowEnum.Always;

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
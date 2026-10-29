using Gobln.Pager.Mvc;

namespace Gobln.Pager.MvcTest.Helper;

/// <summary>
/// An <see cref="IPagerOptions"/> that is not a <see cref="PagerOptions"/>, the way an
/// application might bring its own options type.
/// </summary>
public class CustomPagerOptions : IPagerOptions
{
    public WhenToShowEnum ShowPager { get; set; } = WhenToShowEnum.IfNecessary;
    public WhenToShowEnum ShowButtonFirstLink { get; set; } = WhenToShowEnum.Always;
    public WhenToShowEnum ShowButtonPreviousLink { get; set; } = WhenToShowEnum.Always;
    public WhenToShowEnum ShowPagerButtonLinks { get; set; } = WhenToShowEnum.Always;
    public WhenToShowEnum ShowButtonNextLink { get; set; } = WhenToShowEnum.Always;
    public WhenToShowEnum ShowButtonLastLink { get; set; } = WhenToShowEnum.Always;
    public object? NavTagHtmlAttributes { get; set; }
    public object? UlTagHtmlAttributes { get; set; }
    public string ButtonFirstLinkLabel { get; set; } = "First";
    public string ButtonPreviousLinkLabel { get; set; } = "Previous";
    public bool AllowDisabledPageLink { get; set; } = true;
    public string ButtonNextLinkLabel { get; set; } = "Next";
    public bool DisplayActive { get; set; } = true;
    public string? ButtonPreviousLinkIcon { get; set; }
    public string? ButtonNextLinkIcon { get; set; }
    public string? ButtonFirstLinkIcon { get; set; }
    public string? ButtonLastLinkIcon { get; set; }
    public string ButtonLastLinkLabel { get; set; } = "Last";
    public string PageIndexUrlName { get; set; } = "pageIndex";
    public string? PageIndexDataName { get; set; }
    public string? HRefUrl { get; set; }
    public int SiblingCount { get; set; } = 1;
    public int BoundaryCount { get; set; } = 1;
    public bool ShowEllipsis { get; set; } = true;
    public bool ShowIndexAsRange { get; set; }
}

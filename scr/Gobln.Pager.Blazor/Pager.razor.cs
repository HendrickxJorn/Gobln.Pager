using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Gobln.Pager.Blazor;

public partial class Pager
{
    [Parameter, EditorRequired]
    public IPage Page { get; set; }

    /// <summary>
    /// The number of sibling pages to show on each side of the current page.
    /// </summary>
    [DefaultValue(1)]
    [Description("The number of sibling pages to show on each side of the current page.")]
    [Parameter]
    public int SiblingCount { get; set; } = 1;

    /// <summary>
    /// The number of always visible pages at the beginning and end.
    /// </summary>
    [DefaultValue(0)]
    [Description("The number of always visible pages at the beginning and end.")]
    [Parameter]
    public int BoundaryCount { get; set; } = 0;

    /// <summary>
    /// Show ellipsis.
    /// </summary>
    [DefaultValue(false)]
    [Description("Show ellipsis.")]
    [Parameter]
    public bool ShowEllipsis { get; set; } = false;

    /// <summary>
    /// When to show the pager. IfNecessary: only when there is more than one page.
    /// </summary>
    [DefaultValue(WhenToShowEnum.Always)]
    [Description("When to show the pager. IfNecessary: only when there is more than one page.")]
    [Parameter]
    public WhenToShowEnum ShowPager { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// When to show the First button. IfNecessary: only when not on the first page.
    /// </summary>
    [DefaultValue(WhenToShowEnum.Always)]
    [Description("When to show the First button. IfNecessary: only when not on the first page.")]
    [Parameter]
    public WhenToShowEnum ShowFirstButton { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// When to show the Previous button. IfNecessary: only when not on the first page.
    /// </summary>
    [DefaultValue(WhenToShowEnum.Always)]
    [Description("When to show the Previous button. IfNecessary: only when not on the first page.")]
    [Parameter]
    public WhenToShowEnum ShowPreviousButton { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// When to show the Next button. IfNecessary: only when not on the last page.
    /// </summary>
    [DefaultValue(WhenToShowEnum.Always)]
    [Description("When to show the Next button. IfNecessary: only when not on the last page.")]
    [Parameter]
    public WhenToShowEnum ShowNextButton { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// When to show the Last button. IfNecessary: only when not on the last page.
    /// </summary>
    [DefaultValue(WhenToShowEnum.Always)]
    [Description("When to show the Last button. IfNecessary: only when not on the last page.")]
    [Parameter]
    public WhenToShowEnum ShowLastButton { get; set; } = WhenToShowEnum.Always;

    /// <summary>
    /// The text of the First button. With <see cref="FirstIcon"/>, its aria-label.
    /// </summary>
    [DefaultValue("First")]
    [Description("The text of the First button. With FirstIcon, its aria-label.")]
    [Parameter]
    public string FirstText { get; set; } = "First";

    /// <summary>
    /// The text of the Previous button. With <see cref="PreviousIcon"/>, its aria-label.
    /// </summary>
    [DefaultValue("Previous")]
    [Description("The text of the Previous button. With PreviousIcon, its aria-label.")]
    [Parameter]
    public string PreviousText { get; set; } = "Previous";

    /// <summary>
    /// The text of the Next button. With <see cref="NextIcon"/>, its aria-label.
    /// </summary>
    [DefaultValue("Next")]
    [Description("The text of the Next button. With NextIcon, its aria-label.")]
    [Parameter]
    public string NextText { get; set; } = "Next";

    /// <summary>
    /// The text of the Last button. With <see cref="LastIcon"/>, its aria-label.
    /// </summary>
    [DefaultValue("Last")]
    [Description("The text of the Last button. With LastIcon, its aria-label.")]
    [Parameter]
    public string LastText { get; set; } = "Last";

    /// <summary>
    /// Shown instead of <see cref="FirstText"/>, which becomes the button's aria-label.
    /// </summary>
    [DefaultValue(null)]
    [Description("Shown instead of FirstText, which becomes the button's aria-label.")]
    [Parameter]
    public RenderFragment? FirstIcon { get; set; }

    /// <summary>
    /// Shown instead of <see cref="PreviousText"/>, which becomes the button's aria-label.
    /// </summary>
    [DefaultValue(null)]
    [Description("Shown instead of PreviousText, which becomes the button's aria-label.")]
    [Parameter]
    public RenderFragment? PreviousIcon { get; set; }

    /// <summary>
    /// Shown instead of <see cref="NextText"/>, which becomes the button's aria-label.
    /// </summary>
    [DefaultValue(null)]
    [Description("Shown instead of NextText, which becomes the button's aria-label.")]
    [Parameter]
    public RenderFragment? NextIcon { get; set; }

    /// <summary>
    /// Shown instead of <see cref="LastText"/>, which becomes the button's aria-label.
    /// </summary>
    [DefaultValue(null)]
    [Description("Shown instead of LastText, which becomes the button's aria-label.")]
    [Parameter]
    public RenderFragment? LastIcon { get; set; }

    /// <summary>
    /// The aria-label of the nav, which tells the pager apart from the site's other navigation.
    /// </summary>
    [DefaultValue("Page navigation")]
    [Description("The aria-label of the nav, which tells the pager apart from the site's other navigation.")]
    [Parameter]
    public string AriaLabel { get; set; } = "Page navigation";

    /// <summary>
    /// Additional CSS classes to apply to the pagination list.
    /// </summary>
    [DefaultValue(null)]
    [Description("Additional CSS classes to apply to the pagination list.")]
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// This event is triggered when the page index changes.
    /// </summary>
    [Description("This event is triggered when the page index changes.")]
    [Parameter]
    public EventCallback<int> PageIndexChanged { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Page == null)
        {
            throw new ArgumentNullException(nameof(Page), "Page parameter is required.");
        }

        if (Page.TotalPageCount < 1)
        {
            // nothing to render, but keep values safe.
            // Deliberately leave _lastSeenPageIndex untouched, so the index of the
            // first non-empty Page is still seen as a change and gets adopted below.
            PageIndex = 1;
            TotalPageCount = 1;
        }
        else
        {
            TotalPageCount = Page.TotalPageCount;

            if (!ReferenceEquals(Page, _lastSeenPage) || Page.PageIndex != _lastSeenPageIndex)
            {
                // Parent handed over a new Page, or deliberately changed PageIndex — adopt it.
                // A new Page wins even on the same index: the parent may have reset to it.
                PageIndex = Math.Clamp(Page.PageIndex, 1, TotalPageCount);
                _lastSeenPage = Page;
                _lastSeenPageIndex = Page.PageIndex;
            }
            else
            {
                // Unrelated re-render — keep our local state but re-clamp in case TotalPageCount shrank.
                PageIndex = Math.Clamp(PageIndex, 1, TotalPageCount);
            }
        }
    }

    private IPage? _lastSeenPage;

    private int _lastSeenPageIndex;

    private int PageIndex;

    private int TotalPageCount;

    private string ClassNames => string.IsNullOrWhiteSpace(ClassName)
        ? "pagination"
        : $"pagination {ClassName.Trim()}";

    // Always, or IfNecessary when it is necessary
    private static bool Show(WhenToShowEnum whenToShow, bool isNecessary) =>
        whenToShow == WhenToShowEnum.Always || (whenToShow == WhenToShowEnum.IfNecessary && isNecessary);

    // The page numbers to show, with null where an ellipsis goes
    private IReadOnlyList<int?> PageNumbers => PageWindow.GetPageNumbers(PageIndex, TotalPageCount, SiblingCount, BoundaryCount, ShowEllipsis);

    private async Task InvokePageChange(int pagenm)
    {
        if (TotalPageCount <= 0) return;

        // clamp pagenm to valid range
        var target = Math.Clamp(pagenm, 1, TotalPageCount);

        if (PageIndex != target)
        {
            PageIndex = target;

            await PageIndexChanged.InvokeAsync(target);
        }
    }
}
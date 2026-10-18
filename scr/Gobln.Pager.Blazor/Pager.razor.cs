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

            if (Page.PageIndex != _lastSeenPageIndex)
            {
                // Parent deliberately changed PageIndex — adopt it.
                PageIndex = Math.Clamp(Page.PageIndex, 1, TotalPageCount);
                _lastSeenPageIndex = Page.PageIndex;
            }
            else
            {
                // Unrelated re-render — keep our local state but re-clamp in case TotalPageCount shrank.
                PageIndex = Math.Clamp(PageIndex, 1, TotalPageCount);
            }
        }
    }

    private int _lastSeenPageIndex;

    private int PageIndex;

    private int TotalPageCount;

    private string ClassNames => string.IsNullOrWhiteSpace(ClassName)
        ? "pagination"
        : $"pagination {ClassName.Trim()}";

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
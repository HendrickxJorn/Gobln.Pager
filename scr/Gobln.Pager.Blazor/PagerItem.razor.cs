using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Gobln.Pager.Blazor;

public partial class PagerItem
{
    /// <summary>
    /// The text to display in the pager item.
    /// </summary>
    [DefaultValue(null)]
    [Description("The text to display in the pager item.")]
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Indicates whether the pager item is active.
    /// </summary>
    [DefaultValue(false)]
    [Description("Indicates whether the pager item is active.")]
    [Parameter]
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// Indicates whether the pager item is disabled.
    /// </summary>
    [DefaultValue(false)]
    [Description("Indicates whether the pager item is disabled.")]
    [Parameter]
    public bool IsDisabled { get; set; } = false;

    /// <summary>
    /// Gets or sets additional attributes that will be applied to the component.
    /// </summary>
    [DefaultValue(null)]
    [Description("Gets or sets additional attributes that will be applied to the component.")]
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = [];

    private string ClassNames => "page-item"
        + (IsDisabled ? " disabled" : "")
        + (IsActive ? " active" : "");
}
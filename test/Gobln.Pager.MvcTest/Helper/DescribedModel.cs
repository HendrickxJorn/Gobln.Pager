using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gobln.Pager.MvcTest.Helper;

/// <summary>
/// Model for the DescriptionFor / DisplayNameFor helpers: one property per attribute
/// combination those helpers have to resolve.
/// </summary>
public class DescribedModel
{
    [Description("From DescriptionAttribute")]
    public string? WithDescription { get; set; }

    [Display(Description = "From DisplayAttribute")]
    public string? WithDisplayDescription { get; set; }

    [Description("DescriptionAttribute wins")]
    [Display(Description = "DisplayAttribute loses")]
    public string? WithBoth { get; set; }

    [Display(Name = "Friendly name")]
    public string? WithDisplayName { get; set; }

    public string? Plain { get; set; }
}

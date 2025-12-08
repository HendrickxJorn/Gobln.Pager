using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gobln.Pager.Blazor
{
    public partial class PagerItem : ComponentBase
    {
        protected override void OnParametersSet()
        {

            base.OnParametersSet();
        }

        protected string Classdd
        {
            get
            {
                return "page-item"
                    + (IsDisabled ? " disabled" : "")
                    + (IsActive ? " active" : "");
            }
        }

        /// <summary>
        /// The text to display in the pager item.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// Indicates whether the pager item is active.
        /// </summary>
        [Parameter]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Indicates whether the pager item is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; } = false;

        //https://learn.microsoft.com/en-us/aspnet/core/blazor/components/splat-attributes-and-arbitrary-parameters?view=aspnetcore-8.0
        //https://stackoverflow.com/questions/62448581/blazor-button-use-parent-component-onclick

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; }
    }
}

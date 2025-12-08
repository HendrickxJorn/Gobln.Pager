using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace Gobln.Pager.Blazor
{
    public partial class Pager : ComponentBase
    {
        [Parameter, EditorRequired]
        public IPage Page { get; set; }

        /// <summary>
        /// The number of sibling pages to show on each side of the current page.
        /// </summary>
        [Parameter]
        public int SiblingCount { get; set; } = 1;


        /// <summary>
        /// The number of always visible pages at the beginning and end.
        /// </summary>
        [Parameter]
        public int BoundaryCount { get; set; } = 0;


        /// <summary>
        /// Show ellipsis
        /// </summary>
        [Parameter]
        public bool ShowEllipsis { get; set; } = false;


        // fires when changing the page, retuns the current page
        // todo rename
        [Parameter]
        public EventCallback<int> PageChanging { get; set; }


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Page == null)
            {
                throw new ArgumentNullException(nameof(Page), "Page parameter is required.");
            }

            PageIndex = Page.PageIndex;
            TotalPageCount = Page.TotalPageCount;
        }

        private int PageIndex;

        private int TotalPageCount;

        private int StartBoundery => Math.Min(BoundaryCount, TotalPageCount);

        private int EndBoundery => TotalPageCount - Math.Max(TotalPageCount - BoundaryCount + 1, BoundaryCount + 1) + 1;

        private int StartNumber
        {
            get
            {
                var elipseModifier = ShowEllipsis ? 0 : 0;

                return Math.Max(
                            Math.Min(
                                // Original start
                                PageIndex - SiblingCount,
                                // Page boundery +/- elipse
                                TotalPageCount - BoundaryCount - SiblingCount * 2 - elipseModifier
                            ),
                            // After start +/- elipse
                            BoundaryCount + 1 + elipseModifier
                        );
            }
        }

        private int EndNumber
        {
            get
            {
                var elipseModifier = ShowEllipsis ? 0 : 0;

                return Math.Min(
                            Math.Max(
                                // Original end
                                PageIndex + SiblingCount,
                                // Page boundery +/- elipse
                                BoundaryCount + SiblingCount * 2 + 1 + elipseModifier

                            ),
                            // Before end +/- elipse
                            TotalPageCount - BoundaryCount - elipseModifier
                    );
            }
        }

        private async Task InvokePageChange(int pagenm)
        {
            if (PageIndex != pagenm)
            {
                PageIndex = pagenm;

                await PageChanging.InvokeAsync(pagenm);
            }
        }
    }
}

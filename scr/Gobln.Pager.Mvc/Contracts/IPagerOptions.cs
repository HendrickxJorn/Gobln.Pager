namespace Gobln.Pager.Mvc.Contracts
{
    public interface IPagerOptions
    {
        bool AlwaysShowPager { get; set; }

        /// <summary>
        /// Nav Tag Attributes
        /// </summary>
        object NavTagHtmlAttributes { get; set; }

        /// <summary>
        /// Ul Tag Attributes
        /// </summary>
        object UlTagHtmlAttributes { get; set; }

        PageLinkItem[] ShowItemOrder { get; set; }

        string FirstPageLinkLabel { get; set; }

        string PreviousPageLinkLabel { get; set; }

        bool PreviousPageLinkAlwaysShow { get; set; }

        bool AllowDisabledPageLink { get; set; }

        bool AllowDisabledPageLinkTab { get; set; }

        string NextPageLinkLabel { get; set; }

        bool NextPageLinkAlwaysShow { get; set; }

        bool DisplayActive { get; set; }

        string? PreviousPageLinkIconLabel { get; set; }

        string? NextPageLinkIconLabel { get; set; }

        string? FirstPageLinkIconLabel { get; set; }

        string? LastPageLinkIconLabel { get; set; }

        string LastPageLinkLabel { get; set; }

        bool LastPageLinkAlwaysShow { get; set; }

        bool FirstPageLinkAlwaysShow { get; set; }

        string PageIndexName { get; set; }

        /// <summary>
        /// The url to use for the page links
        /// If null, the url of the page is used
        /// If "#" then all links are "#"
        /// </summary>
        string? HRefUrl { get; set; }
    }
}
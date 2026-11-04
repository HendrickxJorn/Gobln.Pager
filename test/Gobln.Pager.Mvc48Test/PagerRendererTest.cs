using Gobln.Pager.Mvc;
using Gobln.Pager.Mvc48Test.Helper;
using System.Web.Mvc;
using Xunit;

namespace Gobln.Pager.Mvc48Test;

/// <summary>
/// What a renderer is handed for a render, and what it refuses.
/// </summary>
public class PagerRendererTest
{
    private readonly HelperHtml _helperHtml = new();
    private readonly IPage _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);

    /// <summary>
    /// A renderer that needs nothing but the page
    /// </summary>
    private sealed class PageOnlyRenderer : IPagerRenderer
    {
        public MvcHtmlString Render(IPage page) => MvcHtmlString.Create($"page {page.PageIndex} of {page.TotalPageCount}");
    }

    [Fact]
    public void Pager_WithAPageOnlyRenderer_HandsItThePage()
    {
        var result = _helperHtml.CreateHtmlHelper().Pager(_page3, new PageOnlyRenderer()).ToHtmlString();

        Assert.Equal("page 1 of 3", result);
    }

    [Fact]
    public void Render_WithoutOptionsOrUrl_Throws()
    {
        var renderer = new PagerRenderer();

        Assert.Equal("options", Assert.Throws<ArgumentNullException>(() => renderer.Render(_page3, null!, "#")).ParamName);
        Assert.Equal("hRefUrl", Assert.Throws<ArgumentNullException>(() => renderer.Render(_page3, new PagerOptions(), null!)).ParamName);
    }
}

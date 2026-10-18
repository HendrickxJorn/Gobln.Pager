using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using Xunit;

namespace Gobln.Pager.MvcTest;

/// <summary>
/// What a renderer is handed for a render, and that nothing of one render reaches another.
/// </summary>
public class PagerRendererTest
{
    private static readonly Regex Href = new("href=\"([^\"]*)\"");

    private readonly HelperHtml _helperHtml = new();
    private readonly IPage _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);

    /// <summary>
    /// A renderer that isn't a <see cref="PagerRenderer"/>: it shows what it was handed.
    /// </summary>
    private sealed class RecordingRenderer : IPagerRenderer<PagerOptions>
    {
        public IHtmlContent Render(IPage page, PagerOptions options, string hRefUrl)
        {
            return new HtmlString($"{page.TotalPageCount}|{options.ButtonNextLinkLabel}|{hRefUrl}");
        }
    }

    /// <summary>
    /// A <see cref="PagerRenderer"/> whose override reads the options and url of the render.
    /// </summary>
    private sealed class StateReadingRenderer : PagerRenderer
    {
        public override TagBuilder TagCreatorNav()
        {
            var nav = base.TagCreatorNav();

            nav.Attributes["data-url"] = HRefUrl;
            nav.Attributes["data-next"] = PagerOptions.ButtonNextLinkLabel;

            return nav;
        }
    }

    [Fact]
    public void Pager_HandsTheRendererThePageTheOptionsAndTheUrlOfTheRequest()
    {
        var result = _helperHtml.CreateHtmlHelper(url: "http://pager.test/orders")
            .Pager(_page3, new PagerOptions { ButtonNextLinkLabel = "Volgende" }, new RecordingRenderer())
            .ToHtmlString();

        Assert.Equal("3|Volgende|http://pager.test/orders", result);
    }

    [Fact]
    public void Render_BuildsTheLinksOnTheUrlItIsHanded()
    {
        var result = new PagerRenderer().Render(_page3, new PagerOptions(), "http://pager.test/list").ToHtmlString();

        Assert.Contains("""href="http://pager.test/list?pageIndex=2">2</a>""", result);
    }

    [Fact]
    public void Render_WithoutOptionsOrUrl_Throws()
    {
        var renderer = new PagerRenderer();

        Assert.Equal("options", Assert.Throws<ArgumentNullException>(() => renderer.Render(_page3, null!, "#")).ParamName);
        Assert.Equal("hRefUrl", Assert.Throws<ArgumentNullException>(() => renderer.Render(_page3, new PagerOptions(), null!)).ParamName);
    }

    [Fact]
    public void Render_Overrides_SeeTheOptionsAndUrlOfTheRender()
    {
        var result = new StateReadingRenderer()
            .Render(_page3, new PagerOptions { ButtonNextLinkLabel = "Volgende" }, "http://pager.test/list")
            .ToHtmlString();

        Assert.Contains("data-url=\"http://pager.test/list\"", result);
        Assert.Contains("data-next=\"Volgende\"", result);
    }

    [Fact]
    public void Render_KeepsNothingOfTheRenderOnTheRenderer()
    {
        // A renderer may be shared between views and requests (a static field, a singleton).
        // What one render was handed must not be there for the next one to pick up.
        var options = new PagerOptions { ButtonNextLinkLabel = "Volgende" };
        var renderer = new PagerRenderer();

        renderer.Render(_page3, options, "http://first.test/orders");

        Assert.Equal("#", renderer.HRefUrl);
        Assert.NotSame(options, renderer.PagerOptions);
    }

    [Fact]
    public void SharedRenderer_ConcurrentRenders_KeepTheirOwnOptionsAndUrl()
    {
        var renderer = new PagerRenderer();
        var failures = new System.Collections.Concurrent.ConcurrentBag<string>();

        Parallel.For(0, 2000, i =>
        {
            var url = $"http://host{i}.test/list";
            var options = new PagerOptions { ButtonNextLinkLabel = $"Next{i}" };

            var result = renderer.Render(_page3, options, url).ToHtmlString();

            var hrefs = Href.Matches(result).Select(m => m.Groups[1].Value).ToArray();

            if (hrefs.Length == 0 || hrefs.Any(href => !href.StartsWith(url)) || !result.Contains($">Next{i}<"))
            {
                failures.Add($"render {i}: {result}");
            }
        });

        Assert.True(failures.IsEmpty, $"{failures.Count} renders got another render's options or url:\n{failures.FirstOrDefault()}");
    }
}

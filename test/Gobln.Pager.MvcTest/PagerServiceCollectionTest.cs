using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Gobln.Pager.MvcTest;

/// <summary>
/// AddMvcPager and the defaults it installs.
///
/// The defaults live in a process-wide static, so every test here restores them on the
/// way out (see Dispose) and the assembly disables test parallelization.
/// </summary>
public class PagerServiceCollectionTest : IDisposable
{
    private readonly HelperHtml _helperHtml = new();

    private readonly IPage _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);

    public void Dispose()
    {
        // Put the library defaults back so later test classes see a clean slate.
        new ServiceCollection().AddMvcPager(new PagerOptions());

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void AddMvcPager_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddMvcPager());
        Assert.Same(services, services.AddMvcPager(new PagerOptions()));
        Assert.Same(services, services.AddMvcPager(options => options.SiblingCount = 1));
    }

    [Fact]
    public void AddMvcPager_NullOptions_Throws()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => services.AddMvcPager((IPagerOptions)null!));
        Assert.Throws<ArgumentNullException>(() => services.AddMvcPager((Action<IPagerOptions>)null!));
    }

    [Fact]
    public void AddMvcPager_WithOptions_AppliesThemToTheParameterlessPager()
    {
        new ServiceCollection().AddMvcPager(new PagerOptions
        {
            HRefUrl = "#",
            ButtonFirstLinkLabel = "Configured first",
            ButtonLastLinkLabel = "Configured last",
        });

        var html = _helperHtml.CreateHtmlHelper().Pager(_page3).ToHtmlString();

        Assert.Contains("Configured first", html);
        Assert.Contains("Configured last", html);
    }

    [Fact]
    public void AddMvcPager_WithDelegate_AppliesThemToTheParameterlessPager()
    {
        new ServiceCollection().AddMvcPager(options =>
        {
            options.HRefUrl = "#";
            options.ButtonNextLinkLabel = "Configured next";
        });

        var html = _helperHtml.CreateHtmlHelper().Pager(_page3).ToHtmlString();

        Assert.Contains("Configured next", html);
    }

    [Fact]
    public void Pager_DoesNotLeakOneRequestsUrlIntoTheNext()
    {
        // HRefUrl is resolved from the current request when it is not set. That resolved
        // value must land on a per-call copy of the options -- writing it back into the
        // shared defaults would pin the first request's url onto every later one.
        new ServiceCollection().AddMvcPager(new PagerOptions { HRefUrl = null });

        var first = _helperHtml.CreateHtmlHelper("https://first.test/list")
            .Pager(_page3)
            .ToHtmlString();

        var second = _helperHtml.CreateHtmlHelper("https://second.test/list")
            .Pager(_page3)
            .ToHtmlString();

        Assert.Contains("first.test", first);
        Assert.DoesNotContain("second.test", first);

        Assert.Contains("second.test", second);
        Assert.DoesNotContain("first.test", second);
    }
}

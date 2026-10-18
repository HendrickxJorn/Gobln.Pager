using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Gobln.Pager.MvcTest;

/// <summary>
/// AddMvcPager and the defaults it registers in the app's services, which the
/// parameterless Html.Pager(page) overload gets from the request.
/// </summary>
public class PagerServiceCollectionTest
{
    private readonly HelperHtml _helperHtml = new();

    private readonly IPage _page3 = new int[] { 1, 2, 3 }.ToPage(1, 1);

    /// <summary>
    /// An html helper for a request to <paramref name="url"/> in the app with <paramref name="services"/>
    /// </summary>
    private IHtmlHelper HtmlHelperFor(IServiceProvider services, string url = "http://pager.test/list")
    {
        return _helperHtml.CreateHtmlHelper(url: url, services: services);
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
        var services = new ServiceCollection().AddMvcPager(new PagerOptions
        {
            HRefUrl = "#",
            ButtonFirstLinkLabel = "Configured first",
            ButtonLastLinkLabel = "Configured last",
        });

        var html = HtmlHelperFor(services.BuildServiceProvider()).Pager(_page3).ToHtmlString();

        Assert.Contains("Configured first", html);
        Assert.Contains("Configured last", html);
    }

    [Fact]
    public void AddMvcPager_WithCustomOptionsType_AppliesThemToTheParameterlessPager()
    {
        // The overload takes any IPagerOptions, not just PagerOptions
        var services = new ServiceCollection().AddMvcPager(new CustomPagerOptions
        {
            HRefUrl = "#",
            ButtonFirstLinkLabel = "Custom first",
        });

        var html = HtmlHelperFor(services.BuildServiceProvider()).Pager(_page3).ToHtmlString();

        Assert.Contains("Custom first", html);
    }

    [Fact]
    public void AddMvcPager_WithDelegate_AppliesThemToTheParameterlessPager()
    {
        var services = new ServiceCollection().AddMvcPager(options =>
        {
            options.HRefUrl = "#";
            options.ButtonNextLinkLabel = "Configured next";
        });

        var html = HtmlHelperFor(services.BuildServiceProvider()).Pager(_page3).ToHtmlString();

        Assert.Contains("Configured next", html);
    }

    [Fact]
    public void AddMvcPager_TwoApps_KeepTheirOwnDefaults()
    {
        // Two apps in one process (two test hosts, for example) each get the defaults they
        // registered. A process-wide default would hand both whatever was registered last.
        var first = new ServiceCollection()
            .AddMvcPager(options => options.ButtonFirstLinkLabel = "First app")
            .BuildServiceProvider();

        var second = new ServiceCollection()
            .AddMvcPager(options => options.ButtonFirstLinkLabel = "Second app")
            .BuildServiceProvider();

        var firstHtml = HtmlHelperFor(first).Pager(_page3).ToHtmlString();
        var secondHtml = HtmlHelperFor(second).Pager(_page3).ToHtmlString();

        Assert.Contains("First app", firstHtml);
        Assert.DoesNotContain("Second app", firstHtml);

        Assert.Contains("Second app", secondHtml);
        Assert.DoesNotContain("First app", secondHtml);
    }

    [Fact]
    public void AddMvcPager_WithOptions_KeepsACopy()
    {
        // Later changes to the object handed to AddMvcPager don't reach the defaults
        var options = new PagerOptions { ButtonFirstLinkLabel = "Registered" };

        var services = new ServiceCollection().AddMvcPager(options).BuildServiceProvider();

        options.ButtonFirstLinkLabel = "Changed later";

        var html = HtmlHelperFor(services).Pager(_page3).ToHtmlString();

        Assert.Contains("Registered", html);
        Assert.DoesNotContain("Changed later", html);
    }

    [Fact]
    public void Pager_WithoutAddMvcPager_UsesTheLibraryDefaults()
    {
        var html = HtmlHelperFor(new ServiceCollection().BuildServiceProvider()).Pager(_page3).ToHtmlString();

        Assert.Contains(">First<", html);
        Assert.Contains(">Next<", html);
    }

    [Fact]
    public void Pager_DoesNotLeakOneRequestsUrlIntoTheNext()
    {
        // HRefUrl is resolved from the current request when it is not set. That resolved
        // value must land on a per-call copy of the options -- writing it back into the
        // shared defaults would pin the first request's url onto every later one.
        var services = new ServiceCollection()
            .AddMvcPager(new PagerOptions { HRefUrl = null })
            .BuildServiceProvider();

        var first = HtmlHelperFor(services, "https://pager.test/orders")
            .Pager(_page3)
            .ToHtmlString();

        var second = HtmlHelperFor(services, "https://pager.test/products")
            .Pager(_page3)
            .ToHtmlString();

        Assert.Contains("/orders?pageIndex=2", first);
        Assert.DoesNotContain("products", first);

        Assert.Contains("/products?pageIndex=2", second);
        Assert.DoesNotContain("orders", second);
    }
}
